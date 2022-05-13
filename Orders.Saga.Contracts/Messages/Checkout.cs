namespace Orders.Saga.Contracts.Messages;

public record Checkout
{
    public Guid OrderId { get; init; }
    public Guid UserId { get; init; }
};