using MassTransit;
using MediatR;
using Orders.Saga.Contracts.Messages;
using OrdersService.Commands;

namespace OrdersService.Handlers;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Guid>
{
    private readonly IPublishEndpoint _publishEndpoint;

    public CreateOrderCommandHandler(ApplicationDbContext dbContext,IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }
    
    public async Task<Guid> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        var id = Guid.NewGuid();
        var message = new OrderCreated(Guid.NewGuid(), command.UserId);
        await _publishEndpoint.Publish(message, cancellationToken);

        return id;
    }
}