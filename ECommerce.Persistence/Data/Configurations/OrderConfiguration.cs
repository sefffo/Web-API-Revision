using ECommerce.Domain.Entities.OrderModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Persistence.Data.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.Property(o => o.UserEmail).HasMaxLength(100);
            builder.Property(o => o.OrderDate).HasDefaultValueSql("GETUTCDATE()"); //set default value to current UTC date and time when a new order is created
            builder.Property(o => o.Subtotal).HasPrecision(18, 2); //configure the precision and scale for the Subtotal property to ensure accurate storage of decimal values
            builder.OwnsOne(o => o.ShippingAddress, sa =>
            {
                sa.Property(a => a.FirstName).HasMaxLength(100);
                sa.Property(a => a.LastName).HasMaxLength(20);
                sa.Property(a => a.Street).HasMaxLength(200);
                sa.Property(a => a.City).HasMaxLength(100);
                sa.Property(a => a.Country).HasMaxLength(50);
            });


            // //configure the one-to-many relationship between Order and OrderItem
            //builder.HasMany(o => o.Items)
            //       .WithOne(oi => oi.Order)
            //       .HasForeignKey(oi => oi.OrderId)
            //       .OnDelete(DeleteBehavior.Cascade); //when an order is deleted, its associated order items will also be deleted
            // //configure the many-to-one relationship between Order and DeliveryMethod
            //builder.HasOne(o => o.DeliveryMethod)
            //       .WithMany(dm => dm.Orders)
            //       .HasForeignKey(o => o.DeliveryMethodId);
        }
    }
}
