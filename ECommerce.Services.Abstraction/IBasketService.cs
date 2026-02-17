using ECommerce.SharedLibirary.DTO_s.BasketDTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Services.Abstraction
{
    public interface IBasketService
    {
        Task<BasketDTO> GetBasketAsync(string id);
        Task<BasketDTO> CreateOrUpdateBasketAsync(BasketDTO basket);

        Task<bool> DeleteBasketAsync(string id);
    }
}
