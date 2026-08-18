using Nop.Services.Tax;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Tax;
using Nop.Web.Framework.Models.Extensions;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the tax model factory implementation
/// </summary>
public partial class TaxModelFactory : ITaxModelFactory
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly ITaxCategoryService _taxCategoryService;
    protected readonly ITaxPluginManager _taxPluginManager;

    #endregion

    #region Ctor

    public TaxModelFactory(
        ITaxCategoryService taxCategoryService,
        ITaxPluginManager taxPluginManager)
    {
		_logger.LogDebug(">>> Entering TaxModelFactory(taxCategoryService={TaxCategoryService},taxPluginManager={TaxPluginManager})", taxCategoryService, taxPluginManager);
        _taxCategoryService = taxCategoryService;
        _taxPluginManager = taxPluginManager;
		_logger.LogDebug("TaxModelFactory(taxCategoryService={TaxCategoryService},taxPluginManager={TaxPluginManager}): _taxCategoryService → {_taxCategoryService}", taxCategoryService, taxPluginManager, _taxCategoryService);
		_logger.LogDebug("TaxModelFactory(taxCategoryService={TaxCategoryService},taxPluginManager={TaxPluginManager}): _taxPluginManager → {_taxPluginManager}", taxCategoryService, taxPluginManager, _taxPluginManager);
		_logger.LogDebug("<<< Exiting TaxModelFactory(taxCategoryService={TaxCategoryService},taxPluginManager={TaxPluginManager})", taxCategoryService, taxPluginManager);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare tax provider search model
    /// </summary>
    /// <param name="searchModel">Tax provider search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the ax provider search model
    /// </returns>
    public virtual Task<TaxProviderSearchModel> PrepareTaxProviderSearchModelAsync(TaxProviderSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareTaxProviderSearchModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogDebug("<<< Exiting PrepareTaxProviderSearchModelAsync(searchModel={SearchModel})", searchModel);
        return Task.FromResult(searchModel);
    }

    /// <summary>
    /// Prepare paged tax provider list model
    /// </summary>
    /// <param name="searchModel">Tax provider search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the ax provider list model
    /// </returns>
    public virtual async Task<TaxProviderListModel> PrepareTaxProviderListModelAsync(TaxProviderSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareTaxProviderListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //get tax providers
        var taxProviders = (await _taxPluginManager.LoadAllPluginsAsync()).ToPagedList(searchModel);
		_logger.LogDebug("PrepareTaxProviderListModelAsync(searchModel={SearchModel}): taxProviders → {TaxProviders}", searchModel, taxProviders);

        //prepare grid model
        var model = new TaxProviderListModel().PrepareToGrid(searchModel, taxProviders, () =>
        {
            return taxProviders.Select(provider =>
            {
                //fill in model values from the entity
                var taxProviderModel = provider.ToPluginModel<TaxProviderModel>();
				_logger.LogDebug("PrepareTaxProviderListModelAsync(searchModel={SearchModel}): taxProviderModel → {TaxProviderModel}", searchModel, taxProviderModel);

                //fill in additional values (not existing in the entity)
                taxProviderModel.ConfigurationUrl = provider.GetConfigurationPageUrl();
                taxProviderModel.IsPrimaryTaxProvider = _taxPluginManager.IsPluginActive(provider);

                return taxProviderModel;
            });
        });

		_logger.LogDebug("<<< Exiting PrepareTaxProviderListModelAsync(searchModel={SearchModel})", searchModel);
        return model;
    }

    /// <summary>
    /// Prepare tax category search model
    /// </summary>
    /// <param name="searchModel">Tax category search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the ax category search model
    /// </returns>
    public virtual Task<TaxCategorySearchModel> PrepareTaxCategorySearchModelAsync(TaxCategorySearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareTaxCategorySearchModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogDebug("<<< Exiting PrepareTaxCategorySearchModelAsync(searchModel={SearchModel})", searchModel);
        return Task.FromResult(searchModel);
    }

    /// <summary>
    /// Prepare paged tax category list model
    /// </summary>
    /// <param name="searchModel">Tax category search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the ax category list model
    /// </returns>
    public virtual async Task<TaxCategoryListModel> PrepareTaxCategoryListModelAsync(TaxCategorySearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareTaxCategoryListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //get tax categories
        var taxCategories = (await _taxCategoryService.GetAllTaxCategoriesAsync()).ToPagedList(searchModel);
		_logger.LogDebug("PrepareTaxCategoryListModelAsync(searchModel={SearchModel}): taxCategories → {TaxCategories}", searchModel, taxCategories);

        //prepare grid model
        var model = new TaxCategoryListModel().PrepareToGrid(searchModel, taxCategories, () =>
        {
            //fill in model values from the entity
		_logger.LogDebug("<<< Exiting PrepareTaxCategoryListModelAsync(searchModel={SearchModel})", searchModel);
            return taxCategories.Select(taxCategory => taxCategory.ToModel<TaxCategoryModel>());
        });

        return model;
    }

    #endregion
}