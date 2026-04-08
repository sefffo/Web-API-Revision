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



        

    }
}
