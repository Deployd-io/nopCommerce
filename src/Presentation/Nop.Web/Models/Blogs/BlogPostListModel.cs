using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Blogs;

public partial record BlogPostListModel : BaseNopModel
{
    public BlogPostListModel()
    {
		logger.LogDebug(">>> Entering BlogPostListModel()");
        PagingFilteringContext = new BlogPagingFilteringModel();
        BlogPosts = new List<BlogPostModel>();
		logger.LogDebug("<<< Exiting BlogPostListModel()");
		logger.LogDebug("BlogPostListModel(): PagingFilteringContext → {PagingFilteringContext}", PagingFilteringContext);
		logger.LogDebug("BlogPostListModel(): BlogPosts → {BlogPosts}", BlogPosts);
    }

    public int WorkingLanguageId { get; set; }
    public BlogPagingFilteringModel PagingFilteringContext { get; set; }
    public IList<BlogPostModel> BlogPosts { get; set; }
}