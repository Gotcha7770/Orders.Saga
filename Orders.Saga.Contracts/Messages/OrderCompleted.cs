namespace Orders.Saga.Contracts.Messages;

public record OrderCompleted
{
    public Guid OrderId { get; init; }
    
    public DateTime Completed { get; init; }
}