namespace Orders.Saga.Contracts.Messages;

public record PaymentCompleted(Guid OrderId);