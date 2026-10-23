using InventoryService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryService.Infrastructure.Persistence.Configurations;

public class StockConfiguration : IEntityTypeConfiguration<Stock>
{
    public void Configure(EntityTypeBuilder<Stock> builder)
    {
        builder.ToTable("Stocks");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.QuantityAvailable).IsRequired();
        builder.Property(s => s.QuantityReserved).IsRequired();

        builder.HasIndex(s => s.ProductId).IsUnique();
    }
}
