using ECommerce.Presentation.Attributes;
using ECommerce.Services.Abstraction;
using ECommerce.SharedLibirary.DTO_s.OrderDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace ECommerce.Presentation.Controllers
{
    [Authorize]
    public class OrderController(IOrderService orderService) : ApiBaseController
    {
        [HttpPost]
        public async Task<ActionResult<OrderToReturnDTO>> CreateOrder(OrderDTO orderDto)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var result = await orderService.CreateOrderAsync(orderDto, email);

            // Invalidate AllOrders cache for this user so next GET returns fresh data
            if (result.isSuccess)
            {
                var cacheService = HttpContext.RequestServices.GetRequiredService<ICacheService>();
                var cacheKey = $"/api/Order/AllOrders|user-{email}";
                try { await cacheService.RemoveAsync(cacheKey); } catch { }
            }

            return HandleResult(result);
        }

        [HttpGet("DeliveryMethods")]
        public async Task<ActionResult<IEnumerable<DeliveryMethodDTO>>> GetDeliveryMethods()
        {
            var result = await orderService.GetDeliveryMethodsAsync();
            return HandleResult(result);
        }

        [HttpGet("AllOrders")]
        [RedisCache(60)]
        public async Task<ActionResult<IEnumerable<OrderToReturnDTO>>> GetAllOrders()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var result = await orderService.GetAllOrdersAsync(email);
            return HandleResult(result);
        }

        [HttpGet("{orderId}")]
        [RedisCache(60)]
        public async Task<ActionResult<OrderToReturnDTO>> GetOrderById(Guid orderId)
        {
            var result = await orderService.GetOrderById(orderId);
            return HandleResult(result);
        }
    }
}
