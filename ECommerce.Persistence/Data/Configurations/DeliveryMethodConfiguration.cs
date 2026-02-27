using ECommerce.Domain.Entities.OrderModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Persistence.Data.Configurations
{
    public class DeliveryMethodConfiguration : IEntityTypeConfiguration<DeliveryMethod>
    {
        public void Configure(EntityTypeBuilder<DeliveryMethod> builder)
        {
            builder.Property(dm => dm.ShortName).HasMaxLength(50);
            builder.Property(dm => dm.DeliveryTime).HasMaxLength(50);
            builder.Property(dm => dm.Description).HasMaxLength(500);
            builder.Property(dm => dm.Price).HasPrecision(18, 2);

            //relationships


        }
    }
}
