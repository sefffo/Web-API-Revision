using AutoMapper;
using ECommerce.Domain.Entities.ProductModule;
using ECommerce.SharedLibirary.DTO_s.ProductDtos;

namespace ECommerce.Services.MappingProfiles
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {


            CreateMap<Product, ProductDto>()
            //to get them throw the nav props of the product entity and map them to the product dto
            .ForMember(dest => dest.ProductBrand, opt => opt.MapFrom(src => src.ProductBrand.Name))
            .ForMember(dest => dest.ProductType, opt => opt.MapFrom(src => src.ProductType.Name))
            //.ForMember(dist => dist.PictureUrl, options => options.MapFrom(new PictureUrlResolver(configuration)))
            .ForMember(dest => dest.PictureUrl, opt => opt.MapFrom<ProductPictureUrlResolver>())
            .ReverseMap();


            CreateMap<ProductBrand, BrandDto>().ReverseMap();

            CreateMap<ProductType, TypeDto>().ReverseMap();


            //write side of my requests (commands) to create a new product and map it to the product entity to save it in the database
            CreateMap<CreateProductDto, Product>()
             .ForMember(dest => dest.ProductBrand, opt => opt.Ignore()) //  it's a nav prop, set manually
             .ForMember(dest => dest.ProductType, opt => opt.Ignore())  //  same
             .ForMember(dest => dest.BrandId, opt => opt.Ignore())      // set manually in service
             .ForMember(dest => dest.TypeId, opt => opt.Ignore());      //  set manually in service
            CreateMap<CreateBrandDto, ProductBrand>();
            CreateMap<CreateTypeDto, ProductType>();


        }


    }
}
