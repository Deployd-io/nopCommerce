using Nop.Core.Domain.Directory;
using Nop.Services.ScheduleTasks;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Directory;

/// <summary>
/// Represents a task for updating exchange rates
/// </summary>
public partial class UpdateExchangeRateTask : IScheduleTask
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly CurrencySettings _currencySettings;
    protected readonly ICurrencyService _currencyService;

    #endregion

    #region Ctor

    public UpdateExchangeRateTask(CurrencySettings currencySettings,
        ICurrencyService currencyService)
    {
		_logger.LogError(">>> Entering UpdateExchangeRateTask(currencySettings={CurrencySettings},currencyService={CurrencyService})", currencySettings, currencyService);
        _currencySettings = currencySettings;
        _currencyService = currencyService;
		_logger.LogDebug("UpdateExchangeRateTask(currencySettings={CurrencySettings},currencyService={CurrencyService}): _currencySettings → {_currencySettings}", currencySettings, currencyService, _currencySettings);
    _logger.LogWarning("<<< Exiting UpdateExchangeRateTask(currencySettings={CurrencySettings},currencyService={CurrencyService})", currencySettings, currencyService);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Executes a task
    /// </summary>
    public virtual async Task ExecuteAsync()
    {
		_logger.LogError(">>> Entering ExecuteAsync()");
        if (!_currencySettings.AutoUpdateEnabled)
            return;

        var exchangeRates = await _currencyService.GetCurrencyLiveRatesAsync();
		_logger.LogError("ExecuteAsync(): exchangeRates → {ExchangeRates}", exchangeRates);
        foreach (var exchangeRate in exchangeRates)
        {
            var currency = await _currencyService.GetCurrencyByCodeAsync(exchangeRate.CurrencyCode);
			_logger.LogError("ExecuteAsync(): currency → {Currency}", currency);
            if (currency == null)
                continue;

            currency.Rate = exchangeRate.Rate;
			_logger.LogDebug("ExecuteAsync(): currency.Rate → {Arg0}", currency.Rate);
			_logger.LogDebug("ExecuteAsync(): currency.UpdatedOnUtc → {Arg0}", currency.UpdatedOnUtc);
            currency.UpdatedOnUtc = DateTime.UtcNow;
            await _currencyService.UpdateCurrencyAsync(currency);
        }
		_logger.LogWarning("<<< Exiting ExecuteAsync()");
    }

    #endregion
}