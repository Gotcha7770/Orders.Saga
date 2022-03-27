using MassTransit;
using Microsoft.EntityFrameworkCore;
using Orders.Saga.Contracts.Messages;
using StockService.Models;

namespace StockService.Consumers;

// ReSharper disable once ClassNeverInstantiated.Global
public class OrderCreatedConsumer : IConsumer<OrderCreated>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IPublishEndpoint _publishEndpoint;

    public OrderCreatedConsumer(ApplicationDbContext dbContext, IPublishEndpoint publishEndpoint)
    {
        _dbContext = dbContext;
        _publishEndpoint = publishEndpoint;
    }

    public async Task Consume(ConsumeContext<OrderCreated> context)
    {
        var freeProduct = await _dbContext.Products.FirstOrDefaultAsync(x => x.State == ProductState.Free);
        if (freeProduct is null)
        {
            await _publishEndpoint.Publish<StockHasRunOut>(new
            {
                OrderId = context.Message.OrderId
            });
        }
        else
        {
            freeProduct.State = ProductState.Reserved;
            await _dbContext.SaveChangesAsync();

            await _publishEndpoint.Publish<StockReserved>(new
            {
                OrderId = context.Message.OrderId,
                ProductId = freeProduct.Id
            });
        }
    }
}