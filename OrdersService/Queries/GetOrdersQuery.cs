using MediatR;
using OrdersService.Models;

namespace OrdersService.Queries;

public record GetOrdersQuery : IStreamRequest<Order>;