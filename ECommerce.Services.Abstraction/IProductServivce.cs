using ECommerce.SharedLibirary.DTO_s.ProductDtos;


namespace ECommerce.Services.Abstraction
{
    public interface IProductServivce
    {

        Task<IEnumerable<ProductDto>> GetAllProductsAsync();

        Task<ProductDto> GetProductByIdAsync(int id);


        Task<IEnumerable<BrandDto>> GetAllBrandsAsync();


        Task<IEnumerable<TypeDto>> GetAllTypesAsync();  





    }
}
