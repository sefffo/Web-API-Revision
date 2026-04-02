using ECommerce.Services.Abstraction;
using ECommerce.SharedLibirary.DTO_s.PaymentDTOs;
using ECommerce.SharedLibirary.Settings;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ECommerce.Services.Servicies.Payment_Service;

public class FawaterakService : IFawaterakService
{
    private readonly HttpClient _httpClient;
    private readonly FawaterakSettings _settings;

    public FawaterakService(HttpClient httpClient, IOptions<FawaterakSettings> settings)
    {
        _settings = settings.Value;
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(_settings.BaseUrl);
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _settings.ApiKey);
        _httpClient.DefaultRequestHeaders.Accept
            .Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<(string PaymentUrl, string InvoiceId)> CreateInvoiceAsync(CreateInvoiceDTO dto)
    {
        var request = new FawaterakInvoiceRequest
        {
            CartTotal = dto.CartTotal,
            Currency = "EGP",
            Customer = new FawaterakCustomer
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Phone = dto.Phone,
                Address = dto.Address
            },
            CartItems = dto.Items.Select(i => new FawaterakCartItem
            {
                Name = i.Name,
                Price = i.Price,
                Quantity = i.Quantity
            }).ToList(),
            RedirectionUrls = new FawaterakRedirectionUrls
            {
                SuccessUrl = _settings.SuccessUrl,
                FailUrl = _settings.FailUrl,
                PendingUrl = _settings.PendingUrl,
                CallbackUrl = _settings.CallbackUrl
            }
        };

        var json = JsonSerializer.Serialize(request, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        });

        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var httpResponse = await _httpClient.PostAsync("/api/v2/createInvoiceLink", content);
        var body = await httpResponse.Content.ReadAsStringAsync();

        if (!httpResponse.IsSuccessStatusCode)
            throw new Exception($"Fawaterak error {httpResponse.StatusCode}: {body}");

        var result = JsonSerializer.Deserialize<FawaterakInvoiceResponse>(body,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (result?.Status != "success")
            throw new Exception($"Fawaterak returned non-success: {body}");

        return (result.Data.Url, result.Data.InvoiceId);
    }
}