namespace Nop.Services.Tax.Events;

/// <summary>
/// Represents an event that raised when tax total is calculated
/// </summary>
public partial class TaxTotalCalculatedEvent
{
    #region Ctor

    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="taxTotalRequest">Tax total request</param>
    /// <param name="taxTotalResult">Tax total result</param>
    public TaxTotalCalculatedEvent(TaxTotalRequest taxTotalRequest, TaxTotalResult taxTotalResult)
    {
		logger.LogDebug(">>> Entering TaxTotalCalculatedEvent(taxTotalRequest={TaxTotalRequest},taxTotalResult={TaxTotalResult})", taxTotalRequest, taxTotalResult);
        TaxTotalRequest = taxTotalRequest;
        TaxTotalResult = taxTotalResult;
		logger.LogDebug("<<< Exiting TaxTotalCalculatedEvent(taxTotalRequest={TaxTotalRequest},taxTotalResult={TaxTotalResult})", taxTotalRequest, taxTotalResult);
		logger.LogDebug("TaxTotalCalculatedEvent(taxTotalRequest={TaxTotalRequest},taxTotalResult={TaxTotalResult}): TaxTotalResult → {TaxTotalResult1}", taxTotalRequest, taxTotalResult, TaxTotalResult);
		logger.LogDebug("TaxTotalCalculatedEvent(taxTotalRequest={TaxTotalRequest},taxTotalResult={TaxTotalResult}): TaxTotalRequest → {TaxTotalRequest1}", taxTotalRequest, taxTotalResult, TaxTotalRequest);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the tax total request
    /// </summary>
    public TaxTotalRequest TaxTotalRequest { get; }

    /// <summary>
    /// Gets the tax total result
    /// </summary>
    public TaxTotalResult TaxTotalResult { get; }

    #endregion
}