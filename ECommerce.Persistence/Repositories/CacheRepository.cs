using ECommerce.Domain.Interfaces;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Persistence.Repositories
{
    public class CacheRepository : ICacheRepository
    {
        private readonly IDatabase _database;

        public CacheRepository(IConnectionMultiplexer connectionMultiplexer)
        {
            _database = connectionMultiplexer.GetDatabase();
        }

        public async Task<string?> GetAsync(string caacheKey)
        {
            var data = await _database.StringGetAsync(caacheKey);
            return data.IsNullOrEmpty ? null : data.ToString();
        }

        public async Task SetAsync(string cacheKey, string CachedValue, TimeSpan TTL)
        {
            await _database.StringSetAsync(cacheKey, CachedValue, TTL);
        }

        public async Task RemoveAsync(string cacheKey)
        {
            await _database.KeyDeleteAsync(cacheKey);
        }
    }
}
