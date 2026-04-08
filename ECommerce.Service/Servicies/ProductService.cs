using AutoMapper;
using ECommerce.Domain.Entities.ProductModule;
using ECommerce.Domain.Interfaces;
using ECommerce.Services.Abstraction;
using ECommerce.Services.Exceptions;
using ECommerce.Services.Specifications;
using ECommerce.SharedLibirary;
using ECommerce.SharedLibirary.CommonResult;
using ECommerce.SharedLibirary.DTO_s.ProductDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Services.Services
{
    public class ProductService(IUnitOfWork unitOfWork, IMapper mapper) : IProductService
    {
        public async Task<IEnumerable<BrandDto>> GetAllBrandsAsync()
        {
            var repo = unitOfWork.GetRepository<ProductBrand, int>();
            var brands = await repo.GetAllAsync();

            if(brands is null )
            {
                throw new BrandsNotFoundException();
            }
            //mapp from Brand Entity to the dto BrandDto
            var mappedBrands = mapper.Map<IEnumerable<BrandDto>>(brands);
            //if (mappedBrands == null) { return null; }
            return mappedBrands;
        }

        public async Task<PaginatedResult<ProductDto>> GetAllProductsAsync(ProductQueryPrams queryPrams)
        {
            var repo = unitOfWork.GetRepository<Product, int>();
            var spec = new ProductWithBrandAndTypeSpecification(queryPrams);
            var Products = await repo.GetAllAsync(spec);


            if(Products is null)
            {
                throw new ProductsNotFoundException();
            }
           
            //mapp from Brand Entity to the dto BrandDto
            var mappedProducts = mapper.Map<IEnumerable<ProductDto>>(Products);

            var CountofReturnedProducts = mappedProducts.Count();

            var CountSpec = new ProductCountSpecifications(queryPrams);
            var CountOfAllProducts = await repo.CountAsync(CountSpec);
            //if (mappedProducts == null) { return null; }


            return new PaginatedResult<ProductDto>( queryPrams.PageIndex, queryPrams.PageSize, CountOfAllProducts ,mappedProducts);
        }

        public async Task<IEnumerable<TypeDto>> GetAllTypesAsync()
        {
            var repo = unitOfWork.GetRepository<ProductType, int>();
            var Types = await repo.GetAllAsync();
            if(Types is null)
            {
                throw new TypesNotFoundException(); 
            }
            //mapp from Brand Entity to the dto BrandDto
            var mappedTypes = mapper.Map<IEnumerable<TypeDto>>(Types);
            //if (mappedTypes == null) return null;
            return mappedTypes;
        }

        public async Task<Result<ProductDto>> GetProductByIdAsync(int id)
        {
            var repo = unitOfWork.GetRepository<Product, int>();
            var spec = new ProductWithBrandAndTypeSpecification(id);
            var Product = await repo.GetByIdAsync(spec);
            if (Product is null)
            {
               return Error.NotFound("Product.NotFound" , $"Product With {id} is Not Found");

            }
            var mappedProduct = mapper.Map<ProductDto>(Product);
            //if (mappedProduct == null) return null;
            return mappedProduct;
        }
    }
}
