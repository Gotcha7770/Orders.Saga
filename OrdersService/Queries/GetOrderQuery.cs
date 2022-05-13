using MediatR;
using OrdersService.OrderSaga;

namespace OrdersService.Queries;

public class GetOrderQuery : IRequest<Order>
{
    public Guid Id { get; }

    public GetOrderQuery(Guid id)
    {
        Id = id;
    }
}