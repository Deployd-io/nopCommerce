using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Tax;

/// <summary>
/// Represents a request to get tax total
/// </summary>
public partial class TaxTotalRequest
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public TaxTotalRequest()
    {
		_logger.LogDebug(">>> Entering TaxTotalRequest()");
        ShoppingCart = new List<ShoppingCartItem>();
		_logger.LogDebug("TaxTotalRequest(): ShoppingCart → {ShoppingCart}", ShoppingCart);
		_logger.LogDebug("<<< Exiting TaxTotalRequest()");
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets a shopping cart
    /// </summary>
    public IList<ShoppingCartItem> ShoppingCart { get; set; }

    /// <summary>
    /// Gets or sets a customer
    /// </summary>
    public Customer Customer { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to use payment method additional fee
    /// </summary>
    public bool UsePaymentMethodAdditionalFee { get; set; }

    /// <summary>
    /// Gets or sets a store identifier
    /// </summary>
    public int StoreId { get; set; }

    #endregion
}