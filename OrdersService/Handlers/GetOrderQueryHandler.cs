using MediatR;
using Microsoft.EntityFrameworkCore;
using OrdersService.Models;
using OrdersService.Queries;

namespace OrdersService.Handlers;

public class GetOrderQueryHandler : IRequestHandler<GetOrderQuery, Order>
{
    private readonly ApplicationDbContext _dbContext;

    public GetOrderQueryHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<Order> Handle(GetOrderQuery request, CancellationToken cancellationToken)
    {
        return await _dbContext.Orders.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
    }
}