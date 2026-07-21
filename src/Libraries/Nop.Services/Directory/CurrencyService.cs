using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Data;
using Nop.Services.Stores;

namespace Nop.Services.Directory;

/// <summary>
/// Currency service
/// </summary>
public partial class CurrencyService : ICurrencyService
{
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
        _currencySettings = currencySettings;
        _exchangeRatePluginManager = exchangeRatePluginManager;
        _currencyRepository = currencyRepository;
        _storeMappingService = storeMappingService;
    }

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
		logger.LogDebug(">>> Entering DeleteCurrencyAsync(currency={Currency})", currency);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _currencyRepository.DeleteAsync(currency);
		logger.LogDebug("<<< Exiting DeleteCurrencyAsync(currency={Currency})", currency);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("DeleteCurrencyAsync(currency)={Currency}: external service call took {Arg1} ms", currency, __logStart.ElapsedMilliseconds);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("DeleteCurrencyAsync(currency)={Currency}: delete query executed in {Arg1} ms", currency, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("DeleteCurrencyAsync(currency)={Currency}: external service call took {Arg1} ms", currency, __logStart.ElapsedMilliseconds);
	var __logStart = System.Diagnostics.Stopwatch.StartNew();
	logger.LogInformation("DeleteCurrencyAsync(currency)={Currency}: delete query executed in {Arg1} ms", currency, __logStart.ElapsedMilliseconds);
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
		logger.LogDebug(">>> Entering GetCurrencyByIdAsync(currencyId={CurrencyId})", currencyId);
		logger.LogDebug("<<< Exiting GetCurrencyByIdAsync(currencyId={CurrencyId})", currencyId);
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
		logger.LogDebug(">>> Entering GetCurrencyByCodeAsync(currencyCode={CurrencyCode})", currencyCode);
        if (string.IsNullOrEmpty(currencyCode))
		logger.LogDebug("<<< Exiting GetCurrencyByCodeAsync(currencyCode={CurrencyCode})", currencyCode);
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
		logger.LogDebug(">>> Entering GetAllCurrenciesAsync(showHidden={ShowHidden},storeId={StoreId})", showHidden, storeId);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var currencies = await _currencyRepository.GetAllAsync(query =>
        {
            if (!showHidden)
                query = query.Where(c => c.Published);

			logger.LogInformation("GetAllCurrenciesAsync(showHidden,storeId)={ShowHidden},{StoreId}: find query executed in {Arg2} ms", showHidden, storeId, __logStart.ElapsedMilliseconds);
            query = query.OrderBy(c => c.DisplayOrder).ThenBy(c => c.Id);

            return query;
        }, cache => cache.PrepareKeyForDefaultCache(NopDirectoryDefaults.CurrenciesAllCacheKey, showHidden));

        //store mapping
        if (storeId > 0)
            currencies = await currencies
                .WhereAwait(async c => await _storeMappingService.AuthorizeAsync(c, storeId))
                .ToListAsync();
				logger.LogInformation("GetAllCurrenciesAsync(showHidden,storeId)={ShowHidden},{StoreId}: database query executed in {Arg2} ms", showHidden, storeId, __logStart.ElapsedMilliseconds);

		logger.LogDebug("<<< Exiting GetAllCurrenciesAsync(showHidden={ShowHidden},storeId={StoreId})", showHidden, storeId);
        return currencies;
    }

    /// <summary>
    /// Inserts a currency
    /// </summary>
    /// <param name="currency">Currency</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertCurrencyAsync(Currency currency)
    {
		logger.LogDebug(">>> Entering InsertCurrencyAsync(currency={Currency})", currency);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _currencyRepository.InsertAsync(currency);
		logger.LogDebug("<<< Exiting InsertCurrencyAsync(currency={Currency})", currency);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("InsertCurrencyAsync(currency)={Currency}: save query executed in {Arg1} ms", currency, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("InsertCurrencyAsync(currency)={Currency}: save query executed in {Arg1} ms", currency, __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Updates the currency
    /// </summary>
    /// <param name="currency">Currency</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateCurrencyAsync(Currency currency)
    {
		logger.LogDebug(">>> Entering UpdateCurrencyAsync(currency={Currency})", currency);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _currencyRepository.UpdateAsync(currency);
		logger.LogDebug("<<< Exiting UpdateCurrencyAsync(currency={Currency})", currency);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("UpdateCurrencyAsync(currency)={Currency}: update query executed in {Arg1} ms", currency, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("UpdateCurrencyAsync(currency)={Currency}: update query executed in {Arg1} ms", currency, __logStart.ElapsedMilliseconds);
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
		logger.LogDebug(">>> Entering GetCurrencyLiveRatesAsync(currencyCode={CurrencyCode})", currencyCode);
        var exchangeRateProvider = await _exchangeRatePluginManager.LoadPrimaryPluginAsync()
                                   ?? throw new Exception("Active exchange rate provider cannot be loaded");

        currencyCode ??= (await GetCurrencyByIdAsync(_currencySettings.PrimaryExchangeRateCurrencyId))?.CurrencyCode
                         ?? throw new NopException("Primary exchange rate currency is not set");

		logger.LogDebug("<<< Exiting GetCurrencyLiveRatesAsync(currencyCode={CurrencyCode})", currencyCode);
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
        if (amount != decimal.Zero && exchangeRate != decimal.Zero)
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
		logger.LogDebug(">>> Entering ConvertToPrimaryStoreCurrencyAsync(amount={Amount},sourceCurrencyCode={SourceCurrencyCode})", amount, sourceCurrencyCode);
        ArgumentNullException.ThrowIfNull(sourceCurrencyCode);

        var primaryStoreCurrency = await GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId);
        var result = await ConvertCurrencyAsync(amount, sourceCurrencyCode, primaryStoreCurrency);
		logger.LogDebug("ConvertToPrimaryStoreCurrencyAsync(amount={Amount},sourceCurrencyCode={SourceCurrencyCode}): primaryStoreCurrency → {PrimaryStoreCurrency}", amount, sourceCurrencyCode, primaryStoreCurrency);
		logger.LogDebug("ConvertToPrimaryStoreCurrencyAsync(amount={Amount},sourceCurrencyCode={SourceCurrencyCode}): result → {Result}", amount, sourceCurrencyCode, result);

		logger.LogDebug("<<< Exiting ConvertToPrimaryStoreCurrencyAsync(amount={Amount},sourceCurrencyCode={SourceCurrencyCode})", amount, sourceCurrencyCode);
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
		logger.LogDebug(">>> Entering ConvertFromPrimaryStoreCurrencyAsync(amount={Amount},targetCurrencyCode={TargetCurrencyCode})", amount, targetCurrencyCode);
        var primaryStoreCurrency = await GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId);
        var result = await ConvertCurrencyAsync(amount, primaryStoreCurrency, targetCurrencyCode);
		logger.LogDebug("ConvertFromPrimaryStoreCurrencyAsync(amount={Amount},targetCurrencyCode={TargetCurrencyCode}): primaryStoreCurrency → {PrimaryStoreCurrency}", amount, targetCurrencyCode, primaryStoreCurrency);
		logger.LogDebug("ConvertFromPrimaryStoreCurrencyAsync(amount={Amount},targetCurrencyCode={TargetCurrencyCode}): result → {Result}", amount, targetCurrencyCode, result);

		logger.LogDebug("<<< Exiting ConvertFromPrimaryStoreCurrencyAsync(amount={Amount},targetCurrencyCode={TargetCurrencyCode})", amount, targetCurrencyCode);
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
		logger.LogDebug(">>> Entering ConvertCurrencyAsync(amount={Amount},sourceCurrencyCode={SourceCurrencyCode},targetCurrencyCode={TargetCurrencyCode})", amount, sourceCurrencyCode, targetCurrencyCode);
        ArgumentNullException.ThrowIfNull(sourceCurrencyCode);

        ArgumentNullException.ThrowIfNull(targetCurrencyCode);

        var result = amount;

		logger.LogDebug("ConvertCurrencyAsync(amount={Amount},sourceCurrencyCode={SourceCurrencyCode},targetCurrencyCode={TargetCurrencyCode}): result → {Result}", amount, sourceCurrencyCode, targetCurrencyCode, result);
        if (result == decimal.Zero || sourceCurrencyCode.Id == targetCurrencyCode.Id)
            return result;

        result = await ConvertToPrimaryExchangeRateCurrencyAsync(result, sourceCurrencyCode);
        result = await ConvertFromPrimaryExchangeRateCurrencyAsync(result, targetCurrencyCode);
		logger.LogDebug("<<< Exiting ConvertCurrencyAsync(amount={Amount},sourceCurrencyCode={SourceCurrencyCode},targetCurrencyCode={TargetCurrencyCode})", amount, sourceCurrencyCode, targetCurrencyCode);
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
		logger.LogDebug(">>> Entering ConvertToPrimaryExchangeRateCurrencyAsync(amount={Amount},sourceCurrencyCode={SourceCurrencyCode})", amount, sourceCurrencyCode);
        ArgumentNullException.ThrowIfNull(sourceCurrencyCode);

        var primaryExchangeRateCurrency = await GetCurrencyByIdAsync(_currencySettings.PrimaryExchangeRateCurrencyId) ?? throw new Exception("Primary exchange rate currency cannot be loaded");

		logger.LogDebug("ConvertToPrimaryExchangeRateCurrencyAsync(amount={Amount},sourceCurrencyCode={SourceCurrencyCode}): primaryExchangeRateCurrency → {PrimaryExchangeRateCurrency}", amount, sourceCurrencyCode, primaryExchangeRateCurrency);
        var result = amount;
        if (result == decimal.Zero || sourceCurrencyCode.Id == primaryExchangeRateCurrency.Id)
			logger.LogDebug("ConvertToPrimaryExchangeRateCurrencyAsync(amount={Amount},sourceCurrencyCode={SourceCurrencyCode}): result → {Result}", amount, sourceCurrencyCode, result);
            return result;

        var exchangeRate = sourceCurrencyCode.Rate;
        if (exchangeRate == decimal.Zero)
            throw new NopException($"Exchange rate not found for currency [{sourceCurrencyCode.Name}]");
        result /= exchangeRate;

		logger.LogDebug("<<< Exiting ConvertToPrimaryExchangeRateCurrencyAsync(amount={Amount},sourceCurrencyCode={SourceCurrencyCode})", amount, sourceCurrencyCode);
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
		logger.LogDebug(">>> Entering ConvertFromPrimaryExchangeRateCurrencyAsync(amount={Amount},targetCurrencyCode={TargetCurrencyCode})", amount, targetCurrencyCode);
        ArgumentNullException.ThrowIfNull(targetCurrencyCode);

        var primaryExchangeRateCurrency = await GetCurrencyByIdAsync(_currencySettings.PrimaryExchangeRateCurrencyId) ?? throw new Exception("Primary exchange rate currency cannot be loaded");

		logger.LogDebug("ConvertFromPrimaryExchangeRateCurrencyAsync(amount={Amount},targetCurrencyCode={TargetCurrencyCode}): primaryExchangeRateCurrency → {PrimaryExchangeRateCurrency}", amount, targetCurrencyCode, primaryExchangeRateCurrency);
        var result = amount;
        if (result == decimal.Zero || targetCurrencyCode.Id == primaryExchangeRateCurrency.Id)
			logger.LogDebug("ConvertFromPrimaryExchangeRateCurrencyAsync(amount={Amount},targetCurrencyCode={TargetCurrencyCode}): result → {Result}", amount, targetCurrencyCode, result);
            return result;

        var exchangeRate = targetCurrencyCode.Rate;
        if (exchangeRate == decimal.Zero)
            throw new NopException($"Exchange rate not found for currency [{targetCurrencyCode.Name}]");
        result *= exchangeRate;

		logger.LogDebug("<<< Exiting ConvertFromPrimaryExchangeRateCurrencyAsync(amount={Amount},targetCurrencyCode={TargetCurrencyCode})", amount, targetCurrencyCode);
        return result;
    }

    #endregion

    #endregion
}
