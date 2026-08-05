using Nop.Core.Domain.Directory;
using Nop.Services.ScheduleTasks;

namespace Nop.Services.Directory;

/// <summary>
/// Represents a task for updating exchange rates
/// </summary>
public partial class UpdateExchangeRateTask : IScheduleTask
{
    #region Fields

    protected readonly CurrencySettings _currencySettings;
    protected readonly ICurrencyService _currencyService;

    #endregion

    #region Ctor

    public UpdateExchangeRateTask(CurrencySettings currencySettings,
        ICurrencyService currencyService)
    {
        _currencySettings = currencySettings;
        _currencyService = currencyService;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Executes a task
    /// </summary>
    public virtual async Task ExecuteAsync()
    {
		logger.LogDebug(">>> Entering ExecuteAsync()");
        if (!_currencySettings.AutoUpdateEnabled)
            return;

        var exchangeRates = await _currencyService.GetCurrencyLiveRatesAsync();
        foreach (var exchangeRate in exchangeRates)
        {
		logger.LogDebug("ExecuteAsync(): exchangeRates → {ExchangeRates}", exchangeRates);
            var currency = await _currencyService.GetCurrencyByCodeAsync(exchangeRate.CurrencyCode);
            if (currency == null)
				logger.LogDebug("ExecuteAsync(): currency → {Currency}", currency);
                continue;

            currency.Rate = exchangeRate.Rate;
			logger.LogDebug("ExecuteAsync(): currency.Rate → {Arg0}", currency.Rate);
			logger.LogDebug("ExecuteAsync(): currency.UpdatedOnUtc → {Arg0}", currency.UpdatedOnUtc);
            currency.UpdatedOnUtc = DateTime.UtcNow;
            await _currencyService.UpdateCurrencyAsync(currency);
        }
		logger.LogDebug("<<< Exiting ExecuteAsync()");
    }

    #endregion
}