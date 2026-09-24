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
		_logger.LogError(">>> Entering BlogContentModel()");
        BlogPosts = new BlogPostSearchModel();
        BlogComments = new BlogCommentSearchModel();
		_logger.LogError("BlogContentModel(): BlogComments → {BlogComments}", BlogComments);
        SearchTitle = new BlogPostSearchModel().SearchTitle;
		_logger.LogError("BlogContentModel(): SearchTitle → {SearchTitle}", SearchTitle);
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.ContentManagement.Blog.BlogPosts.List.SearchTitle")]
    public string SearchTitle { get; set; }

    public BlogPostSearchModel BlogPosts { get; set; }

    public BlogCommentSearchModel BlogComments { get; set; }

    #endregion
}