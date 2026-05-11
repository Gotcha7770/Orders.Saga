using JetBrains.Annotations;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Orders.Saga.Contracts.Messages;

namespace PaymentService.Consumers;

[UsedImplicitly]
public class CheckoutConsumer : IConsumer<Checkout>
{
    private readonly ApplicationDbContext _dbContext;

    public CheckoutConsumer(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task Consume(ConsumeContext<Checkout> context)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == context.Message.UserId);
        if (user is not { CanPay: true })
            throw new InvalidOperationException("User cannot pay for the order!");

        await context.RespondAsync<PaymentCompleted>(new
        {
            OrderId = context.Message.OrderId,
        });
    }
}