using InventoryService.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Application.Products.Commands.ReleaseStock;

public class ReleaseStockCommandHandler : IRequestHandler<ReleaseStockCommand>
{
    private readonly IAppDbContext _context;

    public ReleaseStockCommandHandler(IAppDbContext context) => _context = context;

    public async Task Handle(ReleaseStockCommand request, CancellationToken ct)
    {
        if (request.Items is null || request.Items.Count == 0)
            return;

        var quantities = request.Items
            .GroupBy(i => i.ProductId)
            .ToDictionary(g => g.Key, g => g.Sum(x => x.Quantity));

        var productIds = quantities.Keys.ToList();

        var stocks = await _context.Stocks
            .Where(s => productIds.Contains(s.ProductId))
            .ToListAsync(ct);

        var now = DateTime.UtcNow;
        foreach (var stock in stocks)
        {
            var requested = quantities[stock.ProductId];
            // Защита от ухода в минус (повторный release, гонки).
            var actual = Math.Min(requested, stock.QuantityReserved);

            stock.QuantityReserved -= actual;
            stock.QuantityAvailable += actual;
            stock.UpdatedAt = now;
        }

        await _context.SaveChangesAsync(ct);
    }
}