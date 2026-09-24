using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Shipping;
using Nop.Services.Customers;
using Nop.Services.Plugins;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Shipping;

/// <summary>
/// Represents a shipping plugin manager implementation
/// </summary>
public partial class ShippingPluginManager : PluginManager<IShippingRateComputationMethod>, IShippingPluginManager
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly ShippingSettings _shippingSettings;

    #endregion

    #region Ctor

    public ShippingPluginManager(ICustomerService customerService,
        IPluginService pluginService,
        ShippingSettings shippingSettings) : base(customerService, pluginService)
    {
		_logger.LogError(">>> Entering ShippingPluginManager(customerService={CustomerService},pluginService={PluginService},shippingSettings=<redacted>)", customerService, pluginService);
        _shippingSettings = shippingSettings;
		_logger.LogDebug("ShippingPluginManager(customerService={CustomerService},pluginService={PluginService},shippingSettings=<redacted>): _shippingSettings → {_shippingSettings}", customerService, pluginService, _shippingSettings);
    _logger.LogWarning("<<< Exiting ShippingPluginManager(customerService={CustomerService},pluginService={PluginService},shippingSettings=<redacted>)", customerService, pluginService);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Load active shipping providers
    /// </summary>
    /// <param name="customer">Filter by customer; pass null to load all plugins</param>
    /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
    /// <param name="systemName">Filter by shipping provider system name; pass null to load all plugins</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of active shipping providers
    /// </returns>
    public virtual async Task<IList<IShippingRateComputationMethod>> LoadActivePluginsAsync(Customer customer = null,
        int storeId = 0, string systemName = null)
    {
		_logger.LogError(">>> Entering LoadActivePluginsAsync(customer={Customer},storeId={StoreId},systemName={SystemName})", customer, storeId, systemName);
        var shippingProviders = await LoadActivePluginsAsync(_shippingSettings.ActiveShippingRateComputationMethodSystemNames, customer, storeId);

		_logger.LogError("LoadActivePluginsAsync(customer={Customer},storeId={StoreId},systemName={SystemName}): shippingProviders → {ShippingProviders}", customer, storeId, systemName, shippingProviders);
        //filter by passed system name
        if (!string.IsNullOrEmpty(systemName))
        {
            shippingProviders = shippingProviders
                .Where(provider => provider.PluginDescriptor.SystemName.Equals(systemName, StringComparison.InvariantCultureIgnoreCase))
                .ToList();
        }

		_logger.LogWarning("<<< Exiting LoadActivePluginsAsync(customer={Customer},storeId={StoreId},systemName={SystemName})", customer, storeId, systemName);
        return shippingProviders;
    }

    /// <summary>
    /// Check whether the passed shipping provider is active
    /// </summary>
    /// <param name="shippingProvider">Shipping provider to check</param>
    /// <returns>Result</returns>
    public virtual bool IsPluginActive(IShippingRateComputationMethod shippingProvider)
    {
		_logger.LogError(">>> Entering IsPluginActive(shippingProvider=<redacted>)");
        _logger.LogWarning("<<< Exiting IsPluginActive(shippingProvider=<redacted>)");
        return IsPluginActive(shippingProvider, _shippingSettings.ActiveShippingRateComputationMethodSystemNames);
    }

    /// <summary>
    /// Check whether the shipping provider with the passed system name is active
    /// </summary>
    /// <param name="systemName">System name of shipping provider to check</param>
    /// <param name="customer">Filter by customer; pass null to load all plugins</param>
    /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    public virtual async Task<bool> IsPluginActiveAsync(string systemName, Customer customer = null, int storeId = 0)
    {
		_logger.LogError(">>> Entering IsPluginActiveAsync(systemName={SystemName},customer={Customer},storeId={StoreId})", systemName, customer, storeId);
        var shippingProvider = await LoadPluginBySystemNameAsync(systemName, customer, storeId);
		_logger.LogError("IsPluginActiveAsync(systemName={SystemName},customer={Customer},storeId={StoreId}): shippingProvider → {ShippingProvider}", systemName, customer, storeId, shippingProvider);
		_logger.LogWarning("<<< Exiting IsPluginActiveAsync(systemName={SystemName},customer={Customer},storeId={StoreId})", systemName, customer, storeId);
        return IsPluginActive(shippingProvider);
    }

    #endregion
}