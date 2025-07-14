using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entities.OrderAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Config
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.OwnsOne(o => o.ShippingAddress, sa =>
            {
                sa.WithOwner();
            });
            builder.OwnsOne(o => o.PaymentSummary, ps =>
            {
                ps.WithOwner();
            });

            builder.Property(x => x.Status)
                .HasConversion(
                    o => o.ToString(), // Convert enum to string for storage
                    o => (OrderStatus)Enum.Parse(typeof(OrderStatus), o) // Convert string back to enum
                );
            builder.Property(x=>x.Subtotal)
                .HasColumnType("decimal(18,2)");
            builder.Property(x=>x.Discount)
                .HasColumnType("decimal(18,2)");
            builder.HasMany(x=>x.OrderItems).WithOne().OnDelete(DeleteBehavior.Cascade); // Ensure that deleting an order deletes its items
            builder.Property(x=>x.OrderDate).HasConversion(
                v => v.ToUniversalTime(), // Store as DateTime
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc) // Ensure UTC kind when reading
            );
        }
    }
}
