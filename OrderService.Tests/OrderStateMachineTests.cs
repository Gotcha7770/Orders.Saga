using System;
using System.Linq;
using System.Threading.Tasks;
using MassTransit.Saga;
using MassTransit.SagaStateMachine;
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
    private readonly Guid _orderId = Guid.NewGuid();
    private readonly Guid _userId = Guid.NewGuid();
    private readonly int _productId = 42;
    private readonly StateMachineTestFixture<OrderStateMachine, Order> _testFixture;

    public OrderStateMachineTests(StateMachineTestFixture<OrderStateMachine, Order> testFixture)
    {
        _testFixture = testFixture;
    }

    [Fact]
    public async Task CreateOrder_ReserveStock_Pending()
    {
        await using var provider = _testFixture.GetProvider();
        var harness = provider.GetRequiredService<ITestHarness>();
        await harness.Start();

        await harness.Bus.Publish(new OrderCreated(_orderId, _userId));
        
        await harness.Consumed.Any<OrderCreated>().ShouldBeTrue();
        
        // OrderCreated event should be consumed by saga too
        var sagaHarness = harness.GetSagaStateMachineHarness<OrderStateMachine, Order>();
        await sagaHarness.Consumed.Any<OrderCreated>().ShouldBeTrue();

        // OrderCreated event should cause state machine created
        var instance = sagaHarness.Created.ContainsInState(_orderId, sagaHarness.StateMachine, sagaHarness.StateMachine.ReserveStock.Pending);
        instance.ShouldNotBeNull();
        instance.UserId.ShouldBe(_userId);
        instance.CreatedOn.Date.ShouldBe(DateTime.UtcNow.Date);

        await harness.Published.Any<ReserveStock>().ShouldBeTrue();
    }

    [Fact]
    public async Task StockReserved_Cause_Transition_To_Checkout_Pending()
    {
        await using var provider = _testFixture.GetProvider();
        var harness = provider.GetRequiredService<ITestHarness>();
        await harness.Start();

        var dictionary = provider.GetRequiredService<IndexedSagaDictionary<Order>>();
        dictionary.Add(new SagaInstance<Order>(new Order 
        { 
            CorrelationId = _orderId,
            CurrentState = 3
        }));

        var sendEndpoint = await harness.Bus.GetPublishSendEndpoint<StockReserved>();
        await sendEndpoint.Send(new StockReserved
        {
            OrderId = _orderId,
            UserId = _userId,
            ProductId = _productId,
        });

        var sagaHarness = harness.GetSagaStateMachineHarness<OrderStateMachine, Order>();
        await sagaHarness.Sagas.Any();

        await harness.Consumed.Any<StockReserved>().ShouldBeTrue();
        await sagaHarness.Consumed.Any<StockReserved>().ShouldBeTrue();
        
        var instance = sagaHarness.Sagas.ContainsInState(_orderId, sagaHarness.StateMachine, sagaHarness.StateMachine.Checkout.Pending);
        instance.ShouldNotBeNull();
    }

    [Fact]
    public void GetGraph()
    {
        var stateMachine = new OrderStateMachine();
        var graph = stateMachine.GetGraph();
        
        graph.Vertices.Count().ShouldBe(12);
    }
}