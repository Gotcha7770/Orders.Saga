using MediatR;
using Orders.Saga.Models;

namespace Orders.Saga.Commands;

public class CreateOrderCommand : IRequest<Order>
{
    public Guid OrderId { get; set; }
    
    public Guid UserId { get; set; }
}