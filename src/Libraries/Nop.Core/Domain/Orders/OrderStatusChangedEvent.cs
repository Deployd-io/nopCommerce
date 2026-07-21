namespace Nop.Core.Domain.Orders;

/// <summary>
/// Order status changed event
/// </summary>
public partial class OrderStatusChangedEvent
{
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="order">Order</param>
    /// <param name="previousOrderStatus">Previous order status</param>
    public OrderStatusChangedEvent(Order order, OrderStatus previousOrderStatus)
    {
		logger.LogDebug(">>> Entering OrderStatusChangedEvent(order={Order},previousOrderStatus={PreviousOrderStatus})", order, previousOrderStatus);
        Order = order;
        PreviousOrderStatus = previousOrderStatus;
		logger.LogDebug("<<< Exiting OrderStatusChangedEvent(order={Order},previousOrderStatus={PreviousOrderStatus})", order, previousOrderStatus);
		logger.LogDebug("OrderStatusChangedEvent(order={Order},previousOrderStatus={PreviousOrderStatus}): PreviousOrderStatus → {PreviousOrderStatus1}", order, previousOrderStatus, PreviousOrderStatus);
		logger.LogDebug("OrderStatusChangedEvent(order={Order},previousOrderStatus={PreviousOrderStatus}): Order → {Order1}", order, previousOrderStatus, Order);
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
