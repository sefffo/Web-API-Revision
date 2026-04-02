using ECommerce.Domain.Entities.BasketModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Domain.Interfaces
{
    public interface IBasketRepository
    {
        Task<CustomerBasket?> GetBasketAsync(string id);
        Task<CustomerBasket?> CreateOrUpdateCustomerBasketAsync(CustomerBasket customerBasket, TimeSpan TTL = default);
        Task<bool> DeleteBasketAsync(string id);

        // Granular item operations
        Task<CustomerBasket?> AddItemAsync(string basketId, BasketItem item);
        Task<CustomerBasket?> UpdateItemQuantityAsync(string basketId, int itemId, int quantity);
        Task<CustomerBasket?> RemoveItemAsync(string basketId, int itemId);
    }
}
