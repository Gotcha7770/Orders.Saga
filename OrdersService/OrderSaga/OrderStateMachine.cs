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
            r => {
                //r.ServiceAddress = settings.ProcessOrderServiceAddress; //otherwise publish
                r.Timeout = TimeSpan.FromMinutes(1);
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
                //.TransitionTo(Pending)
                .TransitionTo(ReserveStock.Pending));

        During(ReserveStock.Pending,
            When(ReserveStock.Completed)
                .TransitionTo(Completed),
            When(ReserveStock.Faulted)
                .TransitionTo(Rejected),
            When(ReserveStock.TimeoutExpired)
                .TransitionTo(Rejected));

        //??? CompositeEvent(() => OrderCompleted, x => x.Completed, StockReserved, PaymentCompleted);

        // During(Pending,
        //     When(OrderCompleted)
        //         .Then(x => x.Saga.CompletedOn = x.Message.Completed)
        //         .PublishAsync(context => context.Init<OrderCompleted>(new
        //         {
        //             OrderId = context.Saga.CorrelationId,
        //             Completed = context.Saga.CompletedOn
        //         }))
        //         .TransitionTo(Completed));
    }
    
    public Event<OrderCreated> OrderCreated { get; init; }
    public Event<OrderCompleted> OrderCompleted { get; init; }
    
    public Request<OrderSaga, ReserveStock, StockReserved> ReserveStock { get; init; }
    //public Request<OrderSaga, RequestPayment, PaymentCompleted> RequestPayment { get; private set; }
}