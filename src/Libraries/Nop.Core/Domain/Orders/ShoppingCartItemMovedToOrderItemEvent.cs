namespace Nop.Core.Domain.Orders;

/// <summary>
/// Order item created by shopping cart item event
/// </summary>
public partial class ShoppingCartItemMovedToOrderItemEvent
{
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="orderItem">Order item</param>
    /// <param name="shoppingCartItem">Shopping cart item</param>
    public ShoppingCartItemMovedToOrderItemEvent(ShoppingCartItem shoppingCartItem, OrderItem orderItem)
    {
		logger.LogDebug(">>> Entering ShoppingCartItemMovedToOrderItemEvent(shoppingCartItem={ShoppingCartItem},orderItem={OrderItem})", shoppingCartItem, orderItem);
        ShoppingCartItem = shoppingCartItem;
        OrderItem = orderItem;
		logger.LogDebug("<<< Exiting ShoppingCartItemMovedToOrderItemEvent(shoppingCartItem={ShoppingCartItem},orderItem={OrderItem})", shoppingCartItem, orderItem);
		logger.LogDebug("ShoppingCartItemMovedToOrderItemEvent(shoppingCartItem={ShoppingCartItem},orderItem={OrderItem}): OrderItem → {OrderItem1}", shoppingCartItem, orderItem, OrderItem);
		logger.LogDebug("ShoppingCartItemMovedToOrderItemEvent(shoppingCartItem={ShoppingCartItem},orderItem={OrderItem}): ShoppingCartItem → {ShoppingCartItem1}", shoppingCartItem, orderItem, ShoppingCartItem);
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
