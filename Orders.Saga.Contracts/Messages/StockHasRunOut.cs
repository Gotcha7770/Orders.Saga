namespace Orders.Saga.Contracts.Messages;

public interface StockHasRunOut
{
    Guid OrderId { get; }
}