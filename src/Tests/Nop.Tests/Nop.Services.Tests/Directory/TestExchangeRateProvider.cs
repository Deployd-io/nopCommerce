using Nop.Core.Domain.Directory;
using Nop.Services.Directory;
using Nop.Services.Plugins;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Services.Tests.Directory;

public class TestExchangeRateProvider : BasePlugin, IExchangeRateProvider
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Gets currency live rates
    /// </summary>
    /// <param name="exchangeRateCurrencyCode">Exchange rate currency code</param>
    /// <returns>Exchange rates</returns>
    public Task<IList<ExchangeRate>> GetCurrencyLiveRatesAsync(string exchangeRateCurrencyCode)
    {
		_logger.LogDebug(">>> Entering GetCurrencyLiveRatesAsync(exchangeRateCurrencyCode={ExchangeRateCurrencyCode})", exchangeRateCurrencyCode);
		_logger.LogDebug("<<< Exiting GetCurrencyLiveRatesAsync(exchangeRateCurrencyCode={ExchangeRateCurrencyCode})", exchangeRateCurrencyCode);
        return Task.FromResult<IList<ExchangeRate>>(new List<ExchangeRate>());
    }
}