using Nop.Core.Domain.Blogs;
using Nop.Services.Caching;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Blogs.Caching;

/// <summary>
/// Represents a blog post cache event consumer
/// </summary>
public partial class BlogPostCacheEventConsumer : CacheEventConsumer<BlogPost>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Clear cache data
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected override async Task ClearCacheAsync(BlogPost entity)
    {
		_logger.LogDebug(">>> Entering ClearCacheAsync(entity={Entity})", entity);
        await RemoveByPrefixAsync(NopBlogsDefaults.BlogTagsPrefix);
		_logger.LogDebug("<<< Exiting ClearCacheAsync(entity={Entity})", entity);
    }
}