namespace Orders.Saga.Models;

public record Order(Guid Id, Guid UserId, DateTime OrderDate);