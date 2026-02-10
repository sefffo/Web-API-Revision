using ECommerce.Domain.Entities.ProductModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Services.Specifications
{
    internal class ProductWithBrandAndTypeSpecification : BaseSpecification<Product, int>
    {


        //adding filter to get a specific product with its brand and type
       



        public ProductWithBrandAndTypeSpecification(int id) : base(p => p.Id == id)
        {
            AddInclude(p => p.ProductBrand);
            AddInclude(p => p.ProductType);
        }


        //get all products with their brands and types
        public ProductWithBrandAndTypeSpecification(int? brandId, int? typeId) :
            base(
                //bid is null 
                //tid is null
                //both are not null
                p=> (!brandId.HasValue || p.BrandId == brandId.Value) &&
                    (!typeId.HasValue || p.TypeId == typeId.Value)

            )
        {
            AddInclude(p => p.ProductBrand);
            AddInclude(p => p.ProductType);
        }
    }
}
