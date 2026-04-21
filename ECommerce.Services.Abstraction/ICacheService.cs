using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Services.Abstraction
{
    public interface ICacheService 
    {
        Task<string> GetAsync(string cacheKey);
        Task setAsync(string cacheKey, object CachedValue, TimeSpan TTL);
        Task RemoveAsync(string cacheKey);

        /// <summary>
        /// Removes every cached entry whose key matches the given glob pattern.
        /// Use for cross-user invalidations (RedisCacheAttribute appends "|user-{email}" per-user).
        /// </summary>
        Task RemoveByPatternAsync(string pattern);
    }
}
