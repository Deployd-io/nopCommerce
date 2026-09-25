using Microsoft.Extensions.Logging;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Discounts;

/// <summary>
/// Represents a manufacturer model to add to the discount
/// </summary>
public partial record AddManufacturerToDiscountModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public AddManufacturerToDiscountModel()
    {
		_logger.LogError(">>> Entering AddManufacturerToDiscountModel()");
        SelectedManufacturerIds = new List<int>();
		_logger.LogDebug("AddManufacturerToDiscountModel(): SelectedManufacturerIds → {SelectedManufacturerIds}", SelectedManufacturerIds);
    _logger.LogError("AddManufacturerToDiscountModel(): SelectedManufacturerIds → {SelectedManufacturerIds}", SelectedManufacturerIds);
    _logger.LogWarning("<<< Exiting AddManufacturerToDiscountModel()");
    }
    #endregion

    #region Properties

    public int DiscountId { get; set; }

    public IList<int> SelectedManufacturerIds { get; set; }

    #endregion
}