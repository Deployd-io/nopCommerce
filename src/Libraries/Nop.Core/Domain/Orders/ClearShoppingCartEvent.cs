using Microsoft.Extensions.Logging;
namespace Nop.Core.Domain.Orders;

/// <summary>
/// Shopping cart cleared event
/// </summary>
public partial class ClearShoppingCartEvent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="cart">Shopping cart</param>
    public ClearShoppingCartEvent(IList<ShoppingCartItem> cart)
    {
		_logger.LogDebug(">>> Entering ClearShoppingCartEvent(cart={Cart})", cart);
        Cart = cart;
		_logger.LogDebug("ClearShoppingCartEvent(cart={Cart}): Cart → {Cart1}", cart, Cart);
    }

    /// <summary>
    /// Shopping cart
    /// </summary>
    public IList<ShoppingCartItem> Cart { get; }
}