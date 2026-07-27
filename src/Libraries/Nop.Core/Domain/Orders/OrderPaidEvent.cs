namespace Nop.Core.Domain.Orders;

/// <summary>
/// Order paid event
/// </summary>
public partial class OrderPaidEvent
{
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="order">Order</param>
    public OrderPaidEvent(Order order)
    {
		logger.LogDebug(">>> Entering OrderPaidEvent(order={Order})", order);
        Order = order;
		logger.LogDebug("<<< Exiting OrderPaidEvent(order={Order})", order);
		logger.LogDebug("OrderPaidEvent(order={Order}): Order → {Order1}", order, Order);
    }

    /// <summary>
    /// Order
    /// </summary>
    public Order Order { get; }
}