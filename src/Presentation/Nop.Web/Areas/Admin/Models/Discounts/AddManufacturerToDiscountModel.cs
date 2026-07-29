using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Discounts;

/// <summary>
/// Represents a manufacturer model to add to the discount
/// </summary>
public partial record AddManufacturerToDiscountModel : BaseNopModel
{
    #region Ctor

    public AddManufacturerToDiscountModel()
    {
		logger.LogDebug(">>> Entering AddManufacturerToDiscountModel()");
        SelectedManufacturerIds = new List<int>();
		logger.LogDebug("<<< Exiting AddManufacturerToDiscountModel()");
		logger.LogDebug("AddManufacturerToDiscountModel(): SelectedManufacturerIds → {SelectedManufacturerIds}", SelectedManufacturerIds);
    }
    #endregion

    #region Properties

    public int DiscountId { get; set; }

    public IList<int> SelectedManufacturerIds { get; set; }

    #endregion
}