using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderApi.Models;

namespace Backend.Infrastructure.Configurations
{
    public class OrderDetailConfiguration : IEntityTypeConfiguration<OrderDetail>
    {
        public void Configure(EntityTypeBuilder<OrderDetail> builder)
        {
            builder.HasKey(s => s.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(s => s.Quantity).IsRequired();
            builder.Property(s => s.ProductPrice).IsRequired();
            builder.HasOne(s => s.Order)
                   .WithMany(ad => ad.OrderDetails)
                   .HasForeignKey(ad => ad.OrderId);
        }
    }
}
