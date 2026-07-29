using Nop.Web.Areas.Admin.Models.Directory;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Shipping;

/// <summary>
/// Represents a shipping method restriction model
/// </summary>
public partial record ShippingMethodRestrictionModel : BaseNopModel
{
    #region Ctor

    public ShippingMethodRestrictionModel()
    {
		logger.LogDebug(">>> Entering ShippingMethodRestrictionModel()");
        AvailableShippingMethods = new List<ShippingMethodModel>();
        AvailableCountries = new List<CountryModel>();
		logger.LogDebug("ShippingMethodRestrictionModel(): AvailableShippingMethods → {AvailableShippingMethods}", AvailableShippingMethods);
		logger.LogDebug("ShippingMethodRestrictionModel(): AvailableCountries → {AvailableCountries}", AvailableCountries);
        Restricted = new Dictionary<int, IDictionary<int, bool>>();
		logger.LogDebug("<<< Exiting ShippingMethodRestrictionModel()");
		logger.LogDebug("ShippingMethodRestrictionModel(): Restricted → {Restricted}", Restricted);
    }

    #endregion

    #region Properties

    public IList<ShippingMethodModel> AvailableShippingMethods { get; set; }

    public IList<CountryModel> AvailableCountries { get; set; }

    //[country id] / [shipping method id] / [restricted]
    public IDictionary<int, IDictionary<int, bool>> Restricted { get; set; }

    #endregion
}