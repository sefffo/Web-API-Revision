namespace ECommerce.Services.Servicies.Payment_Service
{
    internal class FawaterakCustomer
    {
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Phone { get; set; } = default!;
        public string Address { get; set; } = default!;
    }

    internal class FawaterakCartItem
    {
        public string Name { get; set; } = default!;
        public string Price { get; set; } = default!;
        public string Quantity { get; set; } = default!;
    }

    internal class FawaterakRedirectionUrls
    {
        public string SuccessUrl { get; set; } = default!;
        public string FailUrl { get; set; } = default!;
        public string PendingUrl { get; set; } = default!;
        public string CallbackUrl { get; set; } = default!;
    }

    internal class FawaterakInvoiceRequest
    {
        public string CartTotal { get; set; } = default!;
        public string Currency { get; set; } = "EGP";
        public FawaterakCustomer Customer { get; set; } = default!;
        public List<FawaterakCartItem> CartItems { get; set; } = [];
        public FawaterakRedirectionUrls RedirectionUrls { get; set; } = default!;
    }

    // ── What we RECEIVE from Fawaterak ─────────────────

    internal class FawaterakInvoiceData
    {
        public string Url { get; set; } = default!;
        public string InvoiceId { get; set; } = default!;
    }

    internal class FawaterakInvoiceResponse
    {
        public string Status { get; set; } = default!;
        public FawaterakInvoiceData Data { get; set; } = default!;
    }
}
