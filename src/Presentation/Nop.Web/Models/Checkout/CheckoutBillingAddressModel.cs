using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Models.Common;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Models.Checkout;

public partial record CheckoutBillingAddressModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public CheckoutBillingAddressModel()
    {
		_logger.LogDebug(">>> Entering CheckoutBillingAddressModel()");
        ExistingAddresses = new List<AddressModel>();
        InvalidExistingAddresses = new List<AddressModel>();
		_logger.LogDebug("CheckoutBillingAddressModel(): ExistingAddresses → {ExistingAddresses}", ExistingAddresses);
		_logger.LogDebug("CheckoutBillingAddressModel(): InvalidExistingAddresses → {InvalidExistingAddresses}", InvalidExistingAddresses);
        BillingNewAddress = new AddressModel();
		_logger.LogDebug("<<< Exiting CheckoutBillingAddressModel()");
		_logger.LogDebug("CheckoutBillingAddressModel(): BillingNewAddress → {BillingNewAddress}", BillingNewAddress);
    }

    public IList<AddressModel> ExistingAddresses { get; set; }
    public IList<AddressModel> InvalidExistingAddresses { get; set; }

    public AddressModel BillingNewAddress { get; set; }

    public bool ShipToSameAddress { get; set; }
    public bool ShipToSameAddressAllowed { get; set; }

    /// <summary>
    /// Used on one-page checkout page
    /// </summary>
    public bool NewAddressPreselected { get; set; }

    public bool EuVatEnabled { get; set; }
    public bool EuVatEnabledForGuests { get; set; }

    [NopResourceDisplayName("Checkout.VatNumber")]
    public string VatNumber { get; set; }
}