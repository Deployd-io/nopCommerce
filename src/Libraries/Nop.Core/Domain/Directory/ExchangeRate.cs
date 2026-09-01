using Microsoft.Extensions.Logging;
namespace Nop.Core.Domain.Directory;

/// <summary>
/// Represents an exchange rate
/// </summary>
public partial class ExchangeRate
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Creates a new instance of the ExchangeRate class
    /// </summary>
    public ExchangeRate()
    {
		_logger.LogDebug(">>> Entering ExchangeRate()");
        CurrencyCode = string.Empty;
		_logger.LogDebug("ExchangeRate(): CurrencyCode → {CurrencyCode}", CurrencyCode);
        Rate = 1.0m;
    }

    /// <summary>
    /// The three letter ISO code for the Exchange Rate, e.g. USD
    /// </summary>
    public string CurrencyCode { get; set; }

    /// <summary>
    /// The conversion rate of this currency from the base currency
    /// </summary>
    public decimal Rate { get; set; }

    /// <summary>
    /// When was this exchange rate updated from the data source (the data XML feed)
    /// </summary>
    public DateTime UpdatedOn { get; set; }

    /// <summary>
    /// Format the rate into a string with the currency code, e.g. "USD 0.72543"
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
		_logger.LogDebug(">>> Entering ToString()");
        return $"{CurrencyCode} {Rate}";
    }
}