using InventoryService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryService.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Sku)
            .IsRequired()
            .HasMaxLength(64);

        builder.HasIndex(p => p.Sku).IsUnique();

        builder.Property(p => p.Price)
            .HasPrecision(18, 2);

        builder.HasOne(p => p.Stock)
            .WithOne(s => s.Product)
            .HasForeignKey<Stock>(s => s.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
