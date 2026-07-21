using Nop.Core.Domain.Blogs;
using Nop.Services.Caching;

namespace Nop.Services.Blogs.Caching;

/// <summary>
/// Represents a blog comment cache event consumer
/// </summary>
public partial class BlogCommentCacheEventConsumer : CacheEventConsumer<BlogComment>
{
    /// <summary>
    /// Clear cache data
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected override async Task ClearCacheAsync(BlogComment entity)
    {
		logger.LogDebug(">>> Entering ClearCacheAsync(entity={Entity})", entity);
        await RemoveByPrefixAsync(NopBlogsDefaults.BlogCommentsNumberPrefix, entity.BlogPostId);
		logger.LogDebug("<<< Exiting ClearCacheAsync(entity={Entity})", entity);
    }
}
