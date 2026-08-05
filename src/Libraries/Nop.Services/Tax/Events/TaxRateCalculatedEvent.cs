namespace Nop.Services.Tax.Events;

/// <summary>
/// Represents an event that raised when tax rate is calculated
/// </summary>
public partial class TaxRateCalculatedEvent
{
    #region Ctor

    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="taxRateResult">Tax rate result</param>
    public TaxRateCalculatedEvent(TaxRateResult taxRateResult)
    {
		logger.LogDebug(">>> Entering TaxRateCalculatedEvent(taxRateResult={TaxRateResult})", taxRateResult);
        TaxRateResult = taxRateResult;
		logger.LogDebug("<<< Exiting TaxRateCalculatedEvent(taxRateResult={TaxRateResult})", taxRateResult);
		logger.LogDebug("TaxRateCalculatedEvent(taxRateResult={TaxRateResult}): TaxRateResult → {TaxRateResult1}", taxRateResult, TaxRateResult);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the tax rate result
    /// </summary>
    public TaxRateResult TaxRateResult { get; }

    #endregion
}