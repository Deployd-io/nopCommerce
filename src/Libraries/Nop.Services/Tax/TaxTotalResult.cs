namespace Nop.Services.Tax;

/// <summary>
/// Represents a result of tax total calculation
/// </summary>
public partial class TaxTotalResult : BaseNopResult
{
    #region Ctor

    public TaxTotalResult()
    {
		logger.LogDebug(">>> Entering TaxTotalResult()");
        TaxRates = new SortedDictionary<decimal, decimal>();
		logger.LogDebug("<<< Exiting TaxTotalResult()");
		logger.LogDebug("TaxTotalResult(): TaxRates → {TaxRates}", TaxRates);
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
