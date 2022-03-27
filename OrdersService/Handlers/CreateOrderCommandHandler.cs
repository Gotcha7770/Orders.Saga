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
            Id = command.OrderId,
            UserId = command.UserId,
            State = OrderState.Pending,
            OrderDate = DateTimeOffset.Now.UtcDateTime
        };
        _dbContext.Orders.Add(order);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _publishEndpoint.Publish<OrderCreated>(new
        {
            OrderId = order.Id,
            UserId = order.UserId,
            Created = order.OrderDate
        },
            cancellationToken);

        return order;
    }
}