using MassTransit;
using Microsoft.EntityFrameworkCore;
using Orders.Saga.Contracts.Messages;
using OrdersService.Models;

namespace OrdersService.Consumers;

public class StocksReleasedConsumer : IConsumer<StocksReleased>
{
    private readonly ApplicationDbContext _dbContext;

    public StocksReleasedConsumer(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task Consume(ConsumeContext<StocksReleased> context)
    {
        var order = await _dbContext.Orders.FirstOrDefaultAsync(x => x.Id == context.Message.OrderId);
        if (order is not null)
        {
            order.State = OrderState.Rejected;
            await _dbContext.SaveChangesAsync();
        }
    }
}