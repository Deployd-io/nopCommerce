using Microsoft.Extensions.Logging;
using Nop.Web.Framework.Models;

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
		_logger.LogError(">>> Entering DatesRangesSearchModel()");
        DeliveryDateSearchModel = new DeliveryDateSearchModel();
        ProductAvailabilityRangeSearchModel = new ProductAvailabilityRangeSearchModel();
		_logger.LogError("DatesRangesSearchModel(): ProductAvailabilityRangeSearchModel → {ProductAvailabilityRangeSearchModel}", ProductAvailabilityRangeSearchModel);
    }

    #endregion

    #region Properties

    public DeliveryDateSearchModel DeliveryDateSearchModel { get; set; }

    public ProductAvailabilityRangeSearchModel ProductAvailabilityRangeSearchModel { get; set; }

    #endregion
}