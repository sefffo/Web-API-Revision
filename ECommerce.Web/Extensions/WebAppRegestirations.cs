using ECommerce.Domain.Interfaces;
using ECommerce.Persistence.Data.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Web.Extensions
{
    public static class WebAppRegestirations
    {
        public static WebApplication MigrateDatabase( this WebApplication app)
        {
            


            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            if (dbContext.Database.GetPendingMigrations().Any())
            {
                dbContext.Database.Migrate();
            }

            return app;
        }

        public static WebApplication SeedData(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var dataInitializer = scope.ServiceProvider.GetRequiredService<IDataInitializer>(); //to create initial data and auto migrations 
            dataInitializer.Initialize();
            return app;
        }
    }
}
