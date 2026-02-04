using ECommerce.Domain.Entities.ProductModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Persistence.Data.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.Property(p => p.Name).HasMaxLength(100);
            builder.Property(p => p.Description).HasMaxLength(500);
            builder.Property(p => p.PictureUrl).HasMaxLength(200);
            builder.Property(p => p.Price).HasPrecision(18,2);


            builder.HasOne(p => p.ProductBrand)
                   .WithMany(p=>p.Products)
                   .HasForeignKey(p => p.BrandId);


            builder.HasOne(p => p.ProductType)
                .WithMany(p=>p.Products)
                .HasForeignKey(p => p.TypeId);
        }
    }
}
