using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;

namespace OrdersService.OrderSaga;

public class OrderSagaDbContext : SagaDbContext
{
    public OrderSagaDbContext(DbContextOptions<OrderSagaDbContext> options)
        : base(options)
    {
    }

    protected override IEnumerable<ISagaClassMap> Configurations
    {
        get
        {
            yield return new OrderInstanceMap();
        }
    }
}