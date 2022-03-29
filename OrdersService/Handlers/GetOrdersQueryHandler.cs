using MediatR;
using Microsoft.EntityFrameworkCore;
using OrdersService.Models;
using OrdersService.Queries;

namespace OrdersService.Handlers;

public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, Order[]>
{
    private readonly ApplicationDbContext _dbContext;

    public GetOrdersQueryHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<Order[]> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        return await _dbContext.Orders.AsNoTracking().ToArrayAsync(cancellationToken);
    }
}