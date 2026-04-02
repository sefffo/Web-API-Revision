using System.Text.Json.Serialization;

namespace ECommerce.Services.Servicies.Payment_Service
{
    internal class FawaterakCustomer
    {
        [JsonPropertyName("first_name")]
        public string FirstName { get; set; } = default!;

        [JsonPropertyName("last_name")]
        public string LastName { get; set; } = default!;

        [JsonPropertyName("email")]
        public string Email { get; set; } = default!;

        [JsonPropertyName("phone")]
        public string Phone { get; set; } = default!;

        [JsonPropertyName("address")]
        public string Address { get; set; } = default!;
    }

    internal class FawaterakCartItem
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = default!;

        [JsonPropertyName("price")]
        public string Price { get; set; } = default!;

        [JsonPropertyName("quantity")]
        public string Quantity { get; set; } = default!;
    }

    internal class FawaterakRedirectionUrls
    {
        [JsonPropertyName("successUrl")]
        public string SuccessUrl { get; set; } = default!;

        [JsonPropertyName("failUrl")]
        public string FailUrl { get; set; } = default!;

        [JsonPropertyName("pendingUrl")]
        public string PendingUrl { get; set; } = default!;

        [JsonPropertyName("callbackUrl")]
        public string CallbackUrl { get; set; } = default!;
    }

    internal class FawaterakInvoiceRequest
    {
        [JsonPropertyName("providerKey")]
        public string ProviderKey { get; set; } = default!;

        [JsonPropertyName("cartTotal")]
        public string CartTotal { get; set; } = default!;

        [JsonPropertyName("currency")]
        public string Currency { get; set; } = "EGP";

        [JsonPropertyName("customer")]
        public FawaterakCustomer Customer { get; set; } = default!;

        [JsonPropertyName("cartItems")]
        public List<FawaterakCartItem> CartItems { get; set; } = [];

        [JsonPropertyName("redirectionUrls")]
        public FawaterakRedirectionUrls RedirectionUrls { get; set; } = default!;
    }

    internal class FawaterakInvoiceData
    {
        [JsonPropertyName("url")]
        public string Url { get; set; } = default!;

        [JsonPropertyName("invoiceId")]
        public long InvoiceId { get; set; }
    }

    internal class FawaterakInvoiceResponse
    {
        [JsonPropertyName("status")]
        public string Status { get; set; } = default!;

        [JsonPropertyName("data")]
        public FawaterakInvoiceData Data { get; set; } = default!;
    }
}
