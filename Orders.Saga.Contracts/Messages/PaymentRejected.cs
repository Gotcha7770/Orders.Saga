namespace Orders.Saga.Contracts.Messages;

public interface PaymentRejected
{
    Guid OrderId { get; }
    int ProductId { get; }
}