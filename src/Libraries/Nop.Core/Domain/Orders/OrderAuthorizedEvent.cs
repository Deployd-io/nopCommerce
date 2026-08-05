using Microsoft.Extensions.Logging;
﻿namespace Nop.Core.Domain.Orders;

/// <summary>
/// Order authorized event
/// </summary>
public partial class OrderAuthorizedEvent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="order">Order</param>
    public OrderAuthorizedEvent(Order order)
    {
		_logger.LogDebug(">>> Entering OrderAuthorizedEvent(order={Order})", order);
        Order = order;
		_logger.LogDebug("<<< Exiting OrderAuthorizedEvent(order={Order})", order);
		_logger.LogDebug("OrderAuthorizedEvent(order={Order}): Order → {Order1}", order, Order);
    }

    /// <summary>
    /// Order
    /// </summary>
    public Order Order { get; }
}