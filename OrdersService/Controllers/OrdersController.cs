using MediatR;
using Microsoft.AspNetCore.Mvc;
using Orders.Saga.Commands;
using Orders.Saga.Queries;

namespace Orders.Saga.Controllers;

[ApiController]
[Route("api/orders/")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] Guid orderId)
    {
        var order = _mediator.Send(new GetOrderQuery(orderId));

        return Ok(order);
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderCommand command)
    {
        var order = await _mediator.Send(command);

        return Created($"api/orders/{order.Id}", order);
    }
}