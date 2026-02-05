using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using ECommerce.Persistence.Data.DbContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Persistence.Repositories
{
    public class GenericRepository<TEntity, Tkey>(ApplicationDbContext context) : IGenericRepository<TEntity, Tkey> where TEntity : BaseEntity<Tkey>
    {

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            var result = await context.Set<TEntity>().ToListAsync();

            return result;
        }

        public async Task<TEntity> GetByIdAsync(Tkey id)
            => await context.Set<TEntity>().FindAsync(id);

        public async Task AddAsync(TEntity entity)
            => await context.Set<TEntity>().AddAsync(entity); 
        

        public void Remove(TEntity entity)
        => context.Set<TEntity>().Remove(entity);

        public void Update(TEntity entity)
        => context.Set<TEntity>().Update(entity);
    }
}
