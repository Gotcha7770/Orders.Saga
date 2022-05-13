namespace Orders.Saga.Contracts.Messages;

public record OrderCreated(Guid OrderId, Guid UserId);