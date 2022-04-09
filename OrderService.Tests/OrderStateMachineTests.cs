using System;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Orders.Saga.Contracts.Messages;
using OrderService.Tests.Infrastructure;
using OrdersService.OrderSaga;
using Shouldly;
using Xunit;

namespace OrderService.Tests;

public class OrderStateMachineTests : IClassFixture<StateMachineTestFixture<OrderStateMachine, OrderSaga>>
{
    private readonly StateMachineTestFixture<OrderStateMachine, OrderSaga> _testFixture;

    public OrderStateMachineTests(StateMachineTestFixture<OrderStateMachine, OrderSaga> testFixture)
    {
        _testFixture = testFixture;
    }

    [Fact]
    public async Task CreateOrderTest()
    {
        await using var provider = _testFixture.GetProvider();
        // await using var provider = _testFixture.GetProvider((cfg) =>
        // {
        //     cfg.ReceiveEndpoint(e =>
        //     {
        //         e.Handler<ReserveStock>(async context =>
        //         {
        //             await context.RespondAsync(new StockReserved
        //             {
        //                 OrderId = context.Message.OrderId,
        //                 UserId = context.Message.UserId,
        //                 ProductId = 1
        //             });
        //         });
        //     });
        // });
        
        var harness = provider.GetRequiredService<ITestHarness>();
        await harness.Start();
        
        var sagaId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        await harness.Bus.Publish<OrderCreated>(new
        {
            OrderId = sagaId,
            UserId = userId
        });
        
        await harness.Consumed.Any<OrderCreated>().ShouldBeTrue();
        
        // OrderCreated event should be consumed by saga too
        var sagaHarness = harness.GetSagaStateMachineHarness<OrderStateMachine, OrderSaga>();
        await sagaHarness.Consumed.Any<OrderCreated>().ShouldBeTrue();

        // OrderCreated event should cause state machine created
        var instance = sagaHarness.Created.ContainsInState(sagaId, sagaHarness.StateMachine, sagaHarness.StateMachine.ReserveStock.Pending);
        instance.ShouldNotBeNull();
        instance.CreatedBy.ShouldBe(userId);
        instance.CreatedOn.Date.ShouldBe(DateTimeOffset.UtcNow.Date);

        await harness.Published.Any<ReserveStock>().ShouldBeTrue();
    }
}