using ECommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Formats.Tar;
using System.Linq.Expressions;
using System.Text;

namespace ECommerce.Domain.Interfaces
{
    public interface ISpecifications<TEntity, Tkey> where TEntity : BaseEntity<Tkey>
    {
        public ICollection<Expression<Func<TEntity, object>>> IncludeExplressions { get; }

        public Expression<Func<TEntity, bool>> Criteria { get; }




    }
}
