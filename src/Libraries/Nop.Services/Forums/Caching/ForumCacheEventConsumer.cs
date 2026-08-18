using Nop.Core.Domain.Forums;
using Nop.Services.Caching;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Forums.Caching;

/// <summary>
/// Represents a forum cache event consumer
/// </summary>
public partial class ForumCacheEventConsumer : CacheEventConsumer<Forum>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Clear cache data
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected override async Task ClearCacheAsync(Forum entity)
    {
		_logger.LogDebug(">>> Entering ClearCacheAsync(entity={Entity})", entity);
        await RemoveAsync(NopForumDefaults.ForumByForumGroupCacheKey, entity.ForumGroupId);
		_logger.LogDebug("<<< Exiting ClearCacheAsync(entity={Entity})", entity);
    }
}