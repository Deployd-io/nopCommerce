using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Data;
using Nop.Services.Stores;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Directory;

/// <summary>
/// Currency service
/// </summary>
public partial class CurrencyService : ICurrencyService
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly CurrencySettings _currencySettings;
    protected readonly IExchangeRatePluginManager _exchangeRatePluginManager;
    protected readonly IRepository<Currency> _currencyRepository;
    protected readonly IStoreMappingService _storeMappingService;

    #endregion

    #region Ctor

    public CurrencyService(CurrencySettings currencySettings,
        IExchangeRatePluginManager exchangeRatePluginManager,
        IRepository<Currency> currencyRepository,
        IStoreMappingService storeMappingService)
    {
		_logger.LogDebug(">>> Entering CurrencyService(currencySettings={CurrencySettings},exchangeRatePluginManager={ExchangeRatePluginManager},currencyRepository={CurrencyRepository},storeMappingService=<redacted>)", currencySettings, exchangeRatePluginManager, currencyRepository);
        _currencySettings = currencySettings;
        _exchangeRatePluginManager = exchangeRatePluginManager;
		_logger.LogDebug("CurrencyService(currencySettings={CurrencySettings},exchangeRatePluginManager={ExchangeRatePluginManager},currencyRepository={CurrencyRepository},storeMappingService=<redacted>): _exchangeRatePluginManager → {_exchangeRatePluginManager}", currencySettings, exchangeRatePluginManager, currencyRepository, _exchangeRatePluginManager);
		_logger.LogDebug("CurrencyService(currencySettings={CurrencySettings},exchangeRatePluginManager={ExchangeRatePluginManager},currencyRepository={CurrencyRepository},storeMappingService=<redacted>): _currencySettings → {_currencySettings}", currencySettings, exchangeRatePluginManager, currencyRepository, _currencySettings);
        _currencyRepository = currencyRepository;
		_logger.LogDebug("CurrencyService(currencySettings={CurrencySettings},exchangeRatePluginManager={ExchangeRatePluginManager},currencyRepository={CurrencyRepository},storeMappingService=<redacted>): _currencyRepository → {_currencyRepository}", currencySettings, exchangeRatePluginManager, currencyRepository, _currencyRepository);
		_logger.LogDebug("<<< Exiting CurrencyService(currencySettings={CurrencySettings},exchangeRatePluginManager={ExchangeRatePluginManager},currencyRepository={CurrencyRepository},storeMappingService=<redacted>)", currencySettings, exchangeRatePluginManager, currencyRepository);
        _storeMappingService = storeMappingService;
    }
	_logger.LogDebug("CurrencyService(currencySettings={CurrencySettings},exchangeRatePluginManager={ExchangeRatePluginManager},currencyRepository={CurrencyRepository},storeMappingService=<redacted>): _storeMappingService → {_storeMappingService}", currencySettings, exchangeRatePluginManager, currencyRepository, _storeMappingService);

    #endregion

    #region Methods

    #region Currency

    /// <summary>
    /// Deletes currency
    /// </summary>
    /// <param name="currency">Currency</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteCurrencyAsync(Currency currency)
    {
		_logger.LogDebug(">>> Entering DeleteCurrencyAsync(currency={Currency})", currency);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _currencyRepository.DeleteAsync(currency);
		_logger.LogInformation("DeleteCurrencyAsync(currency)={Currency}: external service call took {Arg1} ms", currency, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting DeleteCurrencyAsync(currency={Currency})", currency);
    }

    /// <summary>
    /// Gets a currency
    /// </summary>
    /// <param name="currencyId">Currency identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the currency
    /// </returns>
    public virtual async Task<Currency> GetCurrencyByIdAsync(int currencyId)
    {
		_logger.LogDebug(">>> Entering GetCurrencyByIdAsync(currencyId={CurrencyId})", currencyId);
		_logger.LogDebug("<<< Exiting GetCurrencyByIdAsync(currencyId={CurrencyId})", currencyId);
        return await _currencyRepository.GetByIdAsync(currencyId, cache => default);
    }

    /// <summary>
    /// Gets a currency by code
    /// </summary>
    /// <param name="currencyCode">Currency code</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the currency
    /// </returns>
    public virtual async Task<Currency> GetCurrencyByCodeAsync(string currencyCode)
    {
		_logger.LogDebug(">>> Entering GetCurrencyByCodeAsync(currencyCode={CurrencyCode})", currencyCode);
        if (string.IsNullOrEmpty(currencyCode))
		_logger.LogDebug("<<< Exiting GetCurrencyByCodeAsync(currencyCode={CurrencyCode})", currencyCode);
            return null;

        return (await GetAllCurrenciesAsync(true))
            .FirstOrDefault(c => c.CurrencyCode.ToLowerInvariant() == currencyCode.ToLowerInvariant());
    }

    /// <summary>
    /// Gets all currencies
    /// </summary>
    /// <param name="showHidden">A value indicating whether to show hidden records</param>
    /// <param name="storeId">Load records allowed only in a specified store; pass 0 to load all records</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the currencies
    /// </returns>
    public virtual async Task<IList<Currency>> GetAllCurrenciesAsync(bool showHidden = false, int storeId = 0)
    {
		_logger.LogDebug(">>> Entering GetAllCurrenciesAsync(showHidden={ShowHidden},storeId={StoreId})", showHidden, storeId);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var currencies = await _currencyRepository.GetAllAsync(query =>
        {
            if (!showHidden)
                query = query.Where(c => c.Published);

			_logger.LogInformation("GetAllCurrenciesAsync(showHidden,storeId)={ShowHidden},{StoreId}: find query executed in {Arg2} ms", showHidden, storeId, __logStart.ElapsedMilliseconds);
            query = query.OrderBy(c => c.DisplayOrder).ThenBy(c => c.Id);

            return query;
        }, cache => cache.PrepareKeyForDefaultCache(NopDirectoryDefaults.CurrenciesAllCacheKey, showHidden));

        //store mapping
        if (storeId > 0)
            currencies = await currencies
                .WhereAwait(async c => await _storeMappingService.AuthorizeAsync(c, storeId))
                .ToListAsync();
				_logger.LogInformation("GetAllCurrenciesAsync(showHidden,storeId)={ShowHidden},{StoreId}: database query executed in {Arg2} ms", showHidden, storeId, __logStart.ElapsedMilliseconds);

		_logger.LogDebug("<<< Exiting GetAllCurrenciesAsync(showHidden={ShowHidden},storeId={StoreId})", showHidden, storeId);
        return currencies;
    }

    /// <summary>
    /// Inserts a currency
    /// </summary>
    /// <param name="currency">Currency</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertCurrencyAsync(Currency currency)
    {
		_logger.LogDebug(">>> Entering InsertCurrencyAsync(currency={Currency})", currency);
        await _currencyRepository.InsertAsync(currency);
		_logger.LogDebug("<<< Exiting InsertCurrencyAsync(currency={Currency})", currency);
    }

    /// <summary>
    /// Updates the currency
    /// </summary>
    /// <param name="currency">Currency</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateCurrencyAsync(Currency currency)
    {
		_logger.LogDebug(">>> Entering UpdateCurrencyAsync(currency={Currency})", currency);
        await _currencyRepository.UpdateAsync(currency);
		_logger.LogDebug("<<< Exiting UpdateCurrencyAsync(currency={Currency})", currency);
    }

    #endregion

    #region Conversions

    /// <summary>
    /// Gets live rates regarding the passed currency
    /// </summary>
    /// <param name="currencyCode">Currency code; pass null to use primary exchange rate currency</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the exchange rates
    /// </returns>
    public virtual async Task<IList<ExchangeRate>> GetCurrencyLiveRatesAsync(string currencyCode = null)
    {
		_logger.LogDebug(">>> Entering GetCurrencyLiveRatesAsync(currencyCode={CurrencyCode})", currencyCode);
        var exchangeRateProvider = await _exchangeRatePluginManager.LoadPrimaryPluginAsync()
                                   ?? throw new Exception("Active exchange rate provider cannot be loaded");

        currencyCode ??= (await GetCurrencyByIdAsync(_currencySettings.PrimaryExchangeRateCurrencyId))?.CurrencyCode
                         ?? throw new NopException("Primary exchange rate currency is not set");

		_logger.LogDebug("<<< Exiting GetCurrencyLiveRatesAsync(currencyCode={CurrencyCode})", currencyCode);
        return await exchangeRateProvider.GetCurrencyLiveRatesAsync(currencyCode);
    }

    /// <summary>
    /// Converts currency
    /// </summary>
    /// <param name="amount">Amount</param>
    /// <param name="exchangeRate">Currency exchange rate</param>
    /// <returns>Converted value</returns>
    public virtual decimal ConvertCurrency(decimal amount, decimal exchangeRate)
    {
		_logger.LogDebug(">>> Entering ConvertCurrency(amount={Amount},exchangeRate={ExchangeRate})", amount, exchangeRate);
        if (amount != decimal.Zero && exchangeRate != decimal.Zero)
		_logger.LogDebug("<<< Exiting ConvertCurrency(amount={Amount},exchangeRate={ExchangeRate})", amount, exchangeRate);
            return amount * exchangeRate;

        return decimal.Zero;
    }

    /// <summary>
    /// Converts to primary store currency 
    /// </summary>
    /// <param name="amount">Amount</param>
    /// <param name="sourceCurrencyCode">Source currency code</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the converted value
    /// </returns>
    public virtual async Task<decimal> ConvertToPrimaryStoreCurrencyAsync(decimal amount, Currency sourceCurrencyCode)
    {
		_logger.LogDebug(">>> Entering ConvertToPrimaryStoreCurrencyAsync(amount={Amount},sourceCurrencyCode={SourceCurrencyCode})", amount, sourceCurrencyCode);
        ArgumentNullException.ThrowIfNull(sourceCurrencyCode);

        var primaryStoreCurrency = await GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId);
		_logger.LogDebug("ConvertToPrimaryStoreCurrencyAsync(amount={Amount},sourceCurrencyCode={SourceCurrencyCode}): primaryStoreCurrency → {PrimaryStoreCurrency}", amount, sourceCurrencyCode, primaryStoreCurrency);
        var result = await ConvertCurrencyAsync(amount, sourceCurrencyCode, primaryStoreCurrency);
		_logger.LogDebug("ConvertToPrimaryStoreCurrencyAsync(amount={Amount},sourceCurrencyCode={SourceCurrencyCode}): result → {Result}", amount, sourceCurrencyCode, result);

		_logger.LogDebug("<<< Exiting ConvertToPrimaryStoreCurrencyAsync(amount={Amount},sourceCurrencyCode={SourceCurrencyCode})", amount, sourceCurrencyCode);
        return result;
    }

    /// <summary>
    /// Converts from primary store currency
    /// </summary>
    /// <param name="amount">Amount</param>
    /// <param name="targetCurrencyCode">Target currency code</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the converted value
    /// </returns>
    public virtual async Task<decimal> ConvertFromPrimaryStoreCurrencyAsync(decimal amount, Currency targetCurrencyCode)
    {
		_logger.LogDebug(">>> Entering ConvertFromPrimaryStoreCurrencyAsync(amount={Amount},targetCurrencyCode={TargetCurrencyCode})", amount, targetCurrencyCode);
        var primaryStoreCurrency = await GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId);
        var result = await ConvertCurrencyAsync(amount, primaryStoreCurrency, targetCurrencyCode);
		_logger.LogDebug("ConvertFromPrimaryStoreCurrencyAsync(amount={Amount},targetCurrencyCode={TargetCurrencyCode}): result → {Result}", amount, targetCurrencyCode, result);
		_logger.LogDebug("ConvertFromPrimaryStoreCurrencyAsync(amount={Amount},targetCurrencyCode={TargetCurrencyCode}): primaryStoreCurrency → {PrimaryStoreCurrency}", amount, targetCurrencyCode, primaryStoreCurrency);

		_logger.LogDebug("<<< Exiting ConvertFromPrimaryStoreCurrencyAsync(amount={Amount},targetCurrencyCode={TargetCurrencyCode})", amount, targetCurrencyCode);
		_logger.LogDebug(">>> Entering ConvertCurrencyAsync(amount={Amount},sourceCurrencyCode={SourceCurrencyCode},targetCurrencyCode={TargetCurrencyCode})", amount, sourceCurrencyCode, targetCurrencyCode);
		_logger.LogDebug("<<< Exiting ConvertCurrencyAsync(amount={Amount},sourceCurrencyCode={SourceCurrencyCode},targetCurrencyCode={TargetCurrencyCode})", amount, sourceCurrencyCode, targetCurrencyCode);
        return result;
    }

    /// <summary>
    /// Converts currency
    /// </summary>
    /// <param name="amount">Amount</param>
    /// <param name="sourceCurrencyCode">Source currency code</param>
    /// <param name="targetCurrencyCode">Target currency code</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the converted value
    /// </returns>
    public virtual async Task<decimal> ConvertCurrencyAsync(decimal amount, Currency sourceCurrencyCode, Currency targetCurrencyCode)
    {
        ArgumentNullException.ThrowIfNull(sourceCurrencyCode);

        ArgumentNullException.ThrowIfNull(targetCurrencyCode);

        var result = amount;
		_logger.LogDebug("ConvertCurrencyAsync(amount={Amount},sourceCurrencyCode={SourceCurrencyCode},targetCurrencyCode={TargetCurrencyCode}): result → {Result}", amount, sourceCurrencyCode, targetCurrencyCode, result);

        if (result == decimal.Zero || sourceCurrencyCode.Id == targetCurrencyCode.Id)
            return result;

        result = await ConvertToPrimaryExchangeRateCurrencyAsync(result, sourceCurrencyCode);
        result = await ConvertFromPrimaryExchangeRateCurrencyAsync(result, targetCurrencyCode);
        return result;
    }

    /// <summary>
    /// Converts to primary exchange rate currency 
    /// </summary>
    /// <param name="amount">Amount</param>
    /// <param name="sourceCurrencyCode">Source currency code</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the converted value
    /// </returns>
    public virtual async Task<decimal> ConvertToPrimaryExchangeRateCurrencyAsync(decimal amount, Currency sourceCurrencyCode)
    {
		_logger.LogDebug(">>> Entering ConvertToPrimaryExchangeRateCurrencyAsync(amount={Amount},sourceCurrencyCode={SourceCurrencyCode})", amount, sourceCurrencyCode);
        ArgumentNullException.ThrowIfNull(sourceCurrencyCode);

        var primaryExchangeRateCurrency = await GetCurrencyByIdAsync(_currencySettings.PrimaryExchangeRateCurrencyId) ?? throw new Exception("Primary exchange rate currency cannot be loaded");
		_logger.LogDebug("ConvertToPrimaryExchangeRateCurrencyAsync(amount={Amount},sourceCurrencyCode={SourceCurrencyCode}): primaryExchangeRateCurrency → {PrimaryExchangeRateCurrency}", amount, sourceCurrencyCode, primaryExchangeRateCurrency);

        var result = amount;
		_logger.LogDebug("ConvertToPrimaryExchangeRateCurrencyAsync(amount={Amount},sourceCurrencyCode={SourceCurrencyCode}): result → {Result}", amount, sourceCurrencyCode, result);
        if (result == decimal.Zero || sourceCurrencyCode.Id == primaryExchangeRateCurrency.Id)
            return result;

        var exchangeRate = sourceCurrencyCode.Rate;
        if (exchangeRate == decimal.Zero)
			_logger.LogWarning("Throwing NopException from ConvertToPrimaryExchangeRateCurrencyAsync(amount={Amount},sourceCurrencyCode={SourceCurrencyCode})", amount, sourceCurrencyCode);
            throw new NopException($"Exchange rate not found for currency [{sourceCurrencyCode.Name}]");
        result /= exchangeRate;

		_logger.LogDebug("<<< Exiting ConvertToPrimaryExchangeRateCurrencyAsync(amount={Amount},sourceCurrencyCode={SourceCurrencyCode})", amount, sourceCurrencyCode);
        return result;
    }

    /// <summary>
    /// Converts from primary exchange rate currency
    /// </summary>
    /// <param name="amount">Amount</param>
    /// <param name="targetCurrencyCode">Target currency code</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the converted value
    /// </returns>
    public virtual async Task<decimal> ConvertFromPrimaryExchangeRateCurrencyAsync(decimal amount, Currency targetCurrencyCode)
    {
		_logger.LogDebug(">>> Entering ConvertFromPrimaryExchangeRateCurrencyAsync(amount={Amount},targetCurrencyCode={TargetCurrencyCode})", amount, targetCurrencyCode);
        ArgumentNullException.ThrowIfNull(targetCurrencyCode);

        var primaryExchangeRateCurrency = await GetCurrencyByIdAsync(_currencySettings.PrimaryExchangeRateCurrencyId) ?? throw new Exception("Primary exchange rate currency cannot be loaded");
		_logger.LogDebug("ConvertFromPrimaryExchangeRateCurrencyAsync(amount={Amount},targetCurrencyCode={TargetCurrencyCode}): primaryExchangeRateCurrency → {PrimaryExchangeRateCurrency}", amount, targetCurrencyCode, primaryExchangeRateCurrency);

        var result = amount;
		_logger.LogDebug("ConvertFromPrimaryExchangeRateCurrencyAsync(amount={Amount},targetCurrencyCode={TargetCurrencyCode}): result → {Result}", amount, targetCurrencyCode, result);
        if (result == decimal.Zero || targetCurrencyCode.Id == primaryExchangeRateCurrency.Id)
            return result;

        var exchangeRate = targetCurrencyCode.Rate;
        if (exchangeRate == decimal.Zero)
			_logger.LogWarning("Throwing NopException from ConvertFromPrimaryExchangeRateCurrencyAsync(amount={Amount},targetCurrencyCode={TargetCurrencyCode})", amount, targetCurrencyCode);
            throw new NopException($"Exchange rate not found for currency [{targetCurrencyCode.Name}]");
        result *= exchangeRate;

		_logger.LogDebug("<<< Exiting ConvertFromPrimaryExchangeRateCurrencyAsync(amount={Amount},targetCurrencyCode={TargetCurrencyCode})", amount, targetCurrencyCode);
        return result;
    }

    #endregion

    #endregion
}