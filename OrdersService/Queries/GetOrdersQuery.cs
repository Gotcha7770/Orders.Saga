using MediatR;
using OrdersService.Models;

namespace OrdersService.Queries;

public class GetOrdersQuery : IRequest<Order[]>
{ }