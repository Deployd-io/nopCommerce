using Microsoft.Extensions.Logging;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Catalog;

/// <summary>
/// Represents a filter level value model to add to the product
/// </summary>
public partial record AddFilterLevelValueModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public AddFilterLevelValueModel()
    {
		_logger.LogError(">>> Entering AddFilterLevelValueModel()");
        SelectedFilterLevelValueIds = new List<int>();
		_logger.LogDebug("AddFilterLevelValueModel(): SelectedFilterLevelValueIds → {SelectedFilterLevelValueIds}", SelectedFilterLevelValueIds);
    _logger.LogError("AddFilterLevelValueModel(): SelectedFilterLevelValueIds → {SelectedFilterLevelValueIds}", SelectedFilterLevelValueIds);
    _logger.LogWarning("<<< Exiting AddFilterLevelValueModel()");
    }
    #endregion

    #region Properties

    public int ProductId { get; set; }

    public IList<int> SelectedFilterLevelValueIds { get; set; }

    #endregion
}