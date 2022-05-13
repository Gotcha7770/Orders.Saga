using MassTransit;

namespace OrdersService.OrderSaga;

public class Order : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public int CurrentState { get; set; }
    public Guid UserId { get; set; }
    public int ProductId { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime CompletedOn { get; set; }
}