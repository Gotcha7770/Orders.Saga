﻿namespace Orders.Saga.Contracts.Messages;

public interface StockReserved
{
    Guid OrderId { get; }
    int ProductId { get; }
}