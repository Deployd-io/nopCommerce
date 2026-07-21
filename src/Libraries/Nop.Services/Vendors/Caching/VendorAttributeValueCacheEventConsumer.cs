using Nop.Core.Domain.Vendors;
using Nop.Services.Attributes;
using Nop.Services.Caching;

namespace Nop.Services.Vendors.Caching;

/// <summary>
/// Represents a vendor attribute value cache event consumer
/// </summary>
public partial class VendorAttributeValueCacheEventConsumer : CacheEventConsumer<VendorAttributeValue>
{
    /// <summary>
    /// Clear cache data
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected override async Task ClearCacheAsync(VendorAttributeValue entity)
    {
		logger.LogDebug(">>> Entering ClearCacheAsync(entity={Entity})", entity);
        await RemoveAsync(NopAttributeDefaults.AttributeValuesByAttributeCacheKey, nameof(VendorAttribute), entity.AttributeId);
		logger.LogDebug("<<< Exiting ClearCacheAsync(entity={Entity})", entity);
    }
}
