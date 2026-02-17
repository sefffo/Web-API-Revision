using ECommerce.Services.Abstraction;
using ECommerce.SharedLibirary.DTO_s.BasketDTOs;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Presentation.Controllers
{

    [ApiController]
    [Route("api/[Controller]")]
    public class BasketController(IBasketService basketService) : ControllerBase
    {
        //GET : baseUrl/api/Baskets?id={id}

        [HttpGet]
        public async Task<ActionResult<BasketDTO>> GetBasket(string id)
        {

            var basket = await basketService.GetBasketAsync(id);

            return Ok(basket);

        }

        // POST : baseUrl/api/Baskets
        [HttpPost]

        public async Task<ActionResult<BasketDTO>> CreateOrUpdateBasket(BasketDTO basket)
        {
            var Basket = await basketService.CreateOrUpdateBasketAsync(basket);
            return Ok(Basket);
        }


        [HttpDelete("{id}")]

        //Delete : baseUrl / api / baskets / {id}


        public async Task<ActionResult<bool>> DeleteBasket(string id)
        {
            var result = await basketService.DeleteBasketAsync(id);
            return Ok(result);
        }


    }
}
