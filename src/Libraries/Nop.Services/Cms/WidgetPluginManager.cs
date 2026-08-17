using Nop.Core.Domain.Cms;
using Nop.Core.Domain.Customers;
using Nop.Services.Customers;
using Nop.Services.Plugins;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Cms;

/// <summary>
/// Represents a widget plugin manager implementation
/// </summary>
public partial class WidgetPluginManager : PluginManager<IWidgetPlugin>, IWidgetPluginManager
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly WidgetSettings _widgetSettings;

    #endregion

    #region Ctor

    public WidgetPluginManager(ICustomerService customerService,
        IPluginService pluginService,
        WidgetSettings widgetSettings) : base(customerService, pluginService)
    {
		_logger.LogDebug(">>> Entering WidgetPluginManager(customerService={CustomerService},pluginService={PluginService},widgetSettings={WidgetSettings})", customerService, pluginService, widgetSettings);
        _widgetSettings = widgetSettings;
		_logger.LogDebug("<<< Exiting WidgetPluginManager(customerService={CustomerService},pluginService={PluginService},widgetSettings={WidgetSettings})", customerService, pluginService, widgetSettings);
		_logger.LogDebug("WidgetPluginManager(customerService={CustomerService},pluginService={PluginService},widgetSettings={WidgetSettings}): _widgetSettings → {_widgetSettings}", customerService, pluginService, widgetSettings, _widgetSettings);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Load active widgets
    /// </summary>
    /// <param name="customer">Filter by customer; pass null to load all plugins</param>
    /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
    /// <param name="widgetZone">Widget zone; pass null to load all plugins</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of active widget
    /// </returns>
    public virtual async Task<IList<IWidgetPlugin>> LoadActivePluginsAsync(Customer customer = null, int storeId = 0, string widgetZone = null)
    {
		_logger.LogDebug(">>> Entering LoadActivePluginsAsync(customer={Customer},storeId={StoreId},widgetZone={WidgetZone})", customer, storeId, widgetZone);
        var widgets = await LoadActivePluginsAsync(_widgetSettings.ActiveWidgetSystemNames, customer, storeId);

		_logger.LogDebug("LoadActivePluginsAsync(customer={Customer},storeId={StoreId},widgetZone={WidgetZone}): widgets → {Widgets}", customer, storeId, widgetZone, widgets);
        //filter by widget zone
        if (!string.IsNullOrEmpty(widgetZone))
            widgets = await widgets.WhereAwait(async widget =>
                (await widget.GetWidgetZonesAsync()).Contains(widgetZone, StringComparer.InvariantCultureIgnoreCase)).ToListAsync();
				var __logStart = System.Diagnostics.Stopwatch.StartNew();
				_logger.LogInformation("LoadActivePluginsAsync(customer,storeId,widgetZone)={Customer},{StoreId},{WidgetZone}: find query executed in {Arg3} ms", customer, storeId, widgetZone, __logStart.ElapsedMilliseconds);

		_logger.LogDebug("<<< Exiting LoadActivePluginsAsync(customer={Customer},storeId={StoreId},widgetZone={WidgetZone})", customer, storeId, widgetZone);
        return widgets;
    }

    /// <summary>
    /// Check whether the passed widget is active
    /// </summary>
    /// <param name="widget">Widget to check</param>
    /// <returns>Result</returns>
    public virtual bool IsPluginActive(IWidgetPlugin widget)
    {
        return IsPluginActive(widget, _widgetSettings.ActiveWidgetSystemNames);
    }

    /// <summary>
    /// Check whether the widget with the passed system name is active
    /// </summary>
    /// <param name="systemName">System name of widget to check</param>
    /// <param name="customer">Filter by customer; pass null to load all plugins</param>
    /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    public virtual async Task<bool> IsPluginActiveAsync(string systemName, Customer customer = null, int storeId = 0)
    {
		_logger.LogDebug(">>> Entering IsPluginActiveAsync(systemName={SystemName},customer={Customer},storeId={StoreId})", systemName, customer, storeId);
        var widget = await LoadPluginBySystemNameAsync(systemName, customer, storeId);

		_logger.LogDebug("IsPluginActiveAsync(systemName={SystemName},customer={Customer},storeId={StoreId}): widget → {Widget}", systemName, customer, storeId, widget);
		_logger.LogDebug("<<< Exiting IsPluginActiveAsync(systemName={SystemName},customer={Customer},storeId={StoreId})", systemName, customer, storeId);
        return IsPluginActive(widget);
    }

    #endregion
}