namespace Orders.Saga.Contracts.Messages;

public interface OrderCreated
{
    Guid OrderId { get; }
    Guid UserId { get; }
    DateTime Created { get; }
}