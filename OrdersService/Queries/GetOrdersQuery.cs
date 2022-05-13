using MediatR;
using OrdersService.OrderSaga;

namespace OrdersService.Queries;

public class GetOrdersQuery : IRequest<Order[]>
{ }