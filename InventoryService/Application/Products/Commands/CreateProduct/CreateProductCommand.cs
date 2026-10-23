using MediatR;

namespace InventoryService.Application.Products.Commands.CreateProduct;

public record CreateProductCommand(
    string Name,
    string Sku,
    decimal Price,
    int InitialQuantity) : IRequest<Guid>;
