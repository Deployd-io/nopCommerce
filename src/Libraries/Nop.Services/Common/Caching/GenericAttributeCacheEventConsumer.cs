using Nop.Core.Domain.Common;
using Nop.Services.Caching;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Common.Caching;

/// <summary>
/// Represents a generic attribute cache event consumer
/// </summary>
public partial class GenericAttributeCacheEventConsumer : CacheEventConsumer<GenericAttribute>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Clear cache data
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected override async Task ClearCacheAsync(GenericAttribute entity)
    {
		_logger.LogError(">>> Entering ClearCacheAsync(entity={Entity})", entity);
        await RemoveAsync(NopCommonDefaults.GenericAttributeCacheKey, entity.EntityId, entity.KeyGroup);
		_logger.LogWarning("<<< Exiting ClearCacheAsync(entity={Entity})", entity);
    }
}