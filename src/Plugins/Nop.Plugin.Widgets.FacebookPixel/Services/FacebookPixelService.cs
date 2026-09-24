using System.Globalization;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Core.Http.Extensions;
using Nop.Data;
using Nop.Plugin.Widgets.FacebookPixel.Domain;
using Nop.Services.Catalog;
using Nop.Services.Cms;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Tax;
using Nop.Web.Framework.Models.Cms;
using Nop.Web.Models.Catalog;

namespace Nop.Plugin.Widgets.FacebookPixel.Services;

/// <summary>
/// Represents Facebook Pixel service
/// </summary>
public class FacebookPixelService
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Constants

    /// <summary>
    /// Get default tabs number to format scripts indentation
    /// </summary>
    protected const int TABS_NUMBER = 2;

    #endregion

    #region Fields

    protected readonly CurrencySettings _currencySettings;
    protected readonly FacebookConversionsHttpClient _facebookConversionsHttpClient;
    protected readonly ICategoryService _categoryService;
    protected readonly ICountryService _countryService;
    protected readonly ICurrencyService _currencyService;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly IHttpContextAccessor _httpContextAccessor;
    protected readonly ILogger _logger;
    protected readonly IOrderService _orderService;
    protected readonly IOrderTotalCalculationService _orderTotalCalculationService;
    protected readonly IPriceCalculationService _priceCalculationService;
    protected readonly IProductService _productService;
    protected readonly IRepository<FacebookPixelConfiguration> _facebookPixelConfigurationRepository;
    protected readonly IShoppingCartService _shoppingCartService;
    protected readonly IStateProvinceService _stateProvinceService;
    protected readonly IStaticCacheManager _staticCacheManager;
    protected readonly IStoreContext _storeContext;
    protected readonly ITaxService _taxService;
    protected readonly IWebHelper _webHelper;
    protected readonly IWidgetPluginManager _widgetPluginManager;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public FacebookPixelService(CurrencySettings currencySettings,
        FacebookConversionsHttpClient facebookConversionsHttpClient,
        ICategoryService categoryService,
        ICountryService countryService,
        ICurrencyService currencyService,
        IGenericAttributeService genericAttributeService,
        IHttpContextAccessor httpContextAccessor,
        ILogger logger,
        IOrderService orderService,
        IOrderTotalCalculationService orderTotalCalculationService,
        IPriceCalculationService priceCalculationService,
        IProductService productService,
        IRepository<FacebookPixelConfiguration> facebookPixelConfigurationRepository,
        IShoppingCartService shoppingCartService,
        IStateProvinceService stateProvinceService,
        IStaticCacheManager staticCacheManager,
        IStoreContext storeContext,
        ITaxService taxService,
        IWebHelper webHelper,
        IWidgetPluginManager widgetPluginManager,
        IWorkContext workContext)
    {
        _currencySettings = currencySettings;
		_logger.LogDebug("FacebookPixelService(currencySettings={CurrencySettings},facebookConversionsHttpClient={FacebookConversionsHttpClient},categoryService={CategoryService},countryService={CountryService},currencyService={CurrencyService},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},logger={Logger},orderService={OrderService},orderTotalCalculationService={OrderTotalCalculationService},priceCalculationService={PriceCalculationService},productService={ProductService},facebookPixelConfigurationRepository={FacebookPixelConfigurationRepository},shoppingCartService=<redacted>,stateProvinceService={StateProvinceService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},taxService={TaxService},webHelper={WebHelper},widgetPluginManager={WidgetPluginManager},workContext={WorkContext}): _currencySettings → {_currencySettings}", currencySettings, facebookConversionsHttpClient, categoryService, countryService, currencyService, genericAttributeService, httpContextAccessor, logger, orderService, orderTotalCalculationService, priceCalculationService, productService, facebookPixelConfigurationRepository, stateProvinceService, staticCacheManager, storeContext, taxService, webHelper, widgetPluginManager, workContext, _currencySettings);
        _facebookConversionsHttpClient = facebookConversionsHttpClient;
		_logger.LogDebug("FacebookPixelService(currencySettings={CurrencySettings},facebookConversionsHttpClient={FacebookConversionsHttpClient},categoryService={CategoryService},countryService={CountryService},currencyService={CurrencyService},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},logger={Logger},orderService={OrderService},orderTotalCalculationService={OrderTotalCalculationService},priceCalculationService={PriceCalculationService},productService={ProductService},facebookPixelConfigurationRepository={FacebookPixelConfigurationRepository},shoppingCartService=<redacted>,stateProvinceService={StateProvinceService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},taxService={TaxService},webHelper={WebHelper},widgetPluginManager={WidgetPluginManager},workContext={WorkContext}): _facebookConversionsHttpClient → {_facebookConversionsHttpClient}", currencySettings, facebookConversionsHttpClient, categoryService, countryService, currencyService, genericAttributeService, httpContextAccessor, logger, orderService, orderTotalCalculationService, priceCalculationService, productService, facebookPixelConfigurationRepository, stateProvinceService, staticCacheManager, storeContext, taxService, webHelper, widgetPluginManager, workContext, _facebookConversionsHttpClient);
        _categoryService = categoryService;
		_logger.LogDebug("FacebookPixelService(currencySettings={CurrencySettings},facebookConversionsHttpClient={FacebookConversionsHttpClient},categoryService={CategoryService},countryService={CountryService},currencyService={CurrencyService},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},logger={Logger},orderService={OrderService},orderTotalCalculationService={OrderTotalCalculationService},priceCalculationService={PriceCalculationService},productService={ProductService},facebookPixelConfigurationRepository={FacebookPixelConfigurationRepository},shoppingCartService=<redacted>,stateProvinceService={StateProvinceService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},taxService={TaxService},webHelper={WebHelper},widgetPluginManager={WidgetPluginManager},workContext={WorkContext}): _categoryService → {_categoryService}", currencySettings, facebookConversionsHttpClient, categoryService, countryService, currencyService, genericAttributeService, httpContextAccessor, logger, orderService, orderTotalCalculationService, priceCalculationService, productService, facebookPixelConfigurationRepository, stateProvinceService, staticCacheManager, storeContext, taxService, webHelper, widgetPluginManager, workContext, _categoryService);
        _countryService = countryService;
		_logger.LogDebug("FacebookPixelService(currencySettings={CurrencySettings},facebookConversionsHttpClient={FacebookConversionsHttpClient},categoryService={CategoryService},countryService={CountryService},currencyService={CurrencyService},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},logger={Logger},orderService={OrderService},orderTotalCalculationService={OrderTotalCalculationService},priceCalculationService={PriceCalculationService},productService={ProductService},facebookPixelConfigurationRepository={FacebookPixelConfigurationRepository},shoppingCartService=<redacted>,stateProvinceService={StateProvinceService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},taxService={TaxService},webHelper={WebHelper},widgetPluginManager={WidgetPluginManager},workContext={WorkContext}): _countryService → {_countryService}", currencySettings, facebookConversionsHttpClient, categoryService, countryService, currencyService, genericAttributeService, httpContextAccessor, logger, orderService, orderTotalCalculationService, priceCalculationService, productService, facebookPixelConfigurationRepository, stateProvinceService, staticCacheManager, storeContext, taxService, webHelper, widgetPluginManager, workContext, _countryService);
        _currencyService = currencyService;
		_logger.LogDebug("FacebookPixelService(currencySettings={CurrencySettings},facebookConversionsHttpClient={FacebookConversionsHttpClient},categoryService={CategoryService},countryService={CountryService},currencyService={CurrencyService},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},logger={Logger},orderService={OrderService},orderTotalCalculationService={OrderTotalCalculationService},priceCalculationService={PriceCalculationService},productService={ProductService},facebookPixelConfigurationRepository={FacebookPixelConfigurationRepository},shoppingCartService=<redacted>,stateProvinceService={StateProvinceService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},taxService={TaxService},webHelper={WebHelper},widgetPluginManager={WidgetPluginManager},workContext={WorkContext}): _currencyService → {_currencyService}", currencySettings, facebookConversionsHttpClient, categoryService, countryService, currencyService, genericAttributeService, httpContextAccessor, logger, orderService, orderTotalCalculationService, priceCalculationService, productService, facebookPixelConfigurationRepository, stateProvinceService, staticCacheManager, storeContext, taxService, webHelper, widgetPluginManager, workContext, _currencyService);
        _genericAttributeService = genericAttributeService;
		_logger.LogDebug("FacebookPixelService(currencySettings={CurrencySettings},facebookConversionsHttpClient={FacebookConversionsHttpClient},categoryService={CategoryService},countryService={CountryService},currencyService={CurrencyService},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},logger={Logger},orderService={OrderService},orderTotalCalculationService={OrderTotalCalculationService},priceCalculationService={PriceCalculationService},productService={ProductService},facebookPixelConfigurationRepository={FacebookPixelConfigurationRepository},shoppingCartService=<redacted>,stateProvinceService={StateProvinceService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},taxService={TaxService},webHelper={WebHelper},widgetPluginManager={WidgetPluginManager},workContext={WorkContext}): _genericAttributeService → {_genericAttributeService}", currencySettings, facebookConversionsHttpClient, categoryService, countryService, currencyService, genericAttributeService, httpContextAccessor, logger, orderService, orderTotalCalculationService, priceCalculationService, productService, facebookPixelConfigurationRepository, stateProvinceService, staticCacheManager, storeContext, taxService, webHelper, widgetPluginManager, workContext, _genericAttributeService);
        _httpContextAccessor = httpContextAccessor;
		_logger.LogDebug("FacebookPixelService(currencySettings={CurrencySettings},facebookConversionsHttpClient={FacebookConversionsHttpClient},categoryService={CategoryService},countryService={CountryService},currencyService={CurrencyService},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},logger={Logger},orderService={OrderService},orderTotalCalculationService={OrderTotalCalculationService},priceCalculationService={PriceCalculationService},productService={ProductService},facebookPixelConfigurationRepository={FacebookPixelConfigurationRepository},shoppingCartService=<redacted>,stateProvinceService={StateProvinceService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},taxService={TaxService},webHelper={WebHelper},widgetPluginManager={WidgetPluginManager},workContext={WorkContext}): _httpContextAccessor → {_httpContextAccessor}", currencySettings, facebookConversionsHttpClient, categoryService, countryService, currencyService, genericAttributeService, httpContextAccessor, logger, orderService, orderTotalCalculationService, priceCalculationService, productService, facebookPixelConfigurationRepository, stateProvinceService, staticCacheManager, storeContext, taxService, webHelper, widgetPluginManager, workContext, _httpContextAccessor);
        _logger = logger;
		_logger.LogError(">>> Entering FacebookPixelService(currencySettings={CurrencySettings},facebookConversionsHttpClient={FacebookConversionsHttpClient},categoryService={CategoryService},countryService={CountryService},currencyService={CurrencyService},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},logger={Logger},orderService={OrderService},orderTotalCalculationService={OrderTotalCalculationService},priceCalculationService={PriceCalculationService},productService={ProductService},facebookPixelConfigurationRepository={FacebookPixelConfigurationRepository},shoppingCartService=<redacted>,stateProvinceService={StateProvinceService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},taxService={TaxService},webHelper={WebHelper},widgetPluginManager={WidgetPluginManager},workContext={WorkContext})", currencySettings, facebookConversionsHttpClient, categoryService, countryService, currencyService, genericAttributeService, httpContextAccessor, logger, orderService, orderTotalCalculationService, priceCalculationService, productService, facebookPixelConfigurationRepository, stateProvinceService, staticCacheManager, storeContext, taxService, webHelper, widgetPluginManager, workContext);
        _orderService = orderService;
        _orderTotalCalculationService = orderTotalCalculationService;
		_logger.LogDebug("FacebookPixelService(currencySettings={CurrencySettings},facebookConversionsHttpClient={FacebookConversionsHttpClient},categoryService={CategoryService},countryService={CountryService},currencyService={CurrencyService},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},logger={Logger},orderService={OrderService},orderTotalCalculationService={OrderTotalCalculationService},priceCalculationService={PriceCalculationService},productService={ProductService},facebookPixelConfigurationRepository={FacebookPixelConfigurationRepository},shoppingCartService=<redacted>,stateProvinceService={StateProvinceService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},taxService={TaxService},webHelper={WebHelper},widgetPluginManager={WidgetPluginManager},workContext={WorkContext}): _orderService → {_orderService}", currencySettings, facebookConversionsHttpClient, categoryService, countryService, currencyService, genericAttributeService, httpContextAccessor, logger, orderService, orderTotalCalculationService, priceCalculationService, productService, facebookPixelConfigurationRepository, stateProvinceService, staticCacheManager, storeContext, taxService, webHelper, widgetPluginManager, workContext, _orderService);
		_logger.LogDebug("FacebookPixelService(currencySettings={CurrencySettings},facebookConversionsHttpClient={FacebookConversionsHttpClient},categoryService={CategoryService},countryService={CountryService},currencyService={CurrencyService},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},logger={Logger},orderService={OrderService},orderTotalCalculationService={OrderTotalCalculationService},priceCalculationService={PriceCalculationService},productService={ProductService},facebookPixelConfigurationRepository={FacebookPixelConfigurationRepository},shoppingCartService=<redacted>,stateProvinceService={StateProvinceService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},taxService={TaxService},webHelper={WebHelper},widgetPluginManager={WidgetPluginManager},workContext={WorkContext}): _orderTotalCalculationService → {_orderTotalCalculationService}", currencySettings, facebookConversionsHttpClient, categoryService, countryService, currencyService, genericAttributeService, httpContextAccessor, logger, orderService, orderTotalCalculationService, priceCalculationService, productService, facebookPixelConfigurationRepository, stateProvinceService, staticCacheManager, storeContext, taxService, webHelper, widgetPluginManager, workContext, _orderTotalCalculationService);
        _priceCalculationService = priceCalculationService;
        _productService = productService;
		_logger.LogDebug("FacebookPixelService(currencySettings={CurrencySettings},facebookConversionsHttpClient={FacebookConversionsHttpClient},categoryService={CategoryService},countryService={CountryService},currencyService={CurrencyService},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},logger={Logger},orderService={OrderService},orderTotalCalculationService={OrderTotalCalculationService},priceCalculationService={PriceCalculationService},productService={ProductService},facebookPixelConfigurationRepository={FacebookPixelConfigurationRepository},shoppingCartService=<redacted>,stateProvinceService={StateProvinceService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},taxService={TaxService},webHelper={WebHelper},widgetPluginManager={WidgetPluginManager},workContext={WorkContext}): _priceCalculationService → {_priceCalculationService}", currencySettings, facebookConversionsHttpClient, categoryService, countryService, currencyService, genericAttributeService, httpContextAccessor, logger, orderService, orderTotalCalculationService, priceCalculationService, productService, facebookPixelConfigurationRepository, stateProvinceService, staticCacheManager, storeContext, taxService, webHelper, widgetPluginManager, workContext, _priceCalculationService);
		_logger.LogDebug("FacebookPixelService(currencySettings={CurrencySettings},facebookConversionsHttpClient={FacebookConversionsHttpClient},categoryService={CategoryService},countryService={CountryService},currencyService={CurrencyService},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},logger={Logger},orderService={OrderService},orderTotalCalculationService={OrderTotalCalculationService},priceCalculationService={PriceCalculationService},productService={ProductService},facebookPixelConfigurationRepository={FacebookPixelConfigurationRepository},shoppingCartService=<redacted>,stateProvinceService={StateProvinceService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},taxService={TaxService},webHelper={WebHelper},widgetPluginManager={WidgetPluginManager},workContext={WorkContext}): _productService → {_productService}", currencySettings, facebookConversionsHttpClient, categoryService, countryService, currencyService, genericAttributeService, httpContextAccessor, logger, orderService, orderTotalCalculationService, priceCalculationService, productService, facebookPixelConfigurationRepository, stateProvinceService, staticCacheManager, storeContext, taxService, webHelper, widgetPluginManager, workContext, _productService);
        _facebookPixelConfigurationRepository = facebookPixelConfigurationRepository;
        _shoppingCartService = shoppingCartService;
		_logger.LogDebug("FacebookPixelService(currencySettings={CurrencySettings},facebookConversionsHttpClient={FacebookConversionsHttpClient},categoryService={CategoryService},countryService={CountryService},currencyService={CurrencyService},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},logger={Logger},orderService={OrderService},orderTotalCalculationService={OrderTotalCalculationService},priceCalculationService={PriceCalculationService},productService={ProductService},facebookPixelConfigurationRepository={FacebookPixelConfigurationRepository},shoppingCartService=<redacted>,stateProvinceService={StateProvinceService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},taxService={TaxService},webHelper={WebHelper},widgetPluginManager={WidgetPluginManager},workContext={WorkContext}): _facebookPixelConfigurationRepository → {_facebookPixelConfigurationRepository}", currencySettings, facebookConversionsHttpClient, categoryService, countryService, currencyService, genericAttributeService, httpContextAccessor, logger, orderService, orderTotalCalculationService, priceCalculationService, productService, facebookPixelConfigurationRepository, stateProvinceService, staticCacheManager, storeContext, taxService, webHelper, widgetPluginManager, workContext, _facebookPixelConfigurationRepository);
		_logger.LogDebug("FacebookPixelService(currencySettings={CurrencySettings},facebookConversionsHttpClient={FacebookConversionsHttpClient},categoryService={CategoryService},countryService={CountryService},currencyService={CurrencyService},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},logger={Logger},orderService={OrderService},orderTotalCalculationService={OrderTotalCalculationService},priceCalculationService={PriceCalculationService},productService={ProductService},facebookPixelConfigurationRepository={FacebookPixelConfigurationRepository},shoppingCartService=<redacted>,stateProvinceService={StateProvinceService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},taxService={TaxService},webHelper={WebHelper},widgetPluginManager={WidgetPluginManager},workContext={WorkContext}): _shoppingCartService → {_shoppingCartService}", currencySettings, facebookConversionsHttpClient, categoryService, countryService, currencyService, genericAttributeService, httpContextAccessor, logger, orderService, orderTotalCalculationService, priceCalculationService, productService, facebookPixelConfigurationRepository, stateProvinceService, staticCacheManager, storeContext, taxService, webHelper, widgetPluginManager, workContext, _shoppingCartService);
        _stateProvinceService = stateProvinceService;
        _staticCacheManager = staticCacheManager;
		_logger.LogDebug("FacebookPixelService(currencySettings={CurrencySettings},facebookConversionsHttpClient={FacebookConversionsHttpClient},categoryService={CategoryService},countryService={CountryService},currencyService={CurrencyService},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},logger={Logger},orderService={OrderService},orderTotalCalculationService={OrderTotalCalculationService},priceCalculationService={PriceCalculationService},productService={ProductService},facebookPixelConfigurationRepository={FacebookPixelConfigurationRepository},shoppingCartService=<redacted>,stateProvinceService={StateProvinceService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},taxService={TaxService},webHelper={WebHelper},widgetPluginManager={WidgetPluginManager},workContext={WorkContext}): _stateProvinceService → {_stateProvinceService}", currencySettings, facebookConversionsHttpClient, categoryService, countryService, currencyService, genericAttributeService, httpContextAccessor, logger, orderService, orderTotalCalculationService, priceCalculationService, productService, facebookPixelConfigurationRepository, stateProvinceService, staticCacheManager, storeContext, taxService, webHelper, widgetPluginManager, workContext, _stateProvinceService);
		_logger.LogDebug("FacebookPixelService(currencySettings={CurrencySettings},facebookConversionsHttpClient={FacebookConversionsHttpClient},categoryService={CategoryService},countryService={CountryService},currencyService={CurrencyService},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},logger={Logger},orderService={OrderService},orderTotalCalculationService={OrderTotalCalculationService},priceCalculationService={PriceCalculationService},productService={ProductService},facebookPixelConfigurationRepository={FacebookPixelConfigurationRepository},shoppingCartService=<redacted>,stateProvinceService={StateProvinceService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},taxService={TaxService},webHelper={WebHelper},widgetPluginManager={WidgetPluginManager},workContext={WorkContext}): _staticCacheManager → {_staticCacheManager}", currencySettings, facebookConversionsHttpClient, categoryService, countryService, currencyService, genericAttributeService, httpContextAccessor, logger, orderService, orderTotalCalculationService, priceCalculationService, productService, facebookPixelConfigurationRepository, stateProvinceService, staticCacheManager, storeContext, taxService, webHelper, widgetPluginManager, workContext, _staticCacheManager);
        _storeContext = storeContext;
        _taxService = taxService;
		_logger.LogDebug("FacebookPixelService(currencySettings={CurrencySettings},facebookConversionsHttpClient={FacebookConversionsHttpClient},categoryService={CategoryService},countryService={CountryService},currencyService={CurrencyService},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},logger={Logger},orderService={OrderService},orderTotalCalculationService={OrderTotalCalculationService},priceCalculationService={PriceCalculationService},productService={ProductService},facebookPixelConfigurationRepository={FacebookPixelConfigurationRepository},shoppingCartService=<redacted>,stateProvinceService={StateProvinceService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},taxService={TaxService},webHelper={WebHelper},widgetPluginManager={WidgetPluginManager},workContext={WorkContext}): _storeContext → {_storeContext}", currencySettings, facebookConversionsHttpClient, categoryService, countryService, currencyService, genericAttributeService, httpContextAccessor, logger, orderService, orderTotalCalculationService, priceCalculationService, productService, facebookPixelConfigurationRepository, stateProvinceService, staticCacheManager, storeContext, taxService, webHelper, widgetPluginManager, workContext, _storeContext);
		_logger.LogDebug("FacebookPixelService(currencySettings={CurrencySettings},facebookConversionsHttpClient={FacebookConversionsHttpClient},categoryService={CategoryService},countryService={CountryService},currencyService={CurrencyService},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},logger={Logger},orderService={OrderService},orderTotalCalculationService={OrderTotalCalculationService},priceCalculationService={PriceCalculationService},productService={ProductService},facebookPixelConfigurationRepository={FacebookPixelConfigurationRepository},shoppingCartService=<redacted>,stateProvinceService={StateProvinceService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},taxService={TaxService},webHelper={WebHelper},widgetPluginManager={WidgetPluginManager},workContext={WorkContext}): _taxService → {_taxService}", currencySettings, facebookConversionsHttpClient, categoryService, countryService, currencyService, genericAttributeService, httpContextAccessor, logger, orderService, orderTotalCalculationService, priceCalculationService, productService, facebookPixelConfigurationRepository, stateProvinceService, staticCacheManager, storeContext, taxService, webHelper, widgetPluginManager, workContext, _taxService);
        _webHelper = webHelper;
        _widgetPluginManager = widgetPluginManager;
		_logger.LogDebug("FacebookPixelService(currencySettings={CurrencySettings},facebookConversionsHttpClient={FacebookConversionsHttpClient},categoryService={CategoryService},countryService={CountryService},currencyService={CurrencyService},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},logger={Logger},orderService={OrderService},orderTotalCalculationService={OrderTotalCalculationService},priceCalculationService={PriceCalculationService},productService={ProductService},facebookPixelConfigurationRepository={FacebookPixelConfigurationRepository},shoppingCartService=<redacted>,stateProvinceService={StateProvinceService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},taxService={TaxService},webHelper={WebHelper},widgetPluginManager={WidgetPluginManager},workContext={WorkContext}): _webHelper → {_webHelper}", currencySettings, facebookConversionsHttpClient, categoryService, countryService, currencyService, genericAttributeService, httpContextAccessor, logger, orderService, orderTotalCalculationService, priceCalculationService, productService, facebookPixelConfigurationRepository, stateProvinceService, staticCacheManager, storeContext, taxService, webHelper, widgetPluginManager, workContext, _webHelper);
        _workContext = workContext;
		_logger.LogWarning("<<< Exiting FacebookPixelService(currencySettings={CurrencySettings},facebookConversionsHttpClient={FacebookConversionsHttpClient},categoryService={CategoryService},countryService={CountryService},currencyService={CurrencyService},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},logger={Logger},orderService={OrderService},orderTotalCalculationService={OrderTotalCalculationService},priceCalculationService={PriceCalculationService},productService={ProductService},facebookPixelConfigurationRepository={FacebookPixelConfigurationRepository},shoppingCartService=<redacted>,stateProvinceService={StateProvinceService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},taxService={TaxService},webHelper={WebHelper},widgetPluginManager={WidgetPluginManager},workContext={WorkContext})", currencySettings, facebookConversionsHttpClient, categoryService, countryService, currencyService, genericAttributeService, httpContextAccessor, logger, orderService, orderTotalCalculationService, priceCalculationService, productService, facebookPixelConfigurationRepository, stateProvinceService, staticCacheManager, storeContext, taxService, webHelper, widgetPluginManager, workContext);
		_logger.LogDebug("FacebookPixelService(currencySettings={CurrencySettings},facebookConversionsHttpClient={FacebookConversionsHttpClient},categoryService={CategoryService},countryService={CountryService},currencyService={CurrencyService},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},logger={Logger},orderService={OrderService},orderTotalCalculationService={OrderTotalCalculationService},priceCalculationService={PriceCalculationService},productService={ProductService},facebookPixelConfigurationRepository={FacebookPixelConfigurationRepository},shoppingCartService=<redacted>,stateProvinceService={StateProvinceService},staticCacheManager={StaticCacheManager},storeContext={StoreContext},taxService={TaxService},webHelper={WebHelper},widgetPluginManager={WidgetPluginManager},workContext={WorkContext}): _workContext → {_workContext}", currencySettings, facebookConversionsHttpClient, categoryService, countryService, currencyService, genericAttributeService, httpContextAccessor, logger, orderService, orderTotalCalculationService, priceCalculationService, productService, facebookPixelConfigurationRepository, stateProvinceService, staticCacheManager, storeContext, taxService, webHelper, widgetPluginManager, workContext, _workContext);
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Handle function and get result
    /// </summary>
    /// <typeparam name="TResult">Result type</typeparam>
    /// <param name="function">Function</param>
    /// <param name="logErrors">Whether to log errors</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the function result
    /// </returns>
    protected async Task<TResult> HandleFunctionAsync<TResult>(Func<Task<TResult>> function, bool logErrors = true)
    {
		_logger.LogError(">>> Entering HandleFunctionAsync(function={Function},logErrors={LogErrors})", function, logErrors);
        try
        {
            //check whether the plugin is active
            if (!await PluginActiveAsync())
                return default;

            //invoke function
            return await function();
        }
        catch (Exception exception)
        {
			_logger.LogError("Exception in HandleFunctionAsync(function={Function},logErrors={LogErrors}): {Arg2}", function, logErrors, exception.Message);
            if (!logErrors)
                return default;

            var customer = await _workContext.GetCurrentCustomerAsync();
			_logger.LogError("HandleFunctionAsync(function={Function},logErrors={LogErrors}): customer → {Customer}", function, logErrors, customer);
            if (customer.IsSearchEngineAccount() || customer.IsBackgroundTaskAccount())
                return default;

            //log errors
            var error = $"{FacebookPixelDefaults.SystemName} error: {Environment.NewLine}{exception.Message}";
			_logger.LogError("HandleFunctionAsync(function={Function},logErrors={LogErrors}): error → {Error}", function, logErrors, error);
            await _logger.ErrorAsync(error, exception, customer);

			_logger.LogError(">>> Entering PluginActiveAsync()");
			_logger.LogWarning("<<< Exiting PluginActiveAsync()");
            return default;
		_logger.LogWarning("<<< Exiting HandleFunctionAsync(function={Function},logErrors={LogErrors})", function, logErrors);
        }
    }

    /// <summary>
    /// Check whether the plugin is active for the current user and the current store
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    protected async Task<bool> PluginActiveAsync()
    {
		_logger.LogError(">>> Entering PluginActiveAsync()");
        var customer = await _workContext.GetCurrentCustomerAsync();
        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogError("PluginActiveAsync(): store → {Store}", store);
		_logger.LogError("PluginActiveAsync(): customer → {Customer}", customer);
		_logger.LogWarning("<<< Exiting PluginActiveAsync()");
        return await _widgetPluginManager.IsPluginActiveAsync(FacebookPixelDefaults.SystemName, customer, store.Id);
    }

    /// <summary>
    /// Prepare scripts
    /// </summary>
    /// <param name="configurations">Enabled configurations</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the script code
    /// </returns>
    protected async Task<string> PrepareScriptsAsync(IList<FacebookPixelConfiguration> configurations)
    {
		_logger.LogError(">>> Entering PrepareScriptsAsync(configurations={Configurations})", configurations);
        _logger.LogWarning("<<< Exiting PrepareScriptsAsync(configurations={Configurations})", configurations);
        return await PrepareInitScriptAsync(configurations) +
               await PrepareUserPropertiesScriptAsync(configurations) +
               await PreparePageViewScriptAsync(configurations) +
               await PrepareTrackedEventsScriptAsync(configurations);
    }

    /// <summary>
    /// Prepare user info (used with Advanced Matching feature)
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the user info
    /// </returns>
    protected async Task<string> GetUserObjectAsync()
    {
		_logger.LogError(">>> Entering GetUserObjectAsync()");
        //prepare user object
        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogError("GetUserObjectAsync(): customer → {Customer}", customer);
        var email = customer.Email;
		_logger.LogError("GetUserObjectAsync(): email → {Email}", email);
        var firstName = customer.FirstName;
        var lastName = customer.LastName;
        var phone = customer.Phone;
        var gender = customer.Gender;
        var birthday = customer.DateOfBirth;
        var city = customer.City;
        var countryId = customer.CountryId;
        var countryName = (await _countryService.GetCountryByIdAsync(countryId))?.TwoLetterIsoCode;
		_logger.LogError("GetUserObjectAsync(): countryName → {CountryName}", countryName);
        var stateId = customer.StateProvinceId;
        var stateName = (await _stateProvinceService.GetStateProvinceByIdAsync(stateId))?.Abbreviation;
		_logger.LogError("GetUserObjectAsync(): stateName → {StateName}", stateName);
        var zipcode = customer.ZipPostalCode;

		_logger.LogWarning("<<< Exiting GetUserObjectAsync()");
        return FormatEventObject(
        [
            ("em", JavaScriptEncoder.Default.Encode(email?.ToLowerInvariant() ?? string.Empty)),
            ("fn", JavaScriptEncoder.Default.Encode(firstName?.ToLowerInvariant() ?? string.Empty)),
            ("ln", JavaScriptEncoder.Default.Encode(lastName?.ToLowerInvariant() ?? string.Empty)),
            ("ph", new string(phone?.Where(c => char.IsDigit(c)).ToArray()) ?? string.Empty),
            ("external_id", customer.CustomerGuid.ToString().ToLowerInvariant()),
            ("ge", gender?.FirstOrDefault().ToString().ToLowerInvariant()),
            ("db", birthday?.ToString("yyyyMMdd")),
            ("ct", JavaScriptEncoder.Default.Encode(city?.ToLowerInvariant() ?? string.Empty)),
            ("st", stateName?.ToLowerInvariant()),
            ("zp", JavaScriptEncoder.Default.Encode(zipcode?.ToLowerInvariant() ?? string.Empty)),
            ("cn", countryName?.ToLowerInvariant())
        ]);
    }

    /// <summary>
    /// Prepare script to init Facebook Pixel
    /// </summary>
    /// <param name="configurations">Enabled configurations</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the script code
    /// </returns>
    protected async Task<string> PrepareInitScriptAsync(IList<FacebookPixelConfiguration> configurations)
    {
		_logger.LogError(">>> Entering PrepareInitScriptAsync(configurations={Configurations})", configurations);
        //prepare init script
        return await FormatScriptAsync(configurations, async configuration =>
        {
            var customer = await _workContext.GetCurrentCustomerAsync();

            var additionalParameter = configuration.PassUserProperties
                ? $", {{uid: '{customer.CustomerGuid}'}}"
                : (configuration.UseAdvancedMatching
                    ? $", {await GetUserObjectAsync()}"
                    : null);
            return $"fbq('init', '{configuration.PixelId}'{additionalParameter});";
        });
    }

    /// <summary>
    /// Prepare script to pass user properties
    /// </summary>
    /// <param name="configurations">Enabled configurations</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the script code
    /// </returns>
    protected async Task<string> PrepareUserPropertiesScriptAsync(IList<FacebookPixelConfiguration> configurations)
    {
		_logger.LogError(">>> Entering PrepareUserPropertiesScriptAsync(configurations={Configurations})", configurations);
        //filter active configurations
        var activeConfigurations = configurations.Where(configuration => configuration.PassUserProperties).ToList();
		_logger.LogError("PrepareUserPropertiesScriptAsync(configurations={Configurations}): activeConfigurations → {ActiveConfigurations}", configurations, activeConfigurations);
        if (!activeConfigurations.Any())
            return string.Empty;

        //prepare user object
        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogError("PrepareUserPropertiesScriptAsync(configurations={Configurations}): customer → {Customer}", configurations, customer);
        var createdOn = new DateTimeOffset(customer.CreatedOnUtc).ToUnixTimeSeconds().ToString();
		_logger.LogError("PrepareUserPropertiesScriptAsync(configurations={Configurations}): createdOn → {CreatedOn}", configurations, createdOn);
        var city = customer.City;
        var countryId = customer.CountryId;
        var countryName = (await _countryService.GetCountryByIdAsync(countryId))?.TwoLetterIsoCode;
		_logger.LogError("PrepareUserPropertiesScriptAsync(configurations={Configurations}): countryName → {CountryName}", configurations, countryName);
        var currency = (await _workContext.GetWorkingCurrencyAsync())?.CurrencyCode;
		_logger.LogError("PrepareUserPropertiesScriptAsync(configurations={Configurations}): currency → {Currency}", configurations, currency);
        var gender = customer.Gender;
        var language = (await _workContext.GetWorkingLanguageAsync())?.UniqueSeoCode;
		_logger.LogError("PrepareUserPropertiesScriptAsync(configurations={Configurations}): language → {Language}", configurations, language);
        var stateId = customer.StateProvinceId;
        var stateName = (await _stateProvinceService.GetStateProvinceByIdAsync(stateId))?.Abbreviation;
		_logger.LogError("PrepareUserPropertiesScriptAsync(configurations={Configurations}): stateName → {StateName}", configurations, stateName);
        var zipcode = customer.ZipPostalCode;

        var userObject = FormatEventObject(
        [
            ("$account_created_time", createdOn),
            ("$city", JavaScriptEncoder.Default.Encode(city ?? string.Empty)),
            ("$country", countryName),
            ("$currency", currency),
            ("$gender", gender?.FirstOrDefault().ToString()),
            ("$language", language),
            ("$state", stateName),
            ("$zipcode", JavaScriptEncoder.Default.Encode(zipcode ?? string.Empty))
        ]);

        //prepare script
		_logger.LogWarning("<<< Exiting PrepareUserPropertiesScriptAsync(configurations={Configurations})", configurations);
        return await FormatScriptAsync(activeConfigurations, configuration =>
            Task.FromResult($"fbq('setUserProperties', '{configuration.PixelId}', {userObject});"));
    }

    /// <summary>
    /// Prepare script to track "PageView" event
    /// </summary>
    /// <param name="configurations">Enabled configurations</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the script code
    /// </returns>
    protected async Task<string> PreparePageViewScriptAsync(IList<FacebookPixelConfiguration> configurations)
    {
		_logger.LogError(">>> Entering PreparePageViewScriptAsync(configurations={Configurations})", configurations);
        //a single active configuration is enough to track PageView event
        var activeConfigurations = configurations.Where(configuration => configuration.TrackPageView).Take(1).ToList();
		_logger.LogError("PreparePageViewScriptAsync(configurations={Configurations}): activeConfigurations → {ActiveConfigurations}", configurations, activeConfigurations);
		_logger.LogWarning("<<< Exiting PreparePageViewScriptAsync(configurations={Configurations})", configurations);
        return await FormatScriptAsync(activeConfigurations, configuration =>
            Task.FromResult($"fbq('track', '{FacebookPixelDefaults.PAGE_VIEW}');"));
    }

    /// <summary>
    /// Prepare scripts to track events
    /// </summary>
    /// <param name="configurations">Enabled configurations</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the script code
    /// </returns>
    protected async Task<string> PrepareTrackedEventsScriptAsync(IList<FacebookPixelConfiguration> configurations)
    {
		_logger.LogError(">>> Entering PrepareTrackedEventsScriptAsync(configurations={Configurations})", configurations);
        //get previously stored events and remove them from the session data
        var events = (await _httpContextAccessor.HttpContext.Session
                         .GetAsync<IList<TrackedEvent>>(FacebookPixelDefaults.TrackedEventsSessionValue))
                     ?? new List<TrackedEvent>();
        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogError("PrepareTrackedEventsScriptAsync(configurations={Configurations}): store → {Store}", configurations, store);
        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogError("PrepareTrackedEventsScriptAsync(configurations={Configurations}): customer → {Customer}", configurations, customer);
        var activeEvents = events.Where(trackedEvent =>
                trackedEvent.CustomerId == customer.Id && trackedEvent.StoreId == store.Id)
            .ToList();
        await _httpContextAccessor.HttpContext.Session.SetAsync(
            FacebookPixelDefaults.TrackedEventsSessionValue,
            events.Except(activeEvents).ToList());

        if (!activeEvents.Any())
            return string.Empty;

        return await activeEvents.AggregateAwaitAsync(string.Empty, async (preparedScripts, trackedEvent) =>
        {
            //filter active configurations
            var activeConfigurations = trackedEvent.EventName switch
            {
                FacebookPixelDefaults.ADD_TO_CART => configurations.Where(configuration => configuration.TrackAddToCart).ToList(),
                FacebookPixelDefaults.PURCHASE => configurations.Where(configuration => configuration.TrackPurchase).ToList(),
                FacebookPixelDefaults.VIEW_CONTENT => configurations.Where(configuration => configuration.TrackViewContent).ToList(),
                FacebookPixelDefaults.ADD_TO_WISHLIST => configurations.Where(configuration => configuration.TrackAddToWishlist).ToList(),
                FacebookPixelDefaults.INITIATE_CHECKOUT => configurations.Where(configuration => configuration.TrackInitiateCheckout).ToList(),
                FacebookPixelDefaults.SEARCH => configurations.Where(configuration => configuration.TrackSearch).ToList(),
                FacebookPixelDefaults.CONTACT => configurations.Where(configuration => configuration.TrackContact).ToList(),
                FacebookPixelDefaults.COMPLETE_REGISTRATION => configurations.Where(configuration => configuration.TrackCompleteRegistration).ToList(),
                _ => new List<FacebookPixelConfiguration>()
            };
            if (trackedEvent.IsCustomEvent)
            {
				var __logStart = System.Diagnostics.Stopwatch.StartNew();
                activeConfigurations = await configurations.WhereAwait(async configuration =>
                    (await GetCustomEventsAsync(configuration.Id)).Any(customEvent => customEvent.EventName == trackedEvent.EventName)).ToListAsync();
					_logger.LogError("PrepareTrackedEventsScriptAsync(configurations)={Configurations}: database query executed in {Arg1} ms", configurations, __logStart.ElapsedMilliseconds);
            }

            //prepare event scripts
            return preparedScripts + await trackedEvent.EventObjects.AggregateAwaitAsync(string.Empty, async (preparedEventScripts, eventObject) =>
            {
                return preparedEventScripts + await FormatScriptAsync(activeConfigurations, configuration =>
                {
                    //used for accurate event tracking with multiple Facebook Pixels
                    var actionName = configurations.Count > 1
                        ? (trackedEvent.IsCustomEvent ? "trackSingleCustom" : "trackSingle")
                        : (trackedEvent.IsCustomEvent ? "trackCustom" : "track");
                    var additionalParameter = configurations.Count > 1 ? $", '{configuration.PixelId}'" : null;

                    //prepare event script
                    var eventObjectParameter = !string.IsNullOrEmpty(eventObject) ? $", {eventObject}" : null;
                    return Task.FromResult($"fbq('{actionName}'{additionalParameter}, '{trackedEvent.EventName}'{eventObjectParameter});");
                });
            });
        });
    }

    /// <summary>
    /// Prepare script to track event and store it for the further using
    /// </summary>
    /// <param name="eventName">Event name</param>
    /// <param name="eventObject">Event object</param>
    /// <param name="customerId">Customer identifier</param>
    /// <param name="storeId">Store identifier</param>
    /// <param name="isCustomEvent">Whether the event is a custom one</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected async Task PrepareTrackedEventScriptAsync(string eventName, string eventObject,
        int? customerId = null, int? storeId = null, bool isCustomEvent = false)
    {
		_logger.LogError(">>> Entering PrepareTrackedEventScriptAsync(eventName={EventName},eventObject={EventObject},customerId={CustomerId},storeId={StoreId},isCustomEvent={IsCustomEvent})", eventName, eventObject, customerId, storeId, isCustomEvent);
        //prepare script and store it into the session data, we use this later
        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogError("PrepareTrackedEventScriptAsync(eventName={EventName},eventObject={EventObject},customerId={CustomerId},storeId={StoreId},isCustomEvent={IsCustomEvent}): customer → {Customer}", eventName, eventObject, customerId, storeId, isCustomEvent, customer);
        customerId ??= customer.Id;
        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogError("PrepareTrackedEventScriptAsync(eventName={EventName},eventObject={EventObject},customerId={CustomerId},storeId={StoreId},isCustomEvent={IsCustomEvent}): store → {Store}", eventName, eventObject, customerId, storeId, isCustomEvent, store);
        storeId ??= store.Id;
        var events = await _httpContextAccessor.HttpContext.Session
                         .GetAsync<IList<TrackedEvent>>(FacebookPixelDefaults.TrackedEventsSessionValue)
                     ?? new List<TrackedEvent>();
        var activeEvent = events.FirstOrDefault(trackedEvent =>
            trackedEvent.EventName == eventName && trackedEvent.CustomerId == customerId && trackedEvent.StoreId == storeId);
        if (activeEvent == null)
        {
            activeEvent = new TrackedEvent
            {
                EventName = eventName,
                CustomerId = customerId.Value,
                StoreId = storeId.Value,
                IsCustomEvent = isCustomEvent
            };
            events.Add(activeEvent);
        }
        activeEvent.EventObjects.Add(eventObject);
        await _httpContextAccessor.HttpContext.Session.SetAsync(FacebookPixelDefaults.TrackedEventsSessionValue, events);
		_logger.LogWarning("<<< Exiting PrepareTrackedEventScriptAsync(eventName={EventName},eventObject={EventObject},customerId={CustomerId},storeId={StoreId},isCustomEvent={IsCustomEvent})", eventName, eventObject, customerId, storeId, isCustomEvent);
    }

    /// <summary>
    /// Format event object to look pretty
    /// </summary>
    /// <param name="properties">Event object properties</param>
    /// <param name="tabsNumber">Tabs number for indentation script</param>
    /// <returns>Script code</returns>
    protected string FormatEventObject(List<(string Name, object Value)> properties, int? tabsNumber = null)
    {
		_logger.LogError(">>> Entering FormatEventObject(properties={Properties},tabsNumber={TabsNumber})", properties, tabsNumber);
        //local function to format list of objects
        string formatObjectList(List<List<(string Name, object Value)>> objectList)
        {
            var formattedList = objectList.Aggregate(string.Empty, (preparedObjects, propertiesList) =>
            {
                if (propertiesList != null)
                {
                    var value = FormatEventObject(propertiesList, (tabsNumber ?? TABS_NUMBER) + 1);
					_logger.LogError("FormatEventObject(properties={Properties},tabsNumber={TabsNumber}): value → {Value}", properties, tabsNumber, value);
                    preparedObjects += $"{Environment.NewLine}{new string('\t', (tabsNumber ?? TABS_NUMBER) + 1)}{value},";
                }

                return preparedObjects;
            }).TrimEnd(',');
            return $"[{formattedList}]";
        }

        //format single object
        var formattedObject = properties.Aggregate(string.Empty, (preparedObject, property) =>
        {
            if (!string.IsNullOrEmpty(property.Value?.ToString()))
            {
                //format property value
                var value = property.Value is string valueString
                    ? $"'{valueString.Replace("'", "\\'")}'"
                    : (property.Value is List<List<(string Name, object Value)>> valueList
                        ? formatObjectList(valueList)
                        : (property.Value is decimal valueDecimal
                            ? valueDecimal.ToString("F", CultureInfo.InvariantCulture)
                            : property.Value.ToString().ToLowerInvariant()));

                //format object property
                preparedObject += $"{Environment.NewLine}{new string('\t', (tabsNumber ?? TABS_NUMBER) + 1)}{property.Name}: {value},";
            }

            return preparedObject;
        }).TrimEnd(',');

        return $"{{{formattedObject}{Environment.NewLine}{new string('\t', tabsNumber ?? TABS_NUMBER)}}}";
    }

    /// <summary>
    /// Format script to look pretty
    /// </summary>
    /// <param name="configurations">Enabled configurations</param>
    /// <param name="getScript">Function to get script for the passed configuration</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the script code
    /// </returns>
    protected async Task<string> FormatScriptAsync(IList<FacebookPixelConfiguration> configurations, Func<FacebookPixelConfiguration, Task<string>> getScript)
    {
		_logger.LogError(">>> Entering FormatScriptAsync(configurations={Configurations},getScript={GetScript})", configurations, getScript);
        if (!configurations.Any())
            return string.Empty;

        //format script
        var formattedScript = await configurations.AggregateAwaitAsync(string.Empty, async (preparedScripts, configuration) =>
            preparedScripts + Environment.NewLine + new string('\t', TABS_NUMBER) + await getScript(configuration));
        formattedScript += Environment.NewLine;

		_logger.LogWarning("<<< Exiting FormatScriptAsync(configurations={Configurations},getScript={GetScript})", configurations, getScript);
        return formattedScript;
    }

    /// <summary>
    /// Format custom event data to look pretty
    /// </summary>
    /// <param name="customData">Custom data</param>
    /// <returns>Script code</returns>
    protected string FormatCustomData(ConversionsEventCustomData customData)
    {
		_logger.LogError(">>> Entering FormatCustomData(customData={CustomData})", customData);
        List<(string Name, object Value)> getProperties(JObject jObject)
        {
            var result = jObject.ToObject<Dictionary<string, object>>();
			_logger.LogError("FormatCustomData(customData={CustomData}): result → {Result}", customData, result);
            foreach (var pair in result)
            {
                if (pair.Value is JObject nestedObject)
                    result[pair.Key] = getProperties(nestedObject);
                if (pair.Value is JArray nestedArray && nestedArray.OfType<JObject>().Any())
                    result[pair.Key] = nestedArray.OfType<JObject>().Select(obj => getProperties(obj)).ToList();
            }

            return result.Select(pair => (pair.Key, pair.Value)).ToList();
        }

        try
        {
            var customDataObject = JObject.FromObject(customData, new JsonSerializer { NullValueHandling = NullValueHandling.Ignore });
			_logger.LogError("FormatCustomData(customData={CustomData}): customDataObject → {CustomDataObject}", customData, customDataObject);

            return FormatEventObject(getProperties(customDataObject));

        }
        catch
        {
			_logger.LogError("Exception in FormatCustomData(customData={CustomData}): {Arg1}", customData, exception.Message);
            //if something went wrong, just serialize the data without format
            return JsonConvert.SerializeObject(customData, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        }
    }

    /// <summary>
    /// Get configurations
    /// </summary>
    /// <param name="storeId">Store identifier; pass 0 to load all records</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of configurations
    /// </returns>
    protected async Task<IList<FacebookPixelConfiguration>> GetConfigurationsAsync(int storeId = 0)
    {
		_logger.LogError(">>> Entering GetConfigurationsAsync(storeId={StoreId})", storeId);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var key = _staticCacheManager.PrepareKeyForDefaultCache(FacebookPixelDefaults.ConfigurationsCacheKey, storeId);

		_logger.LogError("GetConfigurationsAsync(storeId={StoreId}): key → {Key}", storeId, key);
        var query = _facebookPixelConfigurationRepository.Table;
		_logger.LogError("GetConfigurationsAsync(storeId={StoreId}): query → {Query}", storeId, query);

        //filter by the store
        if (storeId > 0)
            query = query.Where(configuration => configuration.StoreId == storeId);

        query = query.OrderBy(configuration => configuration.Id);

		_logger.LogError("GetConfigurationsAsync(storeId)={StoreId}: find query executing", storeId);
		_logger.LogError("GetConfigurationsAsync(storeId)={StoreId}: external service call took {Arg1} ms", storeId, __logStart.ElapsedMilliseconds);
        return await _staticCacheManager.GetAsync(key, async () => await query.ToListAsync());
    }

    /// <summary>
    /// Prepare Pixel script and send requests to Conversions API for the passed event
    /// </summary>
    /// <param name="prepareModel">Function to prepare model</param>
    /// <param name="eventName">Event name</param>
    /// <param name="storeId">Store identifier; pass null to load records for the current store</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the value whether handling was successful
    /// </returns>
    protected async Task<bool> HandleEventAsync(Func<Task<ConversionsEvent>> prepareModel, string eventName, int? storeId = null)
    {
		_logger.LogError(">>> Entering HandleEventAsync(prepareModel={PrepareModel},eventName={EventName},storeId={StoreId})", prepareModel, eventName, storeId);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        storeId ??= (await _storeContext.GetCurrentStoreAsync()).Id;
        var configurations = (await GetConfigurationsAsync(storeId ?? 0)).Where(configuration => eventName switch
        {
            FacebookPixelDefaults.ADD_TO_CART => configuration.TrackAddToCart,
            FacebookPixelDefaults.ADD_TO_WISHLIST => configuration.TrackAddToWishlist,
            FacebookPixelDefaults.PURCHASE => configuration.TrackPurchase,
            FacebookPixelDefaults.VIEW_CONTENT => configuration.TrackViewContent,
            FacebookPixelDefaults.INITIATE_CHECKOUT => configuration.TrackInitiateCheckout,
            FacebookPixelDefaults.PAGE_VIEW => configuration.TrackPageView,
            FacebookPixelDefaults.SEARCH => configuration.TrackSearch,
            FacebookPixelDefaults.CONTACT => configuration.TrackContact,
            FacebookPixelDefaults.COMPLETE_REGISTRATION => configuration.TrackCompleteRegistration,
            _ => false
        }).ToList();

        var conversionsApiConfigurations = configurations.Where(configuration => configuration.ConversionsApiEnabled).ToList();
		_logger.LogError("HandleEventAsync(prepareModel={PrepareModel},eventName={EventName},storeId={StoreId}): conversionsApiConfigurations → {ConversionsApiConfigurations}", prepareModel, eventName, storeId, conversionsApiConfigurations);
        var pixelConfigurations = configurations.Where(configuration => configuration.PixelScriptEnabled).ToList();
		_logger.LogError("HandleEventAsync(prepareModel={PrepareModel},eventName={EventName},storeId={StoreId}): pixelConfigurations → {PixelConfigurations}", prepareModel, eventName, storeId, pixelConfigurations);
        if (!conversionsApiConfigurations.Any() && !pixelConfigurations.Any())
            return false;

        var model = await prepareModel();
		_logger.LogError("HandleEventAsync(prepareModel={PrepareModel},eventName={EventName},storeId={StoreId}): model → {Model}", prepareModel, eventName, storeId, model);

        if (pixelConfigurations.Any())
            await PrepareEventScriptAsync(model);

        var logErrors = true; //set it to false to ignore Conversions API errors 
        foreach (var configuration in conversionsApiConfigurations)
        {
            await HandleFunctionAsync(async () =>
            {
                var response = await _facebookConversionsHttpClient.SendEventAsync(configuration, model);
				_logger.LogError("HandleEventAsync(prepareModel={PrepareModel},eventName={EventName},storeId={StoreId}): response → {Response}", prepareModel, eventName, storeId, response);
                var error = JsonConvert.DeserializeAnonymousType(response, new { Error = new ApiError() })?.Error;
				_logger.LogError("HandleEventAsync(prepareModel={PrepareModel},eventName={EventName},storeId={StoreId}): error → {Error}", prepareModel, eventName, storeId, error);
                if (!string.IsNullOrEmpty(error?.Message))
					_logger.LogError("Throwing NopException from HandleEventAsync(prepareModel={PrepareModel},eventName={EventName},storeId={StoreId})", prepareModel, eventName, storeId);
                    throw new NopException($"{error.Code} - {error.Message}{Environment.NewLine}Debug ID: {error.DebugId}");

                return true;
            }, logErrors);
        }

		_logger.LogError("HandleEventAsync(prepareModel,eventName,storeId)={PrepareModel},{EventName},{StoreId}: external service call took {Arg3} ms", prepareModel, eventName, storeId, __logStart.ElapsedMilliseconds);
		_logger.LogWarning("<<< Exiting HandleEventAsync(prepareModel={PrepareModel},eventName={EventName},storeId={StoreId})", prepareModel, eventName, storeId);
        return true;
    }

    /// <summary>
    /// Prepare user data for conversions api
    /// </summary>
    /// <returns>
    /// <param name="customer">Customer</param>
    /// A task that represents the asynchronous operation
    /// The task result contains the user data
    /// </returns>
    protected async Task<ConversionsEventUserData> PrepareUserDataAsync(Customer customer = null)
    {
		_logger.LogError(">>> Entering PrepareUserDataAsync(customer={Customer})", customer);
        //prepare user object
        customer ??= await _workContext.GetCurrentCustomerAsync();
        var twoLetterCountryIsoCode = (await _countryService.GetCountryByIdAsync(customer.CountryId))?.TwoLetterIsoCode;
		_logger.LogError("PrepareUserDataAsync(customer={Customer}): twoLetterCountryIsoCode → {TwoLetterCountryIsoCode}", customer, twoLetterCountryIsoCode);
        var stateName = (await _stateProvinceService.GetStateProvinceByIdAsync(customer.StateProvinceId))?.Abbreviation;
		_logger.LogError("PrepareUserDataAsync(customer={Customer}): stateName → {StateName}", customer, stateName);
        var ipAddress = _webHelper.GetCurrentIpAddress();
		_logger.LogError("PrepareUserDataAsync(customer={Customer}): ipAddress → {IpAddress}", customer, ipAddress);
        var userAgent = _httpContextAccessor.HttpContext?.Request?.Headers[HeaderNames.UserAgent].ToString();
		_logger.LogError("PrepareUserDataAsync(customer={Customer}): userAgent → {UserAgent}", customer, userAgent);

		_logger.LogWarning("<<< Exiting PrepareUserDataAsync(customer={Customer})", customer);
        return new ConversionsEventUserData
        {
            EmailAddress = [HashHelper.CreateHash(Encoding.UTF8.GetBytes(customer.Email?.ToLowerInvariant() ?? string.Empty), "SHA256")],
            FirstName = [HashHelper.CreateHash(Encoding.UTF8.GetBytes(customer.FirstName?.ToLowerInvariant() ?? string.Empty), "SHA256")],
            LastName = [HashHelper.CreateHash(Encoding.UTF8.GetBytes(customer.LastName?.ToLowerInvariant() ?? string.Empty), "SHA256")],
            PhoneNumber = [HashHelper.CreateHash(Encoding.UTF8.GetBytes(customer.Phone?.ToLowerInvariant() ?? string.Empty), "SHA256")],
            ExternalId = [HashHelper.CreateHash(Encoding.UTF8.GetBytes(customer?.CustomerGuid.ToString()?.ToLowerInvariant() ?? string.Empty), "SHA256")],
            Gender = [HashHelper.CreateHash(Encoding.UTF8.GetBytes(customer.Gender?.FirstOrDefault().ToString() ?? string.Empty), "SHA256")],
            DateOfBirth = [HashHelper.CreateHash(Encoding.UTF8.GetBytes(customer.DateOfBirth?.ToString("yyyyMMdd") ?? string.Empty), "SHA256")],
            City = [HashHelper.CreateHash(Encoding.UTF8.GetBytes(customer.City?.ToLowerInvariant() ?? string.Empty), "SHA256")],
            State = [HashHelper.CreateHash(Encoding.UTF8.GetBytes(stateName?.ToLowerInvariant() ?? string.Empty), "SHA256")],
            Zip = [HashHelper.CreateHash(Encoding.UTF8.GetBytes(customer.ZipPostalCode?.ToLowerInvariant() ?? string.Empty), "SHA256")],
            Country = [HashHelper.CreateHash(Encoding.UTF8.GetBytes(twoLetterCountryIsoCode?.ToLowerInvariant() ?? string.Empty), "SHA256")],
            ClientIpAddress = ipAddress?.ToLowerInvariant(),
            ClientUserAgent = userAgent?.ToLowerInvariant(),
            Id = customer.Id
        };
    }

    /// <summary>
    /// Prepare add to cart event model
    /// </summary>
    /// <param name="item">Shopping cart item</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the ConversionsEvent model
    /// </returns>
    protected async Task<ConversionsEvent> PrepareAddToCartEventModelAsync(ShoppingCartItem item)
    {
		_logger.LogError(">>> Entering PrepareAddToCartEventModelAsync(item={Item})", item);
        ArgumentNullException.ThrowIfNull(item);

        //check whether the shopping was initiated by the customer
        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogError("PrepareAddToCartEventModelAsync(item={Item}): customer → {Customer}", item, customer);

        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogError("PrepareAddToCartEventModelAsync(item={Item}): store → {Store}", item, store);

        if (item.CustomerId != customer.Id)
			_logger.LogError("Throwing NopException from PrepareAddToCartEventModelAsync(item={Item})", item);
            throw new NopException("Shopping was not initiated by customer");

        var eventName = item.ShoppingCartTypeId == (int)ShoppingCartType.ShoppingCart
            ? FacebookPixelDefaults.ADD_TO_CART
            : FacebookPixelDefaults.ADD_TO_WISHLIST;

        var product = await _productService.GetProductByIdAsync(item.ProductId);
		_logger.LogError("PrepareAddToCartEventModelAsync(item={Item}): product → {Product}", item, product);
        var categoryMapping = (await _categoryService.GetProductCategoriesByProductIdAsync(product?.Id ?? 0)).FirstOrDefault();
		_logger.LogError("PrepareAddToCartEventModelAsync(item={Item}): categoryMapping → {CategoryMapping}", item, categoryMapping);
        var categoryName = (await _categoryService.GetCategoryByIdAsync(categoryMapping?.CategoryId ?? 0))?.Name;
		_logger.LogError("PrepareAddToCartEventModelAsync(item={Item}): categoryName → {CategoryName}", item, categoryName);
        var sku = product != null ? await _productService.FormatSkuAsync(product, item.AttributesXml) : string.Empty;
		_logger.LogError("PrepareAddToCartEventModelAsync(item={Item}): sku → {Sku}", item, sku);
        var quantity = product != null ? (int?)item.Quantity : null;
		_logger.LogError("PrepareAddToCartEventModelAsync(item={Item}): quantity → {Quantity}", item, quantity);
        var (productPrice, _, _, _) = await _priceCalculationService.GetFinalPriceAsync(product, customer, store, includeDiscounts: false);
        var (price, _) = await _taxService.GetProductPriceAsync(product, productPrice);
        var currentCurrency = await _workContext.GetWorkingCurrencyAsync();
		_logger.LogError("PrepareAddToCartEventModelAsync(item={Item}): currentCurrency → {CurrentCurrency}", item, currentCurrency);
        var priceValue = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(price, currentCurrency);
		_logger.LogError("PrepareAddToCartEventModelAsync(item={Item}): priceValue → {PriceValue}", item, priceValue);
        var currency = currentCurrency?.CurrencyCode;

        var eventObject = new ConversionsEventCustomData
                {
                    ContentCategory = categoryName,
                    ContentIds = [sku],
                    ContentName = product?.Name,
                    ContentType = "product",
                    Contents =
                    [
                    new
                    {
                    id = sku,
                    quantity = quantity,
                    item_price = priceValue
                }
                ],
            Currency = currency,
            Value = priceValue
        };

		_logger.LogWarning("<<< Exiting PrepareAddToCartEventModelAsync(item={Item})", item);
        return new ConversionsEvent
            {
                Data =
                [
                new ConversionsEventDatum
                {
                EventName = eventName,
                EventTime = new DateTimeOffset(item.CreatedOnUtc).ToUnixTimeSeconds(),
                EventSourceUrl = _webHelper.GetThisPageUrl(true),
                ActionSource = "website",
                UserData = await PrepareUserDataAsync(customer),
                CustomData = eventObject,
                StoreId = item.StoreId
            }
            ]
        };
    }

    /// <summary>
    /// Prepare purchase event model
    /// </summary>
    /// <param name="order">Order</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the ConversionsEvent model
    /// </returns>
    protected async Task<ConversionsEvent> PreparePurchaseModelAsync(Order order)
    {
		_logger.LogError(">>> Entering PreparePurchaseModelAsync(order={Order})", order);
        ArgumentNullException.ThrowIfNull(order);

        //check whether the purchase was initiated by the customer
        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogError("PreparePurchaseModelAsync(order={Order}): customer → {Customer}", order, customer);
        if (order.CustomerId != customer.Id)
			_logger.LogError("Throwing NopException from PreparePurchaseModelAsync(order={Order})", order);
            throw new NopException("Purchase was not initiated by customer");

        //prepare event object
        var currency = await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var contentsProperties = await (await _orderService.GetOrderItemsAsync(order.Id)).SelectAwait(async item =>
        {
			_logger.LogError("PreparePurchaseModelAsync(order={Order}): currency → {Currency}", order, currency);
            var product = await _productService.GetProductByIdAsync(item.ProductId);
			_logger.LogError("PreparePurchaseModelAsync(order={Order}): product → {Product}", order, product);
            var sku = product != null ? await _productService.FormatSkuAsync(product, item.AttributesXml) : string.Empty;
			_logger.LogError("PreparePurchaseModelAsync(order={Order}): sku → {Sku}", order, sku);
            var quantity = product != null ? (int?)item.Quantity : null;
			_logger.LogError("PreparePurchaseModelAsync(order={Order}): quantity → {Quantity}", order, quantity);
            return new { id = sku, quantity = quantity };
        }).Cast<object>().ToListAsync();
		_logger.LogError("PreparePurchaseModelAsync(order)={Order}: database query executed in {Arg1} ms", order, __logStart.ElapsedMilliseconds);
        var eventObject = new ConversionsEventCustomData
        {
            ContentType = "product",
            Contents = contentsProperties,
            Currency = currency?.CurrencyCode,
            Value = order.OrderTotal
        };

		_logger.LogWarning("<<< Exiting PreparePurchaseModelAsync(order={Order})", order);
        return new ConversionsEvent
            {
                Data =
                [
                new ConversionsEventDatum
                {
                EventName = FacebookPixelDefaults.PURCHASE,
                EventTime = new DateTimeOffset(order.CreatedOnUtc).ToUnixTimeSeconds(),
                EventSourceUrl = _webHelper.GetThisPageUrl(true),
                ActionSource = "website",
                UserData = await PrepareUserDataAsync(customer),
                CustomData = eventObject,
                StoreId = order.StoreId
            }
            ]
        };
    }

    /// <summary>
    /// Prepare view content event model
    /// </summary>
    /// <param name="productDetails">Product details model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the ConversionsEvent model
    /// </returns>
    protected async Task<ConversionsEvent> PrepareViewContentModelAsync(ProductDetailsModel productDetails)
    {
		_logger.LogError(">>> Entering PrepareViewContentModelAsync(productDetails={ProductDetails})", productDetails);
        ArgumentNullException.ThrowIfNull(productDetails);

        //prepare event object
        var product = await _productService.GetProductByIdAsync(productDetails.Id);
		_logger.LogError("PrepareViewContentModelAsync(productDetails={ProductDetails}): product → {Product}", productDetails, product);
        var categoryMapping = (await _categoryService.GetProductCategoriesByProductIdAsync(product?.Id ?? 0)).FirstOrDefault();
		_logger.LogError("PrepareViewContentModelAsync(productDetails={ProductDetails}): categoryMapping → {CategoryMapping}", productDetails, categoryMapping);
        var categoryName = (await _categoryService.GetCategoryByIdAsync(categoryMapping?.CategoryId ?? 0))?.Name;
		_logger.LogError("PrepareViewContentModelAsync(productDetails={ProductDetails}): categoryName → {CategoryName}", productDetails, categoryName);
        var sku = productDetails.Sku;
        var priceValue = productDetails.ProductPrice.PriceValue;
        var currency = (await _workContext.GetWorkingCurrencyAsync())?.CurrencyCode;
		_logger.LogError("PrepareViewContentModelAsync(productDetails={ProductDetails}): currency → {Currency}", productDetails, currency);

        var eventObject = new ConversionsEventCustomData
        {
            ContentCategory = categoryName,
            ContentIds = [sku],
            ContentName = product?.Name,
            ContentType = "product",
            Currency = currency,
            Value = priceValue
        };

		_logger.LogWarning("<<< Exiting PrepareViewContentModelAsync(productDetails={ProductDetails})", productDetails);
        return new ConversionsEvent
            {
                Data =
                [
                new ConversionsEventDatum
                {
                EventName = FacebookPixelDefaults.VIEW_CONTENT,
                EventTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(),
                EventSourceUrl = _webHelper.GetThisPageUrl(true),
                ActionSource = "website",
                UserData = await PrepareUserDataAsync(),
                CustomData = eventObject
            }
            ]
        };
    }

    /// <summary>
    /// Prepare initiate checkout event model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the ConversionsEvent model
    /// </returns>
    protected async Task<ConversionsEvent> PrepareInitiateCheckoutModelAsync()
    {
		_logger.LogError(">>> Entering PrepareInitiateCheckoutModelAsync()");
        //prepare event object
        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogError("PrepareInitiateCheckoutModelAsync(): customer → {Customer}", customer);
        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogError("PrepareInitiateCheckoutModelAsync(): store → {Store}", store);
        var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);
		_logger.LogError("PrepareInitiateCheckoutModelAsync(): cart → {Cart}", cart);
        var (price, _, _, _, _, _) = await _orderTotalCalculationService.GetShoppingCartTotalAsync(cart, false, false);
        var currentCurrency = await _workContext.GetWorkingCurrencyAsync();
		_logger.LogError("PrepareInitiateCheckoutModelAsync(): currentCurrency → {CurrentCurrency}", currentCurrency);
        var priceValue = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(price ?? 0, currentCurrency);
		_logger.LogError("PrepareInitiateCheckoutModelAsync(): priceValue → {PriceValue}", priceValue);
        var currency = currentCurrency?.CurrencyCode;

		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var contentsProperties = await cart.SelectAwait(async item =>
        {
            var product = await _productService.GetProductByIdAsync(item.ProductId);
			_logger.LogError("PrepareInitiateCheckoutModelAsync(): product → {Product}", product);
            var sku = product != null ? await _productService.FormatSkuAsync(product, item.AttributesXml) : string.Empty;
			_logger.LogError("PrepareInitiateCheckoutModelAsync(): sku → {Sku}", sku);
            var quantity = product != null ? (int?)item.Quantity : null;
			_logger.LogError("PrepareInitiateCheckoutModelAsync(): quantity → {Quantity}", quantity);
            return new { id = sku, quantity = quantity };
        }).Cast<object>().ToListAsync();

		_logger.LogError("PrepareInitiateCheckoutModelAsync(): database query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);
        var eventObject = new ConversionsEventCustomData
        {
            ContentType = "product",
            Contents = contentsProperties,
            Currency = currency,
            Value = priceValue
        };

		_logger.LogWarning("<<< Exiting PrepareInitiateCheckoutModelAsync()");
        return new ConversionsEvent
            {
                Data =
                [
                new ConversionsEventDatum
                {
                EventName = FacebookPixelDefaults.INITIATE_CHECKOUT,
                EventTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(),
                EventSourceUrl = _webHelper.GetThisPageUrl(true),
                ActionSource = "website",
                UserData = await PrepareUserDataAsync(customer),
                CustomData = eventObject
            }
            ]
        };
    }

    /// <summary>
    /// Prepare page view event model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the ConversionsEvent model
    /// </returns>
    protected async Task<ConversionsEvent> PreparePageViewModelAsync()
    {
		_logger.LogError(">>> Entering PreparePageViewModelAsync()");
		_logger.LogWarning("<<< Exiting PreparePageViewModelAsync()");
        return new ConversionsEvent
            {
                Data =
                [
                new ConversionsEventDatum
                {
                EventName = FacebookPixelDefaults.PAGE_VIEW,
                EventTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(),
                EventSourceUrl = _webHelper.GetThisPageUrl(true),
                ActionSource = "website",
                UserData = await PrepareUserDataAsync(),
                CustomData = new ConversionsEventCustomData()
            }
            ]
        };
    }

    /// <summary>
    /// Prepare search event model
    /// </summary>
    /// <param name="searchTerm">Search term</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the ConversionsEvent model
    /// </returns>
    protected async Task<ConversionsEvent> PrepareSearchModelAsync(string searchTerm)
    {
		_logger.LogError(">>> Entering PrepareSearchModelAsync(searchTerm={SearchTerm})", searchTerm);
        //prepare event object
        var eventObject = new ConversionsEventCustomData
        {
            SearchString = JavaScriptEncoder.Default.Encode(searchTerm)
        };

		_logger.LogWarning("<<< Exiting PrepareSearchModelAsync(searchTerm={SearchTerm})", searchTerm);
        return new ConversionsEvent
            {
                Data =
                [
                new ConversionsEventDatum
                {
                EventName = FacebookPixelDefaults.SEARCH,
                EventTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(),
                EventSourceUrl = _webHelper.GetThisPageUrl(true),
                ActionSource = "website",
                UserData = await PrepareUserDataAsync(),
                CustomData = eventObject
            }
            ]
        };
    }

    /// <summary>
    /// Prepare contact event model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the ConversionsEvent model
    /// </returns>
    protected async Task<ConversionsEvent> PrepareContactModelAsync()
    {
		_logger.LogError(">>> Entering PrepareContactModelAsync()");
		_logger.LogWarning("<<< Exiting PrepareContactModelAsync()");
        return new ConversionsEvent
            {
                Data =
                [
                new ConversionsEventDatum
                {
                EventName = FacebookPixelDefaults.CONTACT,
                EventTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(),
                EventSourceUrl = _webHelper.GetThisPageUrl(true),
                ActionSource = "website",
                UserData = await PrepareUserDataAsync(),
                CustomData = new ConversionsEventCustomData()
            }
            ]
        };
    }

    /// <summary>
    /// Prepare complete registration event model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the ConversionsEvent model
    /// </returns>
    protected async Task<ConversionsEvent> PrepareCompleteRegistrationModelAsync()
    {
		_logger.LogError(">>> Entering PrepareCompleteRegistrationModelAsync()");
        //prepare event object
        var eventObject = new ConversionsEventCustomData
        {
            Status = true.ToString()
        };

		_logger.LogWarning("<<< Exiting PrepareCompleteRegistrationModelAsync()");
        return new ConversionsEvent
            {
                Data =
                [
                new ConversionsEventDatum
                {
                EventName = FacebookPixelDefaults.COMPLETE_REGISTRATION,
                EventTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(),
                EventSourceUrl = _webHelper.GetThisPageUrl(true),
                ActionSource = "website",
                UserData = await PrepareUserDataAsync(),
                CustomData = eventObject
            }
            ]
        };
    }

    #endregion

    #region Methods

    #region Scripts

    /// <summary>
    /// Prepare script to track events and store it into the session value
    /// </summary>
    /// <param name="conversionsEvent">Conversions event</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task PrepareEventScriptAsync(ConversionsEvent conversionsEvent)
    {
		_logger.LogError(">>> Entering PrepareEventScriptAsync(conversionsEvent={ConversionsEvent})", conversionsEvent);
        await HandleFunctionAsync(async () =>
        {
            //get current stored events 
            var events = await _httpContextAccessor.HttpContext.Session
                             .GetAsync<IList<TrackedEvent>>(FacebookPixelDefaults.TrackedEventsSessionValue)
                         ?? new List<TrackedEvent>();

            var store = await _storeContext.GetCurrentStoreAsync();
			_logger.LogError("PrepareEventScriptAsync(conversionsEvent={ConversionsEvent}): store → {Store}", conversionsEvent, store);
            foreach (var conversionsEventData in conversionsEvent.Data)
            {
                conversionsEventData.StoreId ??= store.Id;
                var activeEvent = events.FirstOrDefault(trackedEvent =>
                    trackedEvent.EventName == conversionsEventData.EventName &&
                    trackedEvent.CustomerId == conversionsEventData.UserData?.Id &&
                    trackedEvent.StoreId == conversionsEventData.StoreId);
                if (activeEvent is null)
                {
                    activeEvent = new TrackedEvent
                    {
                        EventName = conversionsEventData.EventName,
                        CustomerId = conversionsEventData.UserData?.Id ?? 0,
                        StoreId = conversionsEventData.StoreId ?? 0,
                        IsCustomEvent = conversionsEventData.IsCustomEvent
                    };
                    events.Add(activeEvent);
                }

                activeEvent.EventObjects.Add(FormatCustomData(conversionsEventData.CustomData));
            }

            //update events in the session value
            await _httpContextAccessor.HttpContext.Session.SetAsync(FacebookPixelDefaults.TrackedEventsSessionValue, events);

		_logger.LogWarning("<<< Exiting PrepareEventScriptAsync(conversionsEvent={ConversionsEvent})", conversionsEvent);
            return true;
        });
    }

    /// <summary>
    /// Prepare Facebook Pixel script
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the script code
    /// </returns>
    public async Task<string> PrepareScriptAsync()
    {
		_logger.LogError(">>> Entering PrepareScriptAsync()");
        return await HandleFunctionAsync(async () =>
        {
            //get the enabled configurations
            var store = await _storeContext.GetCurrentStoreAsync();
			var __logStart = System.Diagnostics.Stopwatch.StartNew();
            var configurations = await (await GetConfigurationsAsync(store.Id)).WhereAwait(async configuration =>
            {
                if (!configuration.PixelScriptEnabled)
                    return false;

                if (!configuration.DisableForUsersNotAcceptingCookieConsent)
                    return true;

                //don't display Pixel for users who not accepted Cookie Consent
                var cookieConsentAccepted = await _genericAttributeService.GetAttributeAsync<bool>(await _workContext.GetCurrentCustomerAsync(),
                    NopCustomerDefaults.EuCookieLawAcceptedAttribute, store.Id);
                return cookieConsentAccepted;
            }).ToListAsync();
			_logger.LogError("PrepareScriptAsync(): database query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);
            if (!configurations.Any())
                return string.Empty;

            //base script
		_logger.LogWarning("<<< Exiting PrepareScriptAsync()");
            return $@"
    <!-- Facebook Pixel Code -->
    <script>
	_logger.LogDebug("PrepareScriptAsync(): t.src → {Arg0}", t.src);
	_logger.LogDebug("PrepareScriptAsync(): t.async → {Arg0}", t.async);

        !function (f, b, e, v, n, t, s) {{
            if (f.fbq) return;
            n = f.fbq = function () {{
                n.callMethod ? n.callMethod.apply(n, arguments) : n.queue.push(arguments)
            }};
            if (!f._fbq) f._fbq = n;
            n.push = n;
            n.loaded = !0;
            n.version = '2.0';
            n.agent = '{FacebookPixelDefaults.AgentId}';
            n.queue = [];
            t = b.createElement(e);
            t.async = !0;
            t.src = v;
            s = b.getElementsByTagName(e)[0];
            s.parentNode.insertBefore(t, s)
        }}(window, document, 'script', 'https://connect.facebook.net/en_US/fbevents.js');
		_logger.LogDebug("PrepareScriptAsync(): n.push → {Arg0}", n.push);
		_logger.LogDebug("PrepareScriptAsync(): n.loaded → {Arg0}", n.loaded);
		_logger.LogDebug("PrepareScriptAsync(): n.version → {Arg0}", n.version);
		_logger.LogDebug("PrepareScriptAsync(): n.agent → {Arg0}", n.agent);
		_logger.LogDebug("PrepareScriptAsync(): n.queue → {Arg0}", n.queue);
        {await PrepareScriptsAsync(configurations)}
    </script>
    <!-- End Facebook Pixel Code -->";
        });
    }

    /// <summary>
    /// Prepare Facebook Pixel script
    /// </summary>
    /// <param name="widgetZone">Widget zone to place script</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the script code
    /// </returns>
    public async Task<string> PrepareCustomEventsScriptAsync(string widgetZone)
    {
		_logger.LogError(">>> Entering PrepareCustomEventsScriptAsync(widgetZone={WidgetZone})", widgetZone);
        return await HandleFunctionAsync(async () =>
        {
			var __logStart = System.Diagnostics.Stopwatch.StartNew();
			_logger.LogError("PrepareCustomEventsScriptAsync(widgetZone)={WidgetZone}: database query executed in {Arg1} ms", widgetZone, __logStart.ElapsedMilliseconds);
            var customEvents = await (await GetConfigurationsAsync()).SelectManyAwait(async configuration => await GetCustomEventsAsync(configuration.Id, widgetZone)).ToListAsync();
            foreach (var customEvent in customEvents)
                await PrepareTrackedEventScriptAsync(customEvent.EventName, string.Empty, isCustomEvent: true);

		_logger.LogWarning("<<< Exiting PrepareCustomEventsScriptAsync(widgetZone={WidgetZone})", widgetZone);
            return string.Empty;
        });
    }

    #endregion

    #region Conversions API

    /// <summary>
    /// Send add to cart events
    /// </summary>
    /// <param name="shoppingCartItem">Shopping cart item</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task SendAddToCartEventAsync(ShoppingCartItem shoppingCartItem)
    {
		_logger.LogError(">>> Entering SendAddToCartEventAsync(shoppingCartItem=<redacted>)");
        await HandleFunctionAsync(async () =>
        {
            var eventName = shoppingCartItem.ShoppingCartTypeId == (int)ShoppingCartType.ShoppingCart
                ? FacebookPixelDefaults.ADD_TO_CART
                : FacebookPixelDefaults.ADD_TO_WISHLIST;

		_logger.LogWarning("<<< Exiting SendAddToCartEventAsync(shoppingCartItem=<redacted>)");
            return await HandleEventAsync(() => PrepareAddToCartEventModelAsync(shoppingCartItem), eventName, shoppingCartItem.StoreId);
        });
    }

    /// <summary>
    /// Send purchase events
    /// </summary>
    /// <param name="order">Order</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task SendPurchaseEventAsync(Order order)
    {
		_logger.LogError(">>> Entering SendPurchaseEventAsync(order={Order})", order);
        await HandleFunctionAsync(() =>
            HandleEventAsync(() => PreparePurchaseModelAsync(order), FacebookPixelDefaults.PURCHASE, order.StoreId));
			_logger.LogWarning("<<< Exiting SendPurchaseEventAsync(order={Order})", order);
    }

    /// <summary>
    /// Send view content events
    /// </summary>
    /// <param name="productDetailsModel">Product details model</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task SendViewContentEventAsync(ProductDetailsModel productDetailsModel)
    {
		_logger.LogError(">>> Entering SendViewContentEventAsync(productDetailsModel={ProductDetailsModel})", productDetailsModel);
        await HandleFunctionAsync(() =>
            HandleEventAsync(() => PrepareViewContentModelAsync(productDetailsModel), FacebookPixelDefaults.VIEW_CONTENT));
			_logger.LogWarning("<<< Exiting SendViewContentEventAsync(productDetailsModel={ProductDetailsModel})", productDetailsModel);
    }

    /// <summary>
    /// Send initiate checkout events
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task SendInitiateCheckoutEventAsync()
    {
		_logger.LogError(">>> Entering SendInitiateCheckoutEventAsync()");
        await HandleFunctionAsync(() =>
            HandleEventAsync(() => PrepareInitiateCheckoutModelAsync(), FacebookPixelDefaults.INITIATE_CHECKOUT));
			_logger.LogWarning("<<< Exiting SendInitiateCheckoutEventAsync()");
    }

    /// <summary>
    /// Send page view events
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task SendPageViewEventAsync()
    {
		_logger.LogError(">>> Entering SendPageViewEventAsync()");
        await HandleFunctionAsync(() =>
            HandleEventAsync(() => PreparePageViewModelAsync(), FacebookPixelDefaults.PAGE_VIEW));
			_logger.LogWarning("<<< Exiting SendPageViewEventAsync()");
    }

    /// <summary>
    /// Send search events
    /// </summary>
    /// <param name="searchTerm">Search term</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task SendSearchEventAsync(string searchTerm)
    {
		_logger.LogError(">>> Entering SendSearchEventAsync(searchTerm={SearchTerm})", searchTerm);
        await HandleFunctionAsync(() =>
            HandleEventAsync(() => PrepareSearchModelAsync(searchTerm), FacebookPixelDefaults.SEARCH));
			_logger.LogWarning("<<< Exiting SendSearchEventAsync(searchTerm={SearchTerm})", searchTerm);
    }

    /// <summary>
    /// Send contact events
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task SendContactEventAsync()
    {
		_logger.LogError(">>> Entering SendContactEventAsync()");
        await HandleFunctionAsync(() =>
            HandleEventAsync(() => PrepareContactModelAsync(), FacebookPixelDefaults.CONTACT));
			_logger.LogWarning("<<< Exiting SendContactEventAsync()");
    }

    /// <summary>
    /// Send complete registration events
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task SendCompleteRegistrationEventAsync()
    {
		_logger.LogError(">>> Entering SendCompleteRegistrationEventAsync()");
        await HandleFunctionAsync(() =>
            HandleEventAsync(() => PrepareCompleteRegistrationModelAsync(), FacebookPixelDefaults.COMPLETE_REGISTRATION));
			_logger.LogWarning("<<< Exiting SendCompleteRegistrationEventAsync()");
    }

    #endregion

    #region Configuration

    /// <summary>
    /// Get configurations
    /// </summary>
    /// <param name="storeId">Store identifier; pass 0 to load all records</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the paged list of configurations
    /// </returns>
    public async Task<IPagedList<FacebookPixelConfiguration>> GetPagedConfigurationsAsync(int storeId = 0, int pageIndex = 0, int pageSize = int.MaxValue)
    {
		_logger.LogError(">>> Entering GetPagedConfigurationsAsync(storeId={StoreId},pageIndex={PageIndex},pageSize={PageSize})", storeId, pageIndex, pageSize);
        var query = _facebookPixelConfigurationRepository.Table;

		_logger.LogError("GetPagedConfigurationsAsync(storeId={StoreId},pageIndex={PageIndex},pageSize={PageSize}): query → {Query}", storeId, pageIndex, pageSize, query);
        //filter by the store
        if (storeId > 0)
            query = query.Where(configuration => configuration.StoreId == storeId);

        query = query.OrderBy(configuration => configuration.Id);

		_logger.LogWarning("<<< Exiting GetPagedConfigurationsAsync(storeId={StoreId},pageIndex={PageIndex},pageSize={PageSize})", storeId, pageIndex, pageSize);
        return await query.ToPagedListAsync(pageIndex, pageSize);
    }

    /// <summary>
    /// Get a configuration by the identifier
    /// </summary>
    /// <param name="configurationId">Configuration identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the configuration
    /// </returns>
    public async Task<FacebookPixelConfiguration> GetConfigurationByIdAsync(int configurationId)
    {
		_logger.LogError(">>> Entering GetConfigurationByIdAsync(configurationId={ConfigurationId})", configurationId);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        if (configurationId == 0)
            return null;

		_logger.LogError("GetConfigurationByIdAsync(configurationId)={ConfigurationId}: external service call took {Arg1} ms", configurationId, __logStart.ElapsedMilliseconds);
        return await _staticCacheManager.GetAsync(_staticCacheManager.PrepareKeyForDefaultCache(FacebookPixelDefaults.ConfigurationCacheKey, configurationId), async () =>
            await _facebookPixelConfigurationRepository.GetByIdAsync(configurationId));
    }

    /// <summary>
    /// Insert the configuration
    /// </summary>
    /// <param name="configuration">Configuration</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task InsertConfigurationAsync(FacebookPixelConfiguration configuration)
    {
		_logger.LogError(">>> Entering InsertConfigurationAsync(configuration={Configuration})", configuration);
        ArgumentNullException.ThrowIfNull(configuration);

		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _facebookPixelConfigurationRepository.InsertAsync(configuration, false);
        await _staticCacheManager.RemoveByPrefixAsync(FacebookPixelDefaults.PrefixCacheKey);
		_logger.LogError("InsertConfigurationAsync(configuration)={Configuration}: save query executed in {Arg1} ms", configuration, __logStart.ElapsedMilliseconds);
    _logger.LogWarning("<<< Exiting InsertConfigurationAsync(configuration={Configuration})", configuration);
    }

    /// <summary>
    /// Update the configuration
    /// </summary>
    /// <param name="configuration">Configuration</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task UpdateConfigurationAsync(FacebookPixelConfiguration configuration)
    {
		_logger.LogError(">>> Entering UpdateConfigurationAsync(configuration={Configuration})", configuration);
        ArgumentNullException.ThrowIfNull(configuration);

		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _facebookPixelConfigurationRepository.UpdateAsync(configuration, false);
        await _staticCacheManager.RemoveByPrefixAsync(FacebookPixelDefaults.PrefixCacheKey);
		_logger.LogError("UpdateConfigurationAsync(configuration)={Configuration}: update query executed in {Arg1} ms", configuration, __logStart.ElapsedMilliseconds);
    _logger.LogWarning("<<< Exiting UpdateConfigurationAsync(configuration={Configuration})", configuration);
    }

    /// <summary>
    /// Delete the configuration
    /// </summary>
    /// <param name="configuration">Configuration</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task DeleteConfigurationAsync(FacebookPixelConfiguration configuration)
    {
		_logger.LogError(">>> Entering DeleteConfigurationAsync(configuration={Configuration})", configuration);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(configuration);

		var __logStartDb = System.Diagnostics.Stopwatch.StartNew();
		_logger.LogError("DeleteConfigurationAsync(configuration)={Configuration}: delete query executed in {Arg1} ms", configuration, __logStartDb.ElapsedMilliseconds);
    _logger.LogWarning("<<< Exiting DeleteConfigurationAsync(configuration={Configuration})", configuration);
        await _facebookPixelConfigurationRepository.DeleteAsync(configuration, false);
		_logger.LogError("DeleteConfigurationAsync(configuration)={Configuration}: external service call took {Arg1} ms", configuration, __logStart.ElapsedMilliseconds);
        await _staticCacheManager.RemoveByPrefixAsync(FacebookPixelDefaults.PrefixCacheKey);
    }

    /// <summary>
    /// Get configuration custom events
    /// </summary>
    /// <param name="configurationId">Configuration identifier</param>
    /// <param name="widgetZone">Widget zone name; pass null to load all records</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of custom events
    /// </returns>
    public async Task<IList<CustomEvent>> GetCustomEventsAsync(int configurationId, string widgetZone = null)
    {
		_logger.LogError(">>> Entering GetCustomEventsAsync(configurationId={ConfigurationId},widgetZone={WidgetZone})", configurationId, widgetZone);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var cachedCustomEvents = await _staticCacheManager.GetAsync(_staticCacheManager.PrepareKeyForDefaultCache(FacebookPixelDefaults.CustomEventsCacheKey, configurationId), async () =>
        {
            //load configuration custom events
            var configuration = await GetConfigurationByIdAsync(configurationId);
			_logger.LogError("GetCustomEventsAsync(configurationId={ConfigurationId},widgetZone={WidgetZone}): configuration → {Configuration}", configurationId, widgetZone, configuration);
            var customEventsValue = configuration?.CustomEvents ?? string.Empty;
            var customEvents = JsonConvert.DeserializeObject<List<CustomEvent>>(customEventsValue) ?? new List<CustomEvent>();
			_logger.LogError("GetCustomEventsAsync(configurationId={ConfigurationId},widgetZone={WidgetZone}): customEvents → {CustomEvents}", configurationId, widgetZone, customEvents);
            return customEvents;
        });

        //filter by the widget zone
        if (!string.IsNullOrEmpty(widgetZone))
            cachedCustomEvents = cachedCustomEvents.Where(customEvent => customEvent.WidgetZones?.Contains(widgetZone) ?? false).ToList();
			_logger.LogError("GetCustomEventsAsync(configurationId={ConfigurationId},widgetZone={WidgetZone}): cachedCustomEvents → {CachedCustomEvents}", configurationId, widgetZone, cachedCustomEvents);

		_logger.LogWarning("<<< Exiting GetCustomEventsAsync(configurationId={ConfigurationId},widgetZone={WidgetZone})", configurationId, widgetZone);
        return cachedCustomEvents;
    }

    /// <summary>
    /// Save configuration custom events
    /// </summary>
    /// <param name="configurationId">Configuration identifier</param>
    /// <param name="eventName">Event name</param>
    /// <param name="widgetZones">Widget zones names</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task SaveCustomEventsAsync(int configurationId, string eventName, IList<string> widgetZones)
    {
		_logger.LogError(">>> Entering SaveCustomEventsAsync(configurationId={ConfigurationId},eventName={EventName},widgetZones={WidgetZones})", configurationId, eventName, widgetZones);
        if (string.IsNullOrEmpty(eventName))
            return;

        var configuration = await GetConfigurationByIdAsync(configurationId);
		_logger.LogError("SaveCustomEventsAsync(configurationId={ConfigurationId},eventName={EventName},widgetZones={WidgetZones}): configuration → {Configuration}", configurationId, eventName, widgetZones, configuration);
        if (configuration == null)
            return;

        //load configuration custom events
        var customEventsValue = configuration.CustomEvents ?? string.Empty;
        var customEvents = JsonConvert.DeserializeObject<List<CustomEvent>>(customEventsValue) ?? new List<CustomEvent>();
		_logger.LogError("SaveCustomEventsAsync(configurationId={ConfigurationId},eventName={EventName},widgetZones={WidgetZones}): customEvents → {CustomEvents}", configurationId, eventName, widgetZones, customEvents);

        //try to get an event by the passed name
        var customEvent = customEvents
            .FirstOrDefault(customEvent => eventName.Equals(customEvent.EventName, StringComparison.InvariantCultureIgnoreCase));
        if (customEvent == null)
        {
            //create new one if not exist
            customEvent = new CustomEvent { EventName = eventName };
            customEvents.Add(customEvent);
        }

        //update widget zones of this event
        customEvent.WidgetZones = widgetZones ?? new List<string>();

        //or delete an event
        if (!customEvent.WidgetZones.Any())
            customEvents.Remove(customEvent);

        //update configuration 
        configuration.CustomEvents = JsonConvert.SerializeObject(customEvents);
        await UpdateConfigurationAsync(configuration);
        await _staticCacheManager.RemoveByPrefixAsync(FacebookPixelDefaults.PrefixCacheKey);
        await _staticCacheManager.RemoveByPrefixAsync(WidgetModelDefaults.WidgetPrefixCacheKey);
		_logger.LogWarning("<<< Exiting SaveCustomEventsAsync(configurationId={ConfigurationId},eventName={EventName},widgetZones={WidgetZones})", configurationId, eventName, widgetZones);
    }

    /// <summary>
    /// Get used widget zones for all custom events
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of widget zones names
    /// </returns>
    public async Task<IList<string>> GetCustomEventsWidgetZonesAsync()
    {
		_logger.LogError(">>> Entering GetCustomEventsWidgetZonesAsync()");
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        return await _staticCacheManager.GetAsync(_staticCacheManager.PrepareKeyForDefaultCache(FacebookPixelDefaults.WidgetZonesCacheKey), async () =>
        {
            //load custom events and their widget zones
            var configurations = await GetConfigurationsAsync();
			var __logStartDb = System.Diagnostics.Stopwatch.StartNew();
			_logger.LogError("GetCustomEventsWidgetZonesAsync(): database query executed in {Arg0} ms", __logStartDb.ElapsedMilliseconds);
            var customEvents = await configurations.SelectManyAwait(async configuration => await GetCustomEventsAsync(configuration.Id)).ToListAsync();
            var widgetZones = await customEvents.SelectMany(customEvent => customEvent.WidgetZones).Distinct().ToListAsync();

            return widgetZones;
        });
		_logger.LogError("GetCustomEventsWidgetZonesAsync(): external service call took {Arg0} ms", __logStart.ElapsedMilliseconds);
		_logger.LogWarning("<<< Exiting GetCustomEventsWidgetZonesAsync()");
    }

    #endregion

    #endregion
}