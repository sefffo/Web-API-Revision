namespace ECommerce.Domain.Entities.OrderModule
{

    //complex aggregate root entity representing a customer's order, containing multiple order items and associated details
    public class Order : BaseEntity<Guid>
    {

        public string UserEmail { get; set; } = default!; //store the email of the user who placed the order, for reference and communication purposes

        public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.Now; //the date and time when the order was placed, useful for tracking and historical data

        public OrderStatus Status { get; set; } = OrderStatus.Pending; //the current status of the order, initialized to Pending when a new order is created



        public OrderShippingAddress ShippingAddress { get; set; } = default!;
        //embedded value object representing the shipping address for this order, capturing details at the time of order placement

        public ICollection<OrderItem> Items { get; set; } = []; // initialized to an empty collection to avoid null reference issues, this will hold the individual items included in the order

        //collection of order items included in this order, representing the products and quantities ordered


        public decimal Subtotal { get; set; }  //the total cost of the order before adding delivery fees and taxes, calculated from the order items

        

        public decimal GetTotal ()
            => Subtotal + DeliveryMethod.Price;



        //navigation property to the selected delivery method for this order, linking to the DeliveryMethod entity which contains details about the delivery option chosen by the customer
        public DeliveryMethod DeliveryMethod { get; set; } = default!; //navigation property to the selected delivery method for this order

        //many orders can be associated with one delivery method, but each order has only one delivery method, so this is a many-to-one relationship from Order to DeliveryMethod
        public int DeliveryMethodId { get; set; } //foreign key to the DeliveryMethod entity, linking this order to the specific delivery method chosen by the customer


    }
}
