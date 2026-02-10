using ECommerce.Domain.Entities.ProductModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Services.Specifications
{
    internal class ProductWithBrandAndTypeSpecification : BaseSpecification<Product, int>
    {

        public ProductWithBrandAndTypeSpecification(int id) : base(p => p.Id == id)
        {
            AddInclude(p => p.ProductBrand);
            AddInclude(p => p.ProductType);
        }


        //get all products with their brands and types
        public ProductWithBrandAndTypeSpecification() : base(null)
        {
            AddInclude(p => p.ProductBrand);
            AddInclude(p => p.ProductType);
        }
    }
}
