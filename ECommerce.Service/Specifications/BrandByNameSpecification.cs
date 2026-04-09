using ECommerce.Domain.Entities.ProductModule;

namespace ECommerce.Services.Specifications
{
    public class BrandByNameSpecification : BaseSpecification<ProductBrand, int>
    {
        public BrandByNameSpecification(string name) : base(criteriaExpression: b => b.Name.Trim() == name.Trim()) //==>
                                                                                                                   //we used trim as my DB is Case insensitive and to ignore any spaces before or after the name
                                                                                                                   //so we can benefit from the DB Indexing and make the search faster

        {
        }
    }
}
