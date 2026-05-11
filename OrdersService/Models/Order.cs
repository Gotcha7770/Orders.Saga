namespace OrdersService.Models;

public class Order
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public OrderState State { get; set; }
    public DateTime OrderDate { get; init; }
};