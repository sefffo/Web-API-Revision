using ECommerce.Services.Abstraction;
using ECommerce.SharedLibirary.DTO_s.PaymentDTOs;
using ECommerce.SharedLibirary.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ECommerce.Services.Servicies.Payment_Service;

public class FawaterakService : IFawaterakService
{
    private readonly HttpClient _httpClient;
    private readonly FawaterakSettings _settings;
    private readonly ILogger<FawaterakService> _logger;

    private static readonly JsonSerializerOptions _serializeOptions = new()
    {
        PropertyNamingPolicy = null,
        WriteIndented = false
    };

    private static readonly JsonSerializerOptions _deserializeOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public FawaterakService(
        HttpClient httpClient,
        IOptions<FawaterakSettings> settings,
        ILogger<FawaterakService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
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
            ProviderKey = _settings.ProviderKey,
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

        var json = JsonSerializer.Serialize(request, _serializeOptions);
        _logger.LogInformation("Fawaterak request payload: {Json}", json);

        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var httpResponse = await _httpClient.PostAsync("/api/v2/createInvoiceLink", content);
        var body = await httpResponse.Content.ReadAsStringAsync();

        _logger.LogInformation("Fawaterak response ({Status}): {Body}", httpResponse.StatusCode, body);

        if (!httpResponse.IsSuccessStatusCode)
            throw new Exception($"Fawaterak HTTP error {httpResponse.StatusCode}: {body}");

        var result = JsonSerializer.Deserialize<FawaterakInvoiceResponse>(body, _deserializeOptions);

        if (result?.Status != "success")
            throw new Exception($"Fawaterak returned non-success: {body}");

        return (result.Data.Url, result.Data.InvoiceId.ToString());
    }
}
