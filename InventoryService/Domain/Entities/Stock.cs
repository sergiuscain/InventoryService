namespace InventoryService.Domain.Entities;

public class Stock
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public int QuantityAvailable { get; set; }
    public int QuantityReserved { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    //navigation
    public Product Product { get; set; } = null!;
}
