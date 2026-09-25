using Nop.Core.Caching;
using Nop.Core.Domain.Directory;
using Nop.Services.Caching;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Directory.Caching;

/// <summary>
/// Represents a state province cache event consumer
/// </summary>
public partial class StateProvinceCacheEventConsumer : CacheEventConsumer<StateProvince>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Clear cache by entity event type
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <param name="entityEventType">Entity event type</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected override async Task ClearCacheAsync(StateProvince entity, EntityEventType entityEventType)
    {
		_logger.LogError(">>> Entering ClearCacheAsync(entity={Entity},entityEventType={EntityEventType})", entity, entityEventType);
        await RemoveByPrefixAsync(NopEntityCacheDefaults<StateProvince>.Prefix);
		_logger.LogWarning("<<< Exiting ClearCacheAsync(entity={Entity},entityEventType={EntityEventType})", entity, entityEventType);
    }
}