using Microsoft.Extensions.Logging;
﻿using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Shipping;

/// <summary>
/// Represents a dates and ranges search model
/// </summary>
public partial record DatesRangesSearchModel : BaseSearchModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public DatesRangesSearchModel()
    {
		_logger.LogDebug(">>> Entering DatesRangesSearchModel()");
        DeliveryDateSearchModel = new DeliveryDateSearchModel();
        ProductAvailabilityRangeSearchModel = new ProductAvailabilityRangeSearchModel();
		_logger.LogDebug("<<< Exiting DatesRangesSearchModel()");
		_logger.LogDebug("DatesRangesSearchModel(): ProductAvailabilityRangeSearchModel → {ProductAvailabilityRangeSearchModel}", ProductAvailabilityRangeSearchModel);
		_logger.LogDebug("DatesRangesSearchModel(): DeliveryDateSearchModel → {DeliveryDateSearchModel}", DeliveryDateSearchModel);
    }

    #endregion

    #region Properties

    public DeliveryDateSearchModel DeliveryDateSearchModel { get; set; }

    public ProductAvailabilityRangeSearchModel ProductAvailabilityRangeSearchModel { get; set; }

    #endregion
}