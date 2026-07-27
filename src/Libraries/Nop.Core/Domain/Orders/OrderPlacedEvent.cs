namespace Nop.Core.Domain.Orders;

/// <summary>
/// Order placed event
/// </summary>
public partial class OrderPlacedEvent
{
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="order">Order</param>
    public OrderPlacedEvent(Order order)
    {
		logger.LogDebug(">>> Entering OrderPlacedEvent(order={Order})", order);
        Order = order;
		logger.LogDebug("<<< Exiting OrderPlacedEvent(order={Order})", order);
		logger.LogDebug("OrderPlacedEvent(order={Order}): Order → {Order1}", order, Order);
    }

    /// <summary>
    /// Order
    /// </summary>
    public Order Order { get; }
}