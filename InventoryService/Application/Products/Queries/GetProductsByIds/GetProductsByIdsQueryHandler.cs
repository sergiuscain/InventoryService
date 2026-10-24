using InventoryService.Application.Common.Interfaces;
using InventoryService.Application.Products.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Application.Products.Queries.GetProductsByIds;

public class GetProductsByIdsQueryHandler
    : IRequestHandler<GetProductsByIdsQuery, IReadOnlyList<ProductDto>>
{
    private readonly IAppDbContext _context;

    public GetProductsByIdsQueryHandler(IAppDbContext context) => _context = context;

    public async Task<IReadOnlyList<ProductDto>> Handle(
        GetProductsByIdsQuery request, CancellationToken ct)
    {
        if (request.Ids is null || request.Ids.Count == 0)
            return Array.Empty<ProductDto>();

        var ids = request.Ids.Distinct().ToList();

        return await _context.Products
            .AsNoTracking()
            .Where(p => ids.Contains(p.Id))
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