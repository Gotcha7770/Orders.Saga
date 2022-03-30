using MassTransit;
using Microsoft.EntityFrameworkCore;
using Orders.Saga.Contracts.Messages;
using StockService.Models;

namespace StockService.Consumers;

public class ReserveStockConsumer : IConsumer<ReserveStock>
{
    private readonly ApplicationDbContext _dbContext;

    public ReserveStockConsumer(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task Consume(ConsumeContext<ReserveStock> context)
    {
        var freeProduct = await _dbContext.Products.FirstOrDefaultAsync(x => x.State == ProductState.Free);
        if (freeProduct is null)
            throw new InvalidOperationException("No products left!");

        freeProduct.State = ProductState.Reserved;
        await _dbContext.SaveChangesAsync();
        
        await context.RespondAsync<StockReserved>(new
        {
            OrderId = context.Message.OrderId,
            UserId = context.Message.UserId,
            ProductId = freeProduct.Id
        });
    }
}