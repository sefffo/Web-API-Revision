using ECommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Domain.Interfaces
{
    public interface IGenericRepository<TEntity,Tkey> where TEntity : BaseEntity<Tkey>
    {

        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity> GetByIdAsync(Tkey id);

        Task AddAsync(TEntity entity);

        void Remove(TEntity entity);

        void Update(TEntity entity);

    }
}
