using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Catalog;

/// <summary>
/// Represents a cross-sell product model to add to the product
/// </summary>
public partial record AddCrossSellProductModel : BaseNopModel
{
    #region Ctor

    public AddCrossSellProductModel()
    {
		logger.LogDebug(">>> Entering AddCrossSellProductModel()");
        SelectedProductIds = new List<int>();
		logger.LogDebug("<<< Exiting AddCrossSellProductModel()");
		logger.LogDebug("AddCrossSellProductModel(): SelectedProductIds → {SelectedProductIds}", SelectedProductIds);
    }
    #endregion

    #region Properties

    public int ProductId { get; set; }

    public IList<int> SelectedProductIds { get; set; }

    #endregion
}