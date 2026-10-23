using InventoryService.Application.Common.Interfaces;
using InventoryService.Domain.Entities;
using MediatR;

namespace InventoryService.Application.Products.Commands.CreateProduct;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
{
    private readonly IAppDbContext _context;
    public CreateProductCommandHandler(IAppDbContext context) => _context = context;

    public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Sku = request.Sku,
            Price = request.Price,
            CreatedAt = now,
            UpdatedAt = now
        };

        var stock = new Stock
        {
            Id = Guid.NewGuid(),
            ProductId = product.Id,
            QuantityAvailable = request.InitialQuantity,
            QuantityReserved = 0,
            CreatedAt = now,
            UpdatedAt = now
        };

        _context.Products.Add(product);
        _context.Stocks.Add(stock);

        await _context.SaveChangesAsync(cancellationToken);

        return product.Id;
    }
}
