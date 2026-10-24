using MediatR;

namespace InventoryService.Application.Products.Commands.ReleaseStock;

public record ReleaseStockItem(Guid ProductId, int Quantity);

public record ReleaseStockCommand(IReadOnlyList<ReleaseStockItem> Items) : IRequest;