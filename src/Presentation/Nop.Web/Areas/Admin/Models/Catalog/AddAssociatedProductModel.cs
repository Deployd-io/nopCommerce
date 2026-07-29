using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Catalog;

/// <summary>
/// Represents an associated product model to add to the product
/// </summary>
public partial record AddAssociatedProductModel : BaseNopModel
{
    #region Ctor

    public AddAssociatedProductModel()
    {
		logger.LogDebug(">>> Entering AddAssociatedProductModel()");
        SelectedProductIds = new List<int>();
		logger.LogDebug("<<< Exiting AddAssociatedProductModel()");
		logger.LogDebug("AddAssociatedProductModel(): SelectedProductIds → {SelectedProductIds}", SelectedProductIds);
    }
    #endregion

    #region Properties

    public int ProductId { get; set; }

    public IList<int> SelectedProductIds { get; set; }

    #endregion
}