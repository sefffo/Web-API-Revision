using ECommerce.Services.Abstraction;
using ECommerce.SharedLibirary.DTO_s.ProductDtos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;



namespace ECommerce.Presentation.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController(IProductServivce servivce) : ControllerBase
    {

        [HttpGet]
        //baseUrl: api/products
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAllProductsAsync([FromQuery] int? brandId , [FromQuery] int? typeId)
        {
            var products = await servivce.GetAllProductsAsync(brandId , typeId);

            //must be a response with status code 200 and the list of products in the body
            //on a working project the response must hava a certain form 
            return Ok(products);
        }


        [HttpGet("{id}")]
        //baseurl : api/products/{id}
        public async Task<ActionResult<ProductDto>> GetProductByIdAsync(int id)
        {
            var product = await servivce.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);

        }

        //get all brands
        //baseUrl : api/products/brands
        [HttpGet("brands")]
        public async Task<ActionResult<IEnumerable<BrandDto>>> GetAllBrandsAsync()
        {
            var brands = await servivce.GetAllBrandsAsync();
            return Ok(brands);

        }

        [HttpGet("types")]
        ///baseUrl : api/products/types
        public async Task<ActionResult<IEnumerable<TypeDto>>> GetAllTypesAsync()
        {
            var types = await servivce.GetAllTypesAsync();
            return Ok(types);

        }
    }
}
 