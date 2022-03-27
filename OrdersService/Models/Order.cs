namespace Orders.Saga.Models;

public enum OrderState
{
    Pending,
    Completed,
    Rejected
}

public record Order(Guid Id, Guid UserId, OrderState State, DateTime OrderDate);