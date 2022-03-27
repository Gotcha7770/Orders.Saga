using MediatR;
using OrdersService.Models;

namespace OrdersService.Commands;

public class CreateOrderCommand : IRequest<Order>
{
    public Guid OrderId { get; set; }
    
    public Guid UserId { get; set; }
}