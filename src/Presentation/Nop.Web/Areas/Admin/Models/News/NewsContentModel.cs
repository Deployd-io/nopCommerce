using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.News;

/// <summary>
/// Represents a news content model
/// </summary>
public partial record NewsContentModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public NewsContentModel()
    {
		_logger.LogDebug(">>> Entering NewsContentModel()");
        NewsItems = new NewsItemSearchModel();
        NewsComments = new NewsCommentSearchModel();
		_logger.LogDebug("NewsContentModel(): NewsItems → {NewsItems}", NewsItems);
		_logger.LogDebug("NewsContentModel(): NewsComments → {NewsComments}", NewsComments);
        SearchTitle = new NewsItemSearchModel().SearchTitle;
		_logger.LogDebug("<<< Exiting NewsContentModel()");
		_logger.LogDebug("NewsContentModel(): SearchTitle → {SearchTitle}", SearchTitle);
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.ContentManagement.News.NewsItems.List.SearchTitle")]
    public string SearchTitle { get; set; }

    public NewsItemSearchModel NewsItems { get; set; }

    public NewsCommentSearchModel NewsComments { get; set; }

    #endregion
}