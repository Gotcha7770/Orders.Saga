using MediatR;

namespace Orders.Saga.Commands;

public class CreateOrderCommand : IRequest<Guid>
{
    public Guid Id { get; }

    public CreateOrderCommand(Guid id)
    {
        Id = id;
    }
}