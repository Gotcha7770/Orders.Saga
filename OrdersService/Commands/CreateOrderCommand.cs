using MediatR;
using OrdersService.Models;

namespace OrdersService.Commands;

public record CreateOrderCommand : IRequest<Order>
{
    public required Guid OrderId { get; init; }
    
    public required Guid UserId { get; init; }
}