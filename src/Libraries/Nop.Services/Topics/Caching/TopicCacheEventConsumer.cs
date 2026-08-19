using Nop.Core.Domain.Topics;
using Nop.Services.Caching;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Topics.Caching;

/// <summary>
/// Represents a topic cache event consumer
/// </summary>
public partial class TopicCacheEventConsumer : CacheEventConsumer<Topic>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Clear cache data
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected override async Task ClearCacheAsync(Topic entity)
    {
		_logger.LogDebug(">>> Entering ClearCacheAsync(entity={Entity})", entity);
        await RemoveByPrefixAsync(NopTopicDefaults.TopicBySystemNamePrefix, entity.SystemName);
		_logger.LogDebug("<<< Exiting ClearCacheAsync(entity={Entity})", entity);
    }
}