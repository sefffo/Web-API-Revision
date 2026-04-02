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

        // Granular item operations
        Task<BasketDTO> AddItemAsync(string basketId, BasketItemDTO item);
        Task<BasketDTO> UpdateItemQuantityAsync(string basketId, int itemId, int quantity);
        Task<BasketDTO> RemoveItemAsync(string basketId, int itemId);
    }
}
