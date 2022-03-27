namespace Orders.Saga.Contracts.Messages;

public interface StocksReleased
{
    Guid OrderId { get; }
}