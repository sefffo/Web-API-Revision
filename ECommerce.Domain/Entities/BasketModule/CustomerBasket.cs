namespace ECommerce.Domain.Entities.BasketModule
{
    public class CustomerBasket //it wont inherit from the base entity cuz its gonna be cached 
    {

        public string Id { get; set; } = null!;

        public ICollection<BasketItem> Items { get; set; } = [];




        
    }
}
