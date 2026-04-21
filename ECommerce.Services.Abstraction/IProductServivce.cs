using ECommerce.SharedLibirary;
using ECommerce.SharedLibirary.CommonResult;
using ECommerce.SharedLibirary.DTO_s.ProductDtos;


namespace ECommerce.Services.Abstraction
{
    public interface IProductService
    {

        Task<PaginatedResult<ProductDto>> GetAllProductsAsync(ProductQueryPrams queryPrams);

        Task<Result<ProductDto>> GetProductByIdAsync(int id);


        Task<IEnumerable<BrandDto>> GetAllBrandsAsync();


        Task<IEnumerable<TypeDto>> GetAllTypesAsync();



        Task<Result<BrandDto>> CreateBrandAsync(CreateBrandDto dto);

        Task<Result<TypeDto>> CreateTypeAsync(CreateTypeDto dto);


        Task<Result<ProductDto>> CreateProductAsync(CreateProductDto createProductDto);


        // ----- Deletions (Admin / SuperAdmin only) -----

        /// <summary>
        /// Deletes a product by its id. Returns NotFound if the product does not exist.
        /// </summary>
        Task<Result<bool>> DeleteProductAsync(int id);

        /// <summary>
        /// Deletes a brand by its id. Blocks deletion if any products still reference the brand,
        /// since removing the row would violate the FK constraint on Product.BrandId.
        /// </summary>
        Task<Result<bool>> DeleteBrandAsync(int id);

        /// <summary>
        /// Deletes a product type by its id. Blocks deletion if any products still reference the type,
        /// since removing the row would violate the FK constraint on Product.TypeId.
        /// </summary>
        Task<Result<bool>> DeleteTypeAsync(int id);
    }
}
