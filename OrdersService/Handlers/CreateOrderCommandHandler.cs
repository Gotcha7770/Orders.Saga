using MassTransit;
using MediatR;
using Orders.Saga.Contracts.Messages;
using OrdersService.Commands;
using OrdersService.Models;

namespace OrdersService.Handlers;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Order>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IPublishEndpoint _publishEndpoint;

    public CreateOrderCommandHandler(ApplicationDbContext dbContext,IPublishEndpoint publishEndpoint)
    {
        _dbContext = dbContext;
        _publishEndpoint = publishEndpoint;
    }
    
    public async Task<Order> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = command.UserId,
            State = OrderState.Pending,
            OrderDate = DateTime.UtcNow
        };
        _dbContext.Orders.Add(order);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _publishEndpoint.Publish<OrderCreated>(new
        {
            OrderId = order.Id,
            UserId = order.UserId
        },
            cancellationToken);

        return order;
    }
}