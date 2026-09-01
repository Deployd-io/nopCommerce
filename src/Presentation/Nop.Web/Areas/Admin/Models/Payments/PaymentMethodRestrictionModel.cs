using Nop.Web.Areas.Admin.Models.Directory;
using Nop.Web.Framework.Models;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Payments;

/// <summary>
/// Represents a payment method restriction model
/// </summary>
public partial record PaymentMethodRestrictionModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public PaymentMethodRestrictionModel()
    {
		_logger.LogDebug(">>> Entering PaymentMethodRestrictionModel()");
        AvailablePaymentMethods = new List<PaymentMethodModel>();
		_logger.LogDebug("PaymentMethodRestrictionModel(): AvailablePaymentMethods → {AvailablePaymentMethods}", AvailablePaymentMethods);
        AvailableCountries = new List<CountryModel>();
		_logger.LogDebug("PaymentMethodRestrictionModel(): AvailableCountries → {AvailableCountries}", AvailableCountries);
        Restricted = new Dictionary<string, IDictionary<int, bool>>();
    }

    #endregion

    #region Properties

    public IList<PaymentMethodModel> AvailablePaymentMethods { get; set; }

    public IList<CountryModel> AvailableCountries { get; set; }

    //[payment method system name] / [customer role id] / [restricted]
    public IDictionary<string, IDictionary<int, bool>> Restricted { get; set; }

    #endregion
}