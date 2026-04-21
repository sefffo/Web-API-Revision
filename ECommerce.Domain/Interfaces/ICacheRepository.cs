using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Domain.Interfaces
{
    public interface ICacheRepository 
    {
        Task<string?> GetAsync(string caacheKey);
        Task SetAsync(string cacheKey, string CachedValue, TimeSpan TTL);
        Task RemoveAsync(string cacheKey);

        /// <summary>
        /// Removes every Redis key whose name matches the given pattern.
        /// Pattern uses Redis glob syntax (e.g. "/api/Products*" matches every key starting with "/api/Products").
        /// Used when a single write must invalidate many per-user cached entries at once.
        /// </summary>
        Task RemoveByPatternAsync(string pattern);
    }
}
