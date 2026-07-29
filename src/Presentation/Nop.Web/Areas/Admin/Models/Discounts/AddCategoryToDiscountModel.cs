using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Discounts;

/// <summary>
/// Represents a category model to add to the discount
/// </summary>
public partial record AddCategoryToDiscountModel : BaseNopModel
{
    #region Ctor

    public AddCategoryToDiscountModel()
    {
		logger.LogDebug(">>> Entering AddCategoryToDiscountModel()");
        SelectedCategoryIds = new List<int>();
		logger.LogDebug("<<< Exiting AddCategoryToDiscountModel()");
		logger.LogDebug("AddCategoryToDiscountModel(): SelectedCategoryIds → {SelectedCategoryIds}", SelectedCategoryIds);
    }
    #endregion

    #region Properties

    public int DiscountId { get; set; }

    public IList<int> SelectedCategoryIds { get; set; }

    #endregion
}