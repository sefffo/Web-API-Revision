using ECommerce.Services.Abstraction;
using ECommerce.SharedLibirary.DTO_s.BasketDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.Presentation.Controllers
{
    [Authorize]
    public class BasketController(IBasketService basketService) : ApiBaseController
    {

        private string GetBasketId()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null)
            {
                throw new UnauthorizedAccessException("User is not authorized.");
            }
            return $"basket-{email}";
        }

        [HttpGet]
        public async Task<ActionResult<BasketDTO>> GetBasket()
        {
          
            var basketId = GetBasketId();

            if (basketId is null) return Unauthorized();

            var basket = await basketService.GetBasketAsync(basketId);

            return Ok(basket);
        }

        // POST: /api/Baskets  (full replace — create or overwrite whole basket)
        [HttpPost]
        public async Task<ActionResult<BasketDTO>> CreateOrUpdateBasket(BasketDTO basket)
        {
            var basketId = GetBasketId();

            if (basketId is null) return Unauthorized();


            basket = basket with { id = basketId }; // to ensure the basket ID is tied to the user and not overridden by client input

            var Basket = await basketService.CreateOrUpdateBasketAsync(basket);
            return Ok(Basket);
        }

        // DELETE: /api/Baskets/{id}
        [HttpDelete]
        public async Task<ActionResult<bool>> DeleteBasket()
        {
            var basketId = GetBasketId();

            if (basketId is null) return Unauthorized();

            var result = await basketService.DeleteBasketAsync(basketId);
            return Ok(result);
        }

        // POST: /api/Baskets/{basketId}/items  (add single item)
        [HttpPost("items")]
        public async Task<ActionResult<BasketDTO>> AddItem(BasketItemDTO item)
        {

            var basketIdFromUser = GetBasketId();
            if (basketIdFromUser is null) return Unauthorized();
            var basket = await basketService.AddItemAsync(basketIdFromUser, item);
            return Ok(basket);
        }

        // PUT: /api/Baskets/items/{itemId}?quantity=3
        [HttpPut("items/{itemId}")]
        public async Task<ActionResult<BasketDTO>> UpdateItemQuantity(
            int itemId, [FromQuery] int quantity)
        {

            var basketIdfromUser = GetBasketId();
            if (basketIdfromUser is null) return Unauthorized();
            var basket = await basketService.UpdateItemQuantityAsync(basketIdfromUser, itemId, quantity);
            return Ok(basket);
        }

        // DELETE: /api/Baskets/items/{itemId}
        [HttpDelete("items/{itemId}")]
        public async Task<ActionResult<BasketDTO>> RemoveItem(int itemId)
        {
            var basketIdFromUser = GetBasketId();
            if (basketIdFromUser is null) return Unauthorized();
            var basket = await basketService.RemoveItemAsync(basketIdFromUser, itemId);
            return Ok(basket);
        }
    }
}
