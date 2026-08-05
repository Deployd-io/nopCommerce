using Nop.Core.Domain.Common;
using Nop.Services.Attributes;
using Nop.Services.Caching;

namespace Nop.Services.Common.Caching;

/// <summary>
/// Represents a address attribute cache event consumer
/// </summary>
public partial class AddressAttributeCacheEventConsumer : CacheEventConsumer<AddressAttribute>
{
    /// <summary>
    /// Clear cache data
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected override async Task ClearCacheAsync(AddressAttribute entity)
    {
		logger.LogDebug(">>> Entering ClearCacheAsync(entity={Entity})", entity);
        await RemoveAsync(NopAttributeDefaults.AttributeValuesByAttributeCacheKey, nameof(AddressAttribute), entity);
		logger.LogDebug("<<< Exiting ClearCacheAsync(entity={Entity})", entity);
    }
}