using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.News;

/// <summary>
/// Represents a news item search model
/// </summary>
public partial record NewsItemSearchModel : BaseSearchModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public NewsItemSearchModel()
    {
		_logger.LogDebug(">>> Entering NewsItemSearchModel()");
        AvailableStores = new List<SelectListItem>();
		_logger.LogDebug("<<< Exiting NewsItemSearchModel()");
		_logger.LogDebug("NewsItemSearchModel(): AvailableStores → {AvailableStores}", AvailableStores);
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.ContentManagement.News.NewsItems.List.SearchStore")]
    public int SearchStoreId { get; set; }

    public IList<SelectListItem> AvailableStores { get; set; }

    public string SearchTitle { get; set; }

    public bool HideStoresList { get; set; }

    #endregion
}