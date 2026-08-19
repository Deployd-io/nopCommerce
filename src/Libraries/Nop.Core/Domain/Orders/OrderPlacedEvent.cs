using Microsoft.Extensions.Logging;
namespace Nop.Core.Domain.Orders;

/// <summary>
/// Order placed event
/// </summary>
public partial class OrderPlacedEvent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="order">Order</param>
    public OrderPlacedEvent(Order order)
    {
		_logger.LogDebug(">>> Entering OrderPlacedEvent(order={Order})", order);
        Order = order;
		_logger.LogDebug("OrderPlacedEvent(order={Order}): Order → {Order1}", order, Order);
		_logger.LogDebug("<<< Exiting OrderPlacedEvent(order={Order})", order);
    }

    /// <summary>
    /// Order
    /// </summary>
    public Order Order { get; }
}