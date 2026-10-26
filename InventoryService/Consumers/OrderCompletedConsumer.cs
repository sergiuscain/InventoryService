using InventoryService.Application.Common.Interfaces;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using OrderService.Application.Orders.Events;

namespace InventoryService.Consumers;

public sealed class OrderCompletedConsumer : IConsumer<OrderCompletedEvent>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<OrderCompletedConsumer> _logger;

    public OrderCompletedConsumer(IAppDbContext context, ILogger<OrderCompletedConsumer> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderCompletedEvent> context)
    {
        var e = context.Message;

        var productIds = e.Items.Select(i => i.ProductId).Distinct().ToList();

        var stocks = await _context.Stocks
            .Where(s => productIds.Contains(s.ProductId))
            .ToListAsync(context.CancellationToken);

        var now = DateTime.UtcNow;
        foreach (var stock in stocks)
        {
            var requested = e.Items
                .Where(i => i.ProductId == stock.ProductId)
                .Sum(i => i.Quantity);

            // Защита от ухода в минус (повторное событие, гонка).
            var actual = Math.Min(requested, stock.QuantityReserved);

            stock.QuantityReserved -= actual;
            stock.UpdatedAt = now;
        }

        await _context.SaveChangesAsync(context.CancellationToken);

        _logger.LogInformation(
            "Committed reserve for order {OrderId}: {Count} items",
            e.OrderId, e.Items.Count);
    }
}