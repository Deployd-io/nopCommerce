using Microsoft.Extensions.Logging;
namespace Nop.Core.Domain.Orders;

/// <summary>
/// Order refunded event
/// </summary>
public partial class OrderRefundedEvent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="order">Order</param>
    /// <param name="amount">Amount</param>
    public OrderRefundedEvent(Order order, decimal amount)
    {
		_logger.LogDebug(">>> Entering OrderRefundedEvent(order={Order},amount={Amount})", order, amount);
        Order = order;
        Amount = amount;
		_logger.LogDebug("<<< Exiting OrderRefundedEvent(order={Order},amount={Amount})", order, amount);
		_logger.LogWarning("OrderRefundedEvent(order={Order},amount={Amount}): Amount → {Amount1}", order, amount, Amount);
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