using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Configuration;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Seo;
using Nop.Core.Events;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Authentication.External;
using Nop.Services.Authentication.MultiFactor;
using Nop.Services.Blogs;
using Nop.Services.Catalog;
using Nop.Services.Cms;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Events;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.News;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Plugins;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Pickup;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Services.Topics;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Blogs;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Areas.Admin.Models.Localization;
using Nop.Web.Areas.Admin.Models.News;
using Nop.Web.Areas.Admin.Models.Topics;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Models.Extensions;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Framework.Security;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents common models factory implementation
/// </summary>
public partial class CommonModelFactory : ICommonModelFactory
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly AppSettings _appSettings;
    protected readonly CatalogSettings _catalogSettings;
    protected readonly CurrencySettings _currencySettings;
    protected readonly IActionContextAccessor _actionContextAccessor;
    protected readonly IAuthenticationPluginManager _authenticationPluginManager;
    protected readonly IBaseAdminModelFactory _baseAdminModelFactory;
    protected readonly IBlogService _blogService;
    protected readonly ICategoryService _categoryService;
    protected readonly ICurrencyService _currencyService;
    protected readonly ICustomerService _customerService;
    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly IEventPublisher _eventPublisher;
    protected readonly IExchangeRatePluginManager _exchangeRatePluginManager;
    protected readonly IHttpContextAccessor _httpContextAccessor;
    protected readonly ILanguageService _languageService;
    protected readonly ILocalizationService _localizationService;
    protected readonly IMaintenanceService _maintenanceService;
    protected readonly IManufacturerService _manufacturerService;
    protected readonly IMeasureService _measureService;
    protected readonly IMultiFactorAuthenticationPluginManager _multiFactorAuthenticationPluginManager;
    protected readonly INewsService _newsService;
    protected readonly INopDataProvider _dataProvider;
    protected readonly INopFileProvider _fileProvider;
    protected readonly INopUrlHelper _nopUrlHelper;
    protected readonly IOrderService _orderService;
    protected readonly IPaymentPluginManager _paymentPluginManager;
    protected readonly IPickupPluginManager _pickupPluginManager;
    protected readonly IPluginService _pluginService;
    protected readonly IProductService _productService;
    protected readonly IReturnRequestService _returnRequestService;
    protected readonly ISearchTermService _searchTermService;
    protected readonly IServiceCollection _serviceCollection;
    protected readonly IShippingPluginManager _shippingPluginManager;
    protected readonly IStaticCacheManager _staticCacheManager;
    protected readonly IStoreContext _storeContext;
    protected readonly IStoreService _storeService;
    protected readonly ITaxPluginManager _taxPluginManager;
    protected readonly IThumbService _thumbService;
    protected readonly ITopicService _topicService;
    protected readonly IUrlHelperFactory _urlHelperFactory;
    protected readonly IUrlRecordService _urlRecordService;
    protected readonly IWebHelper _webHelper;
    protected readonly IWidgetPluginManager _widgetPluginManager;
    protected readonly IWorkContext _workContext;
    protected readonly MeasureSettings _measureSettings;
    protected readonly NopHttpClient _nopHttpClient;
    protected readonly ProxySettings _proxySettings;

    #endregion

    #region Ctor

    public CommonModelFactory(AppSettings appSettings,
        CatalogSettings catalogSettings,
        CurrencySettings currencySettings,
        IActionContextAccessor actionContextAccessor,
        IAuthenticationPluginManager authenticationPluginManager,
        IBaseAdminModelFactory baseAdminModelFactory,
        IBlogService blogService,
        ICategoryService categoryService,
        ICurrencyService currencyService,
        ICustomerService customerService,
        IDateTimeHelper dateTimeHelper,
        IEventPublisher eventPublisher,
        IExchangeRatePluginManager exchangeRatePluginManager,
        IHttpContextAccessor httpContextAccessor,
        ILanguageService languageService,
        ILocalizationService localizationService,
        IMaintenanceService maintenanceService,
        IManufacturerService manufacturerService,
        IMeasureService measureService,
        IMultiFactorAuthenticationPluginManager multiFactorAuthenticationPluginManager,
        INewsService newsService,
        INopDataProvider dataProvider,
        INopFileProvider fileProvider,
        INopUrlHelper nopUrlHelper,
        IOrderService orderService,
        IPaymentPluginManager paymentPluginManager,
        IPickupPluginManager pickupPluginManager,
        IPluginService pluginService,
        IProductService productService,
        IReturnRequestService returnRequestService,
        ISearchTermService searchTermService,
        IServiceCollection serviceCollection,
        IShippingPluginManager shippingPluginManager,
        IStaticCacheManager staticCacheManager,
        IStoreContext storeContext,
        IStoreService storeService,
        ITaxPluginManager taxPluginManager,
        IThumbService thumbService,
        ITopicService topicService,
        IUrlHelperFactory urlHelperFactory,
        IUrlRecordService urlRecordService,
        IWebHelper webHelper,
        IWidgetPluginManager widgetPluginManager,
        IWorkContext workContext,
        MeasureSettings measureSettings,
        NopHttpClient nopHttpClient,
        ProxySettings proxySettings)
    {
        _appSettings = appSettings;
        _catalogSettings = catalogSettings;
        _currencySettings = currencySettings;
        _actionContextAccessor = actionContextAccessor;
        _authenticationPluginManager = authenticationPluginManager;
        _baseAdminModelFactory = baseAdminModelFactory;
        _blogService = blogService;
        _categoryService = categoryService;
        _currencyService = currencyService;
        _customerService = customerService;
        _eventPublisher = eventPublisher;
        _dataProvider = dataProvider;
        _dateTimeHelper = dateTimeHelper;
        _exchangeRatePluginManager = exchangeRatePluginManager;
        _httpContextAccessor = httpContextAccessor;
        _languageService = languageService;
        _localizationService = localizationService;
        _maintenanceService = maintenanceService;
        _manufacturerService = manufacturerService;
        _measureService = measureService;
        _multiFactorAuthenticationPluginManager = multiFactorAuthenticationPluginManager;
        _newsService = newsService;
        _fileProvider = fileProvider;
        _nopUrlHelper = nopUrlHelper;
        _orderService = orderService;
        _paymentPluginManager = paymentPluginManager;
        _pickupPluginManager = pickupPluginManager;
        _pluginService = pluginService;
        _productService = productService;
        _returnRequestService = returnRequestService;
        _searchTermService = searchTermService;
        _serviceCollection = serviceCollection;
        _shippingPluginManager = shippingPluginManager;
        _staticCacheManager = staticCacheManager;
        _storeContext = storeContext;
        _storeService = storeService;
        _taxPluginManager = taxPluginManager;
        _thumbService = thumbService;
        _topicService = topicService;
        _urlHelperFactory = urlHelperFactory;
        _urlRecordService = urlRecordService;
        _webHelper = webHelper;
        _widgetPluginManager = widgetPluginManager;
        _workContext = workContext;
        _measureSettings = measureSettings;
        _nopHttpClient = nopHttpClient;
        _proxySettings = proxySettings;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Prepare store URL warning model
    /// </summary>
    /// <param name="models">List of system warning models</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task PrepareStoreUrlWarningModelAsync(IList<SystemWarningModel> models)
    {
		_logger.LogError(">>> Entering PrepareStoreUrlWarningModelAsync(models={Models})", models);
        ArgumentNullException.ThrowIfNull(models);

        //check whether current store URL matches the store configured URL
        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogError("PrepareStoreUrlWarningModelAsync(models={Models}): store → {Store}", models, store);
        var currentStoreUrl = store.Url;
        if (!string.IsNullOrEmpty(currentStoreUrl) &&
            (currentStoreUrl.Equals(_webHelper.GetStoreLocation(false), StringComparison.InvariantCultureIgnoreCase) ||
             currentStoreUrl.Equals(_webHelper.GetStoreLocation(true), StringComparison.InvariantCultureIgnoreCase)))
        {
            models.Add(new SystemWarningModel
            {
                Level = SystemWarningLevel.Pass,
                Text = await _localizationService.GetResourceAsync("Admin.System.Warnings.URL.Match")
            });
				_logger.LogDebug("PrepareStoreUrlWarningModelAsync(models={Models}): Text → {Text}", models, Text);
            return;
        }

        models.Add(new SystemWarningModel
        {
            Level = SystemWarningLevel.Fail,
            Text = string.Format(await _localizationService.GetResourceAsync("Admin.System.Warnings.URL.NoMatch"),
                currentStoreUrl, _webHelper.GetStoreLocation(false))
        });
		_logger.LogWarning("<<< Exiting PrepareStoreUrlWarningModelAsync(models={Models})", models);
    }

    /// <summary>
    /// Prepare recommendations/warnings model
    /// </summary>
    /// <param name="models">List of system warning models</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task PrepareRecommendationsModelAsync(IList<SystemWarningModel> models)
    {
		_logger.LogError(">>> Entering PrepareRecommendationsModelAsync(models={Models})", models);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(models);

        var recommendations = new List<string>();
		_logger.LogError("PrepareRecommendationsModelAsync(models={Models}): recommendations → {Recommendations}", models, recommendations);
        try
        {
            var text = await _nopHttpClient.GetRecommendationsAsync();
			_logger.LogError("PrepareRecommendationsModelAsync(models={Models}): text → {Text}", models, text);
            recommendations = JsonConvert.DeserializeObject<List<string>>(text);
        }
        catch { }
		_logger.LogError("Exception in PrepareRecommendationsModelAsync(models={Models})", models);

        foreach (var recommendation in recommendations ?? new())
        {
            models.Add(new SystemWarningModel
            {
				_logger.LogWarning("<<< Exiting PrepareRecommendationsModelAsync(models={Models})", models);
                Level = SystemWarningLevel.Recommendation,
                Text = recommendation,
                DontEncode = true
            });
			_logger.LogDebug("PrepareRecommendationsModelAsync(models={Models}): DontEncode → {DontEncode}", models, DontEncode);
			_logger.LogError("PrepareRecommendationsModelAsync(models)={Models}: external service call took {Arg1} ms", models, __logStart.ElapsedMilliseconds);
        }
    }

    /// <summary>
    /// Prepare primary exchange rate currency warning model
    /// </summary>
    /// <param name="models">List of system warning models</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task PrepareExchangeRateCurrencyWarningModelAsync(IList<SystemWarningModel> models)
    {
		_logger.LogError(">>> Entering PrepareExchangeRateCurrencyWarningModelAsync(models={Models})", models);
        ArgumentNullException.ThrowIfNull(models);

        //check whether primary exchange rate currency set
        var primaryExchangeRateCurrency = await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryExchangeRateCurrencyId);
		_logger.LogError("PrepareExchangeRateCurrencyWarningModelAsync(models={Models}): primaryExchangeRateCurrency → {PrimaryExchangeRateCurrency}", models, primaryExchangeRateCurrency);
        if (primaryExchangeRateCurrency == null)
        {
            models.Add(new SystemWarningModel
            {
                Level = SystemWarningLevel.Fail,
                Text = await _localizationService.GetResourceAsync("Admin.System.Warnings.ExchangeCurrency.NotSet")
            });
				_logger.LogDebug("PrepareExchangeRateCurrencyWarningModelAsync(models={Models}): Text → {Text}", models, Text);
            return;
        }

        models.Add(new SystemWarningModel
        {
            Level = SystemWarningLevel.Pass,
            Text = await _localizationService.GetResourceAsync("Admin.System.Warnings.ExchangeCurrency.Set")
        });

        //check whether primary exchange rate currency rate configured
        if (primaryExchangeRateCurrency.Rate != 1)
        {
            models.Add(new SystemWarningModel
            {
                Level = SystemWarningLevel.Fail,
                Text = await _localizationService.GetResourceAsync("Admin.System.Warnings.ExchangeCurrency.Rate1")
            });
        }
		_logger.LogWarning("<<< Exiting PrepareExchangeRateCurrencyWarningModelAsync(models={Models})", models);
    }

    /// <summary>
    /// Prepare primary store currency warning model
    /// </summary>
    /// <param name="models">List of system warning models</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task PreparePrimaryStoreCurrencyWarningModelAsync(IList<SystemWarningModel> models)
    {
		_logger.LogError(">>> Entering PreparePrimaryStoreCurrencyWarningModelAsync(models={Models})", models);
        ArgumentNullException.ThrowIfNull(models);

        //check whether primary store currency set
        var primaryStoreCurrency = await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId);
		_logger.LogError("PreparePrimaryStoreCurrencyWarningModelAsync(models={Models}): primaryStoreCurrency → {PrimaryStoreCurrency}", models, primaryStoreCurrency);
        if (primaryStoreCurrency == null)
        {
            models.Add(new SystemWarningModel
            {
                Level = SystemWarningLevel.Fail,
                Text = await _localizationService.GetResourceAsync("Admin.System.Warnings.PrimaryCurrency.NotSet")
            });
				_logger.LogDebug("PreparePrimaryStoreCurrencyWarningModelAsync(models={Models}): Text → {Text}", models, Text);
            return;
        }

        models.Add(new SystemWarningModel
        {
            Level = SystemWarningLevel.Pass,
            Text = await _localizationService.GetResourceAsync("Admin.System.Warnings.PrimaryCurrency.Set")
        });
		_logger.LogWarning("<<< Exiting PreparePrimaryStoreCurrencyWarningModelAsync(models={Models})", models);
    }

    /// <summary>
    /// Prepare base weight warning model
    /// </summary>
    /// <param name="models">List of system warning models</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task PrepareBaseWeightWarningModelAsync(IList<SystemWarningModel> models)
    {
		_logger.LogError(">>> Entering PrepareBaseWeightWarningModelAsync(models={Models})", models);
        ArgumentNullException.ThrowIfNull(models);

        //check whether base measure weight set
        var baseWeight = await _measureService.GetMeasureWeightByIdAsync(_measureSettings.BaseWeightId);
		_logger.LogError("PrepareBaseWeightWarningModelAsync(models={Models}): baseWeight → {BaseWeight}", models, baseWeight);
        if (baseWeight == null)
        {
            models.Add(new SystemWarningModel
            {
                Level = SystemWarningLevel.Fail,
                Text = await _localizationService.GetResourceAsync("Admin.System.Warnings.DefaultWeight.NotSet")
            });
				_logger.LogDebug("PrepareBaseWeightWarningModelAsync(models={Models}): Text → {Text}", models, Text);
            return;
        }

        models.Add(new SystemWarningModel
        {
            Level = SystemWarningLevel.Pass,
            Text = await _localizationService.GetResourceAsync("Admin.System.Warnings.DefaultWeight.Set")
        });

        //check whether base measure weight ratio configured
        if (baseWeight.Ratio != 1)
        {
            models.Add(new SystemWarningModel
            {
                Level = SystemWarningLevel.Fail,
                Text = await _localizationService.GetResourceAsync("Admin.System.Warnings.DefaultWeight.Ratio1")
            });
        }
		_logger.LogWarning("<<< Exiting PrepareBaseWeightWarningModelAsync(models={Models})", models);
    }

    /// <summary>
    /// Prepare base dimension warning model
    /// </summary>
    /// <param name="models">List of system warning models</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task PrepareBaseDimensionWarningModelAsync(IList<SystemWarningModel> models)
    {
		_logger.LogError(">>> Entering PrepareBaseDimensionWarningModelAsync(models={Models})", models);
        ArgumentNullException.ThrowIfNull(models);

        //check whether base measure dimension set
        var baseDimension = await _measureService.GetMeasureDimensionByIdAsync(_measureSettings.BaseDimensionId);
		_logger.LogError("PrepareBaseDimensionWarningModelAsync(models={Models}): baseDimension → {BaseDimension}", models, baseDimension);
        if (baseDimension == null)
        {
            models.Add(new SystemWarningModel
            {
                Level = SystemWarningLevel.Fail,
                Text = await _localizationService.GetResourceAsync("Admin.System.Warnings.DefaultDimension.NotSet")
            });
				_logger.LogDebug("PrepareBaseDimensionWarningModelAsync(models={Models}): Text → {Text}", models, Text);
            return;
        }

        models.Add(new SystemWarningModel
        {
            Level = SystemWarningLevel.Pass,
            Text = await _localizationService.GetResourceAsync("Admin.System.Warnings.DefaultDimension.Set")
        });

        //check whether base measure dimension ratio configured
        if (baseDimension.Ratio != 1)
        {
            models.Add(new SystemWarningModel
            {
                Level = SystemWarningLevel.Fail,
                Text = await _localizationService.GetResourceAsync("Admin.System.Warnings.DefaultDimension.Ratio1")
            });
        }
		_logger.LogWarning("<<< Exiting PrepareBaseDimensionWarningModelAsync(models={Models})", models);
    }

    /// <summary>
    /// Prepare payment methods warning model
    /// </summary>
    /// <param name="models">List of system warning models</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task PreparePaymentMethodsWarningModelAsync(IList<SystemWarningModel> models)
    {
		_logger.LogError(">>> Entering PreparePaymentMethodsWarningModelAsync(models={Models})", models);
        ArgumentNullException.ThrowIfNull(models);

        //check whether payment methods activated
        if ((await _paymentPluginManager.LoadAllPluginsAsync()).Any())
        {
            models.Add(new SystemWarningModel
            {
                Level = SystemWarningLevel.Pass,
                Text = await _localizationService.GetResourceAsync("Admin.System.Warnings.PaymentMethods.OK")
            });
				_logger.LogDebug("PreparePaymentMethodsWarningModelAsync(models={Models}): Text → {Text}", models, Text);
            return;
        }

        models.Add(new SystemWarningModel
        {
            Level = SystemWarningLevel.Fail,
            Text = await _localizationService.GetResourceAsync("Admin.System.Warnings.PaymentMethods.NoActive")
        });
		_logger.LogWarning("<<< Exiting PreparePaymentMethodsWarningModelAsync(models={Models})", models);
    }

    /// <summary>
    /// Prepare performance settings warning model
    /// </summary>
    /// <param name="models">List of system warning models</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task PreparePerformanceSettingsWarningModelAsync(IList<SystemWarningModel> models)
    {
		_logger.LogError(">>> Entering PreparePerformanceSettingsWarningModelAsync(models={Models})", models);
        ArgumentNullException.ThrowIfNull(models);

        //check whether "IgnoreStoreLimitations" setting disabled
        if (!_catalogSettings.IgnoreStoreLimitations && (await _storeService.GetAllStoresAsync()).Count == 1)
        {
            models.Add(new SystemWarningModel
            {
                Level = SystemWarningLevel.Recommendation,
                Text = await _localizationService.GetResourceAsync("Admin.System.Warnings.Performance.IgnoreStoreLimitations")
            });
				_logger.LogDebug("PreparePerformanceSettingsWarningModelAsync(models={Models}): Text → {Text}", models, Text);
        }

        //check whether "IgnoreAcl" setting disabled
        if (!_catalogSettings.IgnoreAcl)
        {
            models.Add(new SystemWarningModel
            {
                Level = SystemWarningLevel.Recommendation,
                Text = await _localizationService.GetResourceAsync("Admin.System.Warnings.Performance.IgnoreAcl")
            });
        }
		_logger.LogWarning("<<< Exiting PreparePerformanceSettingsWarningModelAsync(models={Models})", models);
    }

    /// <summary>
    /// Prepare file permissions warning model
    /// </summary>
    /// <param name="models">List of system warning models</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task PrepareFilePermissionsWarningModelAsync(IList<SystemWarningModel> models)
    {
		_logger.LogError(">>> Entering PrepareFilePermissionsWarningModelAsync(models={Models})", models);
        ArgumentNullException.ThrowIfNull(models);

        var dirPermissionsOk = true;
		_logger.LogError("PrepareFilePermissionsWarningModelAsync(models={Models}): dirPermissionsOk → {DirPermissionsOk}", models, dirPermissionsOk);
        var dirsToCheck = _fileProvider.GetDirectoriesWrite();
		_logger.LogError("PrepareFilePermissionsWarningModelAsync(models={Models}): dirsToCheck → {DirsToCheck}", models, dirsToCheck);
        foreach (var dir in dirsToCheck)
        {
            if (_fileProvider.CheckPermissions(dir, false, true, true, false))
                continue;

            models.Add(new SystemWarningModel
            {
                Level = SystemWarningLevel.Warning,
                Text = string.Format(await _localizationService.GetResourceAsync("Admin.System.Warnings.DirectoryPermission.Wrong"),
                    CurrentOSUser.FullName, dir)
            });
            dirPermissionsOk = false;
        }

        if (dirPermissionsOk)
        {
            models.Add(new SystemWarningModel
            {
                Level = SystemWarningLevel.Pass,
                Text = await _localizationService.GetResourceAsync("Admin.System.Warnings.DirectoryPermission.OK")
            });
				_logger.LogDebug("PrepareFilePermissionsWarningModelAsync(models={Models}): Text → {Text}", models, Text);
        }

        var filePermissionsOk = true;
		_logger.LogError("PrepareFilePermissionsWarningModelAsync(models={Models}): filePermissionsOk → {FilePermissionsOk}", models, filePermissionsOk);
        var filesToCheck = _fileProvider.GetFilesWrite();
		_logger.LogError("PrepareFilePermissionsWarningModelAsync(models={Models}): filesToCheck → {FilesToCheck}", models, filesToCheck);
        foreach (var file in filesToCheck)
        {
            if (_fileProvider.CheckPermissions(file, false, true, true, true))
                continue;

            models.Add(new SystemWarningModel
            {
                Level = SystemWarningLevel.Warning,
                Text = string.Format(await _localizationService.GetResourceAsync("Admin.System.Warnings.FilePermission.Wrong"),
                    CurrentOSUser.FullName, file)
            });
            filePermissionsOk = false;
        }

        if (filePermissionsOk)
        {
            models.Add(new SystemWarningModel
            {
                Level = SystemWarningLevel.Pass,
                Text = await _localizationService.GetResourceAsync("Admin.System.Warnings.FilePermission.OK")
            });
        }
		_logger.LogWarning("<<< Exiting PrepareFilePermissionsWarningModelAsync(models={Models})", models);
    }

    /// <summary>
    /// Prepare backup file search model
    /// </summary>
    /// <param name="searchModel">Backup file search model</param>
    /// <returns>Backup file search model</returns>
    protected virtual BackupFileSearchModel PrepareBackupFileSearchModel(BackupFileSearchModel searchModel)
    {
		_logger.LogError(">>> Entering PrepareBackupFileSearchModel(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogWarning("<<< Exiting PrepareBackupFileSearchModel(searchModel={SearchModel})", searchModel);
        return searchModel;
    }

    /// <summary>
    /// Prepare delete thumb files model
    /// </summary>
    /// <param name="model">Delete thumb files model</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task PrepareDeleteThumbFilesModelAsync(MaintenanceModel.DeleteThumbFilesModel model)
    {
		_logger.LogError(">>> Entering PrepareDeleteThumbFilesModelAsync(model={Model})", model);
        if (_thumbService is not ThumbService thumbService)
        {
            model.IsDeleteThumbsSupported = false;
			_logger.LogDebug("PrepareDeleteThumbFilesModelAsync(model={Model}): model.IsDeleteThumbsSupported → {Arg1}", model, model.IsDeleteThumbsSupported);
            return;
        }

        var (filesCount, filesSize) = await thumbService.GetThumbsInfoAsync();

        model.IsDeleteThumbsSupported = true;
        model.FilesCountText = string.Format(await _localizationService.GetResourceAsync("Admin.System.Maintenance.DeleteThumbFiles.FilesCount"), filesCount);
        model.FilesSizeText = string.Format(await _localizationService.GetResourceAsync("Admin.System.Maintenance.DeleteThumbFiles.FilesSize"), Math.Round(filesSize / 1024M / 1024M, 2));
		_logger.LogWarning("<<< Exiting PrepareDeleteThumbFilesModelAsync(model={Model})", model);
    }

    /// <summary>
    /// Prepare plugins which try to override the same interface warning model
    /// </summary>
    /// <param name="models">List of system warning models</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task PreparePluginsOverrideSameInterfaceWarningModelAsync(IList<SystemWarningModel> models)
    {
		_logger.LogError(">>> Entering PreparePluginsOverrideSameInterfaceWarningModelAsync(models={Models})", models);
        //check whether there are different plugins which try to override the same interface
        var baseLibraries = new[] { "Nop.Core", "Nop.Data", "Nop.Services", "Nop.Web", "Nop.Web.Framework" };
		_logger.LogError("PreparePluginsOverrideSameInterfaceWarningModelAsync(models={Models}): baseLibraries → {BaseLibraries}", models, baseLibraries);
        var overridenServices = _serviceCollection.Where(p =>
                p.ServiceType.FullName != null &&
                p.ServiceType.FullName.StartsWith("Nop.", StringComparison.InvariantCulture) &&
                !p.ServiceType.FullName.StartsWith(
                    typeof(IConsumer<>).FullName?.Replace("~1", string.Empty) ?? string.Empty,
                    StringComparison.InvariantCulture)).Select(p =>
                KeyValuePair.Create(p.ServiceType.FullName, p.ImplementationType?.Assembly.GetName().Name))
            .Where(p => baseLibraries.All(library =>
                !p.Value?.StartsWith(library, StringComparison.InvariantCultureIgnoreCase) ?? false))
            .GroupBy(p => p.Key, p => p.Value)
            .Where(p => p.Count() > 1)
            .ToDictionary(p => p.Key, p => p.ToList());

        foreach (var overridenService in overridenServices)
        {
            var assemblies = overridenService.Value
                .Aggregate("", (current, all) => all + ", " + current).TrimEnd(',', ' ');

            models.Add(new SystemWarningModel
            {
                Level = SystemWarningLevel.Warning,
                Text = string.Format(await _localizationService.GetResourceAsync("Admin.System.Warnings.PluginsOverrideSameService"), overridenService.Key, assemblies)
            });
				_logger.LogDebug("PreparePluginsOverrideSameInterfaceWarningModelAsync(models={Models}): Text → {Text}", models, Text);
        }
		_logger.LogWarning("<<< Exiting PreparePluginsOverrideSameInterfaceWarningModelAsync(models={Models})", models);
    }

    /// <summary>
    /// Prepare plugins collision of loaded assembly warning model
    /// </summary>
    /// <param name="models">List of system warning models</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task PreparePluginsCollisionsWarningModelAsync(IList<SystemWarningModel> models)
    {
		_logger.LogError(">>> Entering PreparePluginsCollisionsWarningModelAsync(models={Models})", models);
        var assemblyCollisions = _pluginService.GetAssemblyCollisions();

		_logger.LogError("PreparePluginsCollisionsWarningModelAsync(models={Models}): assemblyCollisions → {AssemblyCollisions}", models, assemblyCollisions);
        if (assemblyCollisions.Any())
        {
            var warningFormat = await _localizationService
                .GetResourceAsync("Admin.System.Warnings.PluginRequiredAssembly");

            //check whether there are any collision of loaded assembly
            foreach (var assembly in _pluginService.GetAssemblyCollisions())
            {
                //get plugin references message
                var message = assembly.Collisions
                    .Select(item => string.Format(warningFormat, item.PluginName, item.AssemblyVersion))
                    .Aggregate("", (current, all) => all + ", " + current).TrimEnd(',', ' ');

                models.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Warning,
                    Text = string.Format(
                        await _localizationService.GetResourceAsync("Admin.System.Warnings.AssemblyHasCollision"),
                        assembly.ShortName, assembly.AssemblyInMemory, message)
                });
						_logger.LogDebug("PreparePluginsCollisionsWarningModelAsync(models={Models}): Text → {Text}", models, Text);
            }
        }
		_logger.LogWarning("<<< Exiting PreparePluginsCollisionsWarningModelAsync(models={Models})", models);
    }

    /// <summary>
    /// Prepare incompatible plugins warning model 
    /// </summary>
    /// <param name="models">List of system warning models</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task PrepareIncompatibleWarningModelAsync(IList<SystemWarningModel> models)
    {
		_logger.LogError(">>> Entering PrepareIncompatibleWarningModelAsync(models={Models})", models);
        foreach (var incompatiblePlugin in _pluginService.GetIncompatiblePlugins())
        {
            string warning;

            switch (incompatiblePlugin.Value)
            {
                case PluginIncompatibleType.MainAssemblyNotFound:
                    warning = string.Format(
                        await _localizationService.GetResourceAsync("Admin.System.Warnings.PluginMainAssemblyNotFound"),
                        incompatiblePlugin.Key);
						_logger.LogDebug("PrepareIncompatibleWarningModelAsync(models={Models}): warning → {Warning}", models, warning);
                    break;
                case PluginIncompatibleType.NotCompatibleWithCurrentVersion:
                    warning = string.Format(
                        await _localizationService.GetResourceAsync("Admin.System.Warnings.PluginNotCompatibleWithCurrentVersion"),
                        incompatiblePlugin.Key);
                    break;
                default:
                    continue;
            }

            models.Add(new SystemWarningModel
            {
                Level = SystemWarningLevel.Warning,
                Text = warning
            });
			_logger.LogDebug("PrepareIncompatibleWarningModelAsync(models={Models}): Text → {Text}", models, Text);
        }
		_logger.LogWarning("<<< Exiting PrepareIncompatibleWarningModelAsync(models={Models})", models);
    }

    /// <summary>
    /// Prepare plugins installed warning model
    /// </summary>
    /// <param name="models">List of system warning models</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task PreparePluginsInstalledWarningModelAsync(IList<SystemWarningModel> models)
    {
		_logger.LogError(">>> Entering PreparePluginsInstalledWarningModelAsync(models={Models})", models);
        var plugins = await _pluginService.GetPluginDescriptorsAsync<IPlugin>(LoadPluginsMode.NotInstalledOnly);

		_logger.LogError("PreparePluginsInstalledWarningModelAsync(models={Models}): plugins → {Plugins}", models, plugins);
        var notInstalled = plugins.Select(p => p.FriendlyName).ToList();
		_logger.LogError("PreparePluginsInstalledWarningModelAsync(models={Models}): notInstalled → {NotInstalled}", models, notInstalled);

        if (!notInstalled.Any())
            return;

        models.Add(new SystemWarningModel
        {
            Level = SystemWarningLevel.Warning,
            DontEncode = true,
            Text = $"{await _localizationService.GetResourceAsync("Admin.System.Warnings.PluginNotInstalled")}: {string.Join(", ", notInstalled)}. {await _localizationService.GetResourceAsync("Admin.System.Warnings.PluginNotInstalled.HelpText")}"
        });
		_logger.LogDebug("PreparePluginsInstalledWarningModelAsync(models={Models}): Text → {Text}", models, Text);
		_logger.LogWarning("<<< Exiting PreparePluginsInstalledWarningModelAsync(models={Models})", models);
    }

    /// <summary>
    /// Prepare plugins enabled warning model
    /// </summary>
    /// <param name="models">List of system warning models</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task PreparePluginsEnabledWarningModelAsync(IList<SystemWarningModel> models)
    {
		_logger.LogError(">>> Entering PreparePluginsEnabledWarningModelAsync(models={Models})", models);
        var plugins = await _pluginService.GetPluginsAsync<IPlugin>();

		_logger.LogError("PreparePluginsEnabledWarningModelAsync(models={Models}): plugins → {Plugins}", models, plugins);
        var notEnabled = new List<string>();
		_logger.LogError("PreparePluginsEnabledWarningModelAsync(models={Models}): notEnabled → {NotEnabled}", models, notEnabled);
        var notEnabledSystemNames = new List<string>();
		_logger.LogError("PreparePluginsEnabledWarningModelAsync(models={Models}): notEnabledSystemNames → {NotEnabledSystemNames}", models, notEnabledSystemNames);

        foreach (var plugin in plugins)
        {
            var isEnabled = true;
			_logger.LogError("PreparePluginsEnabledWarningModelAsync(models={Models}): isEnabled → {IsEnabled}", models, isEnabled);

            switch (plugin)
            {
                case IPaymentMethod paymentMethod:
                    isEnabled = _paymentPluginManager.IsPluginActive(paymentMethod);
                    break;

                case IShippingRateComputationMethod shippingRateComputationMethod:
                    isEnabled = _shippingPluginManager.IsPluginActive(shippingRateComputationMethod);
                    break;

                case IPickupPointProvider pickupPointProvider:
                    isEnabled = _pickupPluginManager.IsPluginActive(pickupPointProvider);
                    break;

                case ITaxProvider taxProvider:
                    isEnabled = _taxPluginManager.IsPluginActive(taxProvider);
                    break;

                case IExternalAuthenticationMethod externalAuthenticationMethod:
                    isEnabled = _authenticationPluginManager.IsPluginActive(externalAuthenticationMethod);
                    break;

                case IMultiFactorAuthenticationMethod multiFactorAuthenticationMethod:
                    isEnabled = _multiFactorAuthenticationPluginManager.IsPluginActive(multiFactorAuthenticationMethod);
                    break;

                case IWidgetPlugin widgetPlugin:
                    isEnabled = _widgetPluginManager.IsPluginActive(widgetPlugin);
                    break;

                case IExchangeRateProvider exchangeRateProvider:
                    isEnabled = _exchangeRatePluginManager.IsPluginActive(exchangeRateProvider);
                    break;
            }

            if (isEnabled)
                continue;

            notEnabled.Add(plugin.PluginDescriptor.FriendlyName);
            notEnabledSystemNames.Add(plugin.PluginDescriptor.SystemName);
        }

        if (notEnabled.Any())
        {
            //get URL helper
            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
			_logger.LogError("PreparePluginsEnabledWarningModelAsync(models={Models}): urlHelper → {UrlHelper}", models, urlHelper);

            models.Add(new SystemWarningModel
            {
                Level = SystemWarningLevel.Warning,
                DontEncode = true,

                Text = $"{await _localizationService.GetResourceAsync("Admin.System.Warnings.PluginNotEnabled")}: {string.Join(", ", notEnabled)} (<a href=\"{urlHelper.Action("UninstallAndDeleteUnusedPlugins", "Plugin", new { names = notEnabledSystemNames.ToArray() })}\">{await _localizationService.GetResourceAsync("Admin.System.Warnings.PluginNotEnabled.AutoFixAndRestart")}</a>)"
            });
			_logger.LogDebug("PreparePluginsEnabledWarningModelAsync(models={Models}): Text → {Text}", models, Text);
        }
		_logger.LogWarning("<<< Exiting PreparePluginsEnabledWarningModelAsync(models={Models})", models);
    }

    /// <summary>
    /// Prepare multistore preview models for an entity
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <param name="entity">Entity</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of multistore preview models for an entity
    /// </returns>
    protected virtual async Task<IList<MultistorePreviewModel>> PrepareMultistorePreviewModelsForEntityAsync<TEntity>(TEntity entity) where TEntity : BaseEntity, ISlugSupported
    {
		_logger.LogError(">>> Entering PrepareMultistorePreviewModelsForEntityAsync(entity={Entity})", entity);
        var models = new List<MultistorePreviewModel>();
        var stores = await _storeService.GetAllStoresAsync();
		_logger.LogError("PrepareMultistorePreviewModelsForEntityAsync(entity={Entity}): stores → {Stores}", entity, stores);
		_logger.LogError("PrepareMultistorePreviewModelsForEntityAsync(entity={Entity}): models → {Models}", entity, models);

        foreach (var store in stores)
        {
            if (!Uri.TryCreate(store.Url, UriKind.Absolute, out var url))
                continue;

            models.Add(new MultistorePreviewModel
            {
                StoreName = store.Name,
                Url = await _nopUrlHelper
                    .RouteGenericUrlAsync(entity, url.Scheme, url.IsDefaultPort ? url.Host : $"{url.Host}:{url.Port}", null, null, false),
            });
			_logger.LogInformation("PrepareMultistorePreviewModelsForEntityAsync(entity={Entity}): Url → {Url}", entity, Url);
        }

		_logger.LogWarning("<<< Exiting PrepareMultistorePreviewModelsForEntityAsync(entity={Entity})", entity);
        return models;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare system info model
    /// </summary>
    /// <param name="model">System info model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the system info model
    /// </returns>
    public virtual async Task<SystemInfoModel> PrepareSystemInfoModelAsync(SystemInfoModel model)
    {
		_logger.LogError(">>> Entering PrepareSystemInfoModelAsync(model={Model})", model);
        ArgumentNullException.ThrowIfNull(model);

        model.NopVersion = NopVersion.FULL_VERSION;
		_logger.LogDebug("PrepareSystemInfoModelAsync(model={Model}): model.NopVersion → {Arg1}", model, model.NopVersion);
		_logger.LogDebug("PrepareSystemInfoModelAsync(model={Model}): model.ServerTimeZone → {Arg1}", model, model.ServerTimeZone);
		_logger.LogDebug("PrepareSystemInfoModelAsync(model={Model}): model.ServerLocalTime → {Arg1}", model, model.ServerLocalTime);
		_logger.LogDebug("PrepareSystemInfoModelAsync(model={Model}): model.UtcTime → {Arg1}", model, model.UtcTime);
        model.ServerTimeZone = TimeZoneInfo.Local.StandardName;
        model.ServerLocalTime = DateTime.Now;
        model.UtcTime = DateTime.UtcNow;
        model.CurrentUserTime = await _dateTimeHelper.ConvertToUserTimeAsync(DateTime.Now);
        model.HttpHost = _httpContextAccessor.HttpContext.Request.Headers[HeaderNames.Host];
		_logger.LogDebug("PrepareSystemInfoModelAsync(model={Model}): model.HttpHost → {Arg1}", model, model.HttpHost);

        //ensure no exception is thrown
        try
        {
            model.DatabaseCollation = await _dataProvider.GetDataBaseCollationAsync();
            model.OperatingSystem = Environment.OSVersion.VersionString;
			_logger.LogDebug("PrepareSystemInfoModelAsync(model={Model}): model.OperatingSystem → {Arg1}", model, model.OperatingSystem);
			_logger.LogDebug("PrepareSystemInfoModelAsync(model={Model}): model.AspNetInfo → {Arg1}", model, model.AspNetInfo);
			_logger.LogDebug("PrepareSystemInfoModelAsync(model={Model}): model.IsFullTrust → {Arg1}", model, model.IsFullTrust);
            model.AspNetInfo = RuntimeInformation.FrameworkDescription;
            model.IsFullTrust = AppDomain.CurrentDomain.IsFullyTrusted;
        }
        catch
        {
			_logger.LogError("Exception in PrepareSystemInfoModelAsync(model={Model}): {Arg1}", model, ex.Message);
            // ignored
        }

        foreach (var header in _httpContextAccessor.HttpContext.Request.Headers)
        {
            if (header.Key != HeaderNames.Cookie)
                model.Headers.Add(new SystemInfoModel.HeaderModel
                {
                    Name = header.Key,
                    Value = header.Value
                });
				_logger.LogDebug("PrepareSystemInfoModelAsync(model={Model}): Value → {Value}", model, Value);
        }

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            var loadedAssemblyModel = new SystemInfoModel.LoadedAssembly
            {
                FullName = assembly.FullName
            };

            //ensure no exception is thrown
            try
            {
                loadedAssemblyModel.Location = assembly.IsDynamic ? null : assembly.Location;
				_logger.LogDebug("PrepareSystemInfoModelAsync(model={Model}): loadedAssemblyModel.Location → {Arg1}", model, loadedAssemblyModel.Location);
                loadedAssemblyModel.IsDebug = assembly.GetCustomAttributes(typeof(DebuggableAttribute), false)
                    .FirstOrDefault() is DebuggableAttribute attribute && attribute.IsJITOptimizerDisabled;

                //https://stackoverflow.com/questions/2050396/getting-the-date-of-a-net-assembly
                //we use a simple method because the more Jeff Atwood's solution doesn't work anymore 
                //more info at https://blog.codinghorror.com/determining-build-date-the-hard-way/
                loadedAssemblyModel.BuildDate = assembly.IsDynamic ? null : (DateTime?)TimeZoneInfo.ConvertTimeFromUtc(_fileProvider.GetLastWriteTimeUtc(assembly.Location), TimeZoneInfo.Local);

            }
            catch
            {
                // ignored
            }

            model.LoadedAssemblies.Add(loadedAssemblyModel);
        }

        var currentStaticCacheManagerName = _staticCacheManager.GetType().Name;
		_logger.LogError("PrepareSystemInfoModelAsync(model={Model}): currentStaticCacheManagerName → {CurrentStaticCacheManagerName}", model, currentStaticCacheManagerName);

        if (_appSettings.Get<DistributedCacheConfig>().Enabled)
            currentStaticCacheManagerName +=
                $"({await _localizationService.GetLocalizedEnumAsync(_appSettings.Get<DistributedCacheConfig>().DistributedCacheType)})";

        model.CurrentStaticCacheManager = currentStaticCacheManagerName;
		_logger.LogDebug("PrepareSystemInfoModelAsync(model={Model}): model.CurrentStaticCacheManager → {Arg1}", model, model.CurrentStaticCacheManager);

		_logger.LogWarning("<<< Exiting PrepareSystemInfoModelAsync(model={Model})", model);
        return model;
    }

    /// <summary>
    /// Prepare proxy connection warning model
    /// </summary>
    /// <param name="models">List of system warning models</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task PrepareProxyConnectionWarningModelAsync(IList<SystemWarningModel> models)
    {
		_logger.LogError(">>> Entering PrepareProxyConnectionWarningModelAsync(models={Models})", models);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(models);

        //whether proxy is enabled
        if (!_proxySettings.Enabled)
            return;

        try
        {
            await _nopHttpClient.PingAsync();

            //connection is OK
            models.Add(new SystemWarningModel
            {
                Level = SystemWarningLevel.Pass,
                Text = await _localizationService.GetResourceAsync("Admin.System.Warnings.ProxyConnection.OK")
            });
				_logger.LogDebug("PrepareProxyConnectionWarningModelAsync(models={Models}): Text → {Text}", models, Text);
        }
        catch
        {
			_logger.LogError("Exception in PrepareProxyConnectionWarningModelAsync(models={Models}): {Arg1}", models, ex.Message);
            //connection failed
            models.Add(new SystemWarningModel
            {
                Level = SystemWarningLevel.Fail,
                Text = await _localizationService.GetResourceAsync("Admin.System.Warnings.ProxyConnection.Failed")
            });
        }
		_logger.LogError("PrepareProxyConnectionWarningModelAsync(models)={Models}: external service call took {Arg1} ms", models, __logStart.ElapsedMilliseconds);
		_logger.LogWarning("<<< Exiting PrepareProxyConnectionWarningModelAsync(models={Models})", models);
    }

    /// <summary>
    /// Prepare plugins warning model
    /// </summary>
    /// <param name="models">List of system warning models</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task PreparePluginsWarningModelAsync(IList<SystemWarningModel> models)
    {
		_logger.LogError(">>> Entering PreparePluginsWarningModelAsync(models={Models})", models);
        ArgumentNullException.ThrowIfNull(models);

        //incompatible plugins
        await PrepareIncompatibleWarningModelAsync(models);

        //collision of loaded assembly
        await PreparePluginsCollisionsWarningModelAsync(models);

        //override the same interface
        await PreparePluginsOverrideSameInterfaceWarningModelAsync(models);

        //not active plugins
        await PreparePluginsEnabledWarningModelAsync(models);

        //not install plugins
        await PreparePluginsInstalledWarningModelAsync(models);
		_logger.LogWarning("<<< Exiting PreparePluginsWarningModelAsync(models={Models})", models);
    }

    /// <summary>
    /// Prepare system warning models
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of system warning models
    /// </returns>
    public virtual async Task<IList<SystemWarningModel>> PrepareSystemWarningModelsAsync()
    {
		_logger.LogError(">>> Entering PrepareSystemWarningModelsAsync()");
        var models = new List<SystemWarningModel>();

		_logger.LogError("PrepareSystemWarningModelsAsync(): models → {Models}", models);
        //store URL
        await PrepareStoreUrlWarningModelAsync(models);

        //recommendations
        await PrepareRecommendationsModelAsync(models);

        //primary exchange rate currency
        await PrepareExchangeRateCurrencyWarningModelAsync(models);

        //primary store currency
        await PreparePrimaryStoreCurrencyWarningModelAsync(models);

        //base measure weight
        await PrepareBaseWeightWarningModelAsync(models);

        //base dimension weight
        await PrepareBaseDimensionWarningModelAsync(models);

        //payment methods
        await PreparePaymentMethodsWarningModelAsync(models);

        //performance settings
        await PreparePerformanceSettingsWarningModelAsync(models);

        //validate write permissions (the same procedure like during installation)
        await PrepareFilePermissionsWarningModelAsync(models);

        //plugins
        await PreparePluginsWarningModelAsync(models);

        //proxy connection
        await PrepareProxyConnectionWarningModelAsync(models);

        //publish event and add another warnings (for example from plugins) 
        var warningEvent = new SystemWarningCreatedEvent();
		_logger.LogError("PrepareSystemWarningModelsAsync(): warningEvent → {WarningEvent}", warningEvent);
        await _eventPublisher.PublishAsync(warningEvent);
        models.AddRange(warningEvent.SystemWarnings);

		_logger.LogWarning("<<< Exiting PrepareSystemWarningModelsAsync()");
        return models;
    }

    /// <summary>
    /// Prepare maintenance model
    /// </summary>
    /// <param name="model">Maintenance model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the maintenance model
    /// </returns>
    public virtual async Task<MaintenanceModel> PrepareMaintenanceModelAsync(MaintenanceModel model)
    {
		_logger.LogError(">>> Entering PrepareMaintenanceModelAsync(model={Model})", model);
        ArgumentNullException.ThrowIfNull(model);

        model.DeleteGuests.EndDate = DateTime.UtcNow.AddDays(-7);
        model.DeleteGuests.OnlyWithoutShoppingCart = true;
        model.DeleteAbandonedCarts.OlderThan = DateTime.UtcNow.AddDays(-182);

        model.DeleteAlreadySentQueuedEmails.EndDate = DateTime.UtcNow.AddDays(-7);

        model.BackupSupported = _dataProvider.BackupSupported;
		_logger.LogDebug("PrepareMaintenanceModelAsync(model={Model}): model.BackupSupported → {Arg1}", model, model.BackupSupported);

        //prepare nested search model
        PrepareBackupFileSearchModel(model.BackupFileSearchModel);

        //prepare nested DeleteThumbsFiles model
        await PrepareDeleteThumbFilesModelAsync(model.DeleteThumbsFiles);

		_logger.LogWarning("<<< Exiting PrepareMaintenanceModelAsync(model={Model})", model);
        return model;
    }

    /// <summary>
    /// Prepare paged backup file list model
    /// </summary>
    /// <param name="searchModel">Backup file search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the backup file list model
    /// </returns>
    public virtual Task<BackupFileListModel> PrepareBackupFileListModelAsync(BackupFileSearchModel searchModel)
    {
		_logger.LogError(">>> Entering PrepareBackupFileListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //get backup files
        var backupFiles = _maintenanceService.GetAllBackupFiles().ToPagedList(searchModel);
		_logger.LogError("PrepareBackupFileListModelAsync(searchModel={SearchModel}): backupFiles → {BackupFiles}", searchModel, backupFiles);

        //prepare list model
        var model = new BackupFileListModel().PrepareToGrid(searchModel, backupFiles, () =>
        {
            return backupFiles.Select(file => new BackupFileModel
            {
                Name = _fileProvider.GetFileName(file),

                //fill in additional values (not existing in the entity)
                Length = $"{_fileProvider.FileLength(file) / 1024f / 1024f:F2} Mb",

                Link = $"{_webHelper.GetStoreLocation()}db_backups/{_fileProvider.GetFileName(file)}"
            });
        });

		_logger.LogWarning("<<< Exiting PrepareBackupFileListModelAsync(searchModel={SearchModel})", searchModel);
        return Task.FromResult(model);
    }

    /// <summary>
    /// Prepare URL record search model
    /// </summary>
    /// <param name="searchModel">URL record search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the uRL record search model
    /// </returns>
    public virtual async Task<UrlRecordSearchModel> PrepareUrlRecordSearchModelAsync(UrlRecordSearchModel searchModel)
    {
		_logger.LogError(">>> Entering PrepareUrlRecordSearchModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare available languages
        //we insert 0 as 'Standard' language.
        //let's insert -1 for 'All' language selection.
        await _baseAdminModelFactory.PrepareLanguagesAsync(searchModel.AvailableLanguages,
            defaultItemText: await _localizationService.GetResourceAsync("Admin.System.SeNames.List.Language.Standard"));
        searchModel.AvailableLanguages.Insert(0,
            new SelectListItem { Text = await _localizationService.GetResourceAsync("Admin.Common.All"), Value = "-1" });
        searchModel.LanguageId = -1;
		_logger.LogDebug("PrepareUrlRecordSearchModelAsync(searchModel={SearchModel}): searchModel.LanguageId → {Arg1}", searchModel, searchModel.LanguageId);

        //prepare "is active" filter (0 - all; 1 - active only; 2 - inactive only)
        searchModel.AvailableActiveOptions.Add(new SelectListItem
        {
            Value = "0",
            Text = await _localizationService.GetResourceAsync("Admin.System.SeNames.List.IsActive.All")
        });
			_logger.LogDebug("PrepareUrlRecordSearchModelAsync(searchModel={SearchModel}): Text → {Text}", searchModel, Text);
        searchModel.AvailableActiveOptions.Add(new SelectListItem
        {
            Value = "1",
            Text = await _localizationService.GetResourceAsync("Admin.System.SeNames.List.IsActive.ActiveOnly")
        });
        searchModel.AvailableActiveOptions.Add(new SelectListItem
        {
            Value = "2",
            Text = await _localizationService.GetResourceAsync("Admin.System.SeNames.List.IsActive.InactiveOnly")
        });

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogWarning("<<< Exiting PrepareUrlRecordSearchModelAsync(searchModel={SearchModel})", searchModel);
        return searchModel;
    }

    /// <summary>
    /// Prepare paged URL record list model
    /// </summary>
    /// <param name="searchModel">URL record search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the uRL record list model
    /// </returns>
    public virtual async Task<UrlRecordListModel> PrepareUrlRecordListModelAsync(UrlRecordSearchModel searchModel)
    {
		_logger.LogError(">>> Entering PrepareUrlRecordListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        var isActive = searchModel.IsActiveId == 0 ? null : (bool?)(searchModel.IsActiveId == 1);
		_logger.LogError("PrepareUrlRecordListModelAsync(searchModel={SearchModel}): isActive → {IsActive}", searchModel, isActive);
        var languageId = searchModel.LanguageId < 0 ? null : (int?)(searchModel.LanguageId);
		_logger.LogError("PrepareUrlRecordListModelAsync(searchModel={SearchModel}): languageId → {LanguageId}", searchModel, languageId);

        //get URL records
        var urlRecords = await _urlRecordService.GetAllUrlRecordsAsync(slug: searchModel.SeName,
            languageId: languageId, isActive: isActive,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

        //get URL helper
        var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
		_logger.LogError("PrepareUrlRecordListModelAsync(searchModel={SearchModel}): urlHelper → {UrlHelper}", searchModel, urlHelper);

        //prepare list model
        var model = await new UrlRecordListModel().PrepareToGridAsync(searchModel, urlRecords, () =>
        {
            return urlRecords.SelectAwait(async urlRecord =>
            {
                //fill in model values from the entity
                var urlRecordModel = urlRecord.ToModel<UrlRecordModel>();
				_logger.LogError("PrepareUrlRecordListModelAsync(searchModel={SearchModel}): urlRecordModel → {UrlRecordModel}", searchModel, urlRecordModel);

                //fill in additional values (not existing in the entity)
                urlRecordModel.Name = urlRecord.Slug;
				_logger.LogDebug("PrepareUrlRecordListModelAsync(searchModel={SearchModel}): urlRecordModel.Name → {Arg1}", searchModel, urlRecordModel.Name);
                urlRecordModel.Language = urlRecord.LanguageId == 0
                    ? await _localizationService.GetResourceAsync("Admin.System.SeNames.Language.Standard")
                    : (await _languageService.GetLanguageByIdAsync(urlRecord.LanguageId))?.Name ?? "Unknown";

                //details URL
                var detailsUrl = string.Empty;
				_logger.LogError("PrepareUrlRecordListModelAsync(searchModel={SearchModel}): detailsUrl → {DetailsUrl}", searchModel, detailsUrl);
                var entityName = urlRecord.EntityName?.ToLowerInvariant() ?? string.Empty;
				_logger.LogError("PrepareUrlRecordListModelAsync(searchModel={SearchModel}): entityName → {EntityName}", searchModel, entityName);
                switch (entityName)
                {
                    case "blogpost":
                        detailsUrl = urlHelper.Action("BlogPostEdit", "Blog", new { id = urlRecord.EntityId });
                        break;
                    case "category":
                        detailsUrl = urlHelper.Action("Edit", "Category", new { id = urlRecord.EntityId });
                        break;
                    case "manufacturer":
                        detailsUrl = urlHelper.Action("Edit", "Manufacturer", new { id = urlRecord.EntityId });
                        break;
                    case "product":
                        detailsUrl = urlHelper.Action("Edit", "Product", new { id = urlRecord.EntityId });
                        break;
                    case "newsitem":
                        detailsUrl = urlHelper.Action("NewsItemEdit", "News", new { id = urlRecord.EntityId });
                        break;
                    case "topic":
                        detailsUrl = urlHelper.Action("Edit", "Topic", new { id = urlRecord.EntityId });
                        break;
                    case "vendor":
                        detailsUrl = urlHelper.Action("Edit", "Vendor", new { id = urlRecord.EntityId });
                        break;
                }

                urlRecordModel.DetailsUrl = detailsUrl;
				_logger.LogDebug("PrepareUrlRecordListModelAsync(searchModel={SearchModel}): urlRecordModel.DetailsUrl → {Arg1}", searchModel, urlRecordModel.DetailsUrl);

                return urlRecordModel;
            });
        });
		_logger.LogWarning("<<< Exiting PrepareUrlRecordListModelAsync(searchModel={SearchModel})", searchModel);
        return model;
    }

    /// <summary>
    /// Prepare language selector model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the language selector model
    /// </returns>
    public virtual async Task<LanguageSelectorModel> PrepareLanguageSelectorModelAsync()
    {
		_logger.LogError(">>> Entering PrepareLanguageSelectorModelAsync()");
        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogError("PrepareLanguageSelectorModelAsync(): store → {Store}", store);
        var model = new LanguageSelectorModel
        {
            CurrentLanguage = (await _workContext.GetWorkingLanguageAsync()).ToModel<LanguageModel>(),
            AvailableLanguages = (await _languageService
                    .GetAllLanguagesAsync(storeId: store.Id))
                .Select(language => language.ToModel<LanguageModel>()).ToList()
        };

		_logger.LogWarning("<<< Exiting PrepareLanguageSelectorModelAsync()");
        return model;
    }

    /// <summary>
    /// Prepare popular search term search model
    /// </summary>
    /// <param name="searchModel">Popular search term search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the popular search term search model
    /// </returns>
    public virtual Task<PopularSearchTermSearchModel> PreparePopularSearchTermSearchModelAsync(PopularSearchTermSearchModel searchModel)
    {
		_logger.LogError(">>> Entering PreparePopularSearchTermSearchModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetGridPageSize(5);

		_logger.LogWarning("<<< Exiting PreparePopularSearchTermSearchModelAsync(searchModel={SearchModel})", searchModel);
        return Task.FromResult(searchModel);
    }

    /// <summary>
    /// Prepare paged popular search term list model
    /// </summary>
    /// <param name="searchModel">Popular search term search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the popular search term list model
    /// </returns>
    public virtual async Task<PopularSearchTermListModel> PreparePopularSearchTermListModelAsync(PopularSearchTermSearchModel searchModel)
    {
		_logger.LogError(">>> Entering PreparePopularSearchTermListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //get popular search terms
        var searchTermRecordLines = await _searchTermService.GetStatsAsync(pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);
		_logger.LogError("PreparePopularSearchTermListModelAsync(searchModel={SearchModel}): searchTermRecordLines → {SearchTermRecordLines}", searchModel, searchTermRecordLines);

        //prepare list model
        var model = new PopularSearchTermListModel().PrepareToGrid(searchModel, searchTermRecordLines, () =>
        {
            return searchTermRecordLines.Select(searchTerm => new PopularSearchTermModel
            {
                Keyword = searchTerm.Keyword,
                Count = searchTerm.Count
            });
        });

		_logger.LogWarning("<<< Exiting PreparePopularSearchTermListModelAsync(searchModel={SearchModel})", searchModel);
        return model;
    }

    /// <summary>
    /// Prepare common statistics model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the common statistics model
    /// </returns>
    public virtual async Task<CommonStatisticsModel> PrepareCommonStatisticsModelAsync()
    {
		_logger.LogError(">>> Entering PrepareCommonStatisticsModelAsync()");
        var model = new CommonStatisticsModel
        {
            NumberOfOrders = (await _orderService.SearchOrdersAsync(pageIndex: 0, pageSize: 1, getOnlyTotalCount: true)).TotalCount
        };

        var customerRoleIds = new[] { (await _customerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.RegisteredRoleName)).Id };
		_logger.LogError("PrepareCommonStatisticsModelAsync(): customerRoleIds → {CustomerRoleIds}", customerRoleIds);
        model.NumberOfCustomers = (await _customerService.GetAllCustomersAsync(customerRoleIds: customerRoleIds,
            pageIndex: 0, pageSize: 1, getOnlyTotalCount: true)).TotalCount;

        var returnRequestStatus = ReturnRequestStatus.Pending;
        model.NumberOfPendingReturnRequests = (await _returnRequestService.SearchReturnRequestsAsync(rs: returnRequestStatus,
            pageIndex: 0, pageSize: 1, getOnlyTotalCount: true)).TotalCount;

        model.NumberOfLowStockProducts =
            (await _productService.GetLowStockProductsAsync(getOnlyTotalCount: true)).TotalCount +
            (await _productService.GetLowStockProductCombinationsAsync(getOnlyTotalCount: true)).TotalCount;

		_logger.LogWarning("<<< Exiting PrepareCommonStatisticsModelAsync()");
        return model;
    }

    /// <summary>
    /// Prepare multistore preview models
    /// </summary>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <param name="model">Entity model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of multistore preview models
    /// </returns>
    public virtual async Task<IList<MultistorePreviewModel>> PrepareMultistorePreviewModelsAsync<TModel>(TModel model) where TModel : BaseNopEntityModel
    {
		_logger.LogError(">>> Entering PrepareMultistorePreviewModelsAsync(model={Model})", model);
        switch (model)
        {
            case BlogPostModel blogPostModel:
                var blogPost = await _blogService.GetBlogPostByIdAsync(blogPostModel.Id);
				_logger.LogError("PrepareMultistorePreviewModelsAsync(model={Model}): blogPost → {BlogPost}", model, blogPost);
                return await PrepareMultistorePreviewModelsForEntityAsync(blogPost);

            case CategoryModel categoryModel:
                var category = await _categoryService.GetCategoryByIdAsync(categoryModel.Id);
				_logger.LogError("PrepareMultistorePreviewModelsAsync(model={Model}): category → {Category}", model, category);
                return await PrepareMultistorePreviewModelsForEntityAsync(category);

            case ManufacturerModel manufacturerModel:
                var manufacturerEntity = await _manufacturerService.GetManufacturerByIdAsync(manufacturerModel.Id);
				_logger.LogError("PrepareMultistorePreviewModelsAsync(model={Model}): manufacturerEntity → {ManufacturerEntity}", model, manufacturerEntity);
                return await PrepareMultistorePreviewModelsForEntityAsync(manufacturerEntity);

            case NewsItemModel newsItemModel:
                var newsItem = await _newsService.GetNewsByIdAsync(newsItemModel.Id);
				_logger.LogError("PrepareMultistorePreviewModelsAsync(model={Model}): newsItem → {NewsItem}", model, newsItem);
                return await PrepareMultistorePreviewModelsForEntityAsync(newsItem);

            case ProductModel productModel:
                var product = await _productService.GetProductByIdAsync(productModel.Id);
				_logger.LogError("PrepareMultistorePreviewModelsAsync(model={Model}): product → {Product}", model, product);
                return await PrepareMultistorePreviewModelsForEntityAsync(product);

            case TopicModel topicModel:
                var topic = await _topicService.GetTopicByIdAsync(topicModel.Id);
				_logger.LogError("PrepareMultistorePreviewModelsAsync(model={Model}): topic → {Topic}", model, topic);
                return await PrepareMultistorePreviewModelsForEntityAsync(topic);

            default:
				_logger.LogError("Throwing NotImplementedException from PrepareMultistorePreviewModelsAsync(model={Model})", model);
		_logger.LogWarning("<<< Exiting PrepareMultistorePreviewModelsAsync(model={Model})", model);
                throw new NotImplementedException("Unknown entity type");
        }
    }

    #endregion
}