namespace Orders.Saga.Contracts.Messages;

public record OrderCompleted(Guid OrderId);