using Microsoft.Extensions.Logging;
﻿using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Catalog;

/// <summary>
/// Represents a products price range filter model
/// </summary>
public partial record PriceRangeFilterModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Properties

    /// <summary>
    /// Gets or sets a value indicating whether filtering is enabled
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Gets or sets the selected price range
    /// </summary>
    public PriceRangeModel SelectedPriceRange { get; set; }

    /// <summary>
    /// Gets or sets the available price range
    /// </summary>
    public PriceRangeModel AvailablePriceRange { get; set; }

    #endregion

    #region Ctor

    /// <summary>
    /// Ctor
    /// </summary>
    public PriceRangeFilterModel()
    {
		_logger.LogDebug(">>> Entering PriceRangeFilterModel()");
        SelectedPriceRange = new PriceRangeModel();
        AvailablePriceRange = new PriceRangeModel();
		_logger.LogDebug("<<< Exiting PriceRangeFilterModel()");
		_logger.LogDebug("PriceRangeFilterModel(): AvailablePriceRange → {AvailablePriceRange}", AvailablePriceRange);
		_logger.LogDebug("PriceRangeFilterModel(): SelectedPriceRange → {SelectedPriceRange}", SelectedPriceRange);
    }

    #endregion
}