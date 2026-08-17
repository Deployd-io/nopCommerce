using Microsoft.Extensions.Logging;
﻿namespace Nop.Services.Tax.Events;

/// <summary>
/// Represents an event that raised when tax rate is calculated
/// </summary>
public partial class TaxRateCalculatedEvent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="taxRateResult">Tax rate result</param>
    public TaxRateCalculatedEvent(TaxRateResult taxRateResult)
    {
		_logger.LogDebug(">>> Entering TaxRateCalculatedEvent(taxRateResult={TaxRateResult})", taxRateResult);
        TaxRateResult = taxRateResult;
		_logger.LogDebug("<<< Exiting TaxRateCalculatedEvent(taxRateResult={TaxRateResult})", taxRateResult);
		_logger.LogDebug("TaxRateCalculatedEvent(taxRateResult={TaxRateResult}): TaxRateResult → {TaxRateResult1}", taxRateResult, TaxRateResult);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the tax rate result
    /// </summary>
    public TaxRateResult TaxRateResult { get; }

    #endregion
}