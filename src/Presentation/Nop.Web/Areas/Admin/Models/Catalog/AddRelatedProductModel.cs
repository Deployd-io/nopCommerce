using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Catalog;

/// <summary>
/// Represents a related product model to add to the product
/// </summary>
public partial record AddRelatedProductModel : BaseNopModel
{
    #region Ctor

    public AddRelatedProductModel()
    {
		logger.LogDebug(">>> Entering AddRelatedProductModel()");
        SelectedProductIds = new List<int>();
		logger.LogDebug("<<< Exiting AddRelatedProductModel()");
		logger.LogDebug("AddRelatedProductModel(): SelectedProductIds → {SelectedProductIds}", SelectedProductIds);
    }
    #endregion

    #region Properties

    public int ProductId { get; set; }

    public IList<int> SelectedProductIds { get; set; }

    #endregion
}