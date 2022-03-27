namespace Orders.Saga.Contracts.Messages;

public interface PaymentCompleted
{
    Guid OrderId { get; }
}