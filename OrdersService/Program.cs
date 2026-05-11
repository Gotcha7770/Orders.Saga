using System.Reflection;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrdersService;
using OrdersService.Commands;
using OrdersService.Consumers;
using OrdersService.Queries;
using OrdersService.OrderSaga;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, cfg) => cfg.ReadFrom.Configuration(context.Configuration));

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection")));

builder.Services
    .AddOpenApi()
    .AddMediatR(Assembly.GetExecutingAssembly());

builder.Services.AddMassTransit(x =>
{
    // Configure scheduling to use timeouts
    var schedulerEndpoint = new Uri("queue:scheduler");
    x.AddMessageScheduler(schedulerEndpoint);
    
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.UseMessageScheduler(schedulerEndpoint);
        
        // Configure host, virtual host and credentials
        cfg.Host("localhost", "/", h =>
        {
            h.Username("admin");
            h.Password("rabbitmq");
        });

        // Configure endpoints to handle events
        cfg.ConfigureEndpoints(context);
    });

    // Configure saga state machine
    x.AddSagaStateMachine<OrderStateMachine, OrderInstance>()
        .EntityFrameworkRepository(cfg =>
        {
            cfg.ConcurrencyMode = ConcurrencyMode.Pessimistic;
            cfg.LockStatementProvider = new PostgresLockStatementProvider();
            
            cfg.AddDbContext<DbContext, OrderSagaDbContext>((_, bldr) =>
            {
                bldr.UseNpgsql(builder.Configuration.GetConnectionString("SagaDbConnection"));
            });
        });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.MapOpenApi();
}

app
    .UseHttpsRedirection()
    .UseSerilogRequestLogging();

app.MapGet("api/orders/{id:guid}", async (IMediator mediator, Guid id) =>
{
    var order = await mediator.Send(new GetOrderQuery { Id = id });
    return Results.Ok(order);
});

app.MapGet("api/orders/", async (IMediator mediator) =>
{
    var orders = await mediator.Send(new GetOrdersQuery());

    return Results.Ok(orders);
});

app.MapPost("api/orders/", async (IMediator mediator, [FromBody] CreateOrderCommand command, HttpContext httpContext) =>
{
    var order = await mediator.Send(command);

    return Results.Created($"{httpContext.Request.PathBase}/api/orders/{order.Id}", order);
});

app.Run();