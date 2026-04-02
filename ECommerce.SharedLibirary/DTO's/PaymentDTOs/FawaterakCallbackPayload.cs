namespace ECommerce.SharedLibirary.DTO_s.PaymentDTOs
{
    public class FawaterakCallbackPayload
    {
        public string InvoiceId { get; set; } = default!;
        public string PaymentStatus { get; set; } = default!; // "paid", "failed", "pending"
        public decimal Amount { get; set; }
    }
}
