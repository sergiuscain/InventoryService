using MediatR;

namespace InventoryService.Application.Products.Commands.UpdateProduct;

public record UpdateProductCommand(
    Guid Id,
    string Name,
    decimal Price) : IRequest<bool>;
