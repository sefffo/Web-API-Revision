using ECommerce.Domain.Entities.ProductModule;

namespace ECommerce.Services.Specifications
{
    /// <summary>
    /// Matches every product that references the given brand id.
    /// Used before deleting a brand to detect FK conflicts.
    /// </summary>
    internal class ProductByBrandSpecification : BaseSpecification<Product, int>
    {
        public ProductByBrandSpecification(int brandId) : base(p => p.BrandId == brandId) { }
    }

    /// <summary>
    /// Matches every product that references the given product-type id.
    /// Used before deleting a type to detect FK conflicts.
    /// </summary>
    internal class ProductByTypeSpecification : BaseSpecification<Product, int>
    {
        public ProductByTypeSpecification(int typeId) : base(p => p.TypeId == typeId) { }
    }
}
