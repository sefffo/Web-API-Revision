using AutoMapper;
using ECommerce.Domain.Entities.BasketModule;
using ECommerce.SharedLibirary.DTO_s.BasketDTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Services.MappingProfiles
{
    public class BasketProfile : Profile
    {
        public BasketProfile() {

            CreateMap<CustomerBasket, BasketDTO>().ReverseMap();

            CreateMap<BasketItem, BasketItemDTO>().ReverseMap(); //no need for editing cuz they have the same props names 
        }
    }
}
