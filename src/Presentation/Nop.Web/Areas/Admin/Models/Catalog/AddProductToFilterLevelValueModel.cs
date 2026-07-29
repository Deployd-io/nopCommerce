using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Catalog;

/// <summary>
/// Represents a product model to add to the filter level value
/// </summary>
public partial record AddProductToFilterLevelValueModel : BaseNopModel
{
    #region Ctor

    public AddProductToFilterLevelValueModel()
    {
		logger.LogDebug(">>> Entering AddProductToFilterLevelValueModel()");
        SelectedProductIds = new List<int>();
		logger.LogDebug("<<< Exiting AddProductToFilterLevelValueModel()");
		logger.LogDebug("AddProductToFilterLevelValueModel(): SelectedProductIds → {SelectedProductIds}", SelectedProductIds);
    }
    #endregion

    #region Properties

    public int FilterLevelValueId { get; set; }

    public IList<int> SelectedProductIds { get; set; }

    #endregion
}
