using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Blogs;

/// <summary>
/// Represents a blog content model
/// </summary>
public partial record BlogContentModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public BlogContentModel()
    {
		_logger.LogDebug(">>> Entering BlogContentModel()");
        BlogPosts = new BlogPostSearchModel();
        BlogComments = new BlogCommentSearchModel();
		_logger.LogDebug("BlogContentModel(): BlogComments → {BlogComments}", BlogComments);
		_logger.LogDebug("BlogContentModel(): BlogPosts → {BlogPosts}", BlogPosts);
        SearchTitle = new BlogPostSearchModel().SearchTitle;
		_logger.LogDebug("BlogContentModel(): SearchTitle → {SearchTitle}", SearchTitle);
		_logger.LogDebug("<<< Exiting BlogContentModel()");
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.ContentManagement.Blog.BlogPosts.List.SearchTitle")]
    public string SearchTitle { get; set; }

    public BlogPostSearchModel BlogPosts { get; set; }

    public BlogCommentSearchModel BlogComments { get; set; }

    #endregion
}