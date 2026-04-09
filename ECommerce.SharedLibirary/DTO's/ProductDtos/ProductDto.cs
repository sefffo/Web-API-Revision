using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.SharedLibirary.DTO_s.ProductDtos
{
    public class ProductDto
    {

        //data transfer  object for product entity from layer to another layer

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;

        public string PictureUrl { get; set; } = null!;
        public decimal Price { get; set; }



        public string ProductType { get; set; } = null!;
        public string ProductBrand { get; set; } = null!;   
    }
}
