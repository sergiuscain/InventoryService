using MediatR;

namespace InventoryService.Application.Products.Commands.ReserveStock;

public record ReserveStockItem(Guid ProductId, int Quantity);

public record ReserveStockCommand(IReadOnlyList<ReserveStockItem> Items) : IRequest<bool>;