using ECommerce.Domain.Interfaces;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Persistence.Repositories
{
    public class CacheRepository : ICacheRepository
    {
        private readonly IDatabase _database;
        private readonly IConnectionMultiplexer _multiplexer;

        public CacheRepository(IConnectionMultiplexer connectionMultiplexer)
        {
            _multiplexer = connectionMultiplexer;
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

        /// <summary>
        /// Deletes every key matching the given pattern across all configured Redis endpoints.
        /// Uses non-blocking SCAN (pageSize 250) so production isn't stalled even with a large keyspace.
        /// Necessary because cached list responses (e.g. /api/Order/Admin/AllOrders|user-admin@x.com) are
        /// stored per-user — a single admin status update must evict every admin's cached copy.
        /// </summary>
        public async Task RemoveByPatternAsync(string pattern)
        {
            foreach (var endpoint in _multiplexer.GetEndPoints())
            {
                var server = _multiplexer.GetServer(endpoint);
                // Keys() streams via SCAN; safe on production compared to KEYS *
                var keys = server.Keys(_database.Database, pattern: pattern, pageSize: 250).ToArray();
                if (keys.Length > 0)
                    await _database.KeyDeleteAsync(keys);
            }
        }
    }
}
