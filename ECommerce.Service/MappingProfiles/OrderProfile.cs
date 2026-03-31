using AutoMapper;
using ECommerce.Domain.Entities.OrderModule;
using ECommerce.SharedLibirary.DTO_s.OrderDTOs;

namespace ECommerce.Services.MappingProfiles
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<AddressDTO, OrderShippingAddress>().ReverseMap();
            CreateMap<OrderItem, OrderItemDTO>()
                .ForMember(dest => dest.ProductName, src => src.MapFrom(src => src.Product.ProductName))
                .ForMember(dest => dest.PictureUrl, src => src.MapFrom<OrderProductPictureUrlResolver>());
            // In OrderProfile.cs, add this ForMember:
            CreateMap<Order, OrderToReturnDTO>()
               .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.Items))
               .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.ShippingAddress))
               .ForMember(dest => dest.DeliveryMethod, opt => opt.MapFrom(src => src.DeliveryMethod.ShortName))
               .ForMember(dest => dest.OrderStatus, opt => opt.MapFrom(src => src.Status.ToString()));


            CreateMap<DeliveryMethod, DeliveryMethodDTO>();
        }
    }
}
