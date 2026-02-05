using ECommerce.Domain.Entities;
using ECommerce.Domain.Entities.ProductModule;
using ECommerce.Domain.Interfaces;
using ECommerce.Persistence.Data.DbContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ECommerce.Persistence.Data.DataSeed
{
    public class DataInitializer(ApplicationDbContext context) : IDataInitializer
    {
        public async Task Initialize()
        {
            try
            {
                var hasProducts = await context.Products.AnyAsync();
                var hasBrands = await context.ProductBrands.AnyAsync();
                var hasTypes = await context.ProductTypes.AnyAsync();
                if (hasProducts && hasBrands && hasTypes) return;

                if (!hasBrands)
                {
                    await SeedDataFromJson<ProductBrand, int>("brands.json", context.ProductBrands);

                }
                if (!hasTypes)
                {
                    await SeedDataFromJson<ProductType, int>("types.json", context.ProductTypes);

                    await context.SaveChangesAsync(); //products depend on brands and types, so we need to save them first before seeding products
                }
                if (!hasProducts)
                {

                    await SeedDataFromJson<Product, int>("products.json", context.Products);
                    await context.SaveChangesAsync(); // Save changes after seeding products

                }

            }
            catch (Exception ex)
            {
                // Log exception or handle it as needed
                throw new Exception("An error occurred while initializing the database.", ex);

            }
        }

        private async Task SeedDataFromJson<TEntity, Tkey>(string fileName, DbSet<TEntity> dbset) where TEntity : BaseEntity<Tkey>
        {
            //S:\Road to full stack\BackEnd\Studying\Rev on API\EComm Aliaa tarek\ECommerceSolution\ECommerce.Persistence\Data\DataSeed\JSONFiles\
            var filePath = @"..\ECommerce.Persistence\Data\DataSeed\JSONFiles\" + fileName;

            if (!File.Exists(filePath)) throw new FileNotFoundException ($"File {fileName} is not Found");

            try
            {
                //var data = File.ReadAllText(filePath); ==> bad practise for large files

                using var stream = File.OpenRead(filePath);
                var data = await JsonSerializer.DeserializeAsync<List<TEntity>>(stream, new JsonSerializerOptions() //3shan lw el data feha capital or small letters w 3shan lw feha camelCase aw pascalCase
                { PropertyNameCaseInsensitive = true });

                if (data == null) return;

                 await dbset.AddRangeAsync(data);
            }
            catch(Exception ex) 
            {
                // Log exception or handle it as needed
                throw new Exception($"An error occurred while seeding data from {fileName}. , {ex}");

            }


        }
    }
}