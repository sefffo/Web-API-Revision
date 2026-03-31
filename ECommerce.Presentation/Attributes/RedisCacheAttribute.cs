using ECommerce.Services.Abstraction;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using System.Text;

namespace ECommerce.Presentation.Attributes
{
    public class RedisCacheAttribute : ActionFilterAttribute
    {
        private readonly int _Time;

        public RedisCacheAttribute(int DurationInMinutes = 5)
        {
            _Time = DurationInMinutes;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var CacheService = context.HttpContext.RequestServices.GetRequiredService<ICacheService>();
            var CacheKey = CreateCacheKey(context.HttpContext.Request);

            var cacheValue = await CacheService.GetAsync(CacheKey);

            if (cacheValue is not null)
            {
                context.Result = new ContentResult()
                {
                    Content = cacheValue,
                    ContentType = "application/json",
                    StatusCode = StatusCodes.Status200OK,
                };
                return;
            }

            var executedContext = await next.Invoke();

            if (executedContext.Exception is not null) return;

            if (executedContext.Result is OkObjectResult result)
            {
                try
                {
                    await CacheService.setAsync(CacheKey, result.Value, TimeSpan.FromMinutes(_Time));
                }
                catch (Exception)
                {
                    // Caching failed silently
                }
            }
        }

        private string CreateCacheKey(HttpRequest request)
        {
            var sb = new StringBuilder();
            sb.Append(request.Path);

            // Per-user cache key — each user gets their own cached response
            var userEmail = request.HttpContext.User?.FindFirst(ClaimTypes.Email)?.Value;
            if (!string.IsNullOrEmpty(userEmail))
                sb.Append($"|user-{userEmail}");

            foreach (var item in request.Query.OrderBy(k => k.Key))
                sb.Append($"|{item.Key}-{item.Value}");

            return sb.ToString();
        }
    }
}
