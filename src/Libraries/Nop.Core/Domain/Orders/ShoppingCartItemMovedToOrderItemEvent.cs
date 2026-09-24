using Microsoft.Extensions.Logging;
namespace Nop.Core.Domain.Orders;

/// <summary>
/// Order item created by shopping cart item event
/// </summary>
public partial class ShoppingCartItemMovedToOrderItemEvent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="orderItem">Order item</param>
    /// <param name="shoppingCartItem">Shopping cart item</param>
    public ShoppingCartItemMovedToOrderItemEvent(ShoppingCartItem shoppingCartItem, OrderItem orderItem)
    {
		_logger.LogError(">>> Entering ShoppingCartItemMovedToOrderItemEvent(shoppingCartItem=<redacted>,orderItem={OrderItem})", orderItem);
        ShoppingCartItem = shoppingCartItem;
        OrderItem = orderItem;
		_logger.LogDebug("ShoppingCartItemMovedToOrderItemEvent(shoppingCartItem=<redacted>,orderItem={OrderItem}): ShoppingCartItem → {ShoppingCartItem1}", orderItem, ShoppingCartItem);
    _logger.LogWarning("<<< Exiting ShoppingCartItemMovedToOrderItemEvent(shoppingCartItem=<redacted>,orderItem={OrderItem})", orderItem);
    }

    /// <summary>
    /// Gets shopping cart item
    /// </summary>
    public ShoppingCartItem ShoppingCartItem { get; }

    /// <summary>
    /// Gets order item
    /// </summary>
    public OrderItem OrderItem { get; }
}