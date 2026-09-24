using Microsoft.Extensions.Logging;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Tax;

/// <summary>
/// Represents a tax configuration model
/// </summary>
public partial record TaxConfigurationModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public TaxConfigurationModel()
    {
		_logger.LogError(">>> Entering TaxConfigurationModel()");
        TaxProviders = new TaxProviderSearchModel();
        TaxCategories = new TaxCategorySearchModel();
		_logger.LogError("TaxConfigurationModel(): TaxCategories → {TaxCategories}", TaxCategories);
    }

    #endregion

    #region Properties

    public TaxProviderSearchModel TaxProviders { get; set; }

    public TaxCategorySearchModel TaxCategories { get; set; }

    #endregion
}