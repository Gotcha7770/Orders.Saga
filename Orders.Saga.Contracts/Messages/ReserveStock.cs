namespace Orders.Saga.Contracts.Messages;

public interface ReserveStock
{
    Guid OrderId { get; }
    Guid UserId { get; }
}