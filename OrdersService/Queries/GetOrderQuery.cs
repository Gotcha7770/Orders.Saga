using MediatR;
using OrdersService.Models;

namespace OrdersService.Queries;

public class GetOrderQuery : IRequest<Order>
{
    public Guid Id { get; }

    public GetOrderQuery(Guid id)
    {
        Id = id;
    }
}