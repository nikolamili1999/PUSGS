
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderApi.Models;

namespace Backend.Infrastructure.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(s => s.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            
            builder.Property(s => s.Address)
                   .IsRequired();

            builder.Property(s => s.UTCTimeOrderCreated).IsRequired();

            builder.HasMany<OrderDetail>(s => s.OrderDetails)
                   .WithOne(g => g.Order)
                   .HasForeignKey(s => s.OrderId)
                   .IsRequired();
        }
    }
}
