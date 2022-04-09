namespace Orders.Saga.Contracts.Messages;

public record ReserveStock
{
    public Guid OrderId { get; init; }
    public Guid UserId { get; init; }
}