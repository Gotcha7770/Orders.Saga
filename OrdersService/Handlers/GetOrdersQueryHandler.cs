using MediatR;
using Microsoft.EntityFrameworkCore;
using OrdersService.Models;
using OrdersService.Queries;

namespace OrdersService.Handlers;

public class GetOrdersQueryHandler : IStreamRequestHandler<GetOrdersQuery, Order>
{
    private readonly ApplicationDbContext _dbContext;

    public GetOrdersQueryHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public IAsyncEnumerable<Order> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        return  _dbContext.Orders.AsNoTracking().AsAsyncEnumerable();
    }
}