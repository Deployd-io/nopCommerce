using Nop.Core.Domain.Common;
using Nop.Services.Attributes;
using Nop.Services.Caching;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Common.Caching;

/// <summary>
/// Represents a address attribute value cache event consumer
/// </summary>
public partial class AddressAttributeValueCacheEventConsumer : CacheEventConsumer<AddressAttributeValue>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Clear cache data
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected override async Task ClearCacheAsync(AddressAttributeValue entity)
    {
		_logger.LogDebug(">>> Entering ClearCacheAsync(entity={Entity})", entity);
        await RemoveAsync(NopAttributeDefaults.AttributeValuesByAttributeCacheKey, nameof(AddressAttribute), entity.AttributeId);
		_logger.LogDebug("<<< Exiting ClearCacheAsync(entity={Entity})", entity);
    }
}