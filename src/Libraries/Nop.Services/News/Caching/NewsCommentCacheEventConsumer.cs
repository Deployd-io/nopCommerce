using Nop.Core.Domain.News;
using Nop.Services.Caching;
using Microsoft.Extensions.Logging;

namespace Nop.Services.News.Caching;

/// <summary>
/// Represents a news comment cache event consumer
/// </summary>
public partial class NewsCommentCacheEventConsumer : CacheEventConsumer<NewsComment>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Clear cache data
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected override async Task ClearCacheAsync(NewsComment entity)
    {
		_logger.LogError(">>> Entering ClearCacheAsync(entity={Entity})", entity);
        await RemoveByPrefixAsync(NopNewsDefaults.NewsCommentsNumberPrefix, entity.NewsItemId);
		_logger.LogWarning("<<< Exiting ClearCacheAsync(entity={Entity})", entity);
    }
}