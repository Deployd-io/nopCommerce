using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Payments;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Plugins;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Payments;

/// <summary>
/// Represents a payment plugin manager implementation
/// </summary>
public partial class PaymentPluginManager : PluginManager<IPaymentMethod>, IPaymentPluginManager
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly ISettingService _settingService;
    protected readonly PaymentSettings _paymentSettings;

    #endregion

    #region Ctor

    public PaymentPluginManager(ICustomerService customerService,
        IPluginService pluginService,
        ISettingService settingService,
        PaymentSettings paymentSettings) : base(customerService, pluginService)
    {
		_logger.LogDebug(">>> Entering PaymentPluginManager(customerService={CustomerService},pluginService={PluginService},settingService={SettingService},paymentSettings={PaymentSettings})", customerService, pluginService, settingService, paymentSettings);
        _settingService = settingService;
		_logger.LogDebug("PaymentPluginManager(customerService={CustomerService},pluginService={PluginService},settingService={SettingService},paymentSettings={PaymentSettings}): _settingService → {_settingService}", customerService, pluginService, settingService, paymentSettings, _settingService);
        _paymentSettings = paymentSettings;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Load active payment methods
    /// </summary>
    /// <param name="customer">Filter by customer; pass null to load all plugins</param>
    /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
    /// <param name="countryId">Filter by country; pass 0 to load all plugins</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of active payment methods
    /// </returns>
    public virtual async Task<IList<IPaymentMethod>> LoadActivePluginsAsync(Customer customer = null, int storeId = 0,
        int countryId = 0)
    {
		_logger.LogDebug(">>> Entering LoadActivePluginsAsync(customer={Customer},storeId={StoreId},countryId={CountryId})", customer, storeId, countryId);
        var paymentMethods = await LoadActivePluginsAsync(_paymentSettings.ActivePaymentMethodSystemNames, customer, storeId);

        //filter by country
        if (countryId > 0) {
            paymentMethods = await paymentMethods.WhereAwait(async method => !(await GetRestrictedCountryIdsAsync(method)).Contains(countryId)).ToListAsync();
        }

        return paymentMethods;
    }

    /// <summary>
    /// Check whether the passed payment method is active
    /// </summary>
    /// <param name="paymentMethod">Payment method to check</param>
    /// <returns>Result</returns>
    public virtual bool IsPluginActive(IPaymentMethod paymentMethod)
    {
		_logger.LogDebug(">>> Entering IsPluginActive(paymentMethod={PaymentMethod})", paymentMethod);
        return IsPluginActive(paymentMethod, _paymentSettings.ActivePaymentMethodSystemNames);
    }

    /// <summary>
    /// Check whether the payment method with the passed system name is active
    /// </summary>
    /// <param name="systemName">System name of payment method to check</param>
    /// <param name="customer">Filter by customer; pass null to load all plugins</param>
    /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    public virtual async Task<bool> IsPluginActiveAsync(string systemName, Customer customer = null, int storeId = 0)
    {
		_logger.LogDebug(">>> Entering IsPluginActiveAsync(systemName={SystemName},customer={Customer},storeId={StoreId})", systemName, customer, storeId);
        var paymentMethod = await LoadPluginBySystemNameAsync(systemName, customer, storeId);
		_logger.LogDebug("IsPluginActiveAsync(systemName={SystemName},customer={Customer},storeId={StoreId}): paymentMethod → {PaymentMethod}", systemName, customer, storeId, paymentMethod);
		_logger.LogDebug("<<< Exiting IsPluginActiveAsync(systemName={SystemName},customer={Customer},storeId={StoreId})", systemName, customer, storeId);
        return IsPluginActive(paymentMethod);
    }

    /// <summary>
    /// Get countries in which the passed payment method is not allowed
    /// </summary>
    /// <param name="paymentMethod">Payment method</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of country identifiers
    /// </returns>
    public virtual async Task<IList<int>> GetRestrictedCountryIdsAsync(IPaymentMethod paymentMethod)
    {
		_logger.LogDebug(">>> Entering GetRestrictedCountryIdsAsync(paymentMethod={PaymentMethod})", paymentMethod);
        ArgumentNullException.ThrowIfNull(paymentMethod);

        var settingKey = string.Format(NopPaymentDefaults.RestrictedCountriesSettingName, paymentMethod.PluginDescriptor.SystemName);
		_logger.LogDebug("GetRestrictedCountryIdsAsync(paymentMethod={PaymentMethod}): settingKey → {SettingKey}", paymentMethod, settingKey);

		_logger.LogDebug("<<< Exiting GetRestrictedCountryIdsAsync(paymentMethod={PaymentMethod})", paymentMethod);
        return await _settingService.GetSettingByKeyAsync<List<int>>(settingKey) ?? new List<int>();
    }

    /// <summary>
    /// Save countries in which the passed payment method is not allowed
    /// </summary>
    /// <param name="paymentMethod">Payment method</param>
    /// <param name="countryIds">List of country identifiers</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task SaveRestrictedCountriesAsync(IPaymentMethod paymentMethod, IList<int> countryIds)
    {
		_logger.LogDebug(">>> Entering SaveRestrictedCountriesAsync(paymentMethod={PaymentMethod},countryIds={CountryIds})", paymentMethod, countryIds);
        ArgumentNullException.ThrowIfNull(paymentMethod);

        var settingKey = string.Format(NopPaymentDefaults.RestrictedCountriesSettingName, paymentMethod.PluginDescriptor.SystemName);
		_logger.LogDebug("SaveRestrictedCountriesAsync(paymentMethod={PaymentMethod},countryIds={CountryIds}): settingKey → {SettingKey}", paymentMethod, countryIds, settingKey);

        await _settingService.SetSettingAsync(settingKey, countryIds.ToList());
		_logger.LogDebug("<<< Exiting SaveRestrictedCountriesAsync(paymentMethod={PaymentMethod},countryIds={CountryIds})", paymentMethod, countryIds);
    }

    #endregion
}