using ECommerce.Presentation.Attributes;
using ECommerce.Services.Abstraction;
using ECommerce.SharedLibirary;
using ECommerce.SharedLibirary.DTO_s.ProductDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;



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

            return HandleResult<ProductDto>(product) ;

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
    }
}
 