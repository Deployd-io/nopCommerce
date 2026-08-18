using Nop.Core.Caching;
using Nop.Core.Domain.Directory;
using Nop.Services.Caching;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Directory.Caching;

/// <summary>
/// Represents a country cache event consumer
/// </summary>
public partial class CountryCacheEventConsumer : CacheEventConsumer<Country>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Clear cache by entity event type
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <param name="entityEventType">Entity event type</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected override async Task ClearCacheAsync(Country entity, EntityEventType entityEventType)
    {
		_logger.LogDebug(">>> Entering ClearCacheAsync(entity={Entity},entityEventType={EntityEventType})", entity, entityEventType);
        await RemoveByPrefixAsync(NopEntityCacheDefaults<Country>.Prefix);
		_logger.LogDebug("<<< Exiting ClearCacheAsync(entity={Entity},entityEventType={EntityEventType})", entity, entityEventType);
    }
}