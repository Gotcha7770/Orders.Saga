using System.Reflection;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OrdersService;
using OrdersService.OrderSaga;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add json files
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);

// Add Serilog
builder.Host.UseSerilog((context, cfg) =>
{
    cfg.ReadFrom.Configuration(context.Configuration);
    
    if (context.HostingEnvironment.IsProduction())
        cfg.MinimumLevel.Information();
    else
        cfg.MinimumLevel.Debug();
});

// Add db context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection")));

// Add controllers
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add mediatr for assembly
builder.Services.AddMediatR(Assembly.GetExecutingAssembly());


// Add MassTransit
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
    x.AddSagaStateMachine<OrderStateMachine, OrderSaga>()
        .InMemoryRepository();
    
    //x.AddRequestClient<ReserveStock>(new Uri("exchange:order-status"));
    
    // .EntityFrameworkRepository(cfg =>
    // {
    //     cfg.ExistingDbContext<ApplicationDbContext>();
    // });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseSerilogRequestLogging();

app.MapControllers();

app.Run();