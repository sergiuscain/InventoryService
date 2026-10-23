using InventoryService.Application.Common.Interfaces;
using InventoryService.Application.Products.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Application.Products.Queries.GetProducts;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, IReadOnlyList<ProductDto>>
{
    private readonly IAppDbContext _context;

    public GetProductsQueryHandler(IAppDbContext context) => _context = context;
    public async Task<IReadOnlyList<ProductDto>> Handle(GetProductsQuery request, CancellationToken ct)
    {
        var page = request.Page < 1 ? 1 : request.Page;
        var size = request.PageSize is < 1 or > 100 ? 20 : request.PageSize;

        return await _context.Products
            .AsNoTracking()
            .OrderBy(p => p.Name)
            .Skip((page - 1) * size)
            .Take(size)
            .Select(p => new ProductDto(
                p.Id,
                p.Name,
                p.Sku,
                p.Price,
                p.CreatedAt,
                p.UpdatedAt,
                p.Stock != null ? p.Stock.QuantityAvailable : 0,
                p.Stock != null ? p.Stock.QuantityReserved : 0))
            .ToListAsync(ct);
    }
}
