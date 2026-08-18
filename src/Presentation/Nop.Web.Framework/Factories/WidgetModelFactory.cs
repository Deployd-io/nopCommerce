using Microsoft.AspNetCore.Routing;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Services.Cms;
using Nop.Services.Customers;
using Nop.Web.Framework.Models.Cms;
using Nop.Web.Framework.Themes;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.Factories;

/// <summary>
/// Represents the widget model factory
/// </summary>
public partial class WidgetModelFactory : IWidgetModelFactory
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly ICustomerService _customerService;
    protected readonly IShortTermCacheManager _shortTermCacheManager;
    protected readonly IStaticCacheManager _staticCacheManager;
    protected readonly IStoreContext _storeContext;
    protected readonly IThemeContext _themeContext;
    protected readonly IWidgetPluginManager _widgetPluginManager;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public WidgetModelFactory(ICustomerService customerService,
        IShortTermCacheManager shortTermCacheManager,
        IStaticCacheManager staticCacheManager,
        IStoreContext storeContext,
        IThemeContext themeContext,
        IWidgetPluginManager widgetPluginManager,
        IWorkContext workContext)
    {
		_logger.LogDebug(">>> Entering WidgetModelFactory(customerService={CustomerService},shortTermCacheManager={ShortTermCacheManager},staticCacheManager={StaticCacheManager},storeContext={StoreContext},themeContext={ThemeContext},widgetPluginManager={WidgetPluginManager},workContext={WorkContext})", customerService, shortTermCacheManager, staticCacheManager, storeContext, themeContext, widgetPluginManager, workContext);
        _customerService = customerService;
		_logger.LogDebug("WidgetModelFactory(customerService={CustomerService},shortTermCacheManager={ShortTermCacheManager},staticCacheManager={StaticCacheManager},storeContext={StoreContext},themeContext={ThemeContext},widgetPluginManager={WidgetPluginManager},workContext={WorkContext}): _customerService → {_customerService}", customerService, shortTermCacheManager, staticCacheManager, storeContext, themeContext, widgetPluginManager, workContext, _customerService);
        _shortTermCacheManager = shortTermCacheManager;
		_logger.LogDebug("WidgetModelFactory(customerService={CustomerService},shortTermCacheManager={ShortTermCacheManager},staticCacheManager={StaticCacheManager},storeContext={StoreContext},themeContext={ThemeContext},widgetPluginManager={WidgetPluginManager},workContext={WorkContext}): _shortTermCacheManager → {_shortTermCacheManager}", customerService, shortTermCacheManager, staticCacheManager, storeContext, themeContext, widgetPluginManager, workContext, _shortTermCacheManager);
        _staticCacheManager = staticCacheManager;
		_logger.LogDebug("WidgetModelFactory(customerService={CustomerService},shortTermCacheManager={ShortTermCacheManager},staticCacheManager={StaticCacheManager},storeContext={StoreContext},themeContext={ThemeContext},widgetPluginManager={WidgetPluginManager},workContext={WorkContext}): _staticCacheManager → {_staticCacheManager}", customerService, shortTermCacheManager, staticCacheManager, storeContext, themeContext, widgetPluginManager, workContext, _staticCacheManager);
        _storeContext = storeContext;
		_logger.LogDebug("WidgetModelFactory(customerService={CustomerService},shortTermCacheManager={ShortTermCacheManager},staticCacheManager={StaticCacheManager},storeContext={StoreContext},themeContext={ThemeContext},widgetPluginManager={WidgetPluginManager},workContext={WorkContext}): _storeContext → {_storeContext}", customerService, shortTermCacheManager, staticCacheManager, storeContext, themeContext, widgetPluginManager, workContext, _storeContext);
        _themeContext = themeContext;
		_logger.LogDebug("WidgetModelFactory(customerService={CustomerService},shortTermCacheManager={ShortTermCacheManager},staticCacheManager={StaticCacheManager},storeContext={StoreContext},themeContext={ThemeContext},widgetPluginManager={WidgetPluginManager},workContext={WorkContext}): _themeContext → {_themeContext}", customerService, shortTermCacheManager, staticCacheManager, storeContext, themeContext, widgetPluginManager, workContext, _themeContext);
        _widgetPluginManager = widgetPluginManager;
		_logger.LogDebug("WidgetModelFactory(customerService={CustomerService},shortTermCacheManager={ShortTermCacheManager},staticCacheManager={StaticCacheManager},storeContext={StoreContext},themeContext={ThemeContext},widgetPluginManager={WidgetPluginManager},workContext={WorkContext}): _widgetPluginManager → {_widgetPluginManager}", customerService, shortTermCacheManager, staticCacheManager, storeContext, themeContext, widgetPluginManager, workContext, _widgetPluginManager);
        _workContext = workContext;
		_logger.LogDebug("WidgetModelFactory(customerService={CustomerService},shortTermCacheManager={ShortTermCacheManager},staticCacheManager={StaticCacheManager},storeContext={StoreContext},themeContext={ThemeContext},widgetPluginManager={WidgetPluginManager},workContext={WorkContext}): _workContext → {_workContext}", customerService, shortTermCacheManager, staticCacheManager, storeContext, themeContext, widgetPluginManager, workContext, _workContext);
		_logger.LogDebug("<<< Exiting WidgetModelFactory(customerService={CustomerService},shortTermCacheManager={ShortTermCacheManager},staticCacheManager={StaticCacheManager},storeContext={StoreContext},themeContext={ThemeContext},widgetPluginManager={WidgetPluginManager},workContext={WorkContext})", customerService, shortTermCacheManager, staticCacheManager, storeContext, themeContext, widgetPluginManager, workContext);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Get the render widget models
    /// </summary>
    /// <param name="widgetZone">Name of widget zone</param>
    /// <param name="additionalData">Additional data object</param>
    /// <param name="useCache">Value indicating whether to get widget models from cache</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of the render widget models
    /// </returns>
    public virtual async Task<List<RenderWidgetModel>> PrepareRenderWidgetModelAsync(string widgetZone, object additionalData = null, bool useCache = true)
    {
		_logger.LogDebug(">>> Entering PrepareRenderWidgetModelAsync(widgetZone={WidgetZone},additionalData={AdditionalData},useCache={UseCache})", widgetZone, additionalData, useCache);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var theme = await _themeContext.GetWorkingThemeNameAsync();
        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogDebug("PrepareRenderWidgetModelAsync(widgetZone={WidgetZone},additionalData={AdditionalData},useCache={UseCache}): theme → {Theme}", widgetZone, additionalData, useCache, theme);
		_logger.LogDebug("PrepareRenderWidgetModelAsync(widgetZone={WidgetZone},additionalData={AdditionalData},useCache={UseCache}): customer → {Customer}", widgetZone, additionalData, useCache, customer);
        var customerRoleIds = await _customerService.GetCustomerRoleIdsAsync(customer);
        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogDebug("PrepareRenderWidgetModelAsync(widgetZone={WidgetZone},additionalData={AdditionalData},useCache={UseCache}): customerRoleIds → {CustomerRoleIds}", widgetZone, additionalData, useCache, customerRoleIds);
		_logger.LogDebug("PrepareRenderWidgetModelAsync(widgetZone={WidgetZone},additionalData={AdditionalData},useCache={UseCache}): store → {Store}", widgetZone, additionalData, useCache, store);

        if (!useCache)
            return (await _widgetPluginManager.LoadActivePluginsAsync(customer, store.Id, widgetZone))
                .Select(widget => new RenderWidgetModel
                {
                    WidgetViewComponent = widget.GetWidgetViewComponent(widgetZone),
                    WidgetViewComponentArguments = new RouteValueDictionary { ["widgetZone"] = widgetZone, ["additionalData"] = additionalData }
                }).ToList();

        var widgetModels = await _shortTermCacheManager.GetAsync(async () =>
            (await _widgetPluginManager.LoadActivePluginsAsync(customer, store.Id, widgetZone))
            .Select(widget => new RenderWidgetModel
            {
                WidgetViewComponent = widget.GetWidgetViewComponent(widgetZone),
                WidgetViewComponentArguments = new RouteValueDictionary { ["widgetZone"] = widgetZone }
            }), WidgetModelDefaults.WidgetModelKey, customerRoleIds, store, widgetZone, theme);

        //"WidgetViewComponentArguments" property of widget models depends on "additionalData".
        //We need to clone the cached model before modifications (the updated one should not be cached)
        var models = widgetModels.Select(renderModel => new RenderWidgetModel
        {
            WidgetViewComponent = renderModel.WidgetViewComponent,
            WidgetViewComponentArguments = new RouteValueDictionary { ["widgetZone"] = widgetZone, ["additionalData"] = additionalData }
        }).ToList();

		_logger.LogInformation("PrepareRenderWidgetModelAsync(widgetZone,additionalData,useCache)={WidgetZone},{AdditionalData},{UseCache}: external service call took {Arg3} ms", widgetZone, additionalData, useCache, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting PrepareRenderWidgetModelAsync(widgetZone={WidgetZone},additionalData={AdditionalData},useCache={UseCache})", widgetZone, additionalData, useCache);
        return models;
    }

    #endregion
}