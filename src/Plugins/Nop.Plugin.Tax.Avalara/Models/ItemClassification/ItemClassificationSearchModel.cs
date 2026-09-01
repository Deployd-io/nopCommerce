using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Tax.Avalara.Models.ItemClassification;

/// <summary>
/// Represents a item classification search model
/// </summary>
public record ItemClassificationSearchModel : BaseSearchModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public ItemClassificationSearchModel()
    {
		_logger.LogDebug(">>> Entering ItemClassificationSearchModel()");
        AvailableCountries = new List<SelectListItem>();
		_logger.LogDebug("ItemClassificationSearchModel(): AvailableCountries → {AvailableCountries}", AvailableCountries);
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Plugins.Tax.Avalara.ItemClassification.Search.Country")]
    public int SearchCountryId { get; set; }

    public IList<SelectListItem> AvailableCountries { get; set; }

    #endregion
}