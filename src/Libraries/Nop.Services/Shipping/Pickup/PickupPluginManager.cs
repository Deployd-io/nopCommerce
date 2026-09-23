using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Shipping;
using Nop.Services.Customers;
using Nop.Services.Plugins;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Shipping.Pickup;

/// <summary>
/// Represents a pickup point plugin manager implementation
/// </summary>
public partial class PickupPluginManager : PluginManager<IPickupPointProvider>, IPickupPluginManager
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly ShippingSettings _shippingSettings;

    #endregion

    #region Ctor

    public PickupPluginManager(ICustomerService customerService,
        IPluginService pluginService,
        ShippingSettings shippingSettings) : base(customerService, pluginService)
    {
		_logger.LogError(">>> Entering PickupPluginManager(customerService={CustomerService},pluginService={PluginService},shippingSettings=<redacted>)", customerService, pluginService);
        _shippingSettings = shippingSettings;
		_logger.LogDebug("PickupPluginManager(customerService={CustomerService},pluginService={PluginService},shippingSettings=<redacted>): _shippingSettings → {_shippingSettings}", customerService, pluginService, _shippingSettings);
    _logger.LogWarning("<<< Exiting PickupPluginManager(customerService={CustomerService},pluginService={PluginService},shippingSettings=<redacted>)", customerService, pluginService);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Load active pickup point providers
    /// </summary>
    /// <param name="customer">Filter by customer; pass null to load all plugins</param>
    /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
    /// <param name="systemName">Filter by pickup point provider system name; pass null to load all plugins</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of active pickup point providers
    /// </returns>
    public virtual async Task<IList<IPickupPointProvider>> LoadActivePluginsAsync(Customer customer = null, int storeId = 0, string systemName = null)
    {
		_logger.LogError(">>> Entering LoadActivePluginsAsync(customer={Customer},storeId={StoreId},systemName={SystemName})", customer, storeId, systemName);
        var pickupPointProviders = await LoadActivePluginsAsync(_shippingSettings.ActivePickupPointProviderSystemNames, customer, storeId);

		_logger.LogError("LoadActivePluginsAsync(customer={Customer},storeId={StoreId},systemName={SystemName}): pickupPointProviders → {PickupPointProviders}", customer, storeId, systemName, pickupPointProviders);
        //filter by passed system name
        if (!string.IsNullOrEmpty(systemName))
        {
            pickupPointProviders = pickupPointProviders
                .Where(provider => provider.PluginDescriptor.SystemName.Equals(systemName, StringComparison.InvariantCultureIgnoreCase))
                .ToList();
        }

		_logger.LogWarning("<<< Exiting LoadActivePluginsAsync(customer={Customer},storeId={StoreId},systemName={SystemName})", customer, storeId, systemName);
        return pickupPointProviders;
    }

    /// <summary>
    /// Check whether the passed pickup point provider is active
    /// </summary>
    /// <param name="pickupPointProvider">Pickup point provider to check</param>
    /// <returns>Result</returns>
    public virtual bool IsPluginActive(IPickupPointProvider pickupPointProvider)
    {
		_logger.LogError(">>> Entering IsPluginActive(pickupPointProvider={PickupPointProvider})", pickupPointProvider);
        _logger.LogWarning("<<< Exiting IsPluginActive(pickupPointProvider={PickupPointProvider})", pickupPointProvider);
        return IsPluginActive(pickupPointProvider, _shippingSettings.ActivePickupPointProviderSystemNames);
    }

    /// <summary>
    /// Check whether the pickup point provider with the passed system name is active
    /// </summary>
    /// <param name="systemName">System name of pickup point provider to check</param>
    /// <param name="customer">Filter by customer; pass null to load all plugins</param>
    /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    public virtual async Task<bool> IsPluginActiveAsync(string systemName, Customer customer = null, int storeId = 0)
    {
		_logger.LogError(">>> Entering IsPluginActiveAsync(systemName={SystemName},customer={Customer},storeId={StoreId})", systemName, customer, storeId);
        var pickupPointProvider = await LoadPluginBySystemNameAsync(systemName, customer, storeId);
		_logger.LogError("IsPluginActiveAsync(systemName={SystemName},customer={Customer},storeId={StoreId}): pickupPointProvider → {PickupPointProvider}", systemName, customer, storeId, pickupPointProvider);
		_logger.LogWarning("<<< Exiting IsPluginActiveAsync(systemName={SystemName},customer={Customer},storeId={StoreId})", systemName, customer, storeId);
        return IsPluginActive(pickupPointProvider);
    }

    #endregion
}