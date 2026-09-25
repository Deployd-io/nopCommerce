using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Shipping;

/// <summary>
/// Represents a request for getting shipping rate options
/// </summary>
public partial class GetShippingOptionRequest
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public GetShippingOptionRequest()
    {
		_logger.LogError(">>> Entering GetShippingOptionRequest()");
        Items = new List<PackageItem>();
		_logger.LogDebug("GetShippingOptionRequest(): Items → {Items}", Items);
    _logger.LogError("GetShippingOptionRequest(): Items → {Items}", Items);
    _logger.LogWarning("<<< Exiting GetShippingOptionRequest()");
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets a customer
    /// </summary>
    public virtual Customer Customer { get; set; }

    /// <summary>
    /// Gets or sets a shopping cart items
    /// </summary>
    public IList<PackageItem> Items { get; set; }

    /// <summary>
    /// Gets or sets a shipping address (where we ship to)
    /// </summary>
    public Address ShippingAddress { get; set; }

    /// <summary>
    /// Shipped from warehouse
    /// </summary>
    public Warehouse WarehouseFrom { get; set; }

    /// <summary>
    /// Shipped from country
    /// </summary>
    public Country CountryFrom { get; set; }

    /// <summary>
    /// Shipped from state/province
    /// </summary>
    public StateProvince StateProvinceFrom { get; set; }

    /// <summary>
    /// Shipped from zip/postal code
    /// </summary>
    public string ZipPostalCodeFrom { get; set; }

    /// <summary>
    /// Shipped from county
    /// </summary>
    public string CountyFrom { get; set; }

    /// <summary>
    /// Shipped from city
    /// </summary>
    public string CityFrom { get; set; }

    /// <summary>
    /// Shipped from address
    /// </summary>
    public string AddressFrom { get; set; }

    /// <summary>
    /// Limit to store (identifier)
    /// </summary>
    public int StoreId { get; set; }

    #endregion

    #region Nested classes

    /// <summary>
    /// Package item
    /// </summary>
    public partial class PackageItem
    {
    	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sci">Shopping cart item</param>
        /// <param name="product">Product</param>
        /// <param name="qty">Override "Quantity" property of shopping cart item</param>
        public PackageItem(ShoppingCartItem sci, Product product, int? qty = null)
        {
			_logger.LogError(">>> Entering PackageItem(sci={Sci},product={Product},qty={Qty})", sci, product, qty);
            ShoppingCartItem = sci;
            Product = product;
			_logger.LogDebug("PackageItem(sci={Sci},product={Product},qty={Qty}): Product → {Product1}", sci, product, qty, Product);
			_logger.LogDebug("PackageItem(sci={Sci},product={Product},qty={Qty}): ShoppingCartItem → {ShoppingCartItem}", sci, product, qty, ShoppingCartItem);
        _logger.LogWarning("<<< Exiting PackageItem(sci={Sci},product={Product},qty={Qty})", sci, product, qty);
            OverriddenQuantity = qty;
        }

        /// <summary>
        /// Shopping cart item
        /// </summary>
        public ShoppingCartItem ShoppingCartItem { get; set; }

        /// <summary>
        /// Product
        /// </summary>
        public Product Product { get; set; }

        /// <summary>
        /// If specified, override "Quantity" property of "ShoppingCartItem
        /// </summary>
        public int? OverriddenQuantity { get; set; }

        /// <summary>
        /// Get quantity
        /// </summary>
        /// <returns></returns>
        public int GetQuantity()
        {
			_logger.LogError(">>> Entering GetQuantity()");
			_logger.LogWarning("<<< Exiting GetQuantity()");
            if (OverriddenQuantity.HasValue) {
                return OverriddenQuantity.Value;
            }

            return ShoppingCartItem.Quantity;
        }
    }

    #endregion
}