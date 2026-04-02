using ECommerce.Domain.Entities.BasketModule;
using ECommerce.Domain.Interfaces;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace ECommerce.Persistence.Repositories
{
    // its different repo bec it uses a different database (Redis)
    public class BasketRepository : IBasketRepository
    {
        private readonly IDatabase database;

        public BasketRepository(IConnectionMultiplexer connectionMultiplexer)
        {
            database = connectionMultiplexer.GetDatabase();
        }

        public async Task<CustomerBasket?> CreateOrUpdateCustomerBasketAsync(CustomerBasket basket, TimeSpan TTL = default)
        {
            var JsonBasket = JsonSerializer.Serialize(basket);
            var IsCreatedOrUpdated = await database.StringSetAsync(
                basket.Id,
                JsonBasket,
                TTL == default ? TimeSpan.FromDays(7) : TTL);

            return IsCreatedOrUpdated ? await GetBasketAsync(basket.Id) : null;
        }

        public async Task<bool> DeleteBasketAsync(string id)
        {
            return await database.KeyDeleteAsync(id);
        }

        public async Task<CustomerBasket?> GetBasketAsync(string id)
        {
            var basket = await database.StringGetAsync(id);
            return basket.IsNullOrEmpty
                ? null
                : JsonSerializer.Deserialize<CustomerBasket>((string)basket!);
        }

        public async Task<CustomerBasket?> AddItemAsync(string basketId, BasketItem item)
        {
            var basket = await GetBasketAsync(basketId)
                         ?? new CustomerBasket { Id = basketId, Items = new List<BasketItem>() };

            var existing = basket.Items.FirstOrDefault(i => i.Id == item.Id);

            if (existing is not null)
                existing.Quantity += item.Quantity;  // already exists -> increment quantity
            else
                basket.Items.Add(item);              // new item -> add it

            return await CreateOrUpdateCustomerBasketAsync(basket);
        }

        public async Task<CustomerBasket?> UpdateItemQuantityAsync(string basketId, int itemId, int quantity)
        {
            var basket = await GetBasketAsync(basketId);
            if (basket is null) return null;

            var item = basket.Items.FirstOrDefault(i => i.Id == itemId);
            if (item is null) return null;

            item.Quantity = quantity;

            return await CreateOrUpdateCustomerBasketAsync(basket);
        }

        public async Task<CustomerBasket?> RemoveItemAsync(string basketId, int itemId)
        {
            var basket = await GetBasketAsync(basketId);
            if (basket is null) return null;

            var item = basket.Items.FirstOrDefault(i => i.Id == itemId);
            if (item is null) return null;

            basket.Items.Remove(item);

            return await CreateOrUpdateCustomerBasketAsync(basket);
        }
    }
}
