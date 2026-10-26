namespace InventoryService.Events;

public record OrderCompletedEvent(
    Guid OrderId,
    IReadOnlyList<OrderCompletedItem> Items);

public record OrderCompletedItem(Guid ProductId, int Quantity);