using InventoryService.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Application.Products.Commands.ReserveStock;

public class ReserveStockCommandHandler : IRequestHandler<ReserveStockCommand, bool>
{
    private readonly IAppDbContext _context;

    public ReserveStockCommandHandler(IAppDbContext context) => _context = context;

    public async Task<bool> Handle(ReserveStockCommand request, CancellationToken ct)
    {
        if (request.Items is null || request.Items.Count == 0)
            return false;

        var quantities = request.Items
            .GroupBy(i => i.ProductId)
            .ToDictionary(g => g.Key, g => g.Sum(x => x.Quantity));

        if (quantities.Values.Any(q => q <= 0))
            return false;

        var productIds = quantities.Keys.ToList();

        var stocks = await _context.Stocks
            .Where(s => productIds.Contains(s.ProductId))
            .ToListAsync(ct);

        if (stocks.Count != productIds.Count)
            return false;

        foreach (var stock in stocks)
            if (stock.QuantityAvailable < quantities[stock.ProductId])
                return false;

        var now = DateTime.UtcNow;
        foreach (var stock in stocks)
        {
            var need = quantities[stock.ProductId];
            stock.QuantityAvailable -= need;
            stock.QuantityReserved += need;
            stock.UpdatedAt = now;
        }

        await _context.SaveChangesAsync(ct);
        return true;
    }
}