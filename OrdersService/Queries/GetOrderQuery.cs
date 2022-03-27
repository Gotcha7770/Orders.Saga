using MediatR;

namespace OrdersService.Queries;

public class GetOrderQuery : IRequest
{
    public Guid Id { get; }

    public GetOrderQuery(Guid id)
    {
        Id = id;
    }
}