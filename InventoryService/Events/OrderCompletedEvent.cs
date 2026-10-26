namespace OrderService.Application.Orders.Events; // ← тот же, что в OrderService!

public record OrderCompletedEvent(
    Guid OrderId,
    IReadOnlyList<OrderCompletedItem> Items);

public record OrderCompletedItem(Guid ProductId, int Quantity);