namespace ECommerce.Domain.Entities.OrderModule
{
    //own el product details at the time of order , to preserve historical data even if the product details change later
    public class ProductItemOrdered //represent el product details at the time of order, to preserve historical data even if the product details change later
    {
        //owend by the OrderItem entity, not a separate table, so no Id property
        public int ProductId { get; set; } //pk and fk to the Product table, linking the order item to a specific product
        public string ProductName { get; set; } = default!; //store the product name at the time of order
        public string PictureUrl { get; set; } = default!; //store the product picture URL at the time of order
    }
    public class OrderItem : BaseEntity<int>
    {
        public ProductItemOrdered Product { get; set; } = default!; //embedded value object to capture product details at the time of order
        public decimal Price { get; set; } //store the price at the time of order
        public int Quantity { get; set; } //the quantity of this product in the order
        // navigation properties
        //public int OrderId { get; set; } //pk and fk to the Order table, linking this item to a specific order
        //public Order Order { get; set; } = default!; //Navigation property to the associated Order
    }
}
