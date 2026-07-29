using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Tax.Avalara.Models.ItemClassification;

/// <summary>
/// Represents a item classification search model
/// </summary>
public record ItemClassificationSearchModel : BaseSearchModel
{
    #region Ctor

    public ItemClassificationSearchModel()
    {
		logger.LogDebug(">>> Entering ItemClassificationSearchModel()");
        AvailableCountries = new List<SelectListItem>();
		logger.LogDebug("<<< Exiting ItemClassificationSearchModel()");
		logger.LogDebug("ItemClassificationSearchModel(): AvailableCountries → {AvailableCountries}", AvailableCountries);
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Plugins.Tax.Avalara.ItemClassification.Search.Country")]
    public int SearchCountryId { get; set; }

    public IList<SelectListItem> AvailableCountries { get; set; }

    #endregion
}