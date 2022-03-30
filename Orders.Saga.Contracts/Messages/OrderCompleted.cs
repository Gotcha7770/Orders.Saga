﻿namespace Orders.Saga.Contracts.Messages;

public interface OrderCompleted
{
    Guid OrderId { get; }
    DateTime Completed { get; }
}