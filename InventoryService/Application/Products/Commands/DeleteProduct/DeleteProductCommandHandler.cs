using InventoryService.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Application.Products.Commands.DeleteProduct;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, bool>
{
    private readonly IAppDbContext _context;

    public DeleteProductCommandHandler(IAppDbContext context) => _context = context;

    public async Task<bool> Handle(DeleteProductCommand request, CancellationToken ct)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == request.Id, ct);

        if (product is null) return false;

        _context.Products.Remove(product); // Stock удалится каскадно
        await _context.SaveChangesAsync(ct);
        return true;
    }
}
