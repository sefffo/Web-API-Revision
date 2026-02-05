using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using ECommerce.Persistence.Data.DbContexts;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Persistence.Repositories
{
    public class UnitOfWork(ApplicationDbContext context) : IUnitOfWork
    {

        private readonly Dictionary<Type, object> repositories = [];
        public IGenericRepository<TEntity, TKey> GetRepository<TEntity, TKey>() where TEntity : BaseEntity<TKey>
        {
            var EntityType = typeof(TEntity);
            if(repositories.TryGetValue(EntityType , out object? repository ))
            return (IGenericRepository<TEntity,TKey>) repository;


            var newRepository = new GenericRepository<TEntity, TKey>(context);
            repositories.Add(EntityType, newRepository);
            return newRepository;

        }




        public async Task<int> SaveChangesAsync()
           => await context.SaveChangesAsync();
        
    }
}
