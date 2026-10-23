using InventoryService.Application.Products.Common;
using MediatR;

namespace InventoryService.Application.Products.Queries.GetProductById;

public record GetProductByIdQuery(Guid Id) : IRequest<ProductDto?>;
