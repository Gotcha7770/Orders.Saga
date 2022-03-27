using System.Reflection;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PaymentService;
using PaymentService.Consumers;

var builder = WebApplication.CreateBuilder(args);

// Add json files
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);

// Add db context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection")));

// Add mediatr for assembly
builder.Services.AddMediatR(Assembly.GetExecutingAssembly());

// Add MassTransit
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<StockReservedConsumer>();
    
    // Configure host, virtual host and credentials
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("admin");
            h.Password("rabbitmq");
        });
        
        // Configure endpoints to handle events
        cfg.ConfigureEndpoints(context);
        
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.Run();