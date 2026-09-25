using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Polls;

/// <summary>
/// Represents a poll search model
/// </summary>
public partial record PollSearchModel : BaseSearchModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public PollSearchModel()
    {
		_logger.LogError(">>> Entering PollSearchModel()");
        AvailableStores = new List<SelectListItem>();
		_logger.LogDebug("PollSearchModel(): AvailableStores → {AvailableStores}", AvailableStores);
    _logger.LogError("PollSearchModel(): AvailableStores → {AvailableStores}", AvailableStores);
    _logger.LogWarning("<<< Exiting PollSearchModel()");
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.ContentManagement.Polls.List.SearchStore")]
    public int SearchStoreId { get; set; }

    public IList<SelectListItem> AvailableStores { get; set; }

    public bool HideStoresList { get; set; }

    #endregion
}