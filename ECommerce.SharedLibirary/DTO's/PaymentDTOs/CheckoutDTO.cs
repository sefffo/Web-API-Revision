namespace ECommerce.SharedLibirary.DTO_s.PaymentDTOs
{

    public class CheckoutDTO
    {
        public Guid OrderId { get; set; }
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Phone { get; set; } = default!;
        public string Address { get; set; } = default!;
    }
}
