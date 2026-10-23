using InventoryService.Application.Products.Common;
using MediatR;

namespace InventoryService.Application.Products.Queries.GetProducts;

public record GetProductsQuery(int Page = 1, int PageSize = 20) : IRequest<IReadOnlyList<ProductDto>>;