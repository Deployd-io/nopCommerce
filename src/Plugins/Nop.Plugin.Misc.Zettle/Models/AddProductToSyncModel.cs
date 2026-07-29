using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.Zettle.Models;

/// <summary>
/// Represents a product model to add for synchronization
/// </summary>
public record AddProductToSyncModel : BaseNopModel
{
    #region Ctor

    public AddProductToSyncModel()
    {
		logger.LogDebug(">>> Entering AddProductToSyncModel()");
        SelectedProductIds = new List<int>();
		logger.LogDebug("<<< Exiting AddProductToSyncModel()");
		logger.LogDebug("AddProductToSyncModel(): SelectedProductIds → {SelectedProductIds}", SelectedProductIds);
    }

    #endregion

    #region Properties

    public IList<int> SelectedProductIds { get; set; }

    #endregion
}