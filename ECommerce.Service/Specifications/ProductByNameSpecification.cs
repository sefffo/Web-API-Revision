using ECommerce.Domain.Entities.ProductModule;

namespace ECommerce.Services.Specifications
{
    public class ProductByNameSpecification : BaseSpecification<Product, int>
    {
        public ProductByNameSpecification(string name) : base(criteriaExpression: p => p.Name.Trim() == name.Trim())
        {
        }
    }
}
