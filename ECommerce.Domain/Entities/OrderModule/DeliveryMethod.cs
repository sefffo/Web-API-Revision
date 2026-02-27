namespace ECommerce.Domain.Entities.OrderModule
{
    public class DeliveryMethod : BaseEntity<int>
    {
        public string ShortName { get; set; } = default!;
        public decimal Price { get; set; }

        public string DeliveryTime { get; set; } = default!; //e.g., "3-5 business days" 

        public string Description { get; set; } = default!; //e.g., "Standard delivery with tracking"


    }
}
