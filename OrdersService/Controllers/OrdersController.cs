using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrdersService.Commands;
using OrdersService.Queries;

namespace OrdersService.Controllers;

[ApiController]
[Route("api/orders/")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get([FromQuery] Guid id)
    {
        var order = await _mediator.Send(new GetOrderQuery(id));

        return Ok(order);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var orders = await _mediator.Send(new GetOrdersQuery());

        return Ok(orders);
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderCommand command)
    {
        var order = await _mediator.Send(command);

        return Created($"{HttpContext.Request.PathBase}/api/orders/{order.Id}", order);
    }
}