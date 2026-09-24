using Nop.Core.Domain.Stores;
using Nop.Services.Localization;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Stores;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.Extensions;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the store model factory implementation
/// </summary>
public partial class StoreModelFactory : IStoreModelFactory
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IBaseAdminModelFactory _baseAdminModelFactory;
    protected readonly ILocalizationService _localizationService;
    protected readonly ILocalizedModelFactory _localizedModelFactory;
    protected readonly IStoreService _storeService;

    #endregion

    #region Ctor

    public StoreModelFactory(IBaseAdminModelFactory baseAdminModelFactory,
        ILocalizationService localizationService,
        ILocalizedModelFactory localizedModelFactory,
        IStoreService storeService)
    {
		_logger.LogError(">>> Entering StoreModelFactory(baseAdminModelFactory={BaseAdminModelFactory},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},storeService={StoreService})", baseAdminModelFactory, localizationService, localizedModelFactory, storeService);
        _baseAdminModelFactory = baseAdminModelFactory;
        _localizationService = localizationService;
		_logger.LogDebug("StoreModelFactory(baseAdminModelFactory={BaseAdminModelFactory},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},storeService={StoreService}): _localizationService → {_localizationService}", baseAdminModelFactory, localizationService, localizedModelFactory, storeService, _localizationService);
        _localizedModelFactory = localizedModelFactory;
		_logger.LogDebug("StoreModelFactory(baseAdminModelFactory={BaseAdminModelFactory},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},storeService={StoreService}): _localizedModelFactory → {_localizedModelFactory}", baseAdminModelFactory, localizationService, localizedModelFactory, storeService, _localizedModelFactory);
        _storeService = storeService;
		_logger.LogDebug("StoreModelFactory(baseAdminModelFactory={BaseAdminModelFactory},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},storeService={StoreService}): _storeService → {_storeService}", baseAdminModelFactory, localizationService, localizedModelFactory, storeService, _storeService);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare store search model
    /// </summary>
    /// <param name="searchModel">Store search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the store search model
    /// </returns>
    public virtual Task<StoreSearchModel> PrepareStoreSearchModelAsync(StoreSearchModel searchModel)
    {
		_logger.LogError(">>> Entering PrepareStoreSearchModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogWarning("<<< Exiting PrepareStoreSearchModelAsync(searchModel={SearchModel})", searchModel);
        return Task.FromResult(searchModel);
    }

    /// <summary>
    /// Prepare paged store list model
    /// </summary>
    /// <param name="searchModel">Store search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the store list model
    /// </returns>
    public virtual async Task<StoreListModel> PrepareStoreListModelAsync(StoreSearchModel searchModel)
    {
		_logger.LogError(">>> Entering PrepareStoreListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //get stores
        var stores = (await _storeService.GetAllStoresAsync()).ToPagedList(searchModel);
		_logger.LogError("PrepareStoreListModelAsync(searchModel={SearchModel}): stores → {Stores}", searchModel, stores);

        //prepare list model
        var model = new StoreListModel().PrepareToGrid(searchModel, stores, () =>
        {
            //fill in model values from the entity
		_logger.LogWarning("<<< Exiting PrepareStoreListModelAsync(searchModel={SearchModel})", searchModel);
            return stores.Select(store => store.ToModel<StoreModel>());
        });

        return model;
    }

    /// <summary>
    /// Prepare store model
    /// </summary>
    /// <param name="model">Store model</param>
    /// <param name="store">Store</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the store model
    /// </returns>
    public virtual async Task<StoreModel> PrepareStoreModelAsync(StoreModel model, Store store, bool excludeProperties = false)
    {
		_logger.LogError(">>> Entering PrepareStoreModelAsync(model={Model},store={Store},excludeProperties={ExcludeProperties})", model, store, excludeProperties);
        Func<StoreLocalizedModel, int, Task> localizedModelConfiguration = null;

        if (store != null)
        {
            //fill in model values from the entity
            model ??= store.ToModel<StoreModel>();

            //define localized model configuration action
            localizedModelConfiguration = async (locale, languageId) =>
            {
                locale.Name = await _localizationService.GetLocalizedAsync(store, entity => entity.Name, languageId, false, false);
                locale.DefaultTitle = await _localizationService.GetLocalizedAsync(store, entity => entity.DefaultTitle, languageId, false, false);
                locale.DefaultMetaDescription = await _localizationService.GetLocalizedAsync(store, entity => entity.DefaultMetaDescription, languageId, false, false);
                locale.DefaultMetaKeywords = await _localizationService.GetLocalizedAsync(store, entity => entity.DefaultMetaKeywords, languageId, false, false);
                locale.HomepageDescription = await _localizationService.GetLocalizedAsync(store, entity => entity.HomepageDescription, languageId, false, false);
                locale.HomepageTitle = await _localizationService.GetLocalizedAsync(store, entity => entity.HomepageTitle, languageId, false, false);
            };
        }

        //prepare available languages
        await _baseAdminModelFactory.PrepareLanguagesAsync(model.AvailableLanguages,
            defaultItemText: await _localizationService.GetResourceAsync("Admin.Common.EmptyItemText"));

        //prepare localized models
        if (!excludeProperties)
            model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

		_logger.LogWarning("<<< Exiting PrepareStoreModelAsync(model={Model},store={Store},excludeProperties={ExcludeProperties})", model, store, excludeProperties);
        return model;
    }

    #endregion
}