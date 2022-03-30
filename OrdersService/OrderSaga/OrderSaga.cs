using MassTransit;

namespace OrdersService.OrderSaga;

public class OrderSaga : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public int CurrentState { get; set; }
    public Guid CreatedBy { get; set; }
    public int ProductId { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime CompletedOn { get; set; }
}