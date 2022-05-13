using MassTransit;
using Orders.Saga.Contracts.Messages;

namespace OrdersService.OrderSaga;

public class OrderStateMachine : MassTransitStateMachine<Order>
{
    public State Completed { get; init; }
    public State Rejected { get; init; }

    public OrderStateMachine()
    {
        Event(() => OrderCreated, x => x.CorrelateById(context => context.Message.OrderId));
        Event(() => OrderCompleted, x => x.CorrelateById(context => context.Message.OrderId));

        Request(() => ReserveStock, cfg => { cfg.Timeout = TimeSpan.FromMinutes(1); });
        Request(() => Checkout, cfg => { cfg.Timeout = TimeSpan.FromMinutes(1); });
        
        InstanceState(x => x.CurrentState, 
            ReserveStock.Pending,
            Checkout.Pending,
            Completed, 
            Rejected);
        
        Initially(
            When(OrderCreated)
                .Then(x =>
                {
                    x.Saga.UserId = x.Message.UserId;
                    x.Saga.CreatedOn = DateTime.UtcNow;
                })
                .Request(ReserveStock, x => new ReserveStock
                {
                    OrderId = x.Saga.CorrelationId,
                    UserId = x.Saga.UserId
                })
                .TransitionTo(ReserveStock.Pending));

        During(ReserveStock.Pending,
            When(ReserveStock.Completed)
                .Then(x => x.Saga.ProductId = x.Message.ProductId)
                .Request(Checkout, x => new Checkout
                {
                    OrderId = x.Saga.CorrelationId,
                    UserId = x.Saga.UserId
                })
                .TransitionTo(Checkout.Pending),
            When(ReserveStock.Faulted)
                .TransitionTo(Rejected),
            When(ReserveStock.TimeoutExpired)
                .TransitionTo(Rejected));
        
        During(Checkout.Pending,
            When(Checkout.Completed)
                .Then(x => x.Saga.CompletedOn = DateTime.UtcNow)
                .PublishAsync(context => context.Init<OrderCompleted>(new
                {
                    OrderId = context.Saga.CorrelationId,
                    Completed = context.Saga.CompletedOn
                }))
                .TransitionTo(Completed),
            When(Checkout.Faulted)
                .TransitionTo(Rejected),
            When(Checkout.TimeoutExpired)
                .TransitionTo(Rejected));
    }
    
    public Event<OrderCreated> OrderCreated { get; init; }
    public Event<OrderCompleted> OrderCompleted { get; init; }
    
    public Request<Order, ReserveStock, StockReserved> ReserveStock { get; init; }
    public Request<Order, Checkout, PaymentCompleted> Checkout { get; init; }
}