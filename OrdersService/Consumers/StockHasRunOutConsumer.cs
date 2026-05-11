using JetBrains.Annotations;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Orders.Saga.Contracts.Messages;
using OrdersService.Models;

namespace OrdersService.Consumers;

[UsedImplicitly]
public class StockHasRunOutConsumer : IConsumer<StockHasRunOut>
{
    private readonly ApplicationDbContext _dbContext;

    public StockHasRunOutConsumer(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task Consume(ConsumeContext<StockHasRunOut> context)
    {
        var order = await _dbContext.Orders.FirstOrDefaultAsync(x => x.Id == context.Message.OrderId);
        if (order is not null)
        {
            order.State = OrderState.Rejected;
            await _dbContext.SaveChangesAsync();
        }
    }
}