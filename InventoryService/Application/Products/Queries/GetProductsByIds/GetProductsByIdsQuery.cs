using InventoryService.Application.Products.Common;
using MediatR;

namespace InventoryService.Application.Products.Queries.GetProductsByIds;

public record GetProductsByIdsQuery(IReadOnlyList<Guid> Ids) : IRequest<IReadOnlyList<ProductDto>>;