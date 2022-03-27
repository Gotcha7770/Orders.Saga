using MediatR;
using Orders.Saga.Commands;
using Orders.Saga.Models;

namespace Orders.Saga.Handlers;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Order>
{
    private readonly ApplicationDbContext _dbContext;

    public CreateOrderCommandHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<Order> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        var order = new Order(command.OrderId, command.UserId, DateTimeOffset.Now.UtcDateTime);
        _dbContext.Orders.Add(order);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return order;
    }
}