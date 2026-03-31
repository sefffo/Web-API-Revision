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
    }
}
