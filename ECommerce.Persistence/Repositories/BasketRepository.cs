using ECommerce.Domain.Entities.BasketModule;
using ECommerce.Domain.Interfaces;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace ECommerce.Persistence.Repositories
{

    //there's IDistributedcache to use with any caching service 

    //IConnectionMultiplixer used for a certain tech for only ====> redis 
    public class BasketRepository : IBasketRepository
    {

        private readonly IDatabase database;
        public BasketRepository(IConnectionMultiplexer connectionMultiplexer)
        {
            database =  connectionMultiplexer.GetDatabase();
        }

        public async Task<CustomerBasket?> CreateOrUpdateCustomerBasketAsync(CustomerBasket basket, TimeSpan TTL = default)
        {
            //creating the basket


            var JsonBasket =  JsonSerializer.Serialize(basket);

                                //value ==> cutomer basket and we need t seralize it 
            var IsCreatedOrUpdated = await database.StringSetAsync(basket.Id,JsonBasket, (TTL == default) ? TimeSpan.FromDays(7) : TTL);

            if(IsCreatedOrUpdated)
            {
                //var Basket = await database.StringGetAsync(basket.Id);
                //return JsonSerializer.DeserializeAsync<CustomerBasket>(()Basket!);

                return await GetBasketAsync(basket.Id);
            }
            else
            {
                return null;
            }



            
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

    }
}
