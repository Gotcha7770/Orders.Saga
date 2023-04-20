using MassTransit;
using Orders.Saga.Contracts.Messages;
using PaymentService.Models;

namespace PaymentService.Consumers;

// ReSharper disable once ClassNeverInstantiated.Global
public class CheckoutConsumer : IConsumer<Checkout>
{
    private readonly ApplicationDbContext _dbContext;

    public CheckoutConsumer(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task Consume(ConsumeContext<Checkout> context)
    {
        var user = await _dbContext.FindAsync<User>(context.Message.UserId);
        if (user is not { CanPay: true })
            throw new InvalidOperationException("User cannot pay for the order!");

        await context.RespondAsync(new PaymentCompleted(context.Message.OrderId));
    }
}