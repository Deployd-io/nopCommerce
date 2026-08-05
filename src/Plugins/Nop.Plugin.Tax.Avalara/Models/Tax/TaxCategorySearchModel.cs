using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Tax.Avalara.Models.Tax;

/// <summary>
/// Represents a tax category search model
/// </summary>
public record TaxCategorySearchModel : BaseSearchModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public TaxCategorySearchModel()
    {
		_logger.LogDebug(">>> Entering TaxCategorySearchModel()");
        AddTaxCategory = new TaxCategoryModel();
        AvailableTypes = new List<SelectListItem>();
		_logger.LogDebug("<<< Exiting TaxCategorySearchModel()");
		_logger.LogDebug("TaxCategorySearchModel(): AddTaxCategory → {AddTaxCategory}", AddTaxCategory);
		_logger.LogDebug("TaxCategorySearchModel(): AvailableTypes → {AvailableTypes}", AvailableTypes);
    }

    #endregion

    #region Properties

    public TaxCategoryModel AddTaxCategory { get; set; }

    public IList<SelectListItem> AvailableTypes { get; set; }

    #endregion
}