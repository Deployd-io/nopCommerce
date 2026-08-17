using Microsoft.Extensions.Logging;
namespace Nop.Core.Domain.Orders;

/// <summary>
/// Order paid event
/// </summary>
public partial class OrderPaidEvent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="order">Order</param>
    public OrderPaidEvent(Order order)
    {
		_logger.LogDebug(">>> Entering OrderPaidEvent(order={Order})", order);
        Order = order;
		_logger.LogDebug("<<< Exiting OrderPaidEvent(order={Order})", order);
		_logger.LogDebug("OrderPaidEvent(order={Order}): Order → {Order1}", order, Order);
    }

    /// <summary>
    /// Order
    /// </summary>
    public Order Order { get; }
}