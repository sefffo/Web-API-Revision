using AutoMapper;
using ECommerce.Domain.Entities.ProductModule;
using ECommerce.SharedLibirary.DTO_s.ProductDtos;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Services.MappingProfiles
{
    public class ProductPictureUrlResolver (IConfiguration configuration)  : IValueResolver<Product, ProductDto, string>
    {


        public string Resolve(Product source, ProductDto destination, string destMember, ResolutionContext context)
        {
            
            if(string.IsNullOrEmpty(source.PictureUrl))
                return string.Empty;


            if(source.PictureUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                return source.PictureUrl;
            }
            var baseurl = configuration.GetSection("URLs")["BaseUrl"];

            if(baseurl == null)
                return string.Empty;

            var picUrl = $"{baseurl}{source.PictureUrl}";


            return picUrl;

        }
    }
}
