using MediatR;

namespace OrdersService.Commands;

public class CreateOrderCommand : IRequest<Guid>
{
    public Guid UserId { get; set; }
}