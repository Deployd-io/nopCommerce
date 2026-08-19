using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Tax;
using Nop.Services.Customers;
using Nop.Services.Plugins;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Tax;

/// <summary>
/// Represents a tax plugin manager implementation
/// </summary>
public partial class TaxPluginManager : PluginManager<ITaxProvider>, ITaxPluginManager
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly TaxSettings _taxSettings;

    #endregion

    #region Ctor

    public TaxPluginManager(ICustomerService customerService,
        IPluginService pluginService,
        TaxSettings taxSettings) : base(customerService, pluginService)
    {
		_logger.LogDebug(">>> Entering TaxPluginManager(customerService={CustomerService},pluginService={PluginService},taxSettings={TaxSettings})", customerService, pluginService, taxSettings);
        _taxSettings = taxSettings;
		_logger.LogDebug("TaxPluginManager(customerService={CustomerService},pluginService={PluginService},taxSettings={TaxSettings}): _taxSettings → {_taxSettings}", customerService, pluginService, taxSettings, _taxSettings);
		_logger.LogDebug("<<< Exiting TaxPluginManager(customerService={CustomerService},pluginService={PluginService},taxSettings={TaxSettings})", customerService, pluginService, taxSettings);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Load primary active tax provider
    /// </summary>
    /// <param name="customer">Filter by customer; pass null to load all plugins</param>
    /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the ax provider
    /// </returns>
    public virtual async Task<ITaxProvider> LoadPrimaryPluginAsync(Customer customer = null, int storeId = 0)
    {
		_logger.LogDebug(">>> Entering LoadPrimaryPluginAsync(customer={Customer},storeId={StoreId})", customer, storeId);
		_logger.LogDebug("<<< Exiting LoadPrimaryPluginAsync(customer={Customer},storeId={StoreId})", customer, storeId);
        return await LoadPrimaryPluginAsync(_taxSettings.ActiveTaxProviderSystemName, customer, storeId);
    }

    /// <summary>
    /// Check whether the passed tax provider is active
    /// </summary>
    /// <param name="taxProvider">Tax provider to check</param>
    /// <returns>Result</returns>
    public virtual bool IsPluginActive(ITaxProvider taxProvider)
    {
		_logger.LogDebug(">>> Entering IsPluginActive(taxProvider={TaxProvider})", taxProvider);
		_logger.LogDebug("<<< Exiting IsPluginActive(taxProvider={TaxProvider})", taxProvider);
        return IsPluginActive(taxProvider, [_taxSettings.ActiveTaxProviderSystemName]);
    }

    /// <summary>
    /// Check whether the tax provider with the passed system name is active
    /// </summary>
    /// <param name="systemName">System name of tax provider to check</param>
    /// <param name="customer">Filter by customer; pass null to load all plugins</param>
    /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    public virtual async Task<bool> IsPluginActiveAsync(string systemName, Customer customer = null, int storeId = 0)
    {
		_logger.LogDebug(">>> Entering IsPluginActiveAsync(systemName={SystemName},customer={Customer},storeId={StoreId})", systemName, customer, storeId);
        var taxProvider = await LoadPluginBySystemNameAsync(systemName, customer, storeId);
		_logger.LogDebug("IsPluginActiveAsync(systemName={SystemName},customer={Customer},storeId={StoreId}): taxProvider → {TaxProvider}", systemName, customer, storeId, taxProvider);
		_logger.LogDebug("<<< Exiting IsPluginActiveAsync(systemName={SystemName},customer={Customer},storeId={StoreId})", systemName, customer, storeId);
        return IsPluginActive(taxProvider);
    }

    #endregion
}