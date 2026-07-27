namespace Nop.Core.Domain.Orders;

/// <summary>
/// Shopping cart cleared event
/// </summary>
public partial class ClearShoppingCartEvent
{
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="cart">Shopping cart</param>
    public ClearShoppingCartEvent(IList<ShoppingCartItem> cart)
    {
		logger.LogDebug(">>> Entering ClearShoppingCartEvent(cart={Cart})", cart);
        Cart = cart;
		logger.LogDebug("<<< Exiting ClearShoppingCartEvent(cart={Cart})", cart);
		logger.LogDebug("ClearShoppingCartEvent(cart={Cart}): Cart → {Cart1}", cart, Cart);
    }

    /// <summary>
    /// Shopping cart
    /// </summary>
    public IList<ShoppingCartItem> Cart { get; }
}