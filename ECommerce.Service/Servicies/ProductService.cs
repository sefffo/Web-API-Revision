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
    public class ProductService(IUnitOfWork unitOfWork, IMapper mapper , HttpClient httpClient) : IProductService
    {
        public async Task<Result<BrandDto>> CreateBrandAsync(CreateBrandDto dto)
        {
            var repo = unitOfWork.GetRepository<ProductBrand, int>();
            var mappedBrand = mapper.Map<ProductBrand>(dto);



            var existingBrand = await repo.GetByIdAsync(new BrandByNameSpecification(mappedBrand.Name));
            if (existingBrand != null) {
                return Error.BadRequest("Brand.AlreadyExists", $"Brand with name {mappedBrand.Name} already exists.");
            }

            if (mappedBrand is null)
            {
                return Error.InternalServerError("Brand.NotCreated", "Brand Not Created");
            }

            await repo.AddAsync(mappedBrand);
            
            await unitOfWork.SaveChangesAsync();
            var mappedBrandDto = mapper.Map<BrandDto>(mappedBrand);
            return mappedBrandDto;
        }

        public async Task<Result<TypeDto>> CreateTypeAsync(CreateTypeDto dto)
        {
            var repo = unitOfWork.GetRepository<ProductType, int>();


            var mappedType = mapper.Map<ProductType>(dto);

            var existingBrand = await repo.GetByIdAsync(new TypeByNameSpecification(mappedType.Name));
            if (existingBrand != null)
            {
                return Error.BadRequest("Type.AlreadyExists", $"Type with name {mappedType.Name} already exists.");
            }


            if (mappedType is null)
            {
                return Error.InternalServerError("Type.NotCreated", "Type Not Created");
            }

            await repo.AddAsync(mappedType);
            
            await unitOfWork.SaveChangesAsync();
            var mappedTypeDto = mapper.Map<TypeDto>(mappedType);
            return mappedTypeDto;
        }

        public async Task<Result<ProductDto>> CreateProductAsync(CreateProductDto createProductDto)
        {
            var repo = unitOfWork.GetRepository<Product, int>();
            var brandRepo = unitOfWork.GetRepository<ProductBrand, int>();
            var typeRepo = unitOfWork.GetRepository<ProductType, int>();

            //var imageUrl = await httpClient.GetAsync("/api/upload");





            var existingProduct = await repo.GetByIdAsync(new ProductByNameSpecification(createProductDto.Name));
            if (existingProduct != null)
            {
                return Error.BadRequest("Product.AlreadyExists", $"Product with name {createProductDto.Name} already exists.");
            }

            var brand = await brandRepo.GetByIdAsync(new BrandByNameSpecification(createProductDto.ProductBrand));
            if (brand is null)
                return Error.NotFound("Brand.NotFound", "Brand not found, create it first");

            var type = await typeRepo.GetByIdAsync(new TypeByNameSpecification(createProductDto.ProductType));
            if (type is null)
                return Error.NotFound("Type.NotFound", "Type not found, create it first");

            var mappedProduct = mapper.Map<Product>(createProductDto);
            mappedProduct.BrandId = brand.Id;
            mappedProduct.TypeId = type.Id;


            if (mappedProduct is null)
            {
                return Error.InternalServerError("Product.NotCreated", "Product Not Created");
            }


            await repo.AddAsync(mappedProduct);
          
            await unitOfWork.SaveChangesAsync();
            var mappedProductDto = mapper.Map<ProductDto>(mappedProduct);
            return mappedProductDto;
        }

        public async Task<IEnumerable<BrandDto>> GetAllBrandsAsync()
        {
            var repo = unitOfWork.GetRepository<ProductBrand, int>();
            var brands = await repo.GetAllAsync();

            if (brands is null)
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


            if (Products is null)
            {
                throw new ProductsNotFoundException();
            }


            var mappedProducts = mapper.Map<IEnumerable<ProductDto>>(Products);

            var CountofReturnedProducts = mappedProducts.Count();

            var CountSpec = new ProductCountSpecifications(queryPrams);
            var CountOfAllProducts = await repo.CountAsync(CountSpec);



            return new PaginatedResult<ProductDto>(queryPrams.PageIndex, queryPrams.PageSize, CountOfAllProducts, mappedProducts);
        }

        public async Task<IEnumerable<TypeDto>> GetAllTypesAsync()
        {
            var repo = unitOfWork.GetRepository<ProductType, int>();
            var Types = await repo.GetAllAsync();
            if (Types is null)
            {
                throw new TypesNotFoundException();
            }
            //map from Brand Entity to the dto BrandDto
            var mappedTypes = mapper.Map<IEnumerable<TypeDto>>(Types);

            return mappedTypes;
        }

        public async Task<Result<ProductDto>> GetProductByIdAsync(int id)
        {
            var repo = unitOfWork.GetRepository<Product, int>();
            var spec = new ProductWithBrandAndTypeSpecification(id);
            var Product = await repo.GetByIdAsync(spec);
            if (Product is null)
            {
                return Error.NotFound("Product.NotFound", $"Product With {id} is Not Found");

            }
            var mappedProduct = mapper.Map<ProductDto>(Product);
            //if (mappedProduct == null) return null;
            return mappedProduct;
        }

        // ---------------------------------------------------------------------
        // Deletions — Admin / SuperAdmin only (enforced at controller layer)
        // ---------------------------------------------------------------------

        public async Task<Result<bool>> DeleteProductAsync(int id)
        {
            var repo = unitOfWork.GetRepository<Product, int>();

            var product = await repo.GetByIdAsync(id);
            if (product is null)
                return Error.NotFound("Product.NotFound", $"Product with id {id} was not found.");

            repo.Remove(product);
            var affected = await unitOfWork.SaveChangesAsync();
            if (affected == 0)
                return Error.InternalServerError("Product.DeleteFailed", "Could not delete the product.");

            return Result<bool>.Ok(true);
        }

        public async Task<Result<bool>> DeleteBrandAsync(int id)
        {
            var brandRepo = unitOfWork.GetRepository<ProductBrand, int>();
            var productRepo = unitOfWork.GetRepository<Product, int>();

            var brand = await brandRepo.GetByIdAsync(id);
            if (brand is null)
                return Error.NotFound("Brand.NotFound", $"Brand with id {id} was not found.");

            // Block deletion if products still reference this brand — prevents a noisy FK exception
            // and gives the UI a clean, actionable message.
            var referencingProducts = await productRepo.CountAsync(new ProductByBrandSpecification(id));
            if (referencingProducts > 0)
                return Error.BadRequest(
                    "Brand.InUse",
                    $"Cannot delete brand '{brand.Name}' — {referencingProducts} product(s) still reference it. Reassign or delete those products first.");

            brandRepo.Remove(brand);
            var affected = await unitOfWork.SaveChangesAsync();
            if (affected == 0)
                return Error.InternalServerError("Brand.DeleteFailed", "Could not delete the brand.");

            return Result<bool>.Ok(true);
        }

        public async Task<Result<bool>> DeleteTypeAsync(int id)
        {
            var typeRepo = unitOfWork.GetRepository<ProductType, int>();
            var productRepo = unitOfWork.GetRepository<Product, int>();

            var type = await typeRepo.GetByIdAsync(id);
            if (type is null)
                return Error.NotFound("Type.NotFound", $"Product type with id {id} was not found.");

            var referencingProducts = await productRepo.CountAsync(new ProductByTypeSpecification(id));
            if (referencingProducts > 0)
                return Error.BadRequest(
                    "Type.InUse",
                    $"Cannot delete type '{type.Name}' — {referencingProducts} product(s) still reference it. Reassign or delete those products first.");

            typeRepo.Remove(type);
            var affected = await unitOfWork.SaveChangesAsync();
            if (affected == 0)
                return Error.InternalServerError("Type.DeleteFailed", "Could not delete the product type.");

            return Result<bool>.Ok(true);
        }
    }
}
