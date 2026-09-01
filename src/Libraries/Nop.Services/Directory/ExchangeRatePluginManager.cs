using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Services.Customers;
using Nop.Services.Plugins;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Directory;

/// <summary>
/// Represents an exchange rate plugin manager implementation
/// </summary>
public partial class ExchangeRatePluginManager : PluginManager<IExchangeRateProvider>, IExchangeRatePluginManager
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly CurrencySettings _currencySettings;

    #endregion

    #region Ctor

    public ExchangeRatePluginManager(CurrencySettings currencySettings,
        ICustomerService customerService,
        IPluginService pluginService) : base(customerService, pluginService)
    {
		_logger.LogDebug(">>> Entering ExchangeRatePluginManager(currencySettings={CurrencySettings},customerService={CustomerService},pluginService={PluginService})", currencySettings, customerService, pluginService);
        _currencySettings = currencySettings;
		_logger.LogDebug("ExchangeRatePluginManager(currencySettings={CurrencySettings},customerService={CustomerService},pluginService={PluginService}): _currencySettings → {_currencySettings}", currencySettings, customerService, pluginService, _currencySettings);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Load primary active exchange rate provider
    /// </summary>
    /// <param name="customer">Filter by customer; pass null to load all plugins</param>
    /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the exchange rate provider
    /// </returns>
    public virtual async Task<IExchangeRateProvider> LoadPrimaryPluginAsync(Customer customer = null, int storeId = 0)
    {
		_logger.LogDebug(">>> Entering LoadPrimaryPluginAsync(customer={Customer},storeId={StoreId})", customer, storeId);
        return await LoadPrimaryPluginAsync(_currencySettings.ActiveExchangeRateProviderSystemName, customer, storeId);
    }

    /// <summary>
    /// Check whether the passed exchange rate provider is active
    /// </summary>
    /// <param name="exchangeRateProvider">Exchange rate provider to check</param>
    /// <returns>Result</returns>
    public virtual bool IsPluginActive(IExchangeRateProvider exchangeRateProvider)
    {
		_logger.LogDebug(">>> Entering IsPluginActive(exchangeRateProvider={ExchangeRateProvider})", exchangeRateProvider);
        return IsPluginActive(exchangeRateProvider, [_currencySettings.ActiveExchangeRateProviderSystemName]);
    }

    #endregion
}