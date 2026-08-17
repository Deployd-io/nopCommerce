using Microsoft.Extensions.Logging;
﻿using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Blogs;

public partial record BlogPostListModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public BlogPostListModel()
    {
		_logger.LogDebug(">>> Entering BlogPostListModel()");
        PagingFilteringContext = new BlogPagingFilteringModel();
        BlogPosts = new List<BlogPostModel>();
		_logger.LogDebug("<<< Exiting BlogPostListModel()");
		_logger.LogDebug("BlogPostListModel(): BlogPosts → {BlogPosts}", BlogPosts);
		_logger.LogDebug("BlogPostListModel(): PagingFilteringContext → {PagingFilteringContext}", PagingFilteringContext);
    }

    public int WorkingLanguageId { get; set; }
    public BlogPagingFilteringModel PagingFilteringContext { get; set; }
    public IList<BlogPostModel> BlogPosts { get; set; }
}