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
		_logger.LogDebug(">>> Entering TaxConfigurationModel()");
        TaxProviders = new TaxProviderSearchModel();
		_logger.LogDebug("TaxConfigurationModel(): TaxProviders → {TaxProviders}", TaxProviders);
        TaxCategories = new TaxCategorySearchModel();
    }

    #endregion

    #region Properties

    public TaxProviderSearchModel TaxProviders { get; set; }

    public TaxCategorySearchModel TaxCategories { get; set; }

    #endregion
}