using Microsoft.Extensions.Logging;
namespace Nop.Services.Tax;

/// <summary>
/// Represents a result of tax total calculation
/// </summary>
public partial class TaxTotalResult : BaseNopResult
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public TaxTotalResult()
    {
		_logger.LogDebug(">>> Entering TaxTotalResult()");
        TaxRates = new SortedDictionary<decimal, decimal>();
		_logger.LogDebug("TaxTotalResult(): TaxRates → {TaxRates}", TaxRates);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets a tax total
    /// </summary>
    public decimal TaxTotal { get; set; }

    /// <summary>
    /// Gets or sets tax rates
    /// </summary>
    public SortedDictionary<decimal, decimal> TaxRates { get; set; }

    #endregion
}