namespace Nop.Core.Domain.Orders;

/// <summary>
/// Order voided event
/// </summary>
public partial class OrderVoidedEvent
{
    #region Ctor

    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="order">Order</param>
    public OrderVoidedEvent(Order order)
    {
		logger.LogDebug(">>> Entering OrderVoidedEvent(order={Order})", order);
        Order = order;
		logger.LogDebug("<<< Exiting OrderVoidedEvent(order={Order})", order);
		logger.LogDebug("OrderVoidedEvent(order={Order}): Order → {Order1}", order, Order);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Voided order
    /// </summary>
    public Order Order { get; }

    #endregion
}