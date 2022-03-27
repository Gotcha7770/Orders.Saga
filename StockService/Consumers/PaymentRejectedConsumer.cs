using MassTransit;
using Microsoft.EntityFrameworkCore;
using Orders.Saga.Contracts.Messages;
using StockService.Models;

namespace StockService.Consumers;

// ReSharper disable once ClassNeverInstantiated.Global
public class PaymentRejectedConsumer : IConsumer<PaymentRejected>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IPublishEndpoint _publishEndpoint;

    public PaymentRejectedConsumer(ApplicationDbContext dbContext, IPublishEndpoint publishEndpoint)
    {
        _dbContext = dbContext;
        _publishEndpoint = publishEndpoint;
    }
    
    public async Task Consume(ConsumeContext<PaymentRejected> context)
    {
        var product = await _dbContext.Products.FirstOrDefaultAsync(x => x.Id == context.Message.ProductId);
        if (product is not null)
        {
            product.State = ProductState.Free;
            await _dbContext.SaveChangesAsync();
            await _publishEndpoint.Publish<StocksReleased>(new
            {
                OrderId = context.Message.OrderId
            });
        }
    }
}