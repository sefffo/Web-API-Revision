using ECommerce.Domain.Interfaces;
using ECommerce.Services.Abstraction;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ECommerce.Services.Servicies
{
    public class CacheService(ICacheRepository repository) : ICacheService
    {
        // Match ASP.NET Core's MVC JSON response settings so cached payloads use the
        // SAME shape (camelCase property names) as freshly-serialized controller responses.
        // Without this, the first response is camelCase (subTotal, orderDate, userEmail),
        // but subsequent responses served from Redis come back PascalCase (SubTotal, OrderDate,
        // UserEmail), and the frontend reads `undefined` -> NaN in formatCurrency / Invalid Date.
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
        };

        public async Task<string> GetAsync(string cacheKey)
        {
            return await repository.GetAsync(cacheKey);
        }

        public async Task setAsync(string cacheKey, object CachedValue, TimeSpan TTL)
        {
            var value = JsonSerializer.Serialize(CachedValue, _jsonOptions);
            await repository.SetAsync(cacheKey, value, TTL);
        }

        public async Task RemoveAsync(string cacheKey)
        {
            await repository.RemoveAsync(cacheKey);
        }

        public async Task RemoveByPatternAsync(string pattern)
        {
            await repository.RemoveByPatternAsync(pattern);
        }
    }
}
