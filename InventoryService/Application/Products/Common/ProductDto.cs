namespace InventoryService.Application.Products.Common;

public record ProductDto(
    Guid Id,
    string Name,
    string Sku,
    decimal Price,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    int QuantityAvailable,
    int QuantityReserved);
