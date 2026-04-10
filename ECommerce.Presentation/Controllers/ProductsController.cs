using ECommerce.Presentation.Attributes;
using ECommerce.Services.Abstraction;
using ECommerce.SharedLibirary;
using ECommerce.SharedLibirary.CommonResult;
using ECommerce.SharedLibirary.DTO_s.ProductDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;



namespace ECommerce.Presentation.Controllers
{



    [Authorize]
    public class ProductsController(IProductService service) : ApiBaseController
    {

        [HttpGet]
        [RedisCache(10)]
        //baseUrl: api/products
        public async Task<ActionResult<PaginatedResult<ProductDto>>> GetAllProductsAsync([FromQuery] ProductQueryPrams queryPrams)
        {
            var products = await service.GetAllProductsAsync(queryPrams);

            //must be a response with status code 200 and the list of products in the body
            //on a working project the response must hava a certain form 
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


        [HttpPost("CreateBrand")]
        [Authorize (Roles = "Admin")]
        public async Task<IActionResult> CreateBrandAsync([FromBody] CreateBrandDto createBrandDto)
        {
            var createdBrand = await service.CreateBrandAsync(createBrandDto);
            if(!createdBrand.isSuccess)
            {
                return BadRequest(createdBrand.Errors);
            }
            return StatusCode(StatusCodes.Status201Created, createdBrand.value);
        }


        [HttpPost("CreateType")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateTypeAsync([FromBody] CreateTypeDto createTypeDto)
        {
            var createdType = await service.CreateTypeAsync(createTypeDto);
            if (!createdType.isSuccess)
            {
                return BadRequest(createdType.Errors);
            }
            return StatusCode(StatusCodes.Status201Created, createdType.value);
        }


        [HttpPost]
        [Authorize (Roles = "Admin")]
        public async Task<IActionResult> CreateProductAsync([FromBody] CreateProductDto createProductDto)
        { 
            
            Result<ProductDto>? createdProduct = await service.CreateProductAsync(createProductDto);

            if(!createdProduct.isSuccess)
            {
                return BadRequest(createdProduct.Errors);
            }

            return StatusCode(StatusCodes.Status201Created, createdProduct.value);
        }
    }
}
