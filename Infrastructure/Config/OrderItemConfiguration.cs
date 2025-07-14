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
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.Property(x => x.Price)
                .HasColumnType("decimal(18,2)"); // تحديد نوع العمود كسعر عشري مع دقتين عشريتين

            builder.OwnsOne(x => x.ItemOrdered, itemOrdered =>
            {
                itemOrdered.WithOwner(); // تحديد أن ItemOrdered هو كائن مملوك
            });
        }
    }
}
