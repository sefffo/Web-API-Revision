using ECommerce.Domain.Entities.ProductModule;
using ECommerce.Presentation.Attributes;
using ECommerce.Services.Abstraction;
using ECommerce.SharedLibirary;
using ECommerce.SharedLibirary.CommonResult;
using ECommerce.SharedLibirary.DTO_s.ProductDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;



namespace ECommerce.Presentation.Controllers
{



    public class ProductsController(IProductService service) : ApiBaseController
    {

        [HttpGet]
        [RedisCache(10)]
        //baseUrl: api/products
        public async Task<ActionResult<PaginatedResult<ProductDto>>> GetAllProductsAsync([FromQuery] ProductQueryPrams queryPrams)
        {
            var products = await service.GetAllProductsAsync(queryPrams);

            //must be a response with status code 200 and the list of products in the body
            //on a working project the response must have a certain form 
            return Ok(products);
        }


        [HttpGet("{id}")]
        //[RedisCache(10)]
        //baseurl : api/products/{id}
        public async Task<ActionResult<ProductDto>> GetProductByIdAsync(int id)
        {
            var product = await service.GetProductByIdAsync(id);

            return HandleResult<ProductDto>(product);

        }

        //get all brands
        //baseUrl : api/products/brands
        [HttpGet("brands")]
        [RedisCache]
        public async Task<ActionResult<IEnumerable<BrandDto>>> GetAllBrandsAsync()
        {
            var brands = await service.GetAllBrandsAsync();
            return Ok(brands);

        }

        [HttpGet("types")]
        [RedisCache]
        ///baseUrl : api/products/types
        public async Task<ActionResult<IEnumerable<TypeDto>>> GetAllTypesAsync()
        {
            var types = await service.GetAllTypesAsync();
            return Ok(types);

        }


        [HttpPost("brands")]
        [Authorize(Roles = "Admin,SuperAdmin")]

        public async Task<IActionResult> CreateBrandAsync([FromBody] CreateBrandDto createBrandDto)
        {
            var createdBrand = await service.CreateBrandAsync(createBrandDto);
            if(!createdBrand.isSuccess)
            {
                return HandleResult<BrandDto>(createdBrand);
            }

            await InvalidateProductsCacheAsync();

            return StatusCode(StatusCodes.Status201Created, createdBrand.value);
        }


        [HttpPost("types")]
        [Authorize(Roles = "Admin,SuperAdmin")]

        public async Task<IActionResult> CreateTypeAsync([FromBody] CreateTypeDto createTypeDto)
        {
            var createdType = await service.CreateTypeAsync(createTypeDto);
            if (!createdType.isSuccess)
            {
                return HandleResult<TypeDto>(createdType);
            }

            await InvalidateProductsCacheAsync();

            return StatusCode(StatusCodes.Status201Created, createdType.value);
        }


        [HttpPost]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> CreateProductAsync([FromBody] CreateProductDto createProductDto)
        { 
            
            Result<ProductDto>? createdProduct = await service.CreateProductAsync(createProductDto);

            if(!createdProduct.isSuccess)
            {
                return HandleResult<ProductDto>(createdProduct);
            }

            await InvalidateProductsCacheAsync();

            return StatusCode(StatusCodes.Status201Created, createdProduct.value);
        }


        // -------------------------------------------------------------------
        //  DELETES  (Admin / SuperAdmin only)
        // -------------------------------------------------------------------

        /// <summary>Deletes a product by its id. Invalidates cached listings.</summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> DeleteProductAsync(int id)
        {
            var result = await service.DeleteProductAsync(id);
            if (!result.isSuccess)
                return HandleResult<bool>(result);

            await InvalidateProductsCacheAsync();
            return NoContent();
        }

        /// <summary>Deletes a brand. Blocks if any products still reference the brand.</summary>
        [HttpDelete("brands/{id}")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> DeleteBrandAsync(int id)
        {
            var result = await service.DeleteBrandAsync(id);
            if (!result.isSuccess)
                return HandleResult<bool>(result);

            await InvalidateProductsCacheAsync();
            return NoContent();
        }

        /// <summary>Deletes a product type. Blocks if any products still reference the type.</summary>
        [HttpDelete("types/{id}")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> DeleteTypeAsync(int id)
        {
            var result = await service.DeleteTypeAsync(id);
            if (!result.isSuccess)
                return HandleResult<bool>(result);

            await InvalidateProductsCacheAsync();
            return NoContent();
        }

        /// <summary>
        /// Evicts every cached product / brand / type listing across all users.
        /// The RedisCacheAttribute appends "|user-{email}" and query string parts to each key,
        /// so we must use pattern matching to clear every variant at once.
        /// </summary>
        private async Task InvalidateProductsCacheAsync()
        {
            var cache = HttpContext.RequestServices.GetRequiredService<ICacheService>();
            try
            {
                await cache.RemoveByPatternAsync("/api/Products*");
            }
            catch
            {
                // cache eviction failures must never break the write
            }
        }
    }
}
