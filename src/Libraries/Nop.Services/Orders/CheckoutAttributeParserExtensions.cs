using System.Xml;
using Nop.Core.Domain.Orders;
using Nop.Core.Infrastructure;
using Nop.Services.Attributes;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Orders;

/// <summary>
/// Checkout attribute parser extensions
/// </summary>
public static partial class CheckoutAttributeParserExtensions
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Removes checkout attributes which cannot be applied to the current cart and returns an update attributes in XML format
    /// </summary>
    /// <param name="parser">Checkout attribute parser</param>
    /// <param name="attributesXml">Attributes in XML format</param>
    /// <param name="cart">Shopping cart items</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the updated attributes in XML format
    /// </returns>
    public static async Task<string> EnsureOnlyActiveAttributesAsync(this IAttributeParser<CheckoutAttribute, CheckoutAttributeValue> parser, string attributesXml, IList<ShoppingCartItem> cart)
    {
		_logger.LogDebug(">>> Entering EnsureOnlyActiveAttributesAsync(parser={Parser},attributesXml={AttributesXml},cart={Cart})", parser, attributesXml, cart);
        if (string.IsNullOrEmpty(attributesXml))
            return attributesXml;

        var result = attributesXml;
		_logger.LogDebug("EnsureOnlyActiveAttributesAsync(parser={Parser},attributesXml={AttributesXml},cart={Cart}): result → {Result}", parser, attributesXml, cart, result);

        //removing "shippable" checkout attributes if there's no any shippable products in the cart
        //do not inject IShoppingCartService via constructor because it'll cause circular references
        var shoppingCartService = EngineContext.Current.Resolve<IShoppingCartService>();
		_logger.LogDebug("EnsureOnlyActiveAttributesAsync(parser={Parser},attributesXml={AttributesXml},cart={Cart}): shoppingCartService → {ShoppingCartService}", parser, attributesXml, cart, shoppingCartService);
        if (await shoppingCartService.ShoppingCartRequiresShippingAsync(cart))
            return result;

        //find attribute IDs to remove
        var checkoutAttributeIdsToRemove = new List<int>();
		_logger.LogDebug("EnsureOnlyActiveAttributesAsync(parser={Parser},attributesXml={AttributesXml},cart={Cart}): checkoutAttributeIdsToRemove → {CheckoutAttributeIdsToRemove}", parser, attributesXml, cart, checkoutAttributeIdsToRemove);
        var attributes = await parser.ParseAttributesAsync(attributesXml);
		_logger.LogDebug("EnsureOnlyActiveAttributesAsync(parser={Parser},attributesXml={AttributesXml},cart={Cart}): attributes → {Attributes}", parser, attributesXml, cart, attributes);

        foreach (var ca in attributes)
            if (ca.ShippableProductRequired)
                checkoutAttributeIdsToRemove.Add(ca.Id);

        //remove them from XML
        try
        {
            var xmlDoc = new XmlDocument();
			_logger.LogDebug("EnsureOnlyActiveAttributesAsync(parser={Parser},attributesXml={AttributesXml},cart={Cart}): xmlDoc → {XmlDoc}", parser, attributesXml, cart, xmlDoc);
            xmlDoc.LoadXml(attributesXml);

            var nodesToRemove = new List<XmlNode>();
			_logger.LogDebug("EnsureOnlyActiveAttributesAsync(parser={Parser},attributesXml={AttributesXml},cart={Cart}): nodesToRemove → {NodesToRemove}", parser, attributesXml, cart, nodesToRemove);
            var nodes = xmlDoc.SelectNodes(@$"//Attributes/{nameof(CheckoutAttribute)}");
			_logger.LogDebug("EnsureOnlyActiveAttributesAsync(parser={Parser},attributesXml={AttributesXml},cart={Cart}): nodes → {Nodes}", parser, attributesXml, cart, nodes);
            if (nodes != null)
            {
                foreach (XmlNode node in nodes)
                {
                    if (node.Attributes?["ID"] == null)
                        continue;

                    var str1 = node.Attributes["ID"].InnerText.Trim();
					_logger.LogDebug("EnsureOnlyActiveAttributesAsync(parser={Parser},attributesXml={AttributesXml},cart={Cart}): str1 → {Str1}", parser, attributesXml, cart, str1);

                    if (!int.TryParse(str1, out var id))
                        continue;

                    if (checkoutAttributeIdsToRemove.Contains(id))
                        nodesToRemove.Add(node);
                }

                foreach (var node in nodesToRemove)
                    node.ParentNode?.RemoveChild(node);
            }

            result = xmlDoc.OuterXml;
        }
        catch
        {
			_logger.LogError("Exception in EnsureOnlyActiveAttributesAsync(parser={Parser},attributesXml={AttributesXml},cart={Cart}): {Arg3}", parser, attributesXml, cart, ex.Message);
            //ignore
        }

		_logger.LogDebug("<<< Exiting EnsureOnlyActiveAttributesAsync(parser={Parser},attributesXml={AttributesXml},cart={Cart})", parser, attributesXml, cart);
        return result;
    }
}