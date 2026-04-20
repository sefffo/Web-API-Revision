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
    [Route("api/[controller]")]
    public class OrderController(IOrderService orderService) : ApiBaseController
    {
        [HttpPost]
        public async Task<ActionResult<OrderToReturnDTO>> CreateOrder(OrderDTO orderDto)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var result = await orderService.CreateOrderAsync(orderDto, email);

            if (result.isSuccess)
            {
                var cacheService = HttpContext.RequestServices.GetRequiredService<ICacheService>();
                var cacheKey = $"/api/Order/AllOrders|user-{email}";
                try { await cacheService.RemoveAsync(cacheKey); } catch { }
            }

            return HandleResult(result);
        }

        [HttpGet("DeliveryMethods")]
        [AllowAnonymous]
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

        /// <summary>
        /// Returns ALL orders in the system. Restricted to Admin and SuperAdmin roles only.
        /// </summary>
        [HttpGet("Admin/AllOrders")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        [RedisCache(60)]
        public async Task<ActionResult<IEnumerable<OrderToReturnDTO>>> GetAllOrdersForAdmin()
        {
            var result = await orderService.GetAllOrdersForAdminAsync();
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
