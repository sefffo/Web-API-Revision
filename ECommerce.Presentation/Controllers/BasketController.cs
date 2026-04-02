using ECommerce.Services.Abstraction;
using ECommerce.SharedLibirary.DTO_s.BasketDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Presentation.Controllers
{
    [Authorize]
    public class BasketController(IBasketService basketService) : ApiBaseController
    {
        // GET: /api/Baskets?id={id}
        [HttpGet]
        public async Task<ActionResult<BasketDTO>> GetBasket(string id)
        {
            var basket = await basketService.GetBasketAsync(id);
            return Ok(basket);
        }

        // POST: /api/Baskets  (full replace — create or overwrite whole basket)
        [HttpPost]
        public async Task<ActionResult<BasketDTO>> CreateOrUpdateBasket(BasketDTO basket)
        {
            var Basket = await basketService.CreateOrUpdateBasketAsync(basket);
            return Ok(Basket);
        }

        // DELETE: /api/Baskets/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeleteBasket(string id)
        {
            var result = await basketService.DeleteBasketAsync(id);
            return Ok(result);
        }

        // POST: /api/Baskets/{basketId}/items  (add single item)
        [HttpPost("{basketId}/items")]
        public async Task<ActionResult<BasketDTO>> AddItem(string basketId, BasketItemDTO item)
        {
            var basket = await basketService.AddItemAsync(basketId, item);
            return Ok(basket);
        }

        // PUT: /api/Baskets/{basketId}/items/{itemId}?quantity=3
        [HttpPut("{basketId}/items/{itemId}")]
        public async Task<ActionResult<BasketDTO>> UpdateItemQuantity(
            string basketId, int itemId, [FromQuery] int quantity)
        {
            var basket = await basketService.UpdateItemQuantityAsync(basketId, itemId, quantity);
            return Ok(basket);
        }

        // DELETE: /api/Baskets/{basketId}/items/{itemId}
        [HttpDelete("{basketId}/items/{itemId}")]
        public async Task<ActionResult<BasketDTO>> RemoveItem(string basketId, int itemId)
        {
            var basket = await basketService.RemoveItemAsync(basketId, itemId);
            return Ok(basket);
        }
    }
}
