using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.News;

/// <summary>
/// Represents a news content model
/// </summary>
public partial record NewsContentModel : BaseNopModel
{
    #region Ctor

    public NewsContentModel()
    {
		logger.LogDebug(">>> Entering NewsContentModel()");
        NewsItems = new NewsItemSearchModel();
        NewsComments = new NewsCommentSearchModel();
		logger.LogDebug("NewsContentModel(): NewsItems → {NewsItems}", NewsItems);
		logger.LogDebug("NewsContentModel(): NewsComments → {NewsComments}", NewsComments);
        SearchTitle = new NewsItemSearchModel().SearchTitle;
		logger.LogDebug("<<< Exiting NewsContentModel()");
		logger.LogDebug("NewsContentModel(): SearchTitle → {SearchTitle}", SearchTitle);
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.ContentManagement.News.NewsItems.List.SearchTitle")]
    public string SearchTitle { get; set; }

    public NewsItemSearchModel NewsItems { get; set; }

    public NewsCommentSearchModel NewsComments { get; set; }

    #endregion
}