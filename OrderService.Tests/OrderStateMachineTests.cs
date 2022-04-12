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

public class OrderStateMachineTests : IClassFixture<StateMachineTestFixture<OrderStateMachine, OrderInstance>>
{
    private readonly StateMachineTestFixture<OrderStateMachine, OrderInstance> _testFixture;

    public OrderStateMachineTests(StateMachineTestFixture<OrderStateMachine, OrderInstance> testFixture)
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
        await harness.Bus.Publish<OrderCreated>(new
        {
            OrderId = sagaId,
            UserId = userId
        });
        
        await harness.Consumed.Any<OrderCreated>().ShouldBeTrue();
        
        // OrderCreated event should be consumed by saga too
        var sagaHarness = harness.GetSagaStateMachineHarness<OrderStateMachine, OrderInstance>();
        await sagaHarness.Consumed.Any<OrderCreated>().ShouldBeTrue();

        // OrderCreated event should cause state machine created
        var instance = sagaHarness.Created.ContainsInState(sagaId, sagaHarness.StateMachine, sagaHarness.StateMachine.ReserveStock.Pending);
        instance.ShouldNotBeNull();
        instance.CreatedBy.ShouldBe(userId);
        instance.CreatedOn.Date.ShouldBe(DateTimeOffset.UtcNow.Date);

        await harness.Published.Any<ReserveStock>().ShouldBeTrue();
    }
}