using MassTransit;
using Microsoft.EntityFrameworkCore;
using StockService;
using StockService.Consumers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection")));

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<ReserveStockConsumer>();
    
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

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.Run();