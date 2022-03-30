namespace Orders.Saga.Contracts.Messages;

public interface Checkout
{
    Guid OrderId { get; set; }
    Guid UserId { get; set; }
}