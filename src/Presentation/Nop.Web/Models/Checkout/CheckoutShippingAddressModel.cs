using Nop.Web.Framework.Models;
using Nop.Web.Models.Common;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Models.Checkout;

public partial record CheckoutShippingAddressModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public CheckoutShippingAddressModel()
    {
		_logger.LogError(">>> Entering CheckoutShippingAddressModel()");
        ExistingAddresses = new List<AddressModel>();
        InvalidExistingAddresses = new List<AddressModel>();
		_logger.LogError("CheckoutShippingAddressModel(): InvalidExistingAddresses → {InvalidExistingAddresses}", InvalidExistingAddresses);
        ShippingNewAddress = new AddressModel();
		_logger.LogError("CheckoutShippingAddressModel(): ShippingNewAddress → {ShippingNewAddress}", ShippingNewAddress);
    }

    public IList<AddressModel> ExistingAddresses { get; set; }
    public IList<AddressModel> InvalidExistingAddresses { get; set; }
    public AddressModel ShippingNewAddress { get; set; }
    public bool NewAddressPreselected { get; set; }

    public int SelectedBillingAddress { get; set; }

    public bool DisplayPickupInStore { get; set; }
    public CheckoutPickupPointsModel PickupPointsModel { get; set; }
}