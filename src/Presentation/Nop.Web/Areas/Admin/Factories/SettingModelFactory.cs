using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Configuration;
using Nop.Core.Domain;
using Nop.Core.Domain.ArtificialIntelligence;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.FilterLevels;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Gdpr;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Seo;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Translation;
using Nop.Core.Domain.Vendors;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Configuration;
using Nop.Services;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Gdpr;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Stores;
using Nop.Services.Themes;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Settings;
using Nop.Web.Areas.Admin.Models.Stores;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.Extensions;
using Nop.Web.Framework.WebOptimizer;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the setting model factory implementation
/// </summary>
public partial class SettingModelFactory : ISettingModelFactory
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly AppSettings _appSettings;
    protected readonly CurrencySettings _currencySettings;
    protected readonly FilterLevelSettings _filterLevelSettings;
    protected readonly IAddressModelFactory _addressModelFactory;
    protected readonly IAddressAttributeModelFactory _addressAttributeModelFactory;
    protected readonly IAddressService _addressService;
    protected readonly IBaseAdminModelFactory _baseAdminModelFactory;
    protected readonly ICurrencyService _currencyService;
    protected readonly ICustomerAttributeModelFactory _customerAttributeModelFactory;
    protected readonly INopDataProvider _dataProvider;
    protected readonly INopFileProvider _fileProvider;
    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly IGdprService _gdprService;
    protected readonly ILocalizedModelFactory _localizedModelFactory;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly ILanguageService _languageService;
    protected readonly ILocalizationService _localizationService;
    protected readonly IPictureService _pictureService;
    protected readonly IReturnRequestModelFactory _returnRequestModelFactory;
    protected readonly IReviewTypeModelFactory _reviewTypeModelFactory;
    protected readonly ISettingService _settingService;
    protected readonly IStoreContext _storeContext;
    protected readonly IStoreService _storeService;
    protected readonly IThemeProvider _themeProvider;
    protected readonly IVendorAttributeModelFactory _vendorAttributeModelFactory;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public SettingModelFactory(AppSettings appSettings,
        CurrencySettings currencySettings,
        FilterLevelSettings filterLevelSettings,
        IAddressModelFactory addressModelFactory,
        IAddressAttributeModelFactory addressAttributeModelFactory,
        IAddressService addressService,
        IBaseAdminModelFactory baseAdminModelFactory,
        ICurrencyService currencyService,
        ICustomerAttributeModelFactory customerAttributeModelFactory,
        INopDataProvider dataProvider,
        INopFileProvider fileProvider,
        IDateTimeHelper dateTimeHelper,
        IGdprService gdprService,
        ILocalizedModelFactory localizedModelFactory,
        IGenericAttributeService genericAttributeService,
        ILanguageService languageService,
        ILocalizationService localizationService,
        IPictureService pictureService,
        IReturnRequestModelFactory returnRequestModelFactory,
        ISettingService settingService,
        IStoreContext storeContext,
        IStoreService storeService,
        IThemeProvider themeProvider,
        IVendorAttributeModelFactory vendorAttributeModelFactory,
        IReviewTypeModelFactory reviewTypeModelFactory,
        IWorkContext workContext)
    {
        _appSettings = appSettings;
        _currencySettings = currencySettings;
        _filterLevelSettings = filterLevelSettings;
        _addressModelFactory = addressModelFactory;
        _addressAttributeModelFactory = addressAttributeModelFactory;
        _addressService = addressService;
        _baseAdminModelFactory = baseAdminModelFactory;
        _currencyService = currencyService;
        _customerAttributeModelFactory = customerAttributeModelFactory;
        _dataProvider = dataProvider;
        _fileProvider = fileProvider;
        _dateTimeHelper = dateTimeHelper;
        _gdprService = gdprService;
        _localizedModelFactory = localizedModelFactory;
        _genericAttributeService = genericAttributeService;
        _languageService = languageService;
        _localizationService = localizationService;
        _pictureService = pictureService;
        _returnRequestModelFactory = returnRequestModelFactory;
        _settingService = settingService;
        _storeContext = storeContext;
        _storeService = storeService;
        _themeProvider = themeProvider;
        _vendorAttributeModelFactory = vendorAttributeModelFactory;
        _reviewTypeModelFactory = reviewTypeModelFactory;
        _workContext = workContext;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Prepare store theme models
    /// </summary>
    /// <param name="models">List of store theme models</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task PrepareStoreThemeModelsAsync(IList<StoreInformationSettingsModel.ThemeModel> models)
    {
		_logger.LogDebug(">>> Entering PrepareStoreThemeModelsAsync(models={Models})", models);
        ArgumentNullException.ThrowIfNull(models);

        //load settings for a chosen store scope
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
		_logger.LogDebug("PrepareStoreThemeModelsAsync(models={Models}): storeId → {StoreId}", models, storeId);
        var storeInformationSettings = await _settingService.LoadSettingAsync<StoreInformationSettings>(storeId);
		_logger.LogDebug("PrepareStoreThemeModelsAsync(models={Models}): storeInformationSettings → {StoreInformationSettings}", models, storeInformationSettings);

        //get available themes
        var availableThemes = await _themeProvider.GetThemesAsync();
		_logger.LogDebug("PrepareStoreThemeModelsAsync(models={Models}): availableThemes → {AvailableThemes}", models, availableThemes);
        foreach (var theme in availableThemes)
        {
            models.Add(new StoreInformationSettingsModel.ThemeModel
            {
                FriendlyName = theme.FriendlyName,
                SystemName = theme.SystemName,
                PreviewImageUrl = theme.PreviewImageUrl,
                PreviewText = theme.PreviewText,
                SupportRtl = theme.SupportRtl,
                Selected = theme.SystemName.Equals(storeInformationSettings.DefaultStoreTheme, StringComparison.InvariantCultureIgnoreCase)
            });
        }
		_logger.LogDebug("<<< Exiting PrepareStoreThemeModelsAsync(models={Models})", models);
    }

    /// <summary>
    /// Prepare sort option search model
    /// </summary>
    /// <param name="searchModel">Sort option search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the sort option search model
    /// </returns>
    protected virtual Task<SortOptionSearchModel> PrepareSortOptionSearchModelAsync(SortOptionSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareSortOptionSearchModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogDebug("<<< Exiting PrepareSortOptionSearchModelAsync(searchModel={SearchModel})", searchModel);
        return Task.FromResult(searchModel);
    }

    /// <summary>
    /// Prepare filter level search model
    /// </summary>
    /// <param name="searchModel">Filter level search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the filter level search model
    /// </returns>
    protected virtual Task<FilterLevelSearchModel> PrepareFilterLevelSearchModelAsync(FilterLevelSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareFilterLevelSearchModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogDebug("<<< Exiting PrepareFilterLevelSearchModelAsync(searchModel={SearchModel})", searchModel);
        return Task.FromResult(searchModel);
    }

    /// <summary>
    /// Prepare artificial intelligence settings model
    /// </summary>
    /// <param name="model">Artificial intelligence search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the artificial intelligence settings model
    /// </returns>
    protected virtual async Task<ArtificialIntelligenceSettingsModel> PrepareArtificialIntelligenceSettingsModelAsync(ArtificialIntelligenceSettingsModel model)
    {
		_logger.LogDebug(">>> Entering PrepareArtificialIntelligenceSettingsModelAsync(model={Model})", model);
        ArgumentNullException.ThrowIfNull(model);

        var artificialIntelligenceSettings = await _settingService.LoadSettingAsync<ArtificialIntelligenceSettings>();
		_logger.LogDebug("PrepareArtificialIntelligenceSettingsModelAsync(model={Model}): artificialIntelligenceSettings → {ArtificialIntelligenceSettings}", model, artificialIntelligenceSettings);

        model.Enabled = artificialIntelligenceSettings.Enabled;
		_logger.LogDebug("PrepareArtificialIntelligenceSettingsModelAsync(model={Model}): model.Enabled → {Arg1}", model, model.Enabled);
		_logger.LogDebug("PrepareArtificialIntelligenceSettingsModelAsync(model={Model}): model.ChatGptApiKey → {Arg1}", model, model.ChatGptApiKey);
		_logger.LogDebug("PrepareArtificialIntelligenceSettingsModelAsync(model={Model}): model.DeepSeekApiKey → {Arg1}", model, model.DeepSeekApiKey);
		_logger.LogDebug("PrepareArtificialIntelligenceSettingsModelAsync(model={Model}): model.GeminiApiKey → {Arg1}", model, model.GeminiApiKey);
        model.ChatGptApiKey = artificialIntelligenceSettings.ChatGptApiKey;
        model.DeepSeekApiKey = artificialIntelligenceSettings.DeepSeekApiKey;
        model.GeminiApiKey = artificialIntelligenceSettings.GeminiApiKey;
        model.ProviderTypeId = (int)artificialIntelligenceSettings.ProviderType;
        model.AllowProductDescriptionGeneration = artificialIntelligenceSettings.AllowProductDescriptionGeneration;
		_logger.LogDebug("PrepareArtificialIntelligenceSettingsModelAsync(model={Model}): model.AllowProductDescriptionGeneration → {Arg1}", model, model.AllowProductDescriptionGeneration);
		_logger.LogDebug("PrepareArtificialIntelligenceSettingsModelAsync(model={Model}): model.ProductDescriptionQuery → {Arg1}", model, model.ProductDescriptionQuery);
		_logger.LogDebug("PrepareArtificialIntelligenceSettingsModelAsync(model={Model}): model.AllowMetaTitleGeneration → {Arg1}", model, model.AllowMetaTitleGeneration);
		_logger.LogDebug("PrepareArtificialIntelligenceSettingsModelAsync(model={Model}): model.MetaTitleQuery → {Arg1}", model, model.MetaTitleQuery);
		_logger.LogDebug("PrepareArtificialIntelligenceSettingsModelAsync(model={Model}): model.AllowMetaKeywordsGeneration → {Arg1}", model, model.AllowMetaKeywordsGeneration);
		_logger.LogDebug("PrepareArtificialIntelligenceSettingsModelAsync(model={Model}): model.MetaKeywordsQuery → {Arg1}", model, model.MetaKeywordsQuery);
		_logger.LogDebug("PrepareArtificialIntelligenceSettingsModelAsync(model={Model}): model.AllowMetaDescriptionGeneration → {Arg1}", model, model.AllowMetaDescriptionGeneration);
		_logger.LogDebug("PrepareArtificialIntelligenceSettingsModelAsync(model={Model}): model.MetaDescriptionQuery → {Arg1}", model, model.MetaDescriptionQuery);
        model.ProductDescriptionQuery = artificialIntelligenceSettings.ProductDescriptionQuery;
        model.AllowMetaTitleGeneration = artificialIntelligenceSettings.AllowMetaTitleGeneration;
        model.MetaTitleQuery = artificialIntelligenceSettings.MetaTitleQuery;
        model.AllowMetaKeywordsGeneration = artificialIntelligenceSettings.AllowMetaKeywordsGeneration;
        model.MetaKeywordsQuery = artificialIntelligenceSettings.MetaKeywordsQuery;
        model.AllowMetaDescriptionGeneration = artificialIntelligenceSettings.AllowMetaDescriptionGeneration;
        model.MetaDescriptionQuery = artificialIntelligenceSettings.MetaDescriptionQuery;

        //prepare available translation services
        var availableProviderType = await ArtificialIntelligenceProviderType.Gemini.ToSelectListAsync(false);
		_logger.LogDebug("PrepareArtificialIntelligenceSettingsModelAsync(model={Model}): availableProviderType → {AvailableProviderType}", model, availableProviderType);
        model.AvailableProviderType = availableProviderType.ToList();

		_logger.LogDebug("<<< Exiting PrepareArtificialIntelligenceSettingsModelAsync(model={Model})", model);
        return model;
    }

    /// <summary>
    /// Prepare GDPR consent search model
    /// </summary>
    /// <param name="searchModel">GDPR consent search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the gDPR consent search model
    /// </returns>
    protected virtual Task<GdprConsentSearchModel> PrepareGdprConsentSearchModelAsync(GdprConsentSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareGdprConsentSearchModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogDebug("<<< Exiting PrepareGdprConsentSearchModelAsync(searchModel={SearchModel})", searchModel);
        return Task.FromResult(searchModel);
    }

    /// <summary>
    /// Prepare address settings model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the address settings model
    /// </returns>
    protected virtual async Task<AddressSettingsModel> PrepareAddressSettingsModelAsync()
    {
		_logger.LogDebug(">>> Entering PrepareAddressSettingsModelAsync()");
        //load settings for a chosen store scope
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
		_logger.LogDebug("PrepareAddressSettingsModelAsync(): storeId → {StoreId}", storeId);
        var addressSettings = await _settingService.LoadSettingAsync<AddressSettings>(storeId);
		_logger.LogDebug("PrepareAddressSettingsModelAsync(): addressSettings → {AddressSettings}", addressSettings);

        //fill in model values from the entity
        var model = addressSettings.ToSettingsModel<AddressSettingsModel>();
		_logger.LogDebug("PrepareAddressSettingsModelAsync(): model → {Model}", model);

		_logger.LogDebug("<<< Exiting PrepareAddressSettingsModelAsync()");
        return model;
    }

    /// <summary>
    /// Prepare customer settings model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer settings model
    /// </returns>
    protected virtual async Task<CustomerSettingsModel> PrepareCustomerSettingsModelAsync()
    {
		_logger.LogDebug(">>> Entering PrepareCustomerSettingsModelAsync()");
        //load settings for a chosen store scope
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
		_logger.LogDebug("PrepareCustomerSettingsModelAsync(): storeId → {StoreId}", storeId);
        var customerSettings = await _settingService.LoadSettingAsync<CustomerSettings>(storeId);
		_logger.LogDebug("PrepareCustomerSettingsModelAsync(): customerSettings → {CustomerSettings}", customerSettings);

        //fill in model values from the entity
        var model = customerSettings.ToSettingsModel<CustomerSettingsModel>();
		_logger.LogDebug("PrepareCustomerSettingsModelAsync(): model → {Model}", model);

		_logger.LogDebug("<<< Exiting PrepareCustomerSettingsModelAsync()");
        return model;
    }

    /// <summary>
    /// Prepare multi-factor authentication settings model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the multiFactorAuthenticationSettingsModel
    /// </returns>
    protected virtual async Task<MultiFactorAuthenticationSettingsModel> PrepareMultiFactorAuthenticationSettingsModelAsync()
    {
		_logger.LogDebug(">>> Entering PrepareMultiFactorAuthenticationSettingsModelAsync()");
        //load settings for a chosen store scope
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
		_logger.LogDebug("PrepareMultiFactorAuthenticationSettingsModelAsync(): storeId → {StoreId}", storeId);
        var multiFactorAuthenticationSettings = await _settingService.LoadSettingAsync<MultiFactorAuthenticationSettings>(storeId);
		_logger.LogDebug("PrepareMultiFactorAuthenticationSettingsModelAsync(): multiFactorAuthenticationSettings → {MultiFactorAuthenticationSettings}", multiFactorAuthenticationSettings);

        //fill in model values from the entity
        var model = multiFactorAuthenticationSettings.ToSettingsModel<MultiFactorAuthenticationSettingsModel>();
		_logger.LogDebug("PrepareMultiFactorAuthenticationSettingsModelAsync(): model → {Model}", model);

		_logger.LogDebug("<<< Exiting PrepareMultiFactorAuthenticationSettingsModelAsync()");
        return model;

    }

    /// <summary>
    /// Prepare date time settings model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the date time settings model
    /// </returns>
    protected virtual async Task<DateTimeSettingsModel> PrepareDateTimeSettingsModelAsync()
    {
		_logger.LogDebug(">>> Entering PrepareDateTimeSettingsModelAsync()");
        //load settings for a chosen store scope
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
		_logger.LogDebug("PrepareDateTimeSettingsModelAsync(): storeId → {StoreId}", storeId);
        var dateTimeSettings = await _settingService.LoadSettingAsync<DateTimeSettings>(storeId);
		_logger.LogDebug("PrepareDateTimeSettingsModelAsync(): dateTimeSettings → {DateTimeSettings}", dateTimeSettings);

        //fill in model values from the entity
        var model = new DateTimeSettingsModel
        {
            AllowCustomersToSetTimeZone = dateTimeSettings.AllowCustomersToSetTimeZone
        };

        //fill in additional values (not existing in the entity)
        model.DefaultStoreTimeZoneId = _dateTimeHelper.DefaultStoreTimeZone.Id;
		_logger.LogDebug("PrepareDateTimeSettingsModelAsync(): model.DefaultStoreTimeZoneId → {Arg0}", model.DefaultStoreTimeZoneId);

        //prepare available time zones
        await _baseAdminModelFactory.PrepareTimeZonesAsync(model.AvailableTimeZones, false);

		_logger.LogDebug("<<< Exiting PrepareDateTimeSettingsModelAsync()");
        return model;
    }

    /// <summary>
    /// Prepare external authentication settings model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the external authentication settings model
    /// </returns>
    protected virtual async Task<ExternalAuthenticationSettingsModel> PrepareExternalAuthenticationSettingsModelAsync()
    {
		_logger.LogDebug(">>> Entering PrepareExternalAuthenticationSettingsModelAsync()");
        //load settings for a chosen store scope
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
		_logger.LogDebug("PrepareExternalAuthenticationSettingsModelAsync(): storeId → {StoreId}", storeId);
        var externalAuthenticationSettings = await _settingService.LoadSettingAsync<ExternalAuthenticationSettings>(storeId);
		_logger.LogDebug("PrepareExternalAuthenticationSettingsModelAsync(): externalAuthenticationSettings → {ExternalAuthenticationSettings}", externalAuthenticationSettings);

        //fill in model values from the entity
        var model = new ExternalAuthenticationSettingsModel
        {
            AllowCustomersToRemoveAssociations = externalAuthenticationSettings.AllowCustomersToRemoveAssociations
        };

		_logger.LogDebug("<<< Exiting PrepareExternalAuthenticationSettingsModelAsync()");
        return model;
    }

    /// <summary>
    /// Prepare store information settings model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the store information settings model
    /// </returns>
    protected virtual async Task<StoreInformationSettingsModel> PrepareStoreInformationSettingsModelAsync()
    {
		_logger.LogDebug(">>> Entering PrepareStoreInformationSettingsModelAsync()");
        //load settings for a chosen store scope
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
		_logger.LogDebug("PrepareStoreInformationSettingsModelAsync(): storeId → {StoreId}", storeId);
        var storeInformationSettings = await _settingService.LoadSettingAsync<StoreInformationSettings>(storeId);
		_logger.LogDebug("PrepareStoreInformationSettingsModelAsync(): storeInformationSettings → {StoreInformationSettings}", storeInformationSettings);
        var commonSettings = await _settingService.LoadSettingAsync<CommonSettings>(storeId);
		_logger.LogDebug("PrepareStoreInformationSettingsModelAsync(): commonSettings → {CommonSettings}", commonSettings);

        //fill in model values from the entity
        var model = new StoreInformationSettingsModel
        {
            StoreClosed = storeInformationSettings.StoreClosed,
            DefaultStoreTheme = storeInformationSettings.DefaultStoreTheme,
            AllowCustomerToSelectTheme = storeInformationSettings.AllowCustomerToSelectTheme,
            LogoPictureId = storeInformationSettings.LogoPictureId,
            DisplayEuCookieLawWarning = storeInformationSettings.DisplayEuCookieLawWarning,
            FacebookLink = storeInformationSettings.FacebookLink,
            TwitterLink = storeInformationSettings.TwitterLink,
            YoutubeLink = storeInformationSettings.YoutubeLink,
            InstagramLink = storeInformationSettings.InstagramLink,
            SubjectFieldOnContactUsForm = commonSettings.SubjectFieldOnContactUsForm,
            UseSystemEmailForContactUsForm = commonSettings.UseSystemEmailForContactUsForm,
            PopupForTermsOfServiceLinks = commonSettings.PopupForTermsOfServiceLinks
        };

        //prepare available themes
        await PrepareStoreThemeModelsAsync(model.AvailableStoreThemes);

        if (storeId <= 0)
            return model;

        //fill in overridden values
        model.StoreClosed_OverrideForStore = await _settingService.SettingExistsAsync(storeInformationSettings, x => x.StoreClosed, storeId);
        model.DefaultStoreTheme_OverrideForStore = await _settingService.SettingExistsAsync(storeInformationSettings, x => x.DefaultStoreTheme, storeId);
        model.AllowCustomerToSelectTheme_OverrideForStore = await _settingService.SettingExistsAsync(storeInformationSettings, x => x.AllowCustomerToSelectTheme, storeId);
        model.LogoPictureId_OverrideForStore = await _settingService.SettingExistsAsync(storeInformationSettings, x => x.LogoPictureId, storeId);
        model.DisplayEuCookieLawWarning_OverrideForStore = await _settingService.SettingExistsAsync(storeInformationSettings, x => x.DisplayEuCookieLawWarning, storeId);
        model.FacebookLink_OverrideForStore = await _settingService.SettingExistsAsync(storeInformationSettings, x => x.FacebookLink, storeId);
        model.TwitterLink_OverrideForStore = await _settingService.SettingExistsAsync(storeInformationSettings, x => x.TwitterLink, storeId);
        model.YoutubeLink_OverrideForStore = await _settingService.SettingExistsAsync(storeInformationSettings, x => x.YoutubeLink, storeId);
        model.InstagramLink_OverrideForStore = await _settingService.SettingExistsAsync(storeInformationSettings, x => x.InstagramLink, storeId);
        model.SubjectFieldOnContactUsForm_OverrideForStore = await _settingService.SettingExistsAsync(commonSettings, x => x.SubjectFieldOnContactUsForm, storeId);
        model.UseSystemEmailForContactUsForm_OverrideForStore = await _settingService.SettingExistsAsync(commonSettings, x => x.UseSystemEmailForContactUsForm, storeId);
        model.PopupForTermsOfServiceLinks_OverrideForStore = await _settingService.SettingExistsAsync(commonSettings, x => x.PopupForTermsOfServiceLinks, storeId);

		_logger.LogDebug("<<< Exiting PrepareStoreInformationSettingsModelAsync()");
        return model;
    }

    /// <summary>
    /// Prepare Sitemap settings model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the sitemap settings model
    /// </returns>
    protected virtual async Task<SitemapSettingsModel> PrepareSitemapSettingsModelAsync()
    {
		_logger.LogDebug(">>> Entering PrepareSitemapSettingsModelAsync()");
        //load settings for a chosen store scope
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
		_logger.LogDebug("PrepareSitemapSettingsModelAsync(): storeId → {StoreId}", storeId);
        var sitemapSettings = await _settingService.LoadSettingAsync<SitemapSettings>(storeId);
		_logger.LogDebug("PrepareSitemapSettingsModelAsync(): sitemapSettings → {SitemapSettings}", sitemapSettings);

        //fill in model values from the entity
        var model = new SitemapSettingsModel
        {
            SitemapEnabled = sitemapSettings.SitemapEnabled,
            SitemapPageSize = sitemapSettings.SitemapPageSize,
            SitemapIncludeCategories = sitemapSettings.SitemapIncludeCategories,
            SitemapIncludeManufacturers = sitemapSettings.SitemapIncludeManufacturers,
            SitemapIncludeProducts = sitemapSettings.SitemapIncludeProducts,
            SitemapIncludeProductTags = sitemapSettings.SitemapIncludeProductTags,
            SitemapIncludeBlogPosts = sitemapSettings.SitemapIncludeBlogPosts,
            SitemapIncludeNews = sitemapSettings.SitemapIncludeNews,
            SitemapIncludeTopics = sitemapSettings.SitemapIncludeTopics
        };

        if (storeId <= 0)
            return model;

        //fill in overridden values
        model.SitemapEnabled_OverrideForStore = await _settingService.SettingExistsAsync(sitemapSettings, x => x.SitemapEnabled, storeId);
        model.SitemapPageSize_OverrideForStore = await _settingService.SettingExistsAsync(sitemapSettings, x => x.SitemapPageSize, storeId);
        model.SitemapIncludeCategories_OverrideForStore = await _settingService.SettingExistsAsync(sitemapSettings, x => x.SitemapIncludeCategories, storeId);
        model.SitemapIncludeManufacturers_OverrideForStore = await _settingService.SettingExistsAsync(sitemapSettings, x => x.SitemapIncludeManufacturers, storeId);
        model.SitemapIncludeProducts_OverrideForStore = await _settingService.SettingExistsAsync(sitemapSettings, x => x.SitemapIncludeProducts, storeId);
        model.SitemapIncludeProductTags_OverrideForStore = await _settingService.SettingExistsAsync(sitemapSettings, x => x.SitemapIncludeProductTags, storeId);
        model.SitemapIncludeBlogPosts_OverrideForStore = await _settingService.SettingExistsAsync(sitemapSettings, x => x.SitemapIncludeBlogPosts, storeId);
        model.SitemapIncludeNews_OverrideForStore = await _settingService.SettingExistsAsync(sitemapSettings, x => x.SitemapIncludeNews, storeId);
        model.SitemapIncludeTopics_OverrideForStore = await _settingService.SettingExistsAsync(sitemapSettings, x => x.SitemapIncludeTopics, storeId);

		_logger.LogDebug("<<< Exiting PrepareSitemapSettingsModelAsync()");
        return model;
    }

    /// <summary>
    /// Prepare minification settings model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the minification settings model
    /// </returns>
    protected virtual async Task<MinificationSettingsModel> PrepareMinificationSettingsModelAsync()
    {
		_logger.LogDebug(">>> Entering PrepareMinificationSettingsModelAsync()");
        //load settings for a chosen store scope
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
		_logger.LogDebug("PrepareMinificationSettingsModelAsync(): storeId → {StoreId}", storeId);
        var minificationSettings = await _settingService.LoadSettingAsync<CommonSettings>(storeId);
		_logger.LogDebug("PrepareMinificationSettingsModelAsync(): minificationSettings → {MinificationSettings}", minificationSettings);

        //fill in model values from the entity
        var model = new MinificationSettingsModel
        {
            EnableHtmlMinification = minificationSettings.EnableHtmlMinification,
            UseResponseCompression = minificationSettings.UseResponseCompression
        };

        if (storeId <= 0)
            return model;

        //fill in overridden values
        model.EnableHtmlMinification_OverrideForStore = await _settingService.SettingExistsAsync(minificationSettings, x => x.EnableHtmlMinification, storeId);
        model.UseResponseCompression_OverrideForStore = await _settingService.SettingExistsAsync(minificationSettings, x => x.UseResponseCompression, storeId);

		_logger.LogDebug("<<< Exiting PrepareMinificationSettingsModelAsync()");
        return model;
    }

    /// <summary>
    /// Prepare SEO settings model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the sEO settings model
    /// </returns>
    protected virtual async Task<SeoSettingsModel> PrepareSeoSettingsModelAsync()
    {
		_logger.LogDebug(">>> Entering PrepareSeoSettingsModelAsync()");
        //load settings for a chosen store scope
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
		_logger.LogDebug("PrepareSeoSettingsModelAsync(): storeId → {StoreId}", storeId);
        var seoSettings = await _settingService.LoadSettingAsync<SeoSettings>(storeId);
		_logger.LogDebug("PrepareSeoSettingsModelAsync(): seoSettings → {SeoSettings}", seoSettings);

        //fill in model values from the entity
        var model = new SeoSettingsModel
        {
            PageTitleSeparator = seoSettings.PageTitleSeparator,
            PageTitleSeoAdjustment = (int)seoSettings.PageTitleSeoAdjustment,
            PageTitleSeoAdjustmentValues = await seoSettings.PageTitleSeoAdjustment.ToSelectListAsync(),
            GenerateProductMetaDescription = seoSettings.GenerateProductMetaDescription,
            ConvertNonWesternChars = seoSettings.ConvertNonWesternChars,
            CanonicalUrlsEnabled = seoSettings.CanonicalUrlsEnabled,
            WwwRequirement = (int)seoSettings.WwwRequirement,
            WwwRequirementValues = await seoSettings.WwwRequirement.ToSelectListAsync(),

            TwitterMetaTags = seoSettings.TwitterMetaTags,
            OpenGraphMetaTags = seoSettings.OpenGraphMetaTags,
            CustomHeadTags = seoSettings.CustomHeadTags,
            MicrodataEnabled = seoSettings.MicrodataEnabled
        };

        if (storeId <= 0)
            return model;

        //fill in overridden values
        model.PageTitleSeparator_OverrideForStore = await _settingService.SettingExistsAsync(seoSettings, x => x.PageTitleSeparator, storeId);
        model.PageTitleSeoAdjustment_OverrideForStore = await _settingService.SettingExistsAsync(seoSettings, x => x.PageTitleSeoAdjustment, storeId);
        model.GenerateProductMetaDescription_OverrideForStore = await _settingService.SettingExistsAsync(seoSettings, x => x.GenerateProductMetaDescription, storeId);
        model.ConvertNonWesternChars_OverrideForStore = await _settingService.SettingExistsAsync(seoSettings, x => x.ConvertNonWesternChars, storeId);
        model.CanonicalUrlsEnabled_OverrideForStore = await _settingService.SettingExistsAsync(seoSettings, x => x.CanonicalUrlsEnabled, storeId);
        model.WwwRequirement_OverrideForStore = await _settingService.SettingExistsAsync(seoSettings, x => x.WwwRequirement, storeId);
        model.TwitterMetaTags_OverrideForStore = await _settingService.SettingExistsAsync(seoSettings, x => x.TwitterMetaTags, storeId);
        model.OpenGraphMetaTags_OverrideForStore = await _settingService.SettingExistsAsync(seoSettings, x => x.OpenGraphMetaTags, storeId);
        model.CustomHeadTags_OverrideForStore = await _settingService.SettingExistsAsync(seoSettings, x => x.CustomHeadTags, storeId);
        model.MicrodataEnabled_OverrideForStore = await _settingService.SettingExistsAsync(seoSettings, x => x.MicrodataEnabled, storeId);

		_logger.LogDebug("<<< Exiting PrepareSeoSettingsModelAsync()");
        return model;
    }

    /// <summary>
    /// Prepare security settings model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the security settings model
    /// </returns>
    protected virtual async Task<SecuritySettingsModel> PrepareSecuritySettingsModelAsync()
    {
		_logger.LogDebug(">>> Entering PrepareSecuritySettingsModelAsync()");
        //load settings for a chosen store scope
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
		_logger.LogDebug("PrepareSecuritySettingsModelAsync(): storeId → {StoreId}", storeId);
        var securitySettings = await _settingService.LoadSettingAsync<SecuritySettings>(storeId);
		_logger.LogDebug("PrepareSecuritySettingsModelAsync(): securitySettings → {SecuritySettings}", securitySettings);

        //fill in model values from the entity
        var model = new SecuritySettingsModel
        {
            EncryptionKey = securitySettings.EncryptionKey,
            HoneypotEnabled = securitySettings.HoneypotEnabled
        };

        //fill in additional values (not existing in the entity)
        if (securitySettings.AdminAreaAllowedIpAddresses != null)
            model.AdminAreaAllowedIpAddresses = string.Join(",", securitySettings.AdminAreaAllowedIpAddresses);

		_logger.LogDebug("<<< Exiting PrepareSecuritySettingsModelAsync()");
        return model;
    }

    /// <summary>
    /// Prepare captcha settings model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the captcha settings model
    /// </returns>
    protected virtual async Task<CaptchaSettingsModel> PrepareCaptchaSettingsModelAsync()
    {
		_logger.LogDebug(">>> Entering PrepareCaptchaSettingsModelAsync()");
        //load settings for a chosen store scope
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
		_logger.LogDebug("PrepareCaptchaSettingsModelAsync(): storeId → {StoreId}", storeId);
        var captchaSettings = await _settingService.LoadSettingAsync<CaptchaSettings>(storeId);
		_logger.LogDebug("PrepareCaptchaSettingsModelAsync(): captchaSettings → {CaptchaSettings}", captchaSettings);

        //fill in model values from the entity
        var model = captchaSettings.ToSettingsModel<CaptchaSettingsModel>();
		_logger.LogDebug("PrepareCaptchaSettingsModelAsync(): model → {Model}", model);

        model.CaptchaTypeValues = await captchaSettings.CaptchaType.ToSelectListAsync();

        if (storeId <= 0)
            return model;

        model.Enabled_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.Enabled, storeId);
        model.ShowOnLoginPage_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.ShowOnLoginPage, storeId);
        model.ShowOnRegistrationPage_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.ShowOnRegistrationPage, storeId);
        model.ShowOnContactUsPage_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.ShowOnContactUsPage, storeId);
        model.ShowOnEmailWishlistToFriendPage_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.ShowOnEmailWishlistToFriendPage, storeId);
        model.ShowOnEmailProductToFriendPage_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.ShowOnEmailProductToFriendPage, storeId);
        model.ShowOnBlogCommentPage_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.ShowOnBlogCommentPage, storeId);
        model.ShowOnNewsCommentPage_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.ShowOnNewsCommentPage, storeId);
        model.ShowOnNewsLetterPage_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.ShowOnNewsletterPage, storeId);
        model.ShowOnProductReviewPage_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.ShowOnProductReviewPage, storeId);
        model.ShowOnApplyVendorPage_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.ShowOnApplyVendorPage, storeId);
        model.ShowOnForgotPasswordPage_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.ShowOnForgotPasswordPage, storeId);
        model.ShowOnForum_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.ShowOnForum, storeId);
        model.ShowOnCheckoutPageForGuests_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.ShowOnCheckoutPageForGuests, storeId);
        model.ReCaptchaPublicKey_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.ReCaptchaPublicKey, storeId);
        model.ReCaptchaPrivateKey_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.ReCaptchaPrivateKey, storeId);
        model.CaptchaType_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.CaptchaType, storeId);
        model.ReCaptchaV3ScoreThreshold_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.ReCaptchaV3ScoreThreshold, storeId);

		_logger.LogDebug("<<< Exiting PrepareCaptchaSettingsModelAsync()");
        return model;
    }

    /// <summary>
    /// Prepare PDF settings model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the pDF settings model
    /// </returns>
    protected virtual async Task<PdfSettingsModel> PreparePdfSettingsModelAsync()
    {
		_logger.LogDebug(">>> Entering PreparePdfSettingsModelAsync()");
        //load settings for a chosen store scope
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
		_logger.LogDebug("PreparePdfSettingsModelAsync(): storeId → {StoreId}", storeId);
        var pdfSettings = await _settingService.LoadSettingAsync<PdfSettings>(storeId);
		_logger.LogDebug("PreparePdfSettingsModelAsync(): pdfSettings → {PdfSettings}", pdfSettings);

        //fill in model values from the entity
        var model = new PdfSettingsModel
        {
            LetterPageSizeEnabled = pdfSettings.LetterPageSizeEnabled,
            LogoPictureId = pdfSettings.LogoPictureId,
            DisablePdfInvoicesForPendingOrders = pdfSettings.DisablePdfInvoicesForPendingOrders,
            InvoiceFooterTextColumn1 = pdfSettings.InvoiceFooterTextColumn1,
            InvoiceFooterTextColumn2 = pdfSettings.InvoiceFooterTextColumn2
        };

        if (storeId <= 0)
            return model;

        //fill in overridden values
        model.LetterPageSizeEnabled_OverrideForStore = await _settingService.SettingExistsAsync(pdfSettings, x => x.LetterPageSizeEnabled, storeId);
        model.LogoPictureId_OverrideForStore = await _settingService.SettingExistsAsync(pdfSettings, x => x.LogoPictureId, storeId);
        model.DisablePdfInvoicesForPendingOrders_OverrideForStore = await _settingService.SettingExistsAsync(pdfSettings, x => x.DisablePdfInvoicesForPendingOrders, storeId);
        model.InvoiceFooterTextColumn1_OverrideForStore = await _settingService.SettingExistsAsync(pdfSettings, x => x.InvoiceFooterTextColumn1, storeId);
        model.InvoiceFooterTextColumn2_OverrideForStore = await _settingService.SettingExistsAsync(pdfSettings, x => x.InvoiceFooterTextColumn2, storeId);

		_logger.LogDebug("<<< Exiting PreparePdfSettingsModelAsync()");
        return model;
    }

    /// <summary>
    /// Prepare localization settings model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the localization settings model
    /// </returns>
    protected virtual async Task<LocalizationSettingsModel> PrepareLocalizationSettingsModelAsync()
    {
		_logger.LogDebug(">>> Entering PrepareLocalizationSettingsModelAsync()");
        //load settings for a chosen store scope
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
		_logger.LogDebug("PrepareLocalizationSettingsModelAsync(): storeId → {StoreId}", storeId);
        var localizationSettings = await _settingService.LoadSettingAsync<LocalizationSettings>(storeId);
		_logger.LogDebug("PrepareLocalizationSettingsModelAsync(): localizationSettings → {LocalizationSettings}", localizationSettings);

        //fill in model values from the entity
        var model = new LocalizationSettingsModel
        {
            UseImagesForLanguageSelection = localizationSettings.UseImagesForLanguageSelection,
            SeoFriendlyUrlsForLanguagesEnabled = localizationSettings.SeoFriendlyUrlsForLanguagesEnabled,
            AutomaticallyDetectLanguage = localizationSettings.AutomaticallyDetectLanguage,
            LoadAllLocaleRecordsOnStartup = localizationSettings.LoadAllLocaleRecordsOnStartup,
            LoadAllLocalizedPropertiesOnStartup = localizationSettings.LoadAllLocalizedPropertiesOnStartup,
            LoadAllUrlRecordsOnStartup = localizationSettings.LoadAllUrlRecordsOnStartup
        };

		_logger.LogDebug("<<< Exiting PrepareLocalizationSettingsModelAsync()");
        return model;
    }

    /// <summary>
    /// Prepare translation settings model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the translation settings model
    /// </returns>
    protected virtual async Task<TranslationSettingsModel> PrepareTranslationSettingsModelAsync()
    {
		_logger.LogDebug(">>> Entering PrepareTranslationSettingsModelAsync()");
        //load settings for a chosen store scope
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
		_logger.LogDebug("PrepareTranslationSettingsModelAsync(): storeId → {StoreId}", storeId);
        var translationSettings = await _settingService.LoadSettingAsync<TranslationSettings>(storeId);
		_logger.LogDebug("PrepareTranslationSettingsModelAsync(): translationSettings → {TranslationSettings}", translationSettings);

        //fill in model values from the entity
        var model = new TranslationSettingsModel
        {
            AllowPreTranslate = translationSettings.AllowPreTranslate,
            TranslateFromLanguageId = translationSettings.TranslateFromLanguageId,
            NotTranslateLanguages = translationSettings.NotTranslateLanguages ?? new List<int>(),
            GoogleApiKey = translationSettings.GoogleApiKey,
            DeepLAuthKey = translationSettings.DeepLAuthKey,
            TranslationServiceId = translationSettings.TranslationServiceId
        };

        //prepare available translation services
        var availableTranslationServices = await TranslationServiceType.GoogleTranslate.ToSelectListAsync(false);
		_logger.LogDebug("PrepareTranslationSettingsModelAsync(): availableTranslationServices → {AvailableTranslationServices}", availableTranslationServices);
        model.AvailableTranslationService = availableTranslationServices.ToList();

        //prepare available languages
        await _baseAdminModelFactory.PrepareLanguagesAsync(model.AvailableLanguages, false);

		_logger.LogDebug("<<< Exiting PrepareTranslationSettingsModelAsync()");
        return model;
    }

    /// <summary>
    /// Prepare admin area settings model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the admin area settings model
    /// </returns>
    protected virtual async Task<AdminAreaSettingsModel> PrepareAdminAreaSettingsModelAsync()
    {
		_logger.LogDebug(">>> Entering PrepareAdminAreaSettingsModelAsync()");
        //load settings for a chosen store scope
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
		_logger.LogDebug("PrepareAdminAreaSettingsModelAsync(): storeId → {StoreId}", storeId);
        var adminAreaSettings = await _settingService.LoadSettingAsync<AdminAreaSettings>(storeId);
		_logger.LogDebug("PrepareAdminAreaSettingsModelAsync(): adminAreaSettings → {AdminAreaSettings}", adminAreaSettings);

        //fill in model values from the entity
        var model = new AdminAreaSettingsModel
        {
            UseRichEditorInMessageTemplates = adminAreaSettings.UseRichEditorInMessageTemplates,
            UseStickyHeaderLayout = adminAreaSettings.UseStickyHeaderLayout
        };

        //fill in overridden values
        if (storeId > 0)
        {
            model.UseRichEditorInMessageTemplates_OverrideForStore = await _settingService.SettingExistsAsync(adminAreaSettings, x => x.UseRichEditorInMessageTemplates, storeId);
        }

		_logger.LogDebug("<<< Exiting PrepareAdminAreaSettingsModelAsync()");
        return model;
    }

    /// <summary>
    /// Prepare setting model to add
    /// </summary>
    /// <param name="model">Setting model to add</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task PrepareAddSettingModelAsync(SettingModel model)
    {
		_logger.LogDebug(">>> Entering PrepareAddSettingModelAsync(model={Model})", model);
        ArgumentNullException.ThrowIfNull(model);

        //prepare available stores
        await _baseAdminModelFactory.PrepareStoresAsync(model.AvailableStores);
		_logger.LogDebug("<<< Exiting PrepareAddSettingModelAsync(model={Model})", model);
    }

    /// <summary>
    /// Prepare custom HTML settings model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the custom HTML settings model
    /// </returns>
    protected virtual async Task<CustomHtmlSettingsModel> PrepareCustomHtmlSettingsModelAsync()
    {
		_logger.LogDebug(">>> Entering PrepareCustomHtmlSettingsModelAsync()");
        //load settings for a chosen store scope
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
		_logger.LogDebug("PrepareCustomHtmlSettingsModelAsync(): storeId → {StoreId}", storeId);
        var commonSettings = await _settingService.LoadSettingAsync<CommonSettings>(storeId);
		_logger.LogDebug("PrepareCustomHtmlSettingsModelAsync(): commonSettings → {CommonSettings}", commonSettings);

        //fill in model values from the entity
        var model = new CustomHtmlSettingsModel
        {
            HeaderCustomHtml = commonSettings.HeaderCustomHtml,
            FooterCustomHtml = commonSettings.FooterCustomHtml
        };

        //fill in overridden values
        if (storeId > 0)
        {
            model.HeaderCustomHtml_OverrideForStore = await _settingService.SettingExistsAsync(commonSettings, x => x.HeaderCustomHtml, storeId);
            model.FooterCustomHtml_OverrideForStore = await _settingService.SettingExistsAsync(commonSettings, x => x.FooterCustomHtml, storeId);
        }

		_logger.LogDebug("<<< Exiting PrepareCustomHtmlSettingsModelAsync()");
        return model;
    }

    /// <summary>
    /// Prepare robots.txt settings model
    /// </summary>
    /// <param name="model">robots.txt model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the robots.txt settings model
    /// </returns>
    protected virtual async Task<RobotsTxtSettingsModel> PrepareRobotsTxtSettingsModelAsync(RobotsTxtSettingsModel model = null)
    {
		_logger.LogDebug(">>> Entering PrepareRobotsTxtSettingsModelAsync(model={Model})", model);
        var additionsInstruction =
            string.Format(
                await _localizationService.GetResourceAsync("Admin.Configuration.Settings.GeneralCommon.RobotsAdditionsInstruction"),
                RobotsTxtDefaults.RobotsAdditionsFileName);

        if (_fileProvider.FileExists(_fileProvider.Combine(_fileProvider.MapPath("~/wwwroot"), RobotsTxtDefaults.RobotsCustomFileName)))
            return new RobotsTxtSettingsModel { CustomFileExists = string.Format(await _localizationService.GetResourceAsync("Admin.Configuration.Settings.GeneralCommon.RobotsCustomFileExists"), RobotsTxtDefaults.RobotsCustomFileName), AdditionsInstruction = additionsInstruction };

        //load settings for a chosen store scope
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
		_logger.LogDebug("PrepareRobotsTxtSettingsModelAsync(model={Model}): storeId → {StoreId}", model, storeId);
        var robotsTxtSettings = await _settingService.LoadSettingAsync<RobotsTxtSettings>(storeId);
		_logger.LogDebug("PrepareRobotsTxtSettingsModelAsync(model={Model}): robotsTxtSettings → {RobotsTxtSettings}", model, robotsTxtSettings);

        model ??= new RobotsTxtSettingsModel
        {
            AllowSitemapXml = robotsTxtSettings.AllowSitemapXml,
            DisallowPaths = string.Join(Environment.NewLine, robotsTxtSettings.DisallowPaths),
            LocalizableDisallowPaths =
                string.Join(Environment.NewLine, robotsTxtSettings.LocalizableDisallowPaths),
            DisallowLanguages = robotsTxtSettings.DisallowLanguages.ToList(),
            AdditionsRules = string.Join(Environment.NewLine, robotsTxtSettings.AdditionsRules),
            AvailableLanguages = new List<SelectListItem>()
        };

        if (!model.AvailableLanguages.Any())
            (model.AvailableLanguages as List<SelectListItem>)?.AddRange((await _languageService.GetAllLanguagesAsync(storeId: storeId)).Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.Name
            }));

        model.AdditionsInstruction = additionsInstruction;
		_logger.LogDebug("PrepareRobotsTxtSettingsModelAsync(model={Model}): model.AdditionsInstruction → {Arg1}", model, model.AdditionsInstruction);

        if (storeId <= 0)
            return model;

        model.AdditionsRules_OverrideForStore = await _settingService.SettingExistsAsync(robotsTxtSettings, x => x.AdditionsRules, storeId);
        model.AllowSitemapXml_OverrideForStore = await _settingService.SettingExistsAsync(robotsTxtSettings, x => x.AllowSitemapXml, storeId);
        model.DisallowLanguages_OverrideForStore = await _settingService.SettingExistsAsync(robotsTxtSettings, x => x.DisallowLanguages, storeId);
        model.DisallowPaths_OverrideForStore = await _settingService.SettingExistsAsync(robotsTxtSettings, x => x.DisallowPaths, storeId);
        model.LocalizableDisallowPaths_OverrideForStore = await _settingService.SettingExistsAsync(robotsTxtSettings, x => x.LocalizableDisallowPaths, storeId);

		_logger.LogDebug("<<< Exiting PrepareRobotsTxtSettingsModelAsync(model={Model})", model);
        return model;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare app settings model
    /// </summary>
    /// <param name="model">AppSettings model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the app settings model
    /// </returns>
    public virtual async Task<AppSettingsModel> PrepareAppSettingsModel(AppSettingsModel model = null)
    {
		_logger.LogDebug(">>> Entering PrepareAppSettingsModel(model={Model})", model);
        model ??= new AppSettingsModel
        {
            CacheConfigModel = _appSettings.Get<CacheConfig>().ToConfigModel<CacheConfigModel>(),
            HostingConfigModel = _appSettings.Get<HostingConfig>().ToConfigModel<HostingConfigModel>(),
            DistributedCacheConfigModel = _appSettings.Get<DistributedCacheConfig>().ToConfigModel<DistributedCacheConfigModel>(),
            InstallationConfigModel = _appSettings.Get<InstallationConfig>().ToConfigModel<InstallationConfigModel>(),
            PluginConfigModel = _appSettings.Get<PluginConfig>().ToConfigModel<PluginConfigModel>(),
            CommonConfigModel = _appSettings.Get<CommonConfig>().ToConfigModel<CommonConfigModel>(),
            DataConfigModel = _appSettings.Get<DataConfig>().ToConfigModel<DataConfigModel>(),
            WebOptimizerConfigModel = _appSettings.Get<WebOptimizerConfig>().ToConfigModel<WebOptimizerConfigModel>(),
        };

        model.DistributedCacheConfigModel.DistributedCacheTypeValues = await _appSettings.Get<DistributedCacheConfig>().DistributedCacheType.ToSelectListAsync();

        model.DataConfigModel.DataProviderTypeValues = await _appSettings.Get<DataConfig>().DataProvider.ToSelectListAsync();

        //Since we decided to use the naming of the DB connections section as in the .net core - "ConnectionStrings",
        //we are forced to adjust our internal model naming to this convention in this check.
        model.EnvironmentVariables.AddRange(from property in model.GetType().GetProperties()
            where property.Name != nameof(AppSettingsModel.EnvironmentVariables)
            from pp in property.PropertyType.GetProperties()
            where Environment.GetEnvironmentVariables().Contains($"{property.Name.Replace("Model", "").Replace("DataConfig", "ConnectionStrings")}__{pp.Name}")
            select $"{property.Name}_{pp.Name}");
		_logger.LogDebug("<<< Exiting PrepareAppSettingsModel(model={Model})", model);
        return model;
    }

    /// <summary>
    /// Prepare blog settings model
    /// </summary>
    /// <param name="model">Blog settings model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the blog settings model
    /// </returns>
    public virtual async Task<BlogSettingsModel> PrepareBlogSettingsModelAsync(BlogSettingsModel model = null)
    {
		_logger.LogDebug(">>> Entering PrepareBlogSettingsModelAsync(model={Model})", model);
        //load settings for a chosen store scope
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
		_logger.LogDebug("PrepareBlogSettingsModelAsync(model={Model}): storeId → {StoreId}", model, storeId);
        var blogSettings = await _settingService.LoadSettingAsync<BlogSettings>(storeId);
		_logger.LogDebug("PrepareBlogSettingsModelAsync(model={Model}): blogSettings → {BlogSettings}", model, blogSettings);

        //fill in model values from the entity
        model ??= blogSettings.ToSettingsModel<BlogSettingsModel>();

        //fill in additional values (not existing in the entity)
        model.ActiveStoreScopeConfiguration = storeId;
		_logger.LogDebug("PrepareBlogSettingsModelAsync(model={Model}): model.ActiveStoreScopeConfiguration → {Arg1}", model, model.ActiveStoreScopeConfiguration);

        if (storeId <= 0)
            return model;

        //fill in overridden values
        model.Enabled_OverrideForStore = await _settingService.SettingExistsAsync(blogSettings, x => x.Enabled, storeId);
        model.PostsPageSize_OverrideForStore = await _settingService.SettingExistsAsync(blogSettings, x => x.PostsPageSize, storeId);
        model.AllowNotRegisteredUsersToLeaveComments_OverrideForStore = await _settingService.SettingExistsAsync(blogSettings, x => x.AllowNotRegisteredUsersToLeaveComments, storeId);
        model.NotifyAboutNewBlogComments_OverrideForStore = await _settingService.SettingExistsAsync(blogSettings, x => x.NotifyAboutNewBlogComments, storeId);
        model.NumberOfTags_OverrideForStore = await _settingService.SettingExistsAsync(blogSettings, x => x.NumberOfTags, storeId);
        model.ShowHeaderRssUrl_OverrideForStore = await _settingService.SettingExistsAsync(blogSettings, x => x.ShowHeaderRssUrl, storeId);
        model.BlogCommentsMustBeApproved_OverrideForStore = await _settingService.SettingExistsAsync(blogSettings, x => x.BlogCommentsMustBeApproved, storeId);

		_logger.LogDebug("<<< Exiting PrepareBlogSettingsModelAsync(model={Model})", model);
        return model;
    }

    /// <summary>
    /// Prepare vendor settings model
    /// </summary>
    /// <param name="model">Vendor settings model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the vendor settings model
    /// </returns>
    public virtual async Task<VendorSettingsModel> PrepareVendorSettingsModelAsync(VendorSettingsModel model = null)
    {
		_logger.LogDebug(">>> Entering PrepareVendorSettingsModelAsync(model={Model})", model);
        //load settings for a chosen store scope
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
		_logger.LogDebug("PrepareVendorSettingsModelAsync(model={Model}): storeId → {StoreId}", model, storeId);
        var vendorSettings = await _settingService.LoadSettingAsync<VendorSettings>(storeId);
		_logger.LogDebug("PrepareVendorSettingsModelAsync(model={Model}): vendorSettings → {VendorSettings}", model, vendorSettings);

        //fill in model values from the entity
        model ??= vendorSettings.ToSettingsModel<VendorSettingsModel>();

        //fill in additional values (not existing in the entity)
        model.ActiveStoreScopeConfiguration = storeId;
		_logger.LogDebug("PrepareVendorSettingsModelAsync(model={Model}): model.ActiveStoreScopeConfiguration → {Arg1}", model, model.ActiveStoreScopeConfiguration);

        //fill in overridden values
        if (storeId > 0)
        {
            model.VendorsBlockItemsToDisplay_OverrideForStore = await _settingService.SettingExistsAsync(vendorSettings, x => x.VendorsBlockItemsToDisplay, storeId);
            model.ShowVendorOnProductDetailsPage_OverrideForStore = await _settingService.SettingExistsAsync(vendorSettings, x => x.ShowVendorOnProductDetailsPage, storeId);
            model.ShowVendorOnOrderDetailsPage_OverrideForStore = await _settingService.SettingExistsAsync(vendorSettings, x => x.ShowVendorOnOrderDetailsPage, storeId);
            model.AllowCustomersToContactVendors_OverrideForStore = await _settingService.SettingExistsAsync(vendorSettings, x => x.AllowCustomersToContactVendors, storeId);
            model.AllowCustomersToApplyForVendorAccount_OverrideForStore = await _settingService.SettingExistsAsync(vendorSettings, x => x.AllowCustomersToApplyForVendorAccount, storeId);
            model.TermsOfServiceEnabled_OverrideForStore = await _settingService.SettingExistsAsync(vendorSettings, x => x.TermsOfServiceEnabled, storeId);
            model.AllowSearchByVendor_OverrideForStore = await _settingService.SettingExistsAsync(vendorSettings, x => x.AllowSearchByVendor, storeId);
            model.AllowVendorsToEditInfo_OverrideForStore = await _settingService.SettingExistsAsync(vendorSettings, x => x.AllowVendorsToEditInfo, storeId);
            model.NotifyStoreOwnerAboutVendorInformationChange_OverrideForStore = await _settingService.SettingExistsAsync(vendorSettings, x => x.NotifyStoreOwnerAboutVendorInformationChange, storeId);
            model.MaximumProductNumber_OverrideForStore = await _settingService.SettingExistsAsync(vendorSettings, x => x.MaximumProductNumber, storeId);
            model.AllowVendorsToImportProducts_OverrideForStore = await _settingService.SettingExistsAsync(vendorSettings, x => x.AllowVendorsToImportProducts, storeId);
        }

        //prepare nested search model
        await _vendorAttributeModelFactory.PrepareVendorAttributeSearchModelAsync(model.VendorAttributeSearchModel);

		_logger.LogDebug("<<< Exiting PrepareVendorSettingsModelAsync(model={Model})", model);
        return model;
    }

    /// <summary>
    /// Prepare forum settings model
    /// </summary>
    /// <param name="model">Forum settings model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the forum settings model
    /// </returns>
    public virtual async Task<ForumSettingsModel> PrepareForumSettingsModelAsync(ForumSettingsModel model = null)
    {
		_logger.LogDebug(">>> Entering PrepareForumSettingsModelAsync(model={Model})", model);
        //load settings for a chosen store scope
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
		_logger.LogDebug("PrepareForumSettingsModelAsync(model={Model}): storeId → {StoreId}", model, storeId);
        var forumSettings = await _settingService.LoadSettingAsync<ForumSettings>(storeId);
		_logger.LogDebug("PrepareForumSettingsModelAsync(model={Model}): forumSettings → {ForumSettings}", model, forumSettings);

        //fill in model values from the entity
        model ??= forumSettings.ToSettingsModel<ForumSettingsModel>();

        //fill in additional values (not existing in the entity)
        model.ActiveStoreScopeConfiguration = storeId;
		_logger.LogDebug("PrepareForumSettingsModelAsync(model={Model}): model.ActiveStoreScopeConfiguration → {Arg1}", model, model.ActiveStoreScopeConfiguration);
        model.ForumEditorValues = await forumSettings.ForumEditor.ToSelectListAsync();

        if (storeId <= 0)
            return model;

        //fill in overridden values
        model.ForumsEnabled_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.ForumsEnabled, storeId);
        model.RelativeDateTimeFormattingEnabled_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.RelativeDateTimeFormattingEnabled, storeId);
        model.ShowCustomersPostCount_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.ShowCustomersPostCount, storeId);
        model.AllowGuestsToCreatePosts_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.AllowGuestsToCreatePosts, storeId);
        model.AllowGuestsToCreateTopics_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.AllowGuestsToCreateTopics, storeId);
        model.AllowCustomersToEditPosts_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.AllowCustomersToEditPosts, storeId);
        model.AllowCustomersToDeletePosts_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.AllowCustomersToDeletePosts, storeId);
        model.AllowPostVoting_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.AllowPostVoting, storeId);
        model.MaxVotesPerDay_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.MaxVotesPerDay, storeId);
        model.AllowCustomersToManageSubscriptions_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.AllowCustomersToManageSubscriptions, storeId);
        model.TopicsPageSize_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.TopicsPageSize, storeId);
        model.PostsPageSize_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.PostsPageSize, storeId);
        model.ForumEditor_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.ForumEditor, storeId);
        model.SignaturesEnabled_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.SignaturesEnabled, storeId);
        model.AllowPrivateMessages_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.AllowPrivateMessages, storeId);
        model.ShowAlertForPM_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.ShowAlertForPM, storeId);
        model.NotifyAboutPrivateMessages_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.NotifyAboutPrivateMessages, storeId);
        model.ActiveDiscussionsFeedEnabled_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.ActiveDiscussionsFeedEnabled, storeId);
        model.ActiveDiscussionsFeedCount_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.ActiveDiscussionsFeedCount, storeId);
        model.ForumFeedsEnabled_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.ForumFeedsEnabled, storeId);
        model.ForumFeedCount_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.ForumFeedCount, storeId);
        model.SearchResultsPageSize_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.SearchResultsPageSize, storeId);
        model.ActiveDiscussionsPageSize_OverrideForStore = await _settingService.SettingExistsAsync(forumSettings, x => x.ActiveDiscussionsPageSize, storeId);

		_logger.LogDebug("<<< Exiting PrepareForumSettingsModelAsync(model={Model})", model);
        return model;
    }

    /// <summary>
    /// Prepare news settings model
    /// </summary>
    /// <param name="model">News settings model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the news settings model
    /// </returns>
    public virtual async Task<NewsSettingsModel> PrepareNewsSettingsModelAsync(NewsSettingsModel model = null)
    {
		_logger.LogDebug(">>> Entering PrepareNewsSettingsModelAsync(model={Model})", model);
        //load settings for a chosen store scope
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
		_logger.LogDebug("PrepareNewsSettingsModelAsync(model={Model}): storeId → {StoreId}", model, storeId);
        var newsSettings = await _settingService.LoadSettingAsync<NewsSettings>(storeId);
		_logger.LogDebug("PrepareNewsSettingsModelAsync(model={Model}): newsSettings → {NewsSettings}", model, newsSettings);

        //fill in model values from the entity
        model ??= newsSettings.ToSettingsModel<NewsSettingsModel>();

        //fill in additional values (not existing in the entity)
        model.ActiveStoreScopeConfiguration = storeId;
		_logger.LogDebug("PrepareNewsSettingsModelAsync(model={Model}): model.ActiveStoreScopeConfiguration → {Arg1}", model, model.ActiveStoreScopeConfiguration);

        if (storeId <= 0)
            return model;

        //fill in overridden values
        model.Enabled_OverrideForStore = await _settingService.SettingExistsAsync(newsSettings, x => x.Enabled, storeId);
        model.AllowNotRegisteredUsersToLeaveComments_OverrideForStore = await _settingService.SettingExistsAsync(newsSettings, x => x.AllowNotRegisteredUsersToLeaveComments, storeId);
        model.NotifyAboutNewNewsComments_OverrideForStore = await _settingService.SettingExistsAsync(newsSettings, x => x.NotifyAboutNewNewsComments, storeId);
        model.ShowNewsOnMainPage_OverrideForStore = await _settingService.SettingExistsAsync(newsSettings, x => x.ShowNewsOnMainPage, storeId);
        model.MainPageNewsCount_OverrideForStore = await _settingService.SettingExistsAsync(newsSettings, x => x.MainPageNewsCount, storeId);
        model.NewsArchivePageSize_OverrideForStore = await _settingService.SettingExistsAsync(newsSettings, x => x.NewsArchivePageSize, storeId);
        model.ShowHeaderRssUrl_OverrideForStore = await _settingService.SettingExistsAsync(newsSettings, x => x.ShowHeaderRssUrl, storeId);
        model.NewsCommentsMustBeApproved_OverrideForStore = await _settingService.SettingExistsAsync(newsSettings, x => x.NewsCommentsMustBeApproved, storeId);

		_logger.LogDebug("<<< Exiting PrepareNewsSettingsModelAsync(model={Model})", model);
        return model;
    }

    /// <summary>
    /// Prepare shipping settings model
    /// </summary>
    /// <param name="model">Shipping settings model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the shipping settings model
    /// </returns>
    public virtual async Task<ShippingSettingsModel> PrepareShippingSettingsModelAsync(ShippingSettingsModel model = null)
    {
		_logger.LogDebug(">>> Entering PrepareShippingSettingsModelAsync(model={Model})", model);
        //load settings for a chosen store scope
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
		_logger.LogDebug("PrepareShippingSettingsModelAsync(model={Model}): storeId → {StoreId}", model, storeId);
        var shippingSettings = await _settingService.LoadSettingAsync<ShippingSettings>(storeId);
		_logger.LogDebug("PrepareShippingSettingsModelAsync(model={Model}): shippingSettings → {ShippingSettings}", model, shippingSettings);

        //fill in model values from the entity
        model ??= shippingSettings.ToSettingsModel<ShippingSettingsModel>();

        //fill in additional values (not existing in the entity)
        model.ActiveStoreScopeConfiguration = storeId;
		_logger.LogDebug("PrepareShippingSettingsModelAsync(model={Model}): model.ActiveStoreScopeConfiguration → {Arg1}", model, model.ActiveStoreScopeConfiguration);
        model.PrimaryStoreCurrencyCode = (await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId))?.CurrencyCode;
        model.SortShippingValues = await shippingSettings.ShippingSorting.ToSelectListAsync();

        //fill in overridden values
        if (storeId > 0)
        {
            model.ShipToSameAddress_OverrideForStore = await _settingService.SettingExistsAsync(shippingSettings, x => x.ShipToSameAddress, storeId);
            model.AllowPickupInStore_OverrideForStore = await _settingService.SettingExistsAsync(shippingSettings, x => x.AllowPickupInStore, storeId);
            model.DisplayPickupPointsOnMap_OverrideForStore = await _settingService.SettingExistsAsync(shippingSettings, x => x.DisplayPickupPointsOnMap, storeId);
            model.IgnoreAdditionalShippingChargeForPickupInStore_OverrideForStore = await _settingService.SettingExistsAsync(shippingSettings, x => x.IgnoreAdditionalShippingChargeForPickupInStore, storeId);
            model.GoogleMapsApiKey_OverrideForStore = await _settingService.SettingExistsAsync(shippingSettings, x => x.GoogleMapsApiKey, storeId);
            model.UseWarehouseLocation_OverrideForStore = await _settingService.SettingExistsAsync(shippingSettings, x => x.UseWarehouseLocation, storeId);
            model.NotifyCustomerAboutShippingFromMultipleLocations_OverrideForStore = await _settingService.SettingExistsAsync(shippingSettings, x => x.NotifyCustomerAboutShippingFromMultipleLocations, storeId);
            model.FreeShippingOverXEnabled_OverrideForStore = await _settingService.SettingExistsAsync(shippingSettings, x => x.FreeShippingOverXEnabled, storeId);
            model.FreeShippingOverXValue_OverrideForStore = await _settingService.SettingExistsAsync(shippingSettings, x => x.FreeShippingOverXValue, storeId);
            model.FreeShippingOverXIncludingTax_OverrideForStore = await _settingService.SettingExistsAsync(shippingSettings, x => x.FreeShippingOverXIncludingTax, storeId);
            model.EstimateShippingCartPageEnabled_OverrideForStore = await _settingService.SettingExistsAsync(shippingSettings, x => x.EstimateShippingCartPageEnabled, storeId);
            model.EstimateShippingProductPageEnabled_OverrideForStore = await _settingService.SettingExistsAsync(shippingSettings, x => x.EstimateShippingProductPageEnabled, storeId);
            model.EstimateShippingCityNameEnabled_OverrideForStore = await _settingService.SettingExistsAsync(shippingSettings, x => x.EstimateShippingCityNameEnabled, storeId);
            model.DisplayShipmentEventsToCustomers_OverrideForStore = await _settingService.SettingExistsAsync(shippingSettings, x => x.DisplayShipmentEventsToCustomers, storeId);
            model.DisplayShipmentEventsToStoreOwner_OverrideForStore = await _settingService.SettingExistsAsync(shippingSettings, x => x.DisplayShipmentEventsToStoreOwner, storeId);
            model.HideShippingTotal_OverrideForStore = await _settingService.SettingExistsAsync(shippingSettings, x => x.HideShippingTotal, storeId);
            model.BypassShippingMethodSelectionIfOnlyOne_OverrideForStore = await _settingService.SettingExistsAsync(shippingSettings, x => x.BypassShippingMethodSelectionIfOnlyOne, storeId);
            model.ConsiderAssociatedProductsDimensions_OverrideForStore = await _settingService.SettingExistsAsync(shippingSettings, x => x.ConsiderAssociatedProductsDimensions, storeId);
            model.ShippingOriginAddress_OverrideForStore = await _settingService.SettingExistsAsync(shippingSettings, x => x.ShippingOriginAddressId, storeId);
            model.ShippingSorting_OverrideForStore = await _settingService.SettingExistsAsync(shippingSettings, x => x.ShippingSorting, storeId);
        }

        //prepare shipping origin address
        var originAddress = await _addressService.GetAddressByIdAsync(shippingSettings.ShippingOriginAddressId);
		_logger.LogDebug("PrepareShippingSettingsModelAsync(model={Model}): originAddress → {OriginAddress}", model, originAddress);
        if (originAddress != null)
            model.ShippingOriginAddress = originAddress.ToModel(model.ShippingOriginAddress);
        await _addressModelFactory.PrepareAddressModelAsync(model.ShippingOriginAddress, originAddress);
        model.ShippingOriginAddress.ZipPostalCodeRequired = true;

		_logger.LogDebug("<<< Exiting PrepareShippingSettingsModelAsync(model={Model})", model);
        return model;
    }

    /// <summary>
    /// Prepare tax settings model
    /// </summary>
    /// <param name="model">Tax settings model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the ax settings model
    /// </returns>
    public virtual async Task<TaxSettingsModel> PrepareTaxSettingsModelAsync(TaxSettingsModel model = null)
    {
		_logger.LogDebug(">>> Entering PrepareTaxSettingsModelAsync(model={Model})", model);
        //load settings for a chosen store scope
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
		_logger.LogDebug("PrepareTaxSettingsModelAsync(model={Model}): storeId → {StoreId}", model, storeId);
        var taxSettings = await _settingService.LoadSettingAsync<TaxSettings>(storeId);
		_logger.LogDebug("PrepareTaxSettingsModelAsync(model={Model}): taxSettings → {TaxSettings}", model, taxSettings);

        //fill in model values from the entity
        model ??= taxSettings.ToSettingsModel<TaxSettingsModel>();
        model.TaxBasedOnValues = await taxSettings.TaxBasedOn.ToSelectListAsync();
        model.TaxDisplayTypeValues = await taxSettings.TaxDisplayType.ToSelectListAsync();

        //fill in additional values (not existing in the entity)
        model.ActiveStoreScopeConfiguration = storeId;
		_logger.LogDebug("PrepareTaxSettingsModelAsync(model={Model}): model.ActiveStoreScopeConfiguration → {Arg1}", model, model.ActiveStoreScopeConfiguration);

        //fill in overridden values
        if (storeId > 0)
        {
            model.AutomaticallyDetectCountry_OverrideForStore = await _settingService.SettingExistsAsync(taxSettings, x => x.AutomaticallyDetectCountry, storeId);
            model.PricesIncludeTax_OverrideForStore = await _settingService.SettingExistsAsync(taxSettings, x => x.PricesIncludeTax, storeId);
            model.AllowCustomersToSelectTaxDisplayType_OverrideForStore = await _settingService.SettingExistsAsync(taxSettings, x => x.AllowCustomersToSelectTaxDisplayType, storeId);
            model.TaxDisplayType_OverrideForStore = await _settingService.SettingExistsAsync(taxSettings, x => x.TaxDisplayType, storeId);
            model.DisplayTaxSuffix_OverrideForStore = await _settingService.SettingExistsAsync(taxSettings, x => x.DisplayTaxSuffix, storeId);
            model.DisplayTaxRates_OverrideForStore = await _settingService.SettingExistsAsync(taxSettings, x => x.DisplayTaxRates, storeId);
            model.HideZeroTax_OverrideForStore = await _settingService.SettingExistsAsync(taxSettings, x => x.HideZeroTax, storeId);
            model.HideTaxInOrderSummary_OverrideForStore = await _settingService.SettingExistsAsync(taxSettings, x => x.HideTaxInOrderSummary, storeId);
            model.ForceTaxExclusionFromOrderSubtotal_OverrideForStore = await _settingService.SettingExistsAsync(taxSettings, x => x.ForceTaxExclusionFromOrderSubtotal, storeId);
            model.DefaultTaxCategoryId_OverrideForStore = await _settingService.SettingExistsAsync(taxSettings, x => x.DefaultTaxCategoryId, storeId);
            model.TaxBasedOn_OverrideForStore = await _settingService.SettingExistsAsync(taxSettings, x => x.TaxBasedOn, storeId);
            model.TaxBasedOnPickupPointAddress_OverrideForStore = await _settingService.SettingExistsAsync(taxSettings, x => x.TaxBasedOnPickupPointAddress, storeId);
            model.DefaultTaxAddress_OverrideForStore = await _settingService.SettingExistsAsync(taxSettings, x => x.DefaultTaxAddressId, storeId);
            model.ShippingIsTaxable_OverrideForStore = await _settingService.SettingExistsAsync(taxSettings, x => x.ShippingIsTaxable, storeId);
            model.ShippingPriceIncludesTax_OverrideForStore = await _settingService.SettingExistsAsync(taxSettings, x => x.ShippingPriceIncludesTax, storeId);
            model.ShippingTaxClassId_OverrideForStore = await _settingService.SettingExistsAsync(taxSettings, x => x.ShippingTaxClassId, storeId);
            model.PaymentMethodAdditionalFeeIsTaxable_OverrideForStore = await _settingService.SettingExistsAsync(taxSettings, x => x.PaymentMethodAdditionalFeeIsTaxable, storeId);
            model.PaymentMethodAdditionalFeeIncludesTax_OverrideForStore = await _settingService.SettingExistsAsync(taxSettings, x => x.PaymentMethodAdditionalFeeIncludesTax, storeId);
            model.PaymentMethodAdditionalFeeTaxClassId_OverrideForStore = await _settingService.SettingExistsAsync(taxSettings, x => x.PaymentMethodAdditionalFeeTaxClassId, storeId);
            model.EuVatEnabled_OverrideForStore = await _settingService.SettingExistsAsync(taxSettings, x => x.EuVatEnabled, storeId);
            model.EuVatEnabledForGuests_OverrideForStore = await _settingService.SettingExistsAsync(taxSettings, x => x.EuVatEnabledForGuests, storeId);
            model.EuVatRequired_OverrideForStore = await _settingService.SettingExistsAsync(taxSettings, x => x.EuVatRequired, storeId);
            model.EuVatShopCountryId_OverrideForStore = await _settingService.SettingExistsAsync(taxSettings, x => x.EuVatShopCountryId, storeId);
            model.EuVatAllowVatExemption_OverrideForStore = await _settingService.SettingExistsAsync(taxSettings, x => x.EuVatAllowVatExemption, storeId);
            model.EuVatUseWebService_OverrideForStore = await _settingService.SettingExistsAsync(taxSettings, x => x.EuVatUseWebService, storeId);
            model.EuVatAssumeValid_OverrideForStore = await _settingService.SettingExistsAsync(taxSettings, x => x.EuVatAssumeValid, storeId);
            model.EuVatEmailAdminWhenNewVatSubmitted_OverrideForStore = await _settingService.SettingExistsAsync(taxSettings, x => x.EuVatEmailAdminWhenNewVatSubmitted, storeId);
            model.HmrcApiUrl_OverrideForStore = await _settingService.SettingExistsAsync(taxSettings, x => x.HmrcApiUrl, storeId);
            model.HmrcClientId_OverrideForStore = await _settingService.SettingExistsAsync(taxSettings, x => x.HmrcClientId, storeId);
            model.HmrcClientSecret_OverrideForStore = await _settingService.SettingExistsAsync(taxSettings, x => x.HmrcClientSecret, storeId);
        }

        //prepare available tax categories
        await _baseAdminModelFactory.PrepareTaxCategoriesAsync(model.TaxCategories);

        //prepare available EU VAT countries
        await _baseAdminModelFactory.PrepareCountriesAsync(model.EuVatShopCountries);

        //prepare default tax address
        var defaultAddress = await _addressService.GetAddressByIdAsync(taxSettings.DefaultTaxAddressId);
		_logger.LogDebug("PrepareTaxSettingsModelAsync(model={Model}): defaultAddress → {DefaultAddress}", model, defaultAddress);
        if (defaultAddress != null)
            model.DefaultTaxAddress = defaultAddress.ToModel(model.DefaultTaxAddress);
        await _addressModelFactory.PrepareAddressModelAsync(model.DefaultTaxAddress, defaultAddress);
        model.DefaultTaxAddress.ZipPostalCodeRequired = true;

		_logger.LogDebug("<<< Exiting PrepareTaxSettingsModelAsync(model={Model})", model);
        return model;
    }

    /// <summary>
    /// Prepare catalog settings model
    /// </summary>
    /// <param name="model">Catalog settings model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the catalog settings model
    /// </returns>
    public virtual async Task<CatalogSettingsModel> PrepareCatalogSettingsModelAsync(CatalogSettingsModel model = null)
    {
		_logger.LogDebug(">>> Entering PrepareCatalogSettingsModelAsync(model={Model})", model);
        //load settings for a chosen store scope
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
		_logger.LogDebug("PrepareCatalogSettingsModelAsync(model={Model}): storeId → {StoreId}", model, storeId);
        var catalogSettings = await _settingService.LoadSettingAsync<CatalogSettings>(storeId);
		_logger.LogDebug("PrepareCatalogSettingsModelAsync(model={Model}): catalogSettings → {CatalogSettings}", model, catalogSettings);

        //fill in model values from the entity
        model ??= catalogSettings.ToSettingsModel<CatalogSettingsModel>();

        //fill in additional values (not existing in the entity)
        model.ActiveStoreScopeConfiguration = storeId;
		_logger.LogDebug("PrepareCatalogSettingsModelAsync(model={Model}): model.ActiveStoreScopeConfiguration → {Arg1}", model, model.ActiveStoreScopeConfiguration);
        model.PrimaryStoreCurrencyCode = (await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId))?.CurrencyCode;
        model.AttributeValueOutOfStockDisplayTypes = await catalogSettings.AttributeValueOutOfStockDisplayType.ToSelectListAsync();
        model.ProductUrlStructureTypes = await ((ProductUrlStructureType)catalogSettings.ProductUrlStructureTypeId).ToSelectListAsync();
        model.AvailableViewModes.Add(new SelectListItem
        {
            Text = await _localizationService.GetResourceAsync("Admin.Catalog.ViewMode.Grid"),
            Value = "grid"
        });
        model.AvailableViewModes.Add(new SelectListItem
        {
            Text = await _localizationService.GetResourceAsync("Admin.Catalog.ViewMode.List"),
            Value = "list"
        });

        //fill in overridden values
        if (storeId > 0)
        {
            model.AllowViewUnpublishedProductPage_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.AllowViewUnpublishedProductPage, storeId);
            model.DisplayDiscontinuedMessageForUnpublishedProducts_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.DisplayDiscontinuedMessageForUnpublishedProducts, storeId);
            model.ShowSkuOnProductDetailsPage_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ShowSkuOnProductDetailsPage, storeId);
            model.ShowSkuOnCatalogPages_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ShowSkuOnCatalogPages, storeId);
            model.ShowManufacturerPartNumber_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ShowManufacturerPartNumber, storeId);
            model.ShowGtin_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ShowGtin, storeId);
            model.ShowFreeShippingNotification_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ShowFreeShippingNotification, storeId);
            model.ShowShortDescriptionOnCatalogPages_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ShowShortDescriptionOnCatalogPages, storeId);
            model.AllowProductSorting_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.AllowProductSorting, storeId);
            model.AllowProductViewModeChanging_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.AllowProductViewModeChanging, storeId);
            model.DefaultViewMode_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.DefaultViewMode, storeId);
            model.ShowProductsFromSubcategories_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ShowProductsFromSubcategories, storeId);
            model.ShowCategoryProductNumber_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ShowCategoryProductNumber, storeId);
            model.ShowCategoryProductNumberIncludingSubcategories_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ShowCategoryProductNumberIncludingSubcategories, storeId);
            model.CategoryBreadcrumbEnabled_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.CategoryBreadcrumbEnabled, storeId);
            model.ShowShareButton_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ShowShareButton, storeId);
            model.PageShareCode_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.PageShareCode, storeId);
            model.ProductReviewsMustBeApproved_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ProductReviewsMustBeApproved, storeId);
            model.OneReviewPerProductFromCustomer_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.OneReviewPerProductFromCustomer, storeId);
            model.AllowAnonymousUsersToReviewProduct_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.AllowAnonymousUsersToReviewProduct, storeId);
            model.ProductReviewPossibleOnlyAfterPurchasing_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ProductReviewPossibleOnlyAfterPurchasing, storeId);
            model.NotifyStoreOwnerAboutNewProductReviews_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.NotifyStoreOwnerAboutNewProductReviews, storeId);
            model.NotifyCustomerAboutProductReviewReply_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.NotifyCustomerAboutProductReviewReply, storeId);
            model.EmailAFriendEnabled_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.EmailAFriendEnabled, storeId);
            model.AllowAnonymousUsersToEmailAFriend_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.AllowAnonymousUsersToEmailAFriend, storeId);
            model.RecentlyViewedProductsNumber_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.RecentlyViewedProductsNumber, storeId);
            model.RecentlyViewedProductsEnabled_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.RecentlyViewedProductsEnabled, storeId);
            model.NewProductsEnabled_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.NewProductsEnabled, storeId);
            model.NewProductsPageSize_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.NewProductsPageSize, storeId);
            model.NewProductsAllowCustomersToSelectPageSize_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.NewProductsAllowCustomersToSelectPageSize, storeId);
            model.NewProductsPageSizeOptions_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.NewProductsPageSizeOptions, storeId);
            model.CompareProductsEnabled_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.CompareProductsEnabled, storeId);
            model.ShowBestsellersOnHomepage_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ShowBestsellersOnHomepage, storeId);
            model.NumberOfBestsellersOnHomepage_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.NumberOfBestsellersOnHomepage, storeId);
            model.SearchPageProductsPerPage_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.SearchPageProductsPerPage, storeId);
            model.SearchPageAllowCustomersToSelectPageSize_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.SearchPageAllowCustomersToSelectPageSize, storeId);
            model.ShowSearchBoxCategories_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ShowSearchBoxCategories, storeId);
            model.SearchPagePriceRangeFiltering_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.SearchPagePriceRangeFiltering, storeId);
            model.SearchPagePriceFrom_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.SearchPagePriceFrom, storeId);
            model.SearchPagePriceTo_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.SearchPagePriceTo, storeId);
            model.SearchPageManuallyPriceRange_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.SearchPageManuallyPriceRange, storeId);
            model.SearchPagePageSizeOptions_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.SearchPagePageSizeOptions, storeId);
            model.ProductSearchAutoCompleteEnabled_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ProductSearchAutoCompleteEnabled, storeId);
            model.ProductSearchAutoCompleteNumberOfProducts_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ProductSearchAutoCompleteNumberOfProducts, storeId);
            model.ShowProductImagesInSearchAutoComplete_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ShowProductImagesInSearchAutoComplete, storeId);
            model.ShowLinkToAllResultInSearchAutoComplete_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ShowLinkToAllResultInSearchAutoComplete, storeId);
            model.ProductSearchTermMinimumLength_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ProductSearchTermMinimumLength, storeId);
            model.ProductsAlsoPurchasedEnabled_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ProductsAlsoPurchasedEnabled, storeId);
            model.ProductsAlsoPurchasedNumber_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ProductsAlsoPurchasedNumber, storeId);
            model.NumberOfProductTags_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.NumberOfProductTags, storeId);
            model.ProductsByTagPageSize_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ProductsByTagPageSize, storeId);
            model.ProductsByTagAllowCustomersToSelectPageSize_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ProductsByTagAllowCustomersToSelectPageSize, storeId);
            model.ProductsByTagPageSizeOptions_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ProductsByTagPageSizeOptions, storeId);
            model.ProductsByTagPriceRangeFiltering_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ProductsByTagPriceRangeFiltering, storeId);
            model.ProductsByTagPriceFrom_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ProductsByTagPriceFrom, storeId);
            model.ProductsByTagPriceTo_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ProductsByTagPriceTo, storeId);
            model.ProductsByTagManuallyPriceRange_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ProductsByTagManuallyPriceRange, storeId);
            model.IncludeShortDescriptionInCompareProducts_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.IncludeShortDescriptionInCompareProducts, storeId);
            model.IncludeFullDescriptionInCompareProducts_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.IncludeFullDescriptionInCompareProducts, storeId);
            model.ManufacturersBlockItemsToDisplay_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ManufacturersBlockItemsToDisplay, storeId);
            model.DisplayTaxShippingInfoFooter_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.DisplayTaxShippingInfoFooter, storeId);
            model.DisplayTaxShippingInfoProductDetailsPage_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.DisplayTaxShippingInfoProductDetailsPage, storeId);
            model.DisplayTaxShippingInfoProductBoxes_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.DisplayTaxShippingInfoProductBoxes, storeId);
            model.DisplayTaxShippingInfoShoppingCart_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.DisplayTaxShippingInfoShoppingCart, storeId);
            model.DisplayTaxShippingInfoWishlist_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.DisplayTaxShippingInfoWishlist, storeId);
            model.DisplayTaxShippingInfoOrderDetailsPage_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.DisplayTaxShippingInfoOrderDetailsPage, storeId);
            model.ShowProductReviewsPerStore_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ShowProductReviewsPerStore, storeId);
            model.ShowProductReviewsOnAccountPage_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ShowProductReviewsTabOnAccountPage, storeId);
            model.ProductReviewsPageSizeOnAccountPage_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ProductReviewsPageSizeOnAccountPage, storeId);
            model.ProductReviewsSortByCreatedDateAscending_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ProductReviewsSortByCreatedDateAscending, storeId);
            model.ExportImportProductAttributes_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ExportImportProductAttributes, storeId);
            model.ExportImportProductSpecificationAttributes_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ExportImportProductSpecificationAttributes, storeId);
            model.ExportImportTierPrices_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ExportImportTierPrices, storeId);
            model.ExportImportProductCategoryBreadcrumb_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ExportImportProductCategoryBreadcrumb, storeId);
            model.ExportImportCategoriesUsingCategoryName_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ExportImportCategoriesUsingCategoryName, storeId);
            model.ExportImportAllowDownloadImages_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ExportImportAllowDownloadImages, storeId);
            model.ExportImportSplitProductsFile_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ExportImportSplitProductsFile, storeId);
            model.RemoveRequiredProducts_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.RemoveRequiredProducts, storeId);
            model.ExportImportRelatedEntitiesByName_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ExportImportRelatedEntitiesByName, storeId);
            model.ExportImportProductUseLimitedToStores_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ExportImportProductUseLimitedToStores, storeId);
            model.ExportImportCategoryUseLimitedToStores_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ExportImportCategoryUseLimitedToStores, storeId);
            model.DisplayDatePreOrderAvailability_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.DisplayDatePreOrderAvailability, storeId);
            model.UseAjaxCatalogProductsLoading_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.UseAjaxCatalogProductsLoading, storeId);
            model.EnableManufacturerFiltering_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.EnableManufacturerFiltering, storeId);
            model.EnablePriceRangeFiltering_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.EnablePriceRangeFiltering, storeId);
            model.EnableSpecificationAttributeFiltering_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.EnableSpecificationAttributeFiltering, storeId);
            model.DisplayFromPrices_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.DisplayFromPrices, storeId);
            model.AttributeValueOutOfStockDisplayType_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.AttributeValueOutOfStockDisplayType, storeId);
            model.AllowCustomersToSearchWithManufacturerName_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.AllowCustomersToSearchWithManufacturerName, storeId);
            model.AllowCustomersToSearchWithCategoryName_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.AllowCustomersToSearchWithCategoryName, storeId);
            model.DisplayAllPicturesOnCatalogPages_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.DisplayAllPicturesOnCatalogPages, storeId);
            model.ProductUrlStructureTypeId_OverrideForStore = await _settingService.SettingExistsAsync(catalogSettings, x => x.ProductUrlStructureTypeId, storeId);
        }

        //prepare nested search model
        await PrepareSortOptionSearchModelAsync(model.SortOptionSearchModel);
        await _reviewTypeModelFactory.PrepareReviewTypeSearchModelAsync(model.ReviewTypeSearchModel);

        await PrepareArtificialIntelligenceSettingsModelAsync(model.ArtificialIntelligenceSettingsModel);

		_logger.LogDebug("<<< Exiting PrepareCatalogSettingsModelAsync(model={Model})", model);
        return model;
    }

    /// <summary>
    /// Prepare filter level settings model
    /// </summary>
    /// <param name="model">Filter level settings model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the filter level settings model
    /// </returns>
    public virtual async Task<FilterLevelSettingsModel> PrepareFilterLevelSettingsModelAsync(FilterLevelSettingsModel model = null)
    {
		_logger.LogDebug(">>> Entering PrepareFilterLevelSettingsModelAsync(model={Model})", model);
        //load settings

        //fill in model values from the entity
        model ??= _filterLevelSettings.ToSettingsModel<FilterLevelSettingsModel>();

        //prepare nested search model
        await PrepareFilterLevelSearchModelAsync(model.FilterLevelSearchModel);
		_logger.LogDebug("<<< Exiting PrepareFilterLevelSettingsModelAsync(model={Model})", model);
        return model;
    }

    /// <summary>
    /// Prepare paged filter level list model
    /// </summary>
    /// <param name="searchModel">Filter level search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the filter level list model
    /// </returns>
    public virtual async Task<FilterLevelListModel> PrepareFilterLevelListModelAsync(FilterLevelSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareFilterLevelListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //get filter levels
        var filterLevels = Enum.GetValues(typeof(FilterLevelEnum)).OfType<FilterLevelEnum>().ToList().ToPagedList(searchModel);
		_logger.LogDebug("PrepareFilterLevelListModelAsync(searchModel={SearchModel}): filterLevels → {FilterLevels}", searchModel, filterLevels);

        //prepare list model
        var model = await new FilterLevelListModel().PrepareToGridAsync(searchModel, filterLevels, () =>
        {
            return filterLevels.SelectAwait(async filterLevel =>
            {
                //fill in model values from the entity
                var filterLevelModel = new FilterLevelModel { Id = (int)filterLevel };
				_logger.LogDebug("PrepareFilterLevelListModelAsync(searchModel={SearchModel}): filterLevelModel → {FilterLevelModel}", searchModel, filterLevelModel);

                //fill in additional values (not existing in the entity)
                filterLevelModel.Name = await _localizationService.GetLocalizedEnumAsync(filterLevel);
                filterLevelModel.Enabled = !_filterLevelSettings.FilterLevelEnumDisabled.Contains((int)filterLevel);

                return filterLevelModel;
            }).OrderBy(filterLevel => filterLevel.Id);
        });

		_logger.LogDebug("<<< Exiting PrepareFilterLevelListModelAsync(searchModel={SearchModel})", searchModel);
        return model;
    }

    /// <summary>
    /// Prepare filter level model
    /// </summary>
    /// <param name="model">Filter level model</param>
    /// <param name="filterLevel">Filter level</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the filter level model
    /// </returns>
    public virtual async Task<FilterLevelModel> PrepareFilterLevelModelAsync(FilterLevelModel model, FilterLevelEnum filterLevel, bool excludeProperties = false)
    {
		_logger.LogDebug(">>> Entering PrepareFilterLevelModelAsync(model={Model},filterLevel={FilterLevel},excludeProperties={ExcludeProperties})", model, filterLevel, excludeProperties);
        Func<FilterLevelLocalizedModel, int, Task> localizedModelConfiguration = null;

        //fill in model values from settings
        model ??= new FilterLevelModel { Id = (int)filterLevel };
        model.Name = await _localizationService.GetLocalizedEnumAsync(filterLevel);
        model.Enabled = !_filterLevelSettings.FilterLevelEnumDisabled.Contains((int)filterLevel);

        //define localized model configuration action
        localizedModelConfiguration = async (locale, languageId) =>
        {
            var resourceName = $"Enums.Nop.Core.Domain.FilterLevels.FilterLevelEnum.{filterLevel}";
			_logger.LogDebug("PrepareFilterLevelModelAsync(model={Model},filterLevel={FilterLevel},excludeProperties={ExcludeProperties}): resourceName → {ResourceName}", model, filterLevel, excludeProperties, resourceName);
			_logger.LogDebug("PrepareFilterLevelModelAsync(model={Model},filterLevel={FilterLevel},excludeProperties={ExcludeProperties}): locale.Name → {Arg3}", model, filterLevel, excludeProperties, locale.Name);
            var resource = await _localizationService.GetLocaleStringResourceByNameAsync(resourceName, languageId, false);
			_logger.LogDebug("PrepareFilterLevelModelAsync(model={Model},filterLevel={FilterLevel},excludeProperties={ExcludeProperties}): resource → {Resource}", model, filterLevel, excludeProperties, resource);
            locale.Name = resource?.ResourceValue ?? string.Empty;
        };

        //prepare localized models
        if (!excludeProperties)
            model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

		_logger.LogDebug("<<< Exiting PrepareFilterLevelModelAsync(model={Model},filterLevel={FilterLevel},excludeProperties={ExcludeProperties})", model, filterLevel, excludeProperties);
        return model;
    }

    /// <summary>
    /// Prepare paged sort option list model
    /// </summary>
    /// <param name="searchModel">Sort option search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the sort option list model
    /// </returns>
    public virtual async Task<SortOptionListModel> PrepareSortOptionListModelAsync(SortOptionSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareSortOptionListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
		_logger.LogDebug("PrepareSortOptionListModelAsync(searchModel={SearchModel}): storeId → {StoreId}", searchModel, storeId);
        var catalogSettings = await _settingService.LoadSettingAsync<CatalogSettings>(storeId);
		_logger.LogDebug("PrepareSortOptionListModelAsync(searchModel={SearchModel}): catalogSettings → {CatalogSettings}", searchModel, catalogSettings);

        //get sort options
        var sortOptions = Enum.GetValues(typeof(ProductSortingEnum)).OfType<ProductSortingEnum>().ToList().ToPagedList(searchModel);
		_logger.LogDebug("PrepareSortOptionListModelAsync(searchModel={SearchModel}): sortOptions → {SortOptions}", searchModel, sortOptions);

        //prepare list model
        var model = await new SortOptionListModel().PrepareToGridAsync(searchModel, sortOptions, () =>
        {
            return sortOptions.SelectAwait(async option =>
            {
                //fill in model values from the entity
                var sortOptionModel = new SortOptionModel { Id = (int)option };
				_logger.LogDebug("PrepareSortOptionListModelAsync(searchModel={SearchModel}): sortOptionModel → {SortOptionModel}", searchModel, sortOptionModel);

                //fill in additional values (not existing in the entity)
                sortOptionModel.Name = await _localizationService.GetLocalizedEnumAsync(option);
                sortOptionModel.IsActive = !catalogSettings.ProductSortingEnumDisabled.Contains((int)option);
                sortOptionModel.DisplayOrder = catalogSettings
                    .ProductSortingEnumDisplayOrder.TryGetValue((int)option, out var value) ? value : (int)option;
					_logger.LogDebug("PrepareSortOptionListModelAsync(searchModel={SearchModel}): sortOptionModel.DisplayOrder → {Arg1}", searchModel, sortOptionModel.DisplayOrder);

                return sortOptionModel;
            }).OrderBy(option => option.DisplayOrder);
        });

		_logger.LogDebug("<<< Exiting PrepareSortOptionListModelAsync(searchModel={SearchModel})", searchModel);
        return model;
    }

    /// <summary>
    /// Prepare reward points settings model
    /// </summary>
    /// <param name="model">Reward points settings model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the reward points settings model
    /// </returns>
    public virtual async Task<RewardPointsSettingsModel> PrepareRewardPointsSettingsModelAsync(RewardPointsSettingsModel model = null)
    {
		_logger.LogDebug(">>> Entering PrepareRewardPointsSettingsModelAsync(model={Model})", model);
        //load settings for a chosen store scope
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
		_logger.LogDebug("PrepareRewardPointsSettingsModelAsync(model={Model}): storeId → {StoreId}", model, storeId);
        var rewardPointsSettings = await _settingService.LoadSettingAsync<RewardPointsSettings>(storeId);
		_logger.LogDebug("PrepareRewardPointsSettingsModelAsync(model={Model}): rewardPointsSettings → {RewardPointsSettings}", model, rewardPointsSettings);

        //fill in model values from the entity
        model ??= rewardPointsSettings.ToSettingsModel<RewardPointsSettingsModel>();

        //fill in additional values (not existing in the entity)
        model.ActiveStoreScopeConfiguration = storeId;
		_logger.LogDebug("PrepareRewardPointsSettingsModelAsync(model={Model}): model.ActiveStoreScopeConfiguration → {Arg1}", model, model.ActiveStoreScopeConfiguration);
        model.PrimaryStoreCurrencyCode = (await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId))?.CurrencyCode;
        model.ActivatePointsImmediately = model.ActivationDelay <= 0;

        if (storeId <= 0)
            return model;

        //fill in overridden values
        model.Enabled_OverrideForStore = await _settingService.SettingExistsAsync(rewardPointsSettings, x => x.Enabled, storeId);
        model.ExchangeRate_OverrideForStore = await _settingService.SettingExistsAsync(rewardPointsSettings, x => x.ExchangeRate, storeId);
        model.MinimumRewardPointsToUse_OverrideForStore = await _settingService.SettingExistsAsync(rewardPointsSettings, x => x.MinimumRewardPointsToUse, storeId);
        model.MaximumRewardPointsToUsePerOrder_OverrideForStore = await _settingService.SettingExistsAsync(rewardPointsSettings, x => x.MaximumRewardPointsToUsePerOrder, storeId);
        model.MaximumRedeemedRate_OverrideForStore = await _settingService.SettingExistsAsync(rewardPointsSettings, x => x.MaximumRedeemedRate, storeId);
        model.PointsForRegistration_OverrideForStore = await _settingService.SettingExistsAsync(rewardPointsSettings, x => x.PointsForRegistration, storeId);
        model.RegistrationPointsValidity_OverrideForStore = await _settingService.SettingExistsAsync(rewardPointsSettings, x => x.RegistrationPointsValidity, storeId);
        model.PointsForPurchases_OverrideForStore = await _settingService.SettingExistsAsync(rewardPointsSettings, x => x.PointsForPurchases_Amount, storeId) || await _settingService.SettingExistsAsync(rewardPointsSettings, x => x.PointsForPurchases_Points, storeId);
        model.MinOrderTotalToAwardPoints_OverrideForStore = await _settingService.SettingExistsAsync(rewardPointsSettings, x => x.MinOrderTotalToAwardPoints, storeId);
        model.PurchasesPointsValidity_OverrideForStore = await _settingService.SettingExistsAsync(rewardPointsSettings, x => x.PurchasesPointsValidity, storeId);
        model.ActivationDelay_OverrideForStore = await _settingService.SettingExistsAsync(rewardPointsSettings, x => x.ActivationDelay, storeId);
        model.DisplayHowMuchWillBeEarned_OverrideForStore = await _settingService.SettingExistsAsync(rewardPointsSettings, x => x.DisplayHowMuchWillBeEarned, storeId);
        model.PageSize_OverrideForStore = await _settingService.SettingExistsAsync(rewardPointsSettings, x => x.PageSize, storeId);

		_logger.LogDebug("<<< Exiting PrepareRewardPointsSettingsModelAsync(model={Model})", model);
        return model;
    }

    /// <summary>
    /// Prepare order settings model
    /// </summary>
    /// <param name="model">Order settings model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the order settings model
    /// </returns>
    public virtual async Task<OrderSettingsModel> PrepareOrderSettingsModelAsync(OrderSettingsModel model = null)
    {
		_logger.LogDebug(">>> Entering PrepareOrderSettingsModelAsync(model={Model})", model);
        //load settings for a chosen store scope
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
		_logger.LogDebug("PrepareOrderSettingsModelAsync(model={Model}): storeId → {StoreId}", model, storeId);
        var orderSettings = await _settingService.LoadSettingAsync<OrderSettings>(storeId);
		_logger.LogDebug("PrepareOrderSettingsModelAsync(model={Model}): orderSettings → {OrderSettings}", model, orderSettings);

        //fill in model values from the entity
        model ??= orderSettings.ToSettingsModel<OrderSettingsModel>();

        //fill in additional values (not existing in the entity)
        model.ActiveStoreScopeConfiguration = storeId;
		_logger.LogDebug("PrepareOrderSettingsModelAsync(model={Model}): model.ActiveStoreScopeConfiguration → {Arg1}", model, model.ActiveStoreScopeConfiguration);
        model.PrimaryStoreCurrencyCode = (await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId))?.CurrencyCode;
        model.OrderIdent = await _dataProvider.GetTableIdentAsync<Order>();

        //fill in overridden values
        if (storeId > 0)
        {
            model.IsReOrderAllowed_OverrideForStore = await _settingService.SettingExistsAsync(orderSettings, x => x.IsReOrderAllowed, storeId);
            model.MinOrderSubtotalAmount_OverrideForStore = await _settingService.SettingExistsAsync(orderSettings, x => x.MinOrderSubtotalAmount, storeId);
            model.MinOrderSubtotalAmountIncludingTax_OverrideForStore = await _settingService.SettingExistsAsync(orderSettings, x => x.MinOrderSubtotalAmountIncludingTax, storeId);
            model.MinOrderTotalAmount_OverrideForStore = await _settingService.SettingExistsAsync(orderSettings, x => x.MinOrderTotalAmount, storeId);
            model.AutoUpdateOrderTotalsOnEditingOrder_OverrideForStore = await _settingService.SettingExistsAsync(orderSettings, x => x.AutoUpdateOrderTotalsOnEditingOrder, storeId);
            model.AnonymousCheckoutAllowed_OverrideForStore = await _settingService.SettingExistsAsync(orderSettings, x => x.AnonymousCheckoutAllowed, storeId);
            model.CheckoutDisabled_OverrideForStore = await _settingService.SettingExistsAsync(orderSettings, x => x.CheckoutDisabled, storeId);
            model.TermsOfServiceOnShoppingCartPage_OverrideForStore = await _settingService.SettingExistsAsync(orderSettings, x => x.TermsOfServiceOnShoppingCartPage, storeId);
            model.TermsOfServiceOnOrderConfirmPage_OverrideForStore = await _settingService.SettingExistsAsync(orderSettings, x => x.TermsOfServiceOnOrderConfirmPage, storeId);
            model.OnePageCheckoutEnabled_OverrideForStore = await _settingService.SettingExistsAsync(orderSettings, x => x.OnePageCheckoutEnabled, storeId);
            model.OnePageCheckoutDisplayOrderTotalsOnPaymentInfoTab_OverrideForStore = await _settingService.SettingExistsAsync(orderSettings, x => x.OnePageCheckoutDisplayOrderTotalsOnPaymentInfoTab, storeId);
            model.DisableBillingAddressCheckoutStep_OverrideForStore = await _settingService.SettingExistsAsync(orderSettings, x => x.DisableBillingAddressCheckoutStep, storeId);
            model.DisableOrderCompletedPage_OverrideForStore = await _settingService.SettingExistsAsync(orderSettings, x => x.DisableOrderCompletedPage, storeId);
            model.DisplayPickupInStoreOnShippingMethodPage_OverrideForStore = await _settingService.SettingExistsAsync(orderSettings, x => x.DisplayPickupInStoreOnShippingMethodPage, storeId);
            model.AttachPdfInvoiceToOrderPlacedEmail_OverrideForStore = await _settingService.SettingExistsAsync(orderSettings, x => x.AttachPdfInvoiceToOrderPlacedEmail, storeId);
            model.AttachPdfInvoiceToOrderPaidEmail_OverrideForStore = await _settingService.SettingExistsAsync(orderSettings, x => x.AttachPdfInvoiceToOrderPaidEmail, storeId);
            model.AttachPdfInvoiceToOrderProcessingEmail_OverrideForStore = await _settingService.SettingExistsAsync(orderSettings, x => x.AttachPdfInvoiceToOrderProcessingEmail, storeId);
            model.AttachPdfInvoiceToOrderCompletedEmail_OverrideForStore = await _settingService.SettingExistsAsync(orderSettings, x => x.AttachPdfInvoiceToOrderCompletedEmail, storeId);
            model.ReturnRequestsEnabled_OverrideForStore = await _settingService.SettingExistsAsync(orderSettings, x => x.ReturnRequestsEnabled, storeId);
            model.ReturnRequestsAllowFiles_OverrideForStore = await _settingService.SettingExistsAsync(orderSettings, x => x.ReturnRequestsAllowFiles, storeId);
            model.ReturnRequestNumberMask_OverrideForStore = await _settingService.SettingExistsAsync(orderSettings, x => x.ReturnRequestNumberMask, storeId);
            model.NumberOfDaysReturnRequestAvailable_OverrideForStore = await _settingService.SettingExistsAsync(orderSettings, x => x.NumberOfDaysReturnRequestAvailable, storeId);
            model.CustomOrderNumberMask_OverrideForStore = await _settingService.SettingExistsAsync(orderSettings, x => x.CustomOrderNumberMask, storeId);
            model.ExportWithProducts_OverrideForStore = await _settingService.SettingExistsAsync(orderSettings, x => x.ExportWithProducts, storeId);
            model.AllowAdminsToBuyCallForPriceProducts_OverrideForStore = await _settingService.SettingExistsAsync(orderSettings, x => x.AllowAdminsToBuyCallForPriceProducts, storeId);
            model.AllowCustomersCancelOrders_OverrideForStore = await _settingService.SettingExistsAsync(orderSettings, x => x.AllowCustomersCancelOrders, storeId);
            model.ShowProductThumbnailInOrderDetailsPage_OverrideForStore = await _settingService.SettingExistsAsync(orderSettings, x => x.ShowProductThumbnailInOrderDetailsPage, storeId);
            model.DeleteGiftCardUsageHistory_OverrideForStore = await _settingService.SettingExistsAsync(orderSettings, x => x.DeleteGiftCardUsageHistory, storeId);
        }

        //prepare nested search models
        await _returnRequestModelFactory.PrepareReturnRequestReasonSearchModelAsync(model.ReturnRequestReasonSearchModel);
        await _returnRequestModelFactory.PrepareReturnRequestActionSearchModelAsync(model.ReturnRequestActionSearchModel);

		_logger.LogDebug("<<< Exiting PrepareOrderSettingsModelAsync(model={Model})", model);
        return model;
    }

    /// <summary>
    /// Prepare shopping cart settings model
    /// </summary>
    /// <param name="model">Shopping cart settings model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the shopping cart settings model
    /// </returns>
    public virtual async Task<ShoppingCartSettingsModel> PrepareShoppingCartSettingsModelAsync(ShoppingCartSettingsModel model = null)
    {
		_logger.LogDebug(">>> Entering PrepareShoppingCartSettingsModelAsync(model={Model})", model);
        //load settings for a chosen store scope
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
		_logger.LogDebug("PrepareShoppingCartSettingsModelAsync(model={Model}): storeId → {StoreId}", model, storeId);
        var shoppingCartSettings = await _settingService.LoadSettingAsync<ShoppingCartSettings>(storeId);
		_logger.LogDebug("PrepareShoppingCartSettingsModelAsync(model={Model}): shoppingCartSettings → {ShoppingCartSettings}", model, shoppingCartSettings);

        //fill in model values from the entity
        model ??= shoppingCartSettings.ToSettingsModel<ShoppingCartSettingsModel>();

        //fill in additional values (not existing in the entity)
        model.ActiveStoreScopeConfiguration = storeId;
		_logger.LogDebug("PrepareShoppingCartSettingsModelAsync(model={Model}): model.ActiveStoreScopeConfiguration → {Arg1}", model, model.ActiveStoreScopeConfiguration);

        if (storeId <= 0)
            return model;

        //fill in overridden values
        model.DisplayCartAfterAddingProduct_OverrideForStore = await _settingService.SettingExistsAsync(shoppingCartSettings, x => x.DisplayCartAfterAddingProduct, storeId);
        model.DisplayWishlistAfterAddingProduct_OverrideForStore = await _settingService.SettingExistsAsync(shoppingCartSettings, x => x.DisplayWishlistAfterAddingProduct, storeId);
        model.MaximumShoppingCartItems_OverrideForStore = await _settingService.SettingExistsAsync(shoppingCartSettings, x => x.MaximumShoppingCartItems, storeId);
        model.MaximumWishlistItems_OverrideForStore = await _settingService.SettingExistsAsync(shoppingCartSettings, x => x.MaximumWishlistItems, storeId);
        model.AllowMultipleWishlist_OverrideForStore = await _settingService.SettingExistsAsync(shoppingCartSettings, x => x.AllowMultipleWishlist, storeId);
        model.MaximumNumberOfCustomWishlist_OverrideForStore = await _settingService.SettingExistsAsync(shoppingCartSettings, x => x.MaximumNumberOfCustomWishlist, storeId);
        model.AllowOutOfStockItemsToBeAddedToWishlist_OverrideForStore = await _settingService.SettingExistsAsync(shoppingCartSettings, x => x.AllowOutOfStockItemsToBeAddedToWishlist, storeId);
        model.MoveItemsFromWishlistToCart_OverrideForStore = await _settingService.SettingExistsAsync(shoppingCartSettings, x => x.MoveItemsFromWishlistToCart, storeId);
        model.CartsSharedBetweenStores_OverrideForStore = await _settingService.SettingExistsAsync(shoppingCartSettings, x => x.CartsSharedBetweenStores, storeId);
        model.ShowProductImagesOnShoppingCart_OverrideForStore = await _settingService.SettingExistsAsync(shoppingCartSettings, x => x.ShowProductImagesOnShoppingCart, storeId);
        model.ShowProductImagesOnWishList_OverrideForStore = await _settingService.SettingExistsAsync(shoppingCartSettings, x => x.ShowProductImagesOnWishList, storeId);
        model.ShowDiscountBox_OverrideForStore = await _settingService.SettingExistsAsync(shoppingCartSettings, x => x.ShowDiscountBox, storeId);
        model.ShowGiftCardBox_OverrideForStore = await _settingService.SettingExistsAsync(shoppingCartSettings, x => x.ShowGiftCardBox, storeId);
        model.CrossSellsNumber_OverrideForStore = await _settingService.SettingExistsAsync(shoppingCartSettings, x => x.CrossSellsNumber, storeId);
        model.EmailWishlistEnabled_OverrideForStore = await _settingService.SettingExistsAsync(shoppingCartSettings, x => x.EmailWishlistEnabled, storeId);
        model.AllowAnonymousUsersToEmailWishlist_OverrideForStore = await _settingService.SettingExistsAsync(shoppingCartSettings, x => x.AllowAnonymousUsersToEmailWishlist, storeId);
        model.MiniShoppingCartEnabled_OverrideForStore = await _settingService.SettingExistsAsync(shoppingCartSettings, x => x.MiniShoppingCartEnabled, storeId);
        model.ShowProductImagesInMiniShoppingCart_OverrideForStore = await _settingService.SettingExistsAsync(shoppingCartSettings, x => x.ShowProductImagesInMiniShoppingCart, storeId);
        model.MiniShoppingCartProductNumber_OverrideForStore = await _settingService.SettingExistsAsync(shoppingCartSettings, x => x.MiniShoppingCartProductNumber, storeId);
        model.AllowCartItemEditing_OverrideForStore = await _settingService.SettingExistsAsync(shoppingCartSettings, x => x.AllowCartItemEditing, storeId);
        model.GroupTierPricesForDistinctShoppingCartItems_OverrideForStore = await _settingService.SettingExistsAsync(shoppingCartSettings, x => x.GroupTierPricesForDistinctShoppingCartItems, storeId);

		_logger.LogDebug("<<< Exiting PrepareShoppingCartSettingsModelAsync(model={Model})", model);
        return model;
    }

    /// <summary>
    /// Prepare media settings model
    /// </summary>
    /// <param name="model">Media settings model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the media settings model
    /// </returns>
    public virtual async Task<MediaSettingsModel> PrepareMediaSettingsModelAsync(MediaSettingsModel model = null)
    {
		_logger.LogDebug(">>> Entering PrepareMediaSettingsModelAsync(model={Model})", model);
        //load settings for a chosen store scope
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
		_logger.LogDebug("PrepareMediaSettingsModelAsync(model={Model}): storeId → {StoreId}", model, storeId);
        var mediaSettings = await _settingService.LoadSettingAsync<MediaSettings>(storeId);
		_logger.LogDebug("PrepareMediaSettingsModelAsync(model={Model}): mediaSettings → {MediaSettings}", model, mediaSettings);

        //fill in model values from the entity
        model ??= mediaSettings.ToSettingsModel<MediaSettingsModel>();

        //fill in additional values (not existing in the entity)
        model.ActiveStoreScopeConfiguration = storeId;
		_logger.LogDebug("PrepareMediaSettingsModelAsync(model={Model}): model.ActiveStoreScopeConfiguration → {Arg1}", model, model.ActiveStoreScopeConfiguration);
        model.PicturesStoredIntoDatabase = await _pictureService.IsStoreInDbAsync();

        if (storeId <= 0)
            return model;

        //fill in overridden values
        model.AvatarPictureSize_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.AvatarPictureSize, storeId);
        model.ProductThumbPictureSize_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.ProductThumbPictureSize, storeId);
        model.ProductDetailsPictureSize_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.ProductDetailsPictureSize, storeId);
        model.ProductThumbPictureSizeOnProductDetailsPage_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.ProductThumbPictureSizeOnProductDetailsPage, storeId);
        model.AssociatedProductPictureSize_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.AssociatedProductPictureSize, storeId);
        model.CategoryThumbPictureSize_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.CategoryThumbPictureSize, storeId);
        model.ManufacturerThumbPictureSize_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.ManufacturerThumbPictureSize, storeId);
        model.VendorThumbPictureSize_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.VendorThumbPictureSize, storeId);
        model.CartThumbPictureSize_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.CartThumbPictureSize, storeId);
        model.OrderThumbPictureSize_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.OrderThumbPictureSize, storeId);
        model.MiniCartThumbPictureSize_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.MiniCartThumbPictureSize, storeId);
        model.MaximumImageSize_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.MaximumImageSize, storeId);
        model.MultipleThumbDirectories_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.MultipleThumbDirectories, storeId);
        model.DefaultImageQuality_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.DefaultImageQuality, storeId);
        model.ImportProductImagesUsingHash_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.ImportProductImagesUsingHash, storeId);
        model.DefaultPictureZoomEnabled_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.DefaultPictureZoomEnabled, storeId);
        model.AllowSvgUploads_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.AllowSvgUploads, storeId);
        model.ProductDefaultImageId_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.ProductDefaultImageId, storeId);

		_logger.LogDebug("<<< Exiting PrepareMediaSettingsModelAsync(model={Model})", model);
        return model;
    }

    /// <summary>
    /// Prepare customer user settings model
    /// </summary>
    /// <param name="model">Customer user settings model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer user settings model
    /// </returns>
    public virtual async Task<CustomerUserSettingsModel> PrepareCustomerUserSettingsModelAsync(CustomerUserSettingsModel model = null)
    {
		_logger.LogDebug(">>> Entering PrepareCustomerUserSettingsModelAsync(model={Model})", model);
        model ??= new CustomerUserSettingsModel
        {
            ActiveStoreScopeConfiguration = await _storeContext.GetActiveStoreScopeConfigurationAsync()
        };

        //prepare customer settings model
        model.CustomerSettings = await PrepareCustomerSettingsModelAsync();

        //prepare CustomerSettings list availableCountries
        await _baseAdminModelFactory.PrepareCountriesAsync(model.CustomerSettings.AvailableCountries);

        //prepare multi-factor authentication settings model
        model.MultiFactorAuthenticationSettings = await PrepareMultiFactorAuthenticationSettingsModelAsync();

        //prepare address settings model
        model.AddressSettings = await PrepareAddressSettingsModelAsync();

        //prepare AddressSettings list availableCountries
        await _baseAdminModelFactory.PrepareCountriesAsync(model.AddressSettings.AvailableCountries);

        //prepare date time settings model
        model.DateTimeSettings = await PrepareDateTimeSettingsModelAsync();

        //prepare external authentication settings model
        model.ExternalAuthenticationSettings = await PrepareExternalAuthenticationSettingsModelAsync();

        //prepare nested search models
        await _customerAttributeModelFactory.PrepareCustomerAttributeSearchModelAsync(model.CustomerAttributeSearchModel);
        await _addressAttributeModelFactory.PrepareAddressAttributeSearchModelAsync(model.AddressAttributeSearchModel);

		_logger.LogDebug("<<< Exiting PrepareCustomerUserSettingsModelAsync(model={Model})", model);
        return model;
    }

    /// <summary>
    /// Prepare GDPR settings model
    /// </summary>
    /// <param name="model">Gdpr settings model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the gDPR settings model
    /// </returns>
    public virtual async Task<GdprSettingsModel> PrepareGdprSettingsModelAsync(GdprSettingsModel model = null)
    {
		_logger.LogDebug(">>> Entering PrepareGdprSettingsModelAsync(model={Model})", model);
        //load settings for a chosen store scope
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
		_logger.LogDebug("PrepareGdprSettingsModelAsync(model={Model}): storeId → {StoreId}", model, storeId);
        var gdprSettings = await _settingService.LoadSettingAsync<GdprSettings>(storeId);
		_logger.LogDebug("PrepareGdprSettingsModelAsync(model={Model}): gdprSettings → {GdprSettings}", model, gdprSettings);

        //fill in model values from the entity
        model ??= gdprSettings.ToSettingsModel<GdprSettingsModel>();

        //fill in additional values (not existing in the entity)
        model.ActiveStoreScopeConfiguration = storeId;
		_logger.LogDebug("PrepareGdprSettingsModelAsync(model={Model}): model.ActiveStoreScopeConfiguration → {Arg1}", model, model.ActiveStoreScopeConfiguration);

        //prepare nested search model
        await PrepareGdprConsentSearchModelAsync(model.GdprConsentSearchModel);

        if (storeId <= 0)
            return model;

        //fill in overridden values
        model.GdprEnabled_OverrideForStore = await _settingService.SettingExistsAsync(gdprSettings, x => x.GdprEnabled, storeId);
        model.LogPrivacyPolicyConsent_OverrideForStore = await _settingService.SettingExistsAsync(gdprSettings, x => x.LogPrivacyPolicyConsent, storeId);
        model.LogNewsLetterConsent_OverrideForStore = await _settingService.SettingExistsAsync(gdprSettings, x => x.LogNewsletterConsent, storeId);
        model.LogUserProfileChanges_OverrideForStore = await _settingService.SettingExistsAsync(gdprSettings, x => x.LogUserProfileChanges, storeId);
        model.DeleteInactiveCustomersAfterMonths_OverrideForStore = await _settingService.SettingExistsAsync(gdprSettings, x => x.DeleteInactiveCustomersAfterMonths, storeId);

		_logger.LogDebug("<<< Exiting PrepareGdprSettingsModelAsync(model={Model})", model);
        return model;
    }

    /// <summary>
    /// Prepare paged GDPR consent list model
    /// </summary>
    /// <param name="searchModel">GDPR search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the gDPR consent list model
    /// </returns>
    public virtual async Task<GdprConsentListModel> PrepareGdprConsentListModelAsync(GdprConsentSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareGdprConsentListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //get sort options
        var consentList = (await _gdprService.GetAllConsentsAsync()).ToPagedList(searchModel);
		_logger.LogDebug("PrepareGdprConsentListModelAsync(searchModel={SearchModel}): consentList → {ConsentList}", searchModel, consentList);

        //prepare list model
        var model = await new GdprConsentListModel().PrepareToGridAsync(searchModel, consentList, () =>
        {
            return consentList.SelectAwait(async consent =>
            {
                var gdprConsentModel = consent.ToModel<GdprConsentModel>();
				_logger.LogDebug("PrepareGdprConsentListModelAsync(searchModel={SearchModel}): gdprConsentModel → {GdprConsentModel}", searchModel, gdprConsentModel);

                var gdprConsent = await _gdprService.GetConsentByIdAsync(gdprConsentModel.Id);
				_logger.LogDebug("PrepareGdprConsentListModelAsync(searchModel={SearchModel}): gdprConsent → {GdprConsent}", searchModel, gdprConsent);
                gdprConsentModel.Message = await _localizationService.GetLocalizedAsync(gdprConsent, entity => entity.Message);
                gdprConsentModel.RequiredMessage = await _localizationService.GetLocalizedAsync(gdprConsent, entity => entity.RequiredMessage);

                return gdprConsentModel;
            });
        });

		_logger.LogDebug("<<< Exiting PrepareGdprConsentListModelAsync(searchModel={SearchModel})", searchModel);
        return model;
    }

    /// <summary>
    /// Prepare GDPR consent model
    /// </summary>
    /// <param name="model">GDPR consent model</param>
    /// <param name="gdprConsent">GDPR consent</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the gDPR consent model
    /// </returns>
    public virtual async Task<GdprConsentModel> PrepareGdprConsentModelAsync(GdprConsentModel model, GdprConsent gdprConsent, bool excludeProperties = false)
    {
		_logger.LogDebug(">>> Entering PrepareGdprConsentModelAsync(model={Model},gdprConsent={GdprConsent},excludeProperties={ExcludeProperties})", model, gdprConsent, excludeProperties);
        Func<GdprConsentLocalizedModel, int, Task> localizedModelConfiguration = null;

        //fill in model values from the entity
        if (gdprConsent != null)
        {
            model ??= gdprConsent.ToModel<GdprConsentModel>();

            //define localized model configuration action
            localizedModelConfiguration = async (locale, languageId) =>
            {
                locale.Message = await _localizationService.GetLocalizedAsync(gdprConsent, entity => entity.Message, languageId, false, false);
                locale.RequiredMessage = await _localizationService.GetLocalizedAsync(gdprConsent, entity => entity.RequiredMessage, languageId, false, false);
            };
        }

        //set default values for the new model
        if (gdprConsent == null)
            model.DisplayOrder = 1;
			_logger.LogDebug("PrepareGdprConsentModelAsync(model={Model},gdprConsent={GdprConsent},excludeProperties={ExcludeProperties}): model.DisplayOrder → {Arg3}", model, gdprConsent, excludeProperties, model.DisplayOrder);

        //prepare localized models
        if (!excludeProperties)
            model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

		_logger.LogDebug("<<< Exiting PrepareGdprConsentModelAsync(model={Model},gdprConsent={GdprConsent},excludeProperties={ExcludeProperties})", model, gdprConsent, excludeProperties);
        return model;
    }

    /// <summary>
    /// Prepare general and common settings model
    /// </summary>
    /// <param name="model">General common settings model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the general and common settings model
    /// </returns>
    public virtual async Task<GeneralCommonSettingsModel> PrepareGeneralCommonSettingsModelAsync(GeneralCommonSettingsModel model = null)
    {
		_logger.LogDebug(">>> Entering PrepareGeneralCommonSettingsModelAsync(model={Model})", model);
        model ??= new GeneralCommonSettingsModel
        {
            ActiveStoreScopeConfiguration = await _storeContext.GetActiveStoreScopeConfigurationAsync()
        };

        //prepare store information settings model
        model.StoreInformationSettings = await PrepareStoreInformationSettingsModelAsync();

        //prepare Sitemap settings model
        model.SitemapSettings = await PrepareSitemapSettingsModelAsync();

        //prepare Minification settings model
        model.MinificationSettings = await PrepareMinificationSettingsModelAsync();

        //prepare SEO settings model
        model.SeoSettings = await PrepareSeoSettingsModelAsync();

        //prepare security settings model
        model.SecuritySettings = await PrepareSecuritySettingsModelAsync();

        //prepare robots.txt settings model
        model.RobotsTxtSettings = await PrepareRobotsTxtSettingsModelAsync();

        //prepare captcha settings model
        model.CaptchaSettings = await PrepareCaptchaSettingsModelAsync();

        //prepare PDF settings model
        model.PdfSettings = await PreparePdfSettingsModelAsync();

        //prepare localization settings model
        model.LocalizationSettings = await PrepareLocalizationSettingsModelAsync();

        //prepare translation settings model
        model.TranslationSettings = await PrepareTranslationSettingsModelAsync();

        //prepare admin area settings model
        model.AdminAreaSettings = await PrepareAdminAreaSettingsModelAsync();

        //prepare custom HTML settings model
        model.CustomHtmlSettings = await PrepareCustomHtmlSettingsModelAsync();

		_logger.LogDebug("<<< Exiting PrepareGeneralCommonSettingsModelAsync(model={Model})", model);
        return model;
    }

    /// <summary>
    /// Prepare product editor settings model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product editor settings model
    /// </returns>
    public virtual async Task<ProductEditorSettingsModel> PrepareProductEditorSettingsModelAsync()
    {
		_logger.LogDebug(">>> Entering PrepareProductEditorSettingsModelAsync()");
        //load settings for a chosen store scope
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
		_logger.LogDebug("PrepareProductEditorSettingsModelAsync(): storeId → {StoreId}", storeId);
        var productEditorSettings = await _settingService.LoadSettingAsync<ProductEditorSettings>(storeId);
		_logger.LogDebug("PrepareProductEditorSettingsModelAsync(): productEditorSettings → {ProductEditorSettings}", productEditorSettings);

        //fill in model values from the entity
        var model = productEditorSettings.ToSettingsModel<ProductEditorSettingsModel>();
		_logger.LogDebug("PrepareProductEditorSettingsModelAsync(): model → {Model}", model);

		_logger.LogDebug("<<< Exiting PrepareProductEditorSettingsModelAsync()");
        return model;
    }

    /// <summary>
    /// Prepare setting search model
    /// </summary>
    /// <param name="searchModel">Setting search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the setting search model
    /// </returns>
    public virtual async Task<SettingSearchModel> PrepareSettingSearchModelAsync(SettingSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareSettingSearchModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare model to add
        await PrepareAddSettingModelAsync(searchModel.AddSetting);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogDebug("<<< Exiting PrepareSettingSearchModelAsync(searchModel={SearchModel})", searchModel);
        return searchModel;
    }

    /// <summary>
    /// Prepare paged setting list model
    /// </summary>
    /// <param name="searchModel">Setting search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the setting list model
    /// </returns>
    public virtual async Task<SettingListModel> PrepareSettingListModelAsync(SettingSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareSettingListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //get settings
        var settings = (await _settingService.GetAllSettingsAsync()).AsQueryable();
		_logger.LogDebug("PrepareSettingListModelAsync(searchModel={SearchModel}): settings → {Settings}", searchModel, settings);

        //filter settings
        if (!string.IsNullOrEmpty(searchModel.SearchSettingName))
            settings = settings.Where(setting => setting.Name.ToLowerInvariant().Contains(searchModel.SearchSettingName.ToLowerInvariant()));
        if (!string.IsNullOrEmpty(searchModel.SearchSettingValue))
            settings = settings.Where(setting => setting.Value.ToLowerInvariant().Contains(searchModel.SearchSettingValue.ToLowerInvariant()));

        var pagedSettings = settings.ToList().ToPagedList(searchModel);
		_logger.LogDebug("PrepareSettingListModelAsync(searchModel={SearchModel}): pagedSettings → {PagedSettings}", searchModel, pagedSettings);

        //prepare list model
        var model = await new SettingListModel().PrepareToGridAsync(searchModel, pagedSettings, () =>
        {
            return pagedSettings.SelectAwait(async setting =>
            {
                //fill in model values from the entity
                var settingModel = setting.ToModel<SettingModel>();
				_logger.LogDebug("PrepareSettingListModelAsync(searchModel={SearchModel}): settingModel → {SettingModel}", searchModel, settingModel);

                //fill in additional values (not existing in the entity)
                settingModel.Store = setting.StoreId > 0
                    ? (await _storeService.GetStoreByIdAsync(setting.StoreId))?.Name ?? "Deleted"
                    : await _localizationService.GetResourceAsync("Admin.Configuration.Settings.AllSettings.Fields.StoreName.AllStores");
					_logger.LogDebug("PrepareSettingListModelAsync(searchModel={SearchModel}): settingModel.Store → {Arg1}", searchModel, settingModel.Store);

                return settingModel;
            });
        });

		_logger.LogDebug("<<< Exiting PrepareSettingListModelAsync(searchModel={SearchModel})", searchModel);
        return model;
    }

    /// <summary>
    /// Prepare setting mode model
    /// </summary>
    /// <param name="modeName">Mode name</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the setting mode model
    /// </returns>
    public virtual async Task<SettingModeModel> PrepareSettingModeModelAsync(string modeName)
    {
		_logger.LogDebug(">>> Entering PrepareSettingModeModelAsync(modeName={ModeName})", modeName);
        var model = new SettingModeModel
        {
            ModeName = modeName,
            Enabled = await _genericAttributeService.GetAttributeAsync<bool>(await _workContext.GetCurrentCustomerAsync(), modeName)
        };

		_logger.LogDebug("<<< Exiting PrepareSettingModeModelAsync(modeName={ModeName})", modeName);
        return model;
    }

    /// <summary>
    /// Prepare store scope configuration model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the store scope configuration model
    /// </returns>
    public virtual async Task<StoreScopeConfigurationModel> PrepareStoreScopeConfigurationModelAsync()
    {
		_logger.LogDebug(">>> Entering PrepareStoreScopeConfigurationModelAsync()");
        var model = new StoreScopeConfigurationModel
        {
            Stores = (await _storeService.GetAllStoresAsync()).Select(store => store.ToModel<StoreModel>()).ToList(),
            StoreId = await _storeContext.GetActiveStoreScopeConfigurationAsync()
        };

		_logger.LogDebug("<<< Exiting PrepareStoreScopeConfigurationModelAsync()");
        return model;
    }

    #endregion
}