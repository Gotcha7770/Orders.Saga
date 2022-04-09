namespace Orders.Saga.Contracts.Messages;

public record StockReserved
{
    public Guid OrderId { get; init; }
    public Guid UserId { get; init; }
    public int ProductId { get; init; }
}