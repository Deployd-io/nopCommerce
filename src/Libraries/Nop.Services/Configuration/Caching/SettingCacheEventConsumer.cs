using Nop.Core.Domain.Configuration;
using Nop.Services.Caching;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Configuration.Caching;

/// <summary>
/// Represents a setting cache event consumer
/// </summary>
public partial class SettingCacheEventConsumer : CacheEventConsumer<Setting>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Clear cache by entity event type
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <param name="entityEventType">Entity event type</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected override Task ClearCacheAsync(Setting entity, EntityEventType entityEventType)
    {
		_logger.LogDebug(">>> Entering ClearCacheAsync(entity={Entity},entityEventType={EntityEventType})", entity, entityEventType);
        //clear setting cache in SettingService
		_logger.LogDebug("<<< Exiting ClearCacheAsync(entity={Entity},entityEventType={EntityEventType})", entity, entityEventType);
        return Task.CompletedTask;
    }
}