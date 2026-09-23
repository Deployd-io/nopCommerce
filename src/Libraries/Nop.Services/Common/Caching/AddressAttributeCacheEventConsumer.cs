using Nop.Core.Domain.Common;
using Nop.Services.Attributes;
using Nop.Services.Caching;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Common.Caching;

/// <summary>
/// Represents a address attribute cache event consumer
/// </summary>
public partial class AddressAttributeCacheEventConsumer : CacheEventConsumer<AddressAttribute>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Clear cache data
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected override async Task ClearCacheAsync(AddressAttribute entity)
    {
		_logger.LogError(">>> Entering ClearCacheAsync(entity={Entity})", entity);
        await RemoveAsync(NopAttributeDefaults.AttributeValuesByAttributeCacheKey, nameof(AddressAttribute), entity);
		_logger.LogWarning("<<< Exiting ClearCacheAsync(entity={Entity})", entity);
    }
}