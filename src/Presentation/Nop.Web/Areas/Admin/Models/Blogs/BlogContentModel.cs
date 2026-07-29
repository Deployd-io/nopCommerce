using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Blogs;

/// <summary>
/// Represents a blog content model
/// </summary>
public partial record BlogContentModel : BaseNopModel
{
    #region Ctor

    public BlogContentModel()
    {
		logger.LogDebug(">>> Entering BlogContentModel()");
        BlogPosts = new BlogPostSearchModel();
        BlogComments = new BlogCommentSearchModel();
		logger.LogDebug("BlogContentModel(): BlogPosts → {BlogPosts}", BlogPosts);
		logger.LogDebug("BlogContentModel(): BlogComments → {BlogComments}", BlogComments);
        SearchTitle = new BlogPostSearchModel().SearchTitle;
		logger.LogDebug("<<< Exiting BlogContentModel()");
		logger.LogDebug("BlogContentModel(): SearchTitle → {SearchTitle}", SearchTitle);
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.ContentManagement.Blog.BlogPosts.List.SearchTitle")]
    public string SearchTitle { get; set; }

    public BlogPostSearchModel BlogPosts { get; set; }

    public BlogCommentSearchModel BlogComments { get; set; }

    #endregion
}