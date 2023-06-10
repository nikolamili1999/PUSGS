using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductApi.Models;

namespace ProductApi.Infrastructure.Configuration
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(s => s.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(s => s.Price).IsRequired();
            builder.Property(s => s.Seller).IsRequired();

            builder.Property(s => s.Name).IsRequired();

            builder.Property(s => s.Price).IsRequired();
            builder.Property(s => s.Quantity).IsRequired();
            builder.Property(s => s.IsDeleted).IsRequired();
        }
    }
}
