using MediatR;
using OrdersService.Models;

namespace OrdersService.Queries;

public record GetOrderQuery : IRequest<Order>
{
    public required Guid Id { get; init; }
}