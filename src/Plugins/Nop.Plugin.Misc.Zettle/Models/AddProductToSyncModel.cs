using Microsoft.Extensions.Logging;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.Zettle.Models;

/// <summary>
/// Represents a product model to add for synchronization
/// </summary>
public record AddProductToSyncModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public AddProductToSyncModel()
    {
		_logger.LogError(">>> Entering AddProductToSyncModel()");
        SelectedProductIds = new List<int>();
		_logger.LogDebug("AddProductToSyncModel(): SelectedProductIds → {SelectedProductIds}", SelectedProductIds);
    _logger.LogError("AddProductToSyncModel(): SelectedProductIds → {SelectedProductIds}", SelectedProductIds);
    _logger.LogWarning("<<< Exiting AddProductToSyncModel()");
    }

    #endregion

    #region Properties

    public IList<int> SelectedProductIds { get; set; }

    #endregion
}