using MassTransit;
using Microsoft.EntityFrameworkCore;
using Orders.Saga.Contracts.Messages;

namespace PaymentService.Consumers;

// ReSharper disable once ClassNeverInstantiated.Global
public class StockReservedConsumer : IConsumer<StockReserved>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IPublishEndpoint _publishEndpoint;

    public StockReservedConsumer(ApplicationDbContext dbContext, IPublishEndpoint publishEndpoint)
    {
        _dbContext = dbContext;
        _publishEndpoint = publishEndpoint;
    }
    
    public async Task Consume(ConsumeContext<StockReserved> context)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == context.Message.UserId);
        if (user is not {CanPay: true})
        {
            await _publishEndpoint.Publish<PaymentRejected>(new
            {
                OrderId = context.Message.OrderId,
                ProductId = context.Message.ProductId
            });
        }
        else
        {
            await _publishEndpoint.Publish<PaymentCompleted>(new
            {
                OrderId = context.Message.OrderId,
            });
        }
    }
}