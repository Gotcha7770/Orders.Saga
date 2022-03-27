namespace OrdersService.Models;

public enum OrderState
{
    Pending,
    Completed,
    Rejected
}

public class Order
{
    public Guid Id { get; init; }
    public Guid UserId { get; set; }
    public OrderState State { get; set; }
    public DateTime OrderDate { get; set; }
};