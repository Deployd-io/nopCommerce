using Nop.Core.Domain.Blogs;
using Nop.Services.Caching;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Blogs.Caching;

/// <summary>
/// Represents a blog comment cache event consumer
/// </summary>
public partial class BlogCommentCacheEventConsumer : CacheEventConsumer<BlogComment>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Clear cache data
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected override async Task ClearCacheAsync(BlogComment entity)
    {
		_logger.LogError(">>> Entering ClearCacheAsync(entity={Entity})", entity);
        await RemoveByPrefixAsync(NopBlogsDefaults.BlogCommentsNumberPrefix, entity.BlogPostId);
		_logger.LogWarning("<<< Exiting ClearCacheAsync(entity={Entity})", entity);
    }
}