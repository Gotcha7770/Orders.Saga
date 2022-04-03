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
        var harness = provider.GetRequiredService<ITestHarness>();
        await harness.Start();
        
        var sagaId = Guid.NewGuid();
        await harness.Bus.Publish<OrderCreated>(new
        {
            OrderId = sagaId,
            UserId = Guid.NewGuid()
        });
        
        await harness.Consumed.Any<OrderCreated>().ShouldBeTrue();
        
        var sagaHarness = harness.GetSagaStateMachineHarness<OrderStateMachine, OrderSaga>();
        await sagaHarness.Consumed.Any<OrderCreated>().ShouldBeTrue();
        await sagaHarness.Created.Any(x => x.CorrelationId == sagaId).ShouldBeTrue();

        var instance = sagaHarness.Created.ContainsInState(sagaId, sagaHarness.StateMachine, sagaHarness.StateMachine.ReserveStock.Pending);
        instance.ShouldNotBeNull();
        //instance.

        await harness.Published.Any<ReserveStock>().ShouldBeTrue();
    }
}