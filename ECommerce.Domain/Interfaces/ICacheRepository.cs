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
    }
}
