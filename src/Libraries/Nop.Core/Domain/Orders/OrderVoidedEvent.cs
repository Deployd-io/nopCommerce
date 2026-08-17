using Microsoft.Extensions.Logging;
namespace Nop.Core.Domain.Orders;

/// <summary>
/// Order voided event
/// </summary>
public partial class OrderVoidedEvent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="order">Order</param>
    public OrderVoidedEvent(Order order)
    {
		_logger.LogDebug(">>> Entering OrderVoidedEvent(order={Order})", order);
        Order = order;
		_logger.LogDebug("<<< Exiting OrderVoidedEvent(order={Order})", order);
		_logger.LogDebug("OrderVoidedEvent(order={Order}): Order → {Order1}", order, Order);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Voided order
    /// </summary>
    public Order Order { get; }

    #endregion
}