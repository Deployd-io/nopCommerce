using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Stores;
using Nop.Plugin.Widgets.GoogleAnalytics.Api;
using Nop.Plugin.Widgets.GoogleAnalytics.Api.Models;
using Nop.Services.Catalog;
using Nop.Services.Cms;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Events;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Stores;

namespace Nop.Plugin.Widgets.GoogleAnalytics;

public class EventConsumer :
    IConsumer<OrderPlacedEvent>,
    IConsumer<OrderPaidEvent>,
    IConsumer<OrderRefundedEvent>
{
    protected readonly CurrencySettings _currencySettings;
    protected readonly GoogleAnalyticsHttpClient _googleAnalyticsHttpClient;
    protected readonly ICategoryService _categoryService;
    protected readonly ICurrencyService _currencyService;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly IHttpContextAccessor _httpContextAccessor;
    protected readonly ILogger _logger;
    protected readonly IOrderService _orderService;
    protected readonly IProductService _productService;
    protected readonly ISettingService _settingService;
    protected readonly IStoreContext _storeContext;
    protected readonly IStoreService _storeService;
    protected readonly IWidgetPluginManager _widgetPluginManager;

    public EventConsumer(
        CurrencySettings currencySettings,
        GoogleAnalyticsHttpClient googleAnalyticsHttpClient,
        ICategoryService categoryService,
        ICurrencyService currencyService,
        IGenericAttributeService genericAttributeService,
        IHttpContextAccessor httpContextAccessor,
        ILogger logger,
        IOrderService orderService,
        IProductService productService,
        ISettingService settingService,
        IStoreContext storeContext,
        IStoreService storeService,
        IWidgetPluginManager widgetPluginManager)
    {
        _currencySettings = currencySettings;
		_logger.LogDebug("EventConsumer(currencySettings={CurrencySettings},googleAnalyticsHttpClient={GoogleAnalyticsHttpClient},categoryService={CategoryService},currencyService={CurrencyService},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},logger={Logger},orderService={OrderService},productService={ProductService},settingService={SettingService},storeContext={StoreContext},storeService={StoreService},widgetPluginManager={WidgetPluginManager}): _currencySettings → {_currencySettings}", currencySettings, googleAnalyticsHttpClient, categoryService, currencyService, genericAttributeService, httpContextAccessor, logger, orderService, productService, settingService, storeContext, storeService, widgetPluginManager, _currencySettings);
        _googleAnalyticsHttpClient = googleAnalyticsHttpClient;
		_logger.LogDebug("EventConsumer(currencySettings={CurrencySettings},googleAnalyticsHttpClient={GoogleAnalyticsHttpClient},categoryService={CategoryService},currencyService={CurrencyService},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},logger={Logger},orderService={OrderService},productService={ProductService},settingService={SettingService},storeContext={StoreContext},storeService={StoreService},widgetPluginManager={WidgetPluginManager}): _googleAnalyticsHttpClient → {_googleAnalyticsHttpClient}", currencySettings, googleAnalyticsHttpClient, categoryService, currencyService, genericAttributeService, httpContextAccessor, logger, orderService, productService, settingService, storeContext, storeService, widgetPluginManager, _googleAnalyticsHttpClient);
        _categoryService = categoryService;
		_logger.LogDebug("EventConsumer(currencySettings={CurrencySettings},googleAnalyticsHttpClient={GoogleAnalyticsHttpClient},categoryService={CategoryService},currencyService={CurrencyService},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},logger={Logger},orderService={OrderService},productService={ProductService},settingService={SettingService},storeContext={StoreContext},storeService={StoreService},widgetPluginManager={WidgetPluginManager}): _categoryService → {_categoryService}", currencySettings, googleAnalyticsHttpClient, categoryService, currencyService, genericAttributeService, httpContextAccessor, logger, orderService, productService, settingService, storeContext, storeService, widgetPluginManager, _categoryService);
        _currencyService = currencyService;
		_logger.LogDebug("EventConsumer(currencySettings={CurrencySettings},googleAnalyticsHttpClient={GoogleAnalyticsHttpClient},categoryService={CategoryService},currencyService={CurrencyService},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},logger={Logger},orderService={OrderService},productService={ProductService},settingService={SettingService},storeContext={StoreContext},storeService={StoreService},widgetPluginManager={WidgetPluginManager}): _currencyService → {_currencyService}", currencySettings, googleAnalyticsHttpClient, categoryService, currencyService, genericAttributeService, httpContextAccessor, logger, orderService, productService, settingService, storeContext, storeService, widgetPluginManager, _currencyService);
        _genericAttributeService = genericAttributeService;
		_logger.LogDebug("EventConsumer(currencySettings={CurrencySettings},googleAnalyticsHttpClient={GoogleAnalyticsHttpClient},categoryService={CategoryService},currencyService={CurrencyService},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},logger={Logger},orderService={OrderService},productService={ProductService},settingService={SettingService},storeContext={StoreContext},storeService={StoreService},widgetPluginManager={WidgetPluginManager}): _genericAttributeService → {_genericAttributeService}", currencySettings, googleAnalyticsHttpClient, categoryService, currencyService, genericAttributeService, httpContextAccessor, logger, orderService, productService, settingService, storeContext, storeService, widgetPluginManager, _genericAttributeService);
        _httpContextAccessor = httpContextAccessor;
		_logger.LogDebug("EventConsumer(currencySettings={CurrencySettings},googleAnalyticsHttpClient={GoogleAnalyticsHttpClient},categoryService={CategoryService},currencyService={CurrencyService},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},logger={Logger},orderService={OrderService},productService={ProductService},settingService={SettingService},storeContext={StoreContext},storeService={StoreService},widgetPluginManager={WidgetPluginManager}): _httpContextAccessor → {_httpContextAccessor}", currencySettings, googleAnalyticsHttpClient, categoryService, currencyService, genericAttributeService, httpContextAccessor, logger, orderService, productService, settingService, storeContext, storeService, widgetPluginManager, _httpContextAccessor);
        _logger = logger;
		_logger.LogDebug(">>> Entering EventConsumer(currencySettings={CurrencySettings},googleAnalyticsHttpClient={GoogleAnalyticsHttpClient},categoryService={CategoryService},currencyService={CurrencyService},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},logger={Logger},orderService={OrderService},productService={ProductService},settingService={SettingService},storeContext={StoreContext},storeService={StoreService},widgetPluginManager={WidgetPluginManager})", currencySettings, googleAnalyticsHttpClient, categoryService, currencyService, genericAttributeService, httpContextAccessor, logger, orderService, productService, settingService, storeContext, storeService, widgetPluginManager);
        _orderService = orderService;
        _productService = productService;
		_logger.LogDebug("EventConsumer(currencySettings={CurrencySettings},googleAnalyticsHttpClient={GoogleAnalyticsHttpClient},categoryService={CategoryService},currencyService={CurrencyService},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},logger={Logger},orderService={OrderService},productService={ProductService},settingService={SettingService},storeContext={StoreContext},storeService={StoreService},widgetPluginManager={WidgetPluginManager}): _orderService → {_orderService}", currencySettings, googleAnalyticsHttpClient, categoryService, currencyService, genericAttributeService, httpContextAccessor, logger, orderService, productService, settingService, storeContext, storeService, widgetPluginManager, _orderService);
		_logger.LogDebug("EventConsumer(currencySettings={CurrencySettings},googleAnalyticsHttpClient={GoogleAnalyticsHttpClient},categoryService={CategoryService},currencyService={CurrencyService},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},logger={Logger},orderService={OrderService},productService={ProductService},settingService={SettingService},storeContext={StoreContext},storeService={StoreService},widgetPluginManager={WidgetPluginManager}): _productService → {_productService}", currencySettings, googleAnalyticsHttpClient, categoryService, currencyService, genericAttributeService, httpContextAccessor, logger, orderService, productService, settingService, storeContext, storeService, widgetPluginManager, _productService);
        _settingService = settingService;
        _storeContext = storeContext;
		_logger.LogDebug("EventConsumer(currencySettings={CurrencySettings},googleAnalyticsHttpClient={GoogleAnalyticsHttpClient},categoryService={CategoryService},currencyService={CurrencyService},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},logger={Logger},orderService={OrderService},productService={ProductService},settingService={SettingService},storeContext={StoreContext},storeService={StoreService},widgetPluginManager={WidgetPluginManager}): _settingService → {_settingService}", currencySettings, googleAnalyticsHttpClient, categoryService, currencyService, genericAttributeService, httpContextAccessor, logger, orderService, productService, settingService, storeContext, storeService, widgetPluginManager, _settingService);
		_logger.LogDebug("EventConsumer(currencySettings={CurrencySettings},googleAnalyticsHttpClient={GoogleAnalyticsHttpClient},categoryService={CategoryService},currencyService={CurrencyService},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},logger={Logger},orderService={OrderService},productService={ProductService},settingService={SettingService},storeContext={StoreContext},storeService={StoreService},widgetPluginManager={WidgetPluginManager}): _storeContext → {_storeContext}", currencySettings, googleAnalyticsHttpClient, categoryService, currencyService, genericAttributeService, httpContextAccessor, logger, orderService, productService, settingService, storeContext, storeService, widgetPluginManager, _storeContext);
        _storeService = storeService;
        _widgetPluginManager = widgetPluginManager;
		_logger.LogDebug("EventConsumer(currencySettings={CurrencySettings},googleAnalyticsHttpClient={GoogleAnalyticsHttpClient},categoryService={CategoryService},currencyService={CurrencyService},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},logger={Logger},orderService={OrderService},productService={ProductService},settingService={SettingService},storeContext={StoreContext},storeService={StoreService},widgetPluginManager={WidgetPluginManager}): _widgetPluginManager → {_widgetPluginManager}", currencySettings, googleAnalyticsHttpClient, categoryService, currencyService, genericAttributeService, httpContextAccessor, logger, orderService, productService, settingService, storeContext, storeService, widgetPluginManager, _widgetPluginManager);
		_logger.LogDebug("<<< Exiting EventConsumer(currencySettings={CurrencySettings},googleAnalyticsHttpClient={GoogleAnalyticsHttpClient},categoryService={CategoryService},currencyService={CurrencyService},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},logger={Logger},orderService={OrderService},productService={ProductService},settingService={SettingService},storeContext={StoreContext},storeService={StoreService},widgetPluginManager={WidgetPluginManager})", currencySettings, googleAnalyticsHttpClient, categoryService, currencyService, genericAttributeService, httpContextAccessor, logger, orderService, productService, settingService, storeContext, storeService, widgetPluginManager);
		_logger.LogDebug("EventConsumer(currencySettings={CurrencySettings},googleAnalyticsHttpClient={GoogleAnalyticsHttpClient},categoryService={CategoryService},currencyService={CurrencyService},genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},logger={Logger},orderService={OrderService},productService={ProductService},settingService={SettingService},storeContext={StoreContext},storeService={StoreService},widgetPluginManager={WidgetPluginManager}): _storeService → {_storeService}", currencySettings, googleAnalyticsHttpClient, categoryService, currencyService, genericAttributeService, httpContextAccessor, logger, orderService, productService, settingService, storeContext, storeService, widgetPluginManager, _storeService);
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    protected async Task<bool> IsPluginEnabledAsync()
    {
		_logger.LogDebug(">>> Entering IsPluginEnabledAsync()");
		_logger.LogDebug("<<< Exiting IsPluginEnabledAsync()");
        return await _widgetPluginManager.IsPluginActiveAsync(GoogleAnalyticsDefaults.SystemName);
    }

    protected async Task SaveCookiesAsync(Order order, GoogleAnalyticsSettings googleAnalyticsSettings, Store store)
    {
		_logger.LogDebug(">>> Entering SaveCookiesAsync(order={Order},googleAnalyticsSettings={GoogleAnalyticsSettings},store={Store})", order, googleAnalyticsSettings, store);
        //try to get cookie
        var httpContext = _httpContextAccessor.HttpContext;

        //client_id
        httpContext.Request.Cookies.TryGetValue(GoogleAnalyticsDefaults.ClientIdCookiesName, out var clientId);
        await _genericAttributeService.SaveAttributeAsync(order, GoogleAnalyticsDefaults.ClientIdAttribute, clientId, store.Id);

        //session_id
        var measurementId = googleAnalyticsSettings.GoogleId.Split('-')[1];
		_logger.LogDebug("SaveCookiesAsync(order={Order},googleAnalyticsSettings={GoogleAnalyticsSettings},store={Store}): measurementId → {MeasurementId}", order, googleAnalyticsSettings, store, measurementId);
        var sessionCookieKey = $"{GoogleAnalyticsDefaults.SessionIdCookiesName}{measurementId}";
		_logger.LogDebug("SaveCookiesAsync(order={Order},googleAnalyticsSettings={GoogleAnalyticsSettings},store={Store}): sessionCookieKey → {SessionCookieKey}", order, googleAnalyticsSettings, store, sessionCookieKey);
        httpContext.Request.Cookies.TryGetValue(sessionCookieKey, out var sessionId);
        await _genericAttributeService.SaveAttributeAsync(order, GoogleAnalyticsDefaults.SessionIdAttribute, sessionId, store.Id);
		_logger.LogDebug("<<< Exiting SaveCookiesAsync(order={Order},googleAnalyticsSettings={GoogleAnalyticsSettings},store={Store})", order, googleAnalyticsSettings, store);
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    protected async Task ProcessOrderEventAsync(Order order, GoogleAnalyticsSettings googleAnalyticsSettings, string eventName)
    {
		_logger.LogDebug(">>> Entering ProcessOrderEventAsync(order={Order},googleAnalyticsSettings={GoogleAnalyticsSettings},eventName={EventName})", order, googleAnalyticsSettings, eventName);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        try
        {
            var store = await _storeService.GetStoreByIdAsync(order.StoreId) ?? await _storeContext.GetCurrentStoreAsync();
			_logger.LogDebug("ProcessOrderEventAsync(order={Order},googleAnalyticsSettings={GoogleAnalyticsSettings},eventName={EventName}): store → {Store}", order, googleAnalyticsSettings, eventName, store);
            var currency = (await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId)).CurrencyCode;
			_logger.LogDebug("ProcessOrderEventAsync(order={Order},googleAnalyticsSettings={GoogleAnalyticsSettings},eventName={EventName}): currency → {Currency}", order, googleAnalyticsSettings, eventName, currency);
            var orderId = order.CustomOrderNumber;
            var orderShipping = googleAnalyticsSettings.IncludingTax ? order.OrderShippingInclTax : order.OrderShippingExclTax;
            var orderTax = order.OrderTax;
            var orderTotal = order.OrderTotal;

            var gaRequest = new EventRequest
            {
                ClientId = await _genericAttributeService.GetAttributeAsync<string>(order, GoogleAnalyticsDefaults.ClientIdAttribute, store.Id),
                UserId = order.CustomerId.ToString(),
                TimestampMicros = (DateTimeOffset.UtcNow - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).Ticks / 10
            };


            var events = new List<Event>();
			_logger.LogDebug("ProcessOrderEventAsync(order={Order},googleAnalyticsSettings={GoogleAnalyticsSettings},eventName={EventName}): events → {Events}", order, googleAnalyticsSettings, eventName, events);
            var gaEvent = new Event
            {
                Name = eventName
            };
            events.Add(gaEvent);

            var gaParams = new Parameters
            {
                Currency = currency,
                TransactionId = orderId,
                EngagementTime = 100,
                SessionId = await _genericAttributeService.GetAttributeAsync<string>(order, GoogleAnalyticsDefaults.SessionIdAttribute, store.Id),
                Value = orderTotal,
                Tax = orderTax,
                Shipping = orderShipping
            };

            var items = new List<Item>();
			_logger.LogDebug("ProcessOrderEventAsync(order={Order},googleAnalyticsSettings={GoogleAnalyticsSettings},eventName={EventName}): items → {Items}", order, googleAnalyticsSettings, eventName, items);
            foreach (var item in await _orderService.GetOrderItemsAsync(order.Id))
            {
                var product = await _productService.GetProductByIdAsync(item.ProductId);
				_logger.LogDebug("ProcessOrderEventAsync(order={Order},googleAnalyticsSettings={GoogleAnalyticsSettings},eventName={EventName}): product → {Product}", order, googleAnalyticsSettings, eventName, product);
                var sku = await _productService.FormatSkuAsync(product, item.AttributesXml);
				_logger.LogDebug("ProcessOrderEventAsync(order={Order},googleAnalyticsSettings={GoogleAnalyticsSettings},eventName={EventName}): sku → {Sku}", order, googleAnalyticsSettings, eventName, sku);
                if (string.IsNullOrEmpty(sku))
                    sku = product.Id.ToString();
                //get category
                var category = (await _categoryService.GetCategoryByIdAsync((await _categoryService.GetProductCategoriesByProductIdAsync(product.Id)).FirstOrDefault()?.CategoryId ?? 0))?.Name;
				_logger.LogDebug("ProcessOrderEventAsync(order={Order},googleAnalyticsSettings={GoogleAnalyticsSettings},eventName={EventName}): category → {Category}", order, googleAnalyticsSettings, eventName, category);
                if (string.IsNullOrEmpty(category))
                    category = "No category";
                var unitPrice = googleAnalyticsSettings.IncludingTax ? item.UnitPriceInclTax : item.UnitPriceExclTax;

                var gaItem = new Item
                {
                    ItemId = sku,
                    ItemName = product.Name,
                    Affiliation = store.Name,
                    ItemCategory = category,
                    Price = unitPrice,
                    Quantity = item.Quantity
                };

                items.Add(gaItem);
            }
            gaParams.Items = items;
			_logger.LogDebug("ProcessOrderEventAsync(order={Order},googleAnalyticsSettings={GoogleAnalyticsSettings},eventName={EventName}): gaParams.Items → {Arg3}", order, googleAnalyticsSettings, eventName, gaParams.Items);
			_logger.LogDebug("ProcessOrderEventAsync(order={Order},googleAnalyticsSettings={GoogleAnalyticsSettings},eventName={EventName}): gaEvent.Params → {Arg3}", order, googleAnalyticsSettings, eventName, gaEvent.Params);
			_logger.LogDebug("ProcessOrderEventAsync(order={Order},googleAnalyticsSettings={GoogleAnalyticsSettings},eventName={EventName}): gaRequest.Events → {Arg3}", order, googleAnalyticsSettings, eventName, gaRequest.Events);
            gaEvent.Params = gaParams;
            gaRequest.Events = events;

            await _googleAnalyticsHttpClient.RequestAsync(gaRequest, googleAnalyticsSettings);
        }
        catch (Exception ex)
        {
            await _logger.InsertLogAsync(LogLevel.Error, "Google Analytics. Error canceling transaction from server side", ex.ToString());
        }
		_logger.LogInformation("ProcessOrderEventAsync(order,googleAnalyticsSettings,eventName)={Order},{GoogleAnalyticsSettings},{EventName}: external service call took {Arg3} ms", order, googleAnalyticsSettings, eventName, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting ProcessOrderEventAsync(order={Order},googleAnalyticsSettings={GoogleAnalyticsSettings},eventName={EventName})", order, googleAnalyticsSettings, eventName);
    }

    /// <summary>
    /// Handles the event
    /// </summary>
    /// <param name="eventMessage">The event message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(OrderRefundedEvent eventMessage)
    {
		_logger.LogDebug(">>> Entering HandleEventAsync(eventMessage={EventMessage})", eventMessage);
        //ensure the plugin is installed and active
        if (!await IsPluginEnabledAsync())
            return;

        var order = eventMessage.Order;

        //settings per store
        var store = await _storeService.GetStoreByIdAsync(order.StoreId) ?? await _storeContext.GetCurrentStoreAsync();
		_logger.LogDebug("HandleEventAsync(eventMessage={EventMessage}): store → {Store}", eventMessage, store);
        var googleAnalyticsSettings = await _settingService.LoadSettingAsync<GoogleAnalyticsSettings>(store.Id);
		_logger.LogDebug("HandleEventAsync(eventMessage={EventMessage}): googleAnalyticsSettings → {GoogleAnalyticsSettings}", eventMessage, googleAnalyticsSettings);

        //ecommerce is disabled
        if (!googleAnalyticsSettings.EnableEcommerce)
            return;

        //if we use HTTP requests to notify GA about new orders (only when they are paid), then we should notify GA about deleted AND paid orders
        var sendRequest = order.PaymentStatus == PaymentStatus.Paid;

        if (sendRequest)
            await ProcessOrderEventAsync(order, googleAnalyticsSettings, GoogleAnalyticsDefaults.OrderRefundedEventName);
			_logger.LogDebug("<<< Exiting HandleEventAsync(eventMessage={EventMessage})", eventMessage);
    }

    /// <summary>
    /// Handles the event
    /// </summary>
    /// <param name="eventMessage">The event message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(OrderPaidEvent eventMessage)
    {
        //ensure the plugin is installed and active
        if (!await IsPluginEnabledAsync())
            return;

        var order = eventMessage.Order;

        //settings per store
        var store = await _storeService.GetStoreByIdAsync(order.StoreId) ?? await _storeContext.GetCurrentStoreAsync();
        var googleAnalyticsSettings = await _settingService.LoadSettingAsync<GoogleAnalyticsSettings>(store.Id);

        //ecommerce is disabled
        if (!googleAnalyticsSettings.EnableEcommerce)
            return;

        //we use HTTP requests to notify GA about new orders (only when they are paid)
        await ProcessOrderEventAsync(order, googleAnalyticsSettings, GoogleAnalyticsDefaults.OrderPaidEventName);
    }

    /// <summary>
    /// Handles the event
    /// </summary>
    /// <param name="eventMessage">The event message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(OrderPlacedEvent eventMessage)
    {
        //ensure the plugin is installed and active
        if (!await IsPluginEnabledAsync())
            return;

        var order = eventMessage.Order;

        //settings per store
        var store = await _storeService.GetStoreByIdAsync(order.StoreId) ?? await _storeContext.GetCurrentStoreAsync();
        var googleAnalyticsSettings = await _settingService.LoadSettingAsync<GoogleAnalyticsSettings>(store.Id);

        //ecommerce is disabled
        if (!googleAnalyticsSettings.EnableEcommerce)
            return;

        await SaveCookiesAsync(order, googleAnalyticsSettings, store);
    }
}