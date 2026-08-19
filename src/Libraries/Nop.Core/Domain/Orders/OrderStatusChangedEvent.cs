using Microsoft.Extensions.Logging;
﻿namespace Nop.Core.Domain.Orders;

/// <summary>
/// Order status changed event
/// </summary>
public partial class OrderStatusChangedEvent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="order">Order</param>
    /// <param name="previousOrderStatus">Previous order status</param>
    public OrderStatusChangedEvent(Order order, OrderStatus previousOrderStatus)
    {
		_logger.LogDebug(">>> Entering OrderStatusChangedEvent(order={Order},previousOrderStatus={PreviousOrderStatus})", order, previousOrderStatus);
        Order = order;
        PreviousOrderStatus = previousOrderStatus;
		_logger.LogDebug("OrderStatusChangedEvent(order={Order},previousOrderStatus={PreviousOrderStatus}): Order → {Order1}", order, previousOrderStatus, Order);
		_logger.LogDebug("OrderStatusChangedEvent(order={Order},previousOrderStatus={PreviousOrderStatus}): PreviousOrderStatus → {PreviousOrderStatus1}", order, previousOrderStatus, PreviousOrderStatus);
		_logger.LogDebug("<<< Exiting OrderStatusChangedEvent(order={Order},previousOrderStatus={PreviousOrderStatus})", order, previousOrderStatus);
    }

    /// <summary>
    /// Order
    /// </summary>
    public Order Order { get; }

    /// <summary>
    /// Previous order status
    /// </summary>
    public OrderStatus PreviousOrderStatus { get; set; }
}