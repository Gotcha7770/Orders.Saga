using System;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace OrderService.Tests.Infrastructure;

public class StateMachineTestFixture<TStateMachine, TInstance>
    where TStateMachine : class, SagaStateMachine<TInstance>
    where TInstance : class, SagaStateMachineInstance
{
    public ServiceProvider GetProvider(Action<IInMemoryBusFactoryConfigurator> configuration = default)
    {
        var collection = new ServiceCollection()
            .AddMassTransitTestHarness(x =>
            {
                x.AddSagaStateMachine<TStateMachine, TInstance>()
                    .InMemoryRepository();

                x.AddPublishMessageScheduler();
                
                x.UsingInMemory((context, cfg) =>
                {
                    cfg.UseInMemoryScheduler();
                    cfg.ConfigureEndpoints(context);

                    configuration?.Invoke(cfg);
                });
            });

        return collection.BuildServiceProvider(true);
    }
}