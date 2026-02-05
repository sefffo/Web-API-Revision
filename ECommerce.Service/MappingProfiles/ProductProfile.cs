using AutoMapper;
using ECommerce.Domain.Entities.ProductModule;
using ECommerce.SharedLibirary.DTO_s.ProductDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Services.MappingProfiles
{
    public class ProductProfile : Profile
    {
        public ProductProfile() { 
        
        
            CreateMap<Product,ProductDto>()
            //to get them throw the nav props of the product entity and map them to the product dto
            .ForMember( dest => dest.ProductBrand , opt => opt.MapFrom(src=>src.ProductBrand.Name))
            .ForMember( dest => dest.ProductType , opt => opt.MapFrom(src=>src.ProductType.Name))
            //.ForMember(dist => dist.PictureUrl, options => options.MapFrom(new PictureUrlResolver(configuration)))
            .ForMember(dest => dest.PictureUrl, opt => opt.MapFrom<ProductPictureUrlResolver>())
            .ReverseMap();






             
    
            CreateMap<ProductBrand,BrandDto>().ReverseMap();

            CreateMap<ProductType,TypeDto>().ReverseMap();


        }
  

    }
}
