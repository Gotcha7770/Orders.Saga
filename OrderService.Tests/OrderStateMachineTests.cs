using System;
using System.Threading.Tasks;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Orders.Saga.Contracts.Messages;
using OrderService.Tests.Infrastructure;
using OrdersService.OrderSaga;
using Shouldly;
using Xunit;

namespace OrderService.Tests;

public class OrderStateMachineTests : IClassFixture<StateMachineTestFixture<OrderStateMachine, Order>>
{
    private readonly StateMachineTestFixture<OrderStateMachine, Order> _testFixture;

    public OrderStateMachineTests(StateMachineTestFixture<OrderStateMachine, Order> testFixture)
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
        var userId = Guid.NewGuid();
        await harness.Bus.Publish(new OrderCreated(sagaId, userId));
        
        await harness.Consumed.Any<OrderCreated>().ShouldBeTrue();
        
        // OrderCreated event should be consumed by saga too
        var sagaHarness = harness.GetSagaStateMachineHarness<OrderStateMachine, Order>();
        await sagaHarness.Consumed.Any<OrderCreated>().ShouldBeTrue();

        // OrderCreated event should cause state machine created
        var instance = sagaHarness.Created.ContainsInState(sagaId, sagaHarness.StateMachine, sagaHarness.StateMachine.ReserveStock.Pending);
        instance.ShouldNotBeNull();
        instance.UserId.ShouldBe(userId);
        instance.CreatedOn.Date.ShouldBe(DateTime.UtcNow.Date);

        await harness.Published.Any<ReserveStock>().ShouldBeTrue();
    }
}