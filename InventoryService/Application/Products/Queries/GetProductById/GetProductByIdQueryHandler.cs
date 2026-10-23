using InventoryService.Application.Common.Interfaces;
using InventoryService.Application.Products.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Application.Products.Queries.GetProductById;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto?>
{
    private readonly IAppDbContext _context;

    public GetProductByIdQueryHandler(IAppDbContext context) => _context = context;
    public async Task<ProductDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        return await _context.Products
            .AsNoTracking()
            .Where(p => p.Id == request.Id)
            .Select(p => new ProductDto(
                p.Id,
                p.Name,
                p.Sku,
                p.Price,
                p.CreatedAt,
                p.UpdatedAt,
                p.Stock != null ? p.Stock.QuantityAvailable : 0,
                p.Stock != null ? p.Stock.QuantityReserved : 0))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
