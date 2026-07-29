using Nop.Core.Domain.Configuration;
using Nop.Services.Caching;

namespace Nop.Services.Configuration.Caching;

/// <summary>
/// Represents a setting cache event consumer
/// </summary>
public partial class SettingCacheEventConsumer : CacheEventConsumer<Setting>
{
    /// <summary>
    /// Clear cache by entity event type
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <param name="entityEventType">Entity event type</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected override Task ClearCacheAsync(Setting entity, EntityEventType entityEventType)
    {
		logger.LogDebug(">>> Entering ClearCacheAsync(entity={Entity},entityEventType={EntityEventType})", entity, entityEventType);
        //clear setting cache in SettingService
		logger.LogDebug("<<< Exiting ClearCacheAsync(entity={Entity},entityEventType={EntityEventType})", entity, entityEventType);
        return Task.CompletedTask;
    }
}