using AutoMapper;
using ECommerce.Domain.Entities.ProductModule;
using ECommerce.Domain.Interfaces;
using ECommerce.Services.Abstraction;
using ECommerce.SharedLibirary.DTO_s.ProductDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Services.Servicies
{
    public class ProductService(IUnitOfWork unitOfWork, IMapper mapper) : IProductServivce
    {
        public async Task<IEnumerable<BrandDto>> GetAllBrandsAsync()
        {
            var repo = unitOfWork.GetRepository<ProductBrand, int>();
            var brands = await repo.GetAllAsync();
            //mapp from Brand Entity to the dto BrandDto
            var mappedBrands = mapper.Map<IEnumerable<BrandDto>>(brands);
            if (mappedBrands == null) { return null; }
            return mappedBrands;
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            var repo = unitOfWork.GetRepository<Product, int>();
            var Products = await repo.GetAllAsync();
            //mapp from Brand Entity to the dto BrandDto
            var mappedProducts = mapper.Map<IEnumerable<ProductDto>>(Products);
            if (mappedProducts == null) { return null; }
            return mappedProducts;
        }

        public async Task<IEnumerable<TypeDto>> GetAllTypesAsync()
        {
            var repo = unitOfWork.GetRepository<ProductType, int>();
            var Types = await repo.GetAllAsync();
            //mapp from Brand Entity to the dto BrandDto
            var mappedTypes = mapper.Map<IEnumerable<TypeDto>>(Types);
            if (mappedTypes == null) return null;
            return mappedTypes;
        }

        public async Task<ProductDto> GetProductByIdAsync(int id)
        {
            var repo = unitOfWork.GetRepository<Product, int>();
            var Product = await repo.GetByIdAsync(id);
            var mappedProduct = mapper.Map<ProductDto>(Product);
            if (mappedProduct == null) return null;
            return mappedProduct;
        }
    }
}
