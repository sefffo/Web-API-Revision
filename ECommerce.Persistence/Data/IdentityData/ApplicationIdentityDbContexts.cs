using ECommerce.Domain.Entities.IdentityModule;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Persistence.Data.IdentityData
{
    public class ApplicationIdentityDbContexts : IdentityDbContext<AppUser>
    {
        public ApplicationIdentityDbContexts(DbContextOptions<ApplicationIdentityDbContexts> options) : base(options)
        {




        }


        override protected void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            builder.Entity<Address>().ToTable("Addresses");


            builder.Entity<AppUser>().ToTable("Users");

            builder.Entity<IdentityRole>().ToTable("Roles");

            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");

            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");







            // Additional model configurations can be added here
            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationIdentityDbContexts).Assembly);
        }

        //public DbSet<Address> Addresses { get; set; }

    }
}
