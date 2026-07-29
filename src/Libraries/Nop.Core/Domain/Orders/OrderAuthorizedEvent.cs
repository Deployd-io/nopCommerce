namespace Nop.Core.Domain.Orders;

/// <summary>
/// Order authorized event
/// </summary>
public partial class OrderAuthorizedEvent
{
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="order">Order</param>
    public OrderAuthorizedEvent(Order order)
    {
		logger.LogDebug(">>> Entering OrderAuthorizedEvent(order={Order})", order);
        Order = order;
		logger.LogDebug("<<< Exiting OrderAuthorizedEvent(order={Order})", order);
		logger.LogDebug("OrderAuthorizedEvent(order={Order}): Order → {Order1}", order, Order);
    }

    /// <summary>
    /// Order
    /// </summary>
    public Order Order { get; }
}