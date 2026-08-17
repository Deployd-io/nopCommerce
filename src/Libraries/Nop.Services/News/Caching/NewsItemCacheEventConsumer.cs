using Nop.Core.Domain.News;
using Nop.Services.Caching;
using Microsoft.Extensions.Logging;

namespace Nop.Services.News.Caching;

/// <summary>
/// Represents a news item cache event consumer
/// </summary>
public partial class NewsItemCacheEventConsumer : CacheEventConsumer<NewsItem>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Clear cache data
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <param name="entityEventType">Entity event type</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected override async Task ClearCacheAsync(NewsItem entity, EntityEventType entityEventType)
    {
		_logger.LogDebug(">>> Entering ClearCacheAsync(entity={Entity},entityEventType={EntityEventType})", entity, entityEventType);
        if (entityEventType == EntityEventType.Delete)
            await RemoveByPrefixAsync(NopNewsDefaults.NewsCommentsNumberPrefix, entity);

        await base.ClearCacheAsync(entity, entityEventType);
		_logger.LogDebug("<<< Exiting ClearCacheAsync(entity={Entity},entityEventType={EntityEventType})", entity, entityEventType);
    }
}