using InventoryService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Application.Common.Interfaces;

public interface IAppDbContext
{
    DbSet<Product> Products { get; }
    DbSet<Stock> Stocks { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
