using Nop.Core.Domain.Directory;
using Nop.Services.Directory;
using Nop.Services.Plugins;

namespace Nop.Tests.Nop.Services.Tests.Directory;

public class TestExchangeRateProvider : BasePlugin, IExchangeRateProvider
{
    /// <summary>
    /// Gets currency live rates
    /// </summary>
    /// <param name="exchangeRateCurrencyCode">Exchange rate currency code</param>
    /// <returns>Exchange rates</returns>
    public Task<IList<ExchangeRate>> GetCurrencyLiveRatesAsync(string exchangeRateCurrencyCode)
    {
		logger.LogDebug(">>> Entering GetCurrencyLiveRatesAsync(exchangeRateCurrencyCode={ExchangeRateCurrencyCode})", exchangeRateCurrencyCode);
		logger.LogDebug("<<< Exiting GetCurrencyLiveRatesAsync(exchangeRateCurrencyCode={ExchangeRateCurrencyCode})", exchangeRateCurrencyCode);
        return Task.FromResult<IList<ExchangeRate>>(new List<ExchangeRate>());
    }
}