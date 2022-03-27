using MassTransit;
using Microsoft.EntityFrameworkCore;
using Orders.Saga.Contracts.Messages;
using OrdersService.Models;

namespace OrdersService.Consumers;

public class PaymentCompletedConsumer : IConsumer<PaymentCompleted>
{
    private readonly ApplicationDbContext _dbContext;

    public PaymentCompletedConsumer(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task Consume(ConsumeContext<PaymentCompleted> context)
    {
        var order = await _dbContext.Orders.FirstOrDefaultAsync(x => x.Id == context.Message.OrderId);
        if (order is not null)
        {
            order.State = OrderState.Completed;
            await _dbContext.SaveChangesAsync();
        }
    }
}