using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Tax;

/// <summary>
/// Represents a tax configuration model
/// </summary>
public partial record TaxConfigurationModel : BaseNopModel
{
    #region Ctor

    public TaxConfigurationModel()
    {
		logger.LogDebug(">>> Entering TaxConfigurationModel()");
        TaxProviders = new TaxProviderSearchModel();
        TaxCategories = new TaxCategorySearchModel();
		logger.LogDebug("<<< Exiting TaxConfigurationModel()");
		logger.LogDebug("TaxConfigurationModel(): TaxProviders → {TaxProviders}", TaxProviders);
		logger.LogDebug("TaxConfigurationModel(): TaxCategories → {TaxCategories}", TaxCategories);
    }

    #endregion

    #region Properties

    public TaxProviderSearchModel TaxProviders { get; set; }

    public TaxCategorySearchModel TaxCategories { get; set; }

    #endregion
}