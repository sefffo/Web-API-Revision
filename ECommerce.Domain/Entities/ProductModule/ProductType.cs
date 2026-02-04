using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Domain.Entities.ProductModule
{
    public class ProductType : BaseEntity<int>
    {
        public string Name { get; set; } = null!;


        //nav props 


        public virtual IEnumerable<Product> Products { get; set; } = new List<Product>();
    }
}
