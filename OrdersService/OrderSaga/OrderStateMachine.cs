using MassTransit;
using Orders.Saga.Contracts.Messages;

namespace OrdersService.OrderSaga;

public class OrderStateMachine : MassTransitStateMachine<OrderSaga>
{
    public State Pending { get; init; }
    public State Completed { get; init; }
    public State Rejected { get; init; }

    public OrderStateMachine()
    {
        InstanceState(x => x.CurrentState, Pending, Completed, Rejected);
        
        Event(() => OrderCreated, x => x.CorrelateById(context => context.Message.OrderId));
        Event(() => OrderCompleted, x => x.CorrelateById(context => context.Message.OrderId));

        Request(() => ReserveStock,
            // x => x.ProcessOrderRequestId, // Optional
            cfg => {
                //r.ServiceAddress = settings.ProcessOrderServiceAddress; //otherwise publish
                cfg.Timeout = TimeSpan.FromMinutes(1);
            });
        
        Request(() => Checkout,
            cfg =>
            {
                cfg.Timeout = TimeSpan.FromMinutes(1);
            });
        
        Initially(
            When(OrderCreated)
                .Then(x =>
                {
                    x.Saga.CreatedBy = x.Message.UserId;
                    x.Saga.CreatedOn = x.Message.Created;
                })
                .Request(ReserveStock, x => x.Init<ReserveStock>(new
                {
                    OrderId = x.Saga.CorrelationId,
                    UserId = x.Saga.CreatedBy
                }))
                .TransitionTo(Pending)
                .TransitionTo(ReserveStock.Pending));

        During(ReserveStock.Pending,
            When(ReserveStock.Completed)
                .Then(x => x.Saga.ProductId = x.Message.ProductId)
                .Request(Checkout, x => x.Init<Checkout>(new
                {
                    OrderId = x.Saga.CorrelationId,
                    UserId = x.Saga.CreatedBy
                }))
                .TransitionTo(Checkout.Pending),
            When(ReserveStock.Faulted)
                .TransitionTo(Rejected),
            When(ReserveStock.TimeoutExpired)
                .TransitionTo(Rejected));
        
        During(ReserveStock.Pending,
            When(Checkout.Completed)
                .Then(x => x.Saga.CompletedOn = DateTimeOffset.UtcNow.DateTime)
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

        //??? CompositeEvent(() => OrderCompleted, x => x.Completed, StockReserved, PaymentCompleted);
    }
    
    public Event<OrderCreated> OrderCreated { get; init; }
    public Event<OrderCompleted> OrderCompleted { get; init; }
    
    public Request<OrderSaga, ReserveStock, StockReserved> ReserveStock { get; init; }
    public Request<OrderSaga, Checkout, PaymentCompleted> Checkout { get; init; }
}