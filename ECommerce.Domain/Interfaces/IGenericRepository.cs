using ECommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ECommerce.Domain.Interfaces
{
    public interface IGenericRepository<TEntity, Tkey> where TEntity : BaseEntity<Tkey>
    {

        Task<IEnumerable<TEntity>> GetAllAsync();


        Task<IEnumerable<TEntity>> GetAllAsync(ISpecifications<TEntity,Tkey>specifications);



        Task<TEntity> GetByIdAsync(ISpecifications<TEntity,Tkey> specifications);


        Task<TEntity> GetByIdAsync(Tkey id);

        Task AddAsync(TEntity entity);

        void Remove(TEntity entity);

        void Update(TEntity entity);

    }
}
