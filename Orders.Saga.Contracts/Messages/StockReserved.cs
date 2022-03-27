namespace Orders.Saga.Contracts.Messages;

public interface StockReserved
{
    Guid OrderId { get; }
    Guid UserId { get; }
    int ProductId { get; }
}