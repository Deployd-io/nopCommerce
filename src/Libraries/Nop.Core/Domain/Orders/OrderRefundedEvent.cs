namespace Nop.Core.Domain.Orders;

/// <summary>
/// Order refunded event
/// </summary>
public partial class OrderRefundedEvent
{
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="order">Order</param>
    /// <param name="amount">Amount</param>
    public OrderRefundedEvent(Order order, decimal amount)
    {
		logger.LogDebug(">>> Entering OrderRefundedEvent(order={Order},amount={Amount})", order, amount);
        Order = order;
        Amount = amount;
		logger.LogDebug("<<< Exiting OrderRefundedEvent(order={Order},amount={Amount})", order, amount);
		logger.LogWarning("OrderRefundedEvent(order={Order},amount={Amount}): Amount → {Amount1}", order, amount, Amount);
		logger.LogDebug("OrderRefundedEvent(order={Order},amount={Amount}): Order → {Order1}", order, amount, Order);
    }

    /// <summary>
    /// Order
    /// </summary>
    public Order Order { get; }

    /// <summary>
    /// Amount
    /// </summary>
    public decimal Amount { get; }
}
