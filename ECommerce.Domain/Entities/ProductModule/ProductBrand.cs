using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Domain.Entities.ProductModule
{
    public class ProductBrand : BaseEntity<int>
    {
        public string Name { get; set; } = null!;


        //navigational props 

        public virtual IEnumerable<Product> Products { get; set; } = new List<Product>();   
    }
}
