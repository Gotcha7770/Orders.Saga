using MediatR;
using Microsoft.AspNetCore.Mvc;
using Orders.Saga.Commands;

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
    
    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderCommand command)
    {
        var id = await _mediator.Send(command);

        return Ok(id);
    }
}