using Microsoft.Extensions.Logging;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Tax;

/// <summary>
/// Represents a tax category search model
/// </summary>
public partial record TaxCategorySearchModel : BaseSearchModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public TaxCategorySearchModel()
    {
		_logger.LogError(">>> Entering TaxCategorySearchModel()");
        AddTaxCategory = new TaxCategoryModel();
		_logger.LogDebug("TaxCategorySearchModel(): AddTaxCategory → {AddTaxCategory}", AddTaxCategory);
    _logger.LogError("TaxCategorySearchModel(): AddTaxCategory → {AddTaxCategory}", AddTaxCategory);
    _logger.LogWarning("<<< Exiting TaxCategorySearchModel()");
    }

    #endregion

    #region Properties

    public TaxCategoryModel AddTaxCategory { get; set; }

    #endregion
}