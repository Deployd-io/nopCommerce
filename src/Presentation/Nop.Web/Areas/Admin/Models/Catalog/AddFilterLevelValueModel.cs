using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Catalog;

/// <summary>
/// Represents a filter level value model to add to the product
/// </summary>
public partial record AddFilterLevelValueModel : BaseNopModel
{
    #region Ctor

    public AddFilterLevelValueModel()
    {
		logger.LogDebug(">>> Entering AddFilterLevelValueModel()");
        SelectedFilterLevelValueIds = new List<int>();
		logger.LogDebug("<<< Exiting AddFilterLevelValueModel()");
		logger.LogDebug("AddFilterLevelValueModel(): SelectedFilterLevelValueIds → {SelectedFilterLevelValueIds}", SelectedFilterLevelValueIds);
    }
    #endregion

    #region Properties

    public int ProductId { get; set; }

    public IList<int> SelectedFilterLevelValueIds { get; set; }

    #endregion
}