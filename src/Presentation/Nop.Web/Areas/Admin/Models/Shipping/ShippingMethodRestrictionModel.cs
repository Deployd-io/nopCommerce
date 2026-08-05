using Nop.Web.Areas.Admin.Models.Directory;
using Nop.Web.Framework.Models;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Shipping;

/// <summary>
/// Represents a shipping method restriction model
/// </summary>
public partial record ShippingMethodRestrictionModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public ShippingMethodRestrictionModel()
    {
		_logger.LogDebug(">>> Entering ShippingMethodRestrictionModel()");
        AvailableShippingMethods = new List<ShippingMethodModel>();
        AvailableCountries = new List<CountryModel>();
		_logger.LogDebug("ShippingMethodRestrictionModel(): AvailableShippingMethods → {AvailableShippingMethods}", AvailableShippingMethods);
		_logger.LogDebug("ShippingMethodRestrictionModel(): AvailableCountries → {AvailableCountries}", AvailableCountries);
        Restricted = new Dictionary<int, IDictionary<int, bool>>();
		_logger.LogDebug("<<< Exiting ShippingMethodRestrictionModel()");
		_logger.LogDebug("ShippingMethodRestrictionModel(): Restricted → {Restricted}", Restricted);
    }

    #endregion

    #region Properties

    public IList<ShippingMethodModel> AvailableShippingMethods { get; set; }

    public IList<CountryModel> AvailableCountries { get; set; }

    //[country id] / [shipping method id] / [restricted]
    public IDictionary<int, IDictionary<int, bool>> Restricted { get; set; }

    #endregion
}