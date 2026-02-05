
using ECommerce.Domain.Interfaces;
using ECommerce.Persistence.Data.DataSeed;
using ECommerce.Persistence.Data.DbContexts;
using ECommerce.Web.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ECommerce.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);



            #region Registers

            builder.Services.AddScoped<IDataInitializer, DataInitializer>();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))); 


            #endregion
            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }




            #region Data Seeding

            //created a seperation of concerns by moving the migration and seeding logic to extension methods
            //open closed principle: we can add new functionality without modifying the existing code, we just need to create new extension methods for the new functionality and call them in the program.cs file
            //by adding any extention method we can enhance the WebApplication class without modifying its source code 

            await app.MigrateDatabaseAsync();

            await app.SeedDataAsync();

            #endregion

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
