using Nop.Core.Domain.Customers;
using Nop.Services.Customers;
using Nop.Services.Plugins;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Authentication.MultiFactor;

/// <summary>
/// Represents an multi-factor authentication plugin manager implementation
/// </summary>
public partial class MultiFactorAuthenticationPluginManager : PluginManager<IMultiFactorAuthenticationMethod>, IMultiFactorAuthenticationPluginManager
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly MultiFactorAuthenticationSettings _multiFactorAuthenticationSettings;

    #endregion

    #region Ctor

    public MultiFactorAuthenticationPluginManager(MultiFactorAuthenticationSettings multiFactorAuthenticationSettings,
        ICustomerService customerService,
        IPluginService pluginService) : base(customerService, pluginService)
    {
		_logger.LogDebug(">>> Entering MultiFactorAuthenticationPluginManager(multiFactorAuthenticationSettings={MultiFactorAuthenticationSettings},customerService={CustomerService},pluginService={PluginService})", multiFactorAuthenticationSettings, customerService, pluginService);
        _multiFactorAuthenticationSettings = multiFactorAuthenticationSettings;
		_logger.LogDebug("<<< Exiting MultiFactorAuthenticationPluginManager(multiFactorAuthenticationSettings={MultiFactorAuthenticationSettings},customerService={CustomerService},pluginService={PluginService})", multiFactorAuthenticationSettings, customerService, pluginService);
		_logger.LogDebug("MultiFactorAuthenticationPluginManager(multiFactorAuthenticationSettings={MultiFactorAuthenticationSettings},customerService={CustomerService},pluginService={PluginService}): _multiFactorAuthenticationSettings → {_multiFactorAuthenticationSettings}", multiFactorAuthenticationSettings, customerService, pluginService, _multiFactorAuthenticationSettings);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Check is active multi-factor authentication methods
    /// </summary>
    /// <param name="customer">Filter by customer; pass null to load all plugins</param>
    /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains true if we have active multi-factor authentication methods
    /// </returns>
    public virtual async Task<bool> HasActivePluginsAsync(Customer customer = null, int storeId = 0)
    {
		_logger.LogDebug(">>> Entering HasActivePluginsAsync(customer={Customer},storeId={StoreId})", customer, storeId);
		_logger.LogDebug("<<< Exiting HasActivePluginsAsync(customer={Customer},storeId={StoreId})", customer, storeId);
        return (await LoadActivePluginsAsync(_multiFactorAuthenticationSettings.ActiveAuthenticationMethodSystemNames, customer, storeId)).Any();
    }

    /// <summary>
    /// Load active multi-factor authentication methods
    /// </summary>
    /// <param name="customer">Filter by customer; pass null to load all plugins</param>
    /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of active multi-factor authentication methods
    /// </returns>
    public virtual async Task<IList<IMultiFactorAuthenticationMethod>> LoadActivePluginsAsync(Customer customer = null, int storeId = 0)
    {
		_logger.LogDebug(">>> Entering LoadActivePluginsAsync(customer={Customer},storeId={StoreId})", customer, storeId);
		_logger.LogDebug("<<< Exiting LoadActivePluginsAsync(customer={Customer},storeId={StoreId})", customer, storeId);
        return await LoadActivePluginsAsync(_multiFactorAuthenticationSettings.ActiveAuthenticationMethodSystemNames, customer, storeId);
    }

    /// <summary>
    /// Check whether the passed multi-factor authentication method is active
    /// </summary>
    /// <param name="authenticationMethod">Authentication method to check</param>
    /// <returns>Result</returns>
    public virtual bool IsPluginActive(IMultiFactorAuthenticationMethod authenticationMethod)
    {
        return IsPluginActive(authenticationMethod, _multiFactorAuthenticationSettings.ActiveAuthenticationMethodSystemNames);
    }

    /// <summary>
    /// Check whether the multi-factor authentication method with the passed system name is active
    /// </summary>
    /// <param name="systemName">System name of authentication method to check</param>
    /// <param name="customer">Filter by customer; pass null to load all plugins</param>
    /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    public virtual async Task<bool> IsPluginActiveAsync(string systemName, Customer customer = null, int storeId = 0)
    {
		_logger.LogDebug(">>> Entering IsPluginActiveAsync(systemName={SystemName},customer={Customer},storeId={StoreId})", systemName, customer, storeId);
        var authenticationMethod = await LoadPluginBySystemNameAsync(systemName, customer, storeId);
		_logger.LogDebug("IsPluginActiveAsync(systemName={SystemName},customer={Customer},storeId={StoreId}): authenticationMethod → {AuthenticationMethod}", systemName, customer, storeId, authenticationMethod);
		_logger.LogDebug("<<< Exiting IsPluginActiveAsync(systemName={SystemName},customer={Customer},storeId={StoreId})", systemName, customer, storeId);
        return IsPluginActive(authenticationMethod);
    }

    #endregion
}