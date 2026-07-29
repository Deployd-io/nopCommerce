using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Catalog;

/// <summary>
/// Represents a product model to add to the category
/// </summary>
public partial record AddProductToCategoryModel : BaseNopModel
{
    #region Ctor

    public AddProductToCategoryModel()
    {
		logger.LogDebug(">>> Entering AddProductToCategoryModel()");
        SelectedProductIds = new List<int>();
		logger.LogDebug("<<< Exiting AddProductToCategoryModel()");
		logger.LogDebug("AddProductToCategoryModel(): SelectedProductIds → {SelectedProductIds}", SelectedProductIds);
    }
    #endregion

    #region Properties

    public int CategoryId { get; set; }

    public IList<int> SelectedProductIds { get; set; }

    #endregion
}