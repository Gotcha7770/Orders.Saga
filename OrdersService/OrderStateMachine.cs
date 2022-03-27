using Automatonymous;
using Orders.Saga.Contracts.Messages;

namespace OrdersService;

public class OrderSaga : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public int CurrentState { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime CompletedOn { get; set; }
}

public class OrderStateMachine : MassTransitStateMachine<OrderSaga>
{
    public State Pending { get; init; }
    public State Completed { get; init; }
    public State Rejected { get; init; }

    public OrderStateMachine()
    {
        InstanceState(x => x.CurrentState, Pending, Completed, Rejected);
        
        Initially(
            When(OrderCreated)
                .TransitionTo(Pending));
        
        During(Pending,
            When(OrderCompleted)
                .TransitionTo(Completed));
    }
    
    public Event<OrderCreated> OrderCreated { get; private set; }
    public Event<OrderCompleted> OrderCompleted { get; private set; }
}