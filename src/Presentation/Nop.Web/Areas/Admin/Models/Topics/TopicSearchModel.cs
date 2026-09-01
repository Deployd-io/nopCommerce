using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Topics;

/// <summary>
/// Represents a topic search model
/// </summary>
public partial record TopicSearchModel : BaseSearchModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public TopicSearchModel()
    {
		_logger.LogDebug(">>> Entering TopicSearchModel()");
        AvailableStores = new List<SelectListItem>();
		_logger.LogDebug("TopicSearchModel(): AvailableStores → {AvailableStores}", AvailableStores);
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.ContentManagement.Topics.List.SearchStore")]
    public int SearchStoreId { get; set; }

    public IList<SelectListItem> AvailableStores { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Topics.List.SearchKeywords")]
    public string SearchKeywords { get; set; }

    public bool HideStoresList { get; set; }

    #endregion
}