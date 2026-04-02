using AutoMapper;
using ECommerce.Domain.Entities.BasketModule;
using ECommerce.Domain.Interfaces;
using ECommerce.Services.Abstraction;
using ECommerce.Services.Exceptions;
using ECommerce.SharedLibirary.DTO_s.BasketDTOs;

namespace ECommerce.Services.Servicies
{
    public class BasketService(IBasketRepository repository, IMapper mapper) : IBasketService
    {
        public async Task<BasketDTO> CreateOrUpdateBasketAsync(BasketDTO basket)
        {
            var MappedBasket = mapper.Map<CustomerBasket>(basket);
            var CreatedOrUpdatedBasket = await repository.CreateOrUpdateCustomerBasketAsync(MappedBasket);
            return mapper.Map<BasketDTO>(CreatedOrUpdatedBasket);
        }

        public async Task<bool> DeleteBasketAsync(string id)
        {
            if (id is null)
                throw new BasketNotFoundException(id);

            return await repository.DeleteBasketAsync(id);
        }

        public async Task<BasketDTO> GetBasketAsync(string id)
        {
            var basket = await repository.GetBasketAsync(id);

            if (basket is null)
                throw new BasketNotFoundException(id);

            return mapper.Map<BasketDTO>(basket);
        }

        public async Task<BasketDTO> AddItemAsync(string basketId, BasketItemDTO item)
        {
            var mappedItem = mapper.Map<BasketItem>(item);
            var result = await repository.AddItemAsync(basketId, mappedItem);

            if (result is null)
                throw new BasketNotFoundException(basketId);

            return mapper.Map<BasketDTO>(result);
        }

        public async Task<BasketDTO> UpdateItemQuantityAsync(string basketId, int itemId, int quantity)
        {
            var result = await repository.UpdateItemQuantityAsync(basketId, itemId, quantity);

            if (result is null)
                throw new BasketNotFoundException(basketId);

            return mapper.Map<BasketDTO>(result);
        }

        public async Task<BasketDTO> RemoveItemAsync(string basketId, int itemId)
        {
            var result = await repository.RemoveItemAsync(basketId, itemId);

            if (result is null)
                throw new BasketNotFoundException(basketId);

            return mapper.Map<BasketDTO>(result);
        }
    }
}
