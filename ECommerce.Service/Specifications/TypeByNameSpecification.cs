using ECommerce.Domain.Entities.ProductModule;

namespace ECommerce.Services.Specifications
{
    public class TypeByNameSpecification : BaseSpecification<ProductType, int>
    {
        public TypeByNameSpecification(string name) : base(criteriaExpression: t => t.Name.Trim() == name.Trim())//==>
                                                                    //we used trim as my DB is Case insensitive and to ignore any spaces before or after the name
                                                                    //so we can benefit from the DB Indexing and make the search faster
        {
        }
    }
}
