using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Services.Caching;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Catalog.Caching;

/// <summary>
/// Represents a product attribute cache event consumer
/// </summary>
public partial class ProductAttributeCacheEventConsumer : CacheEventConsumer<ProductAttribute>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Clear cache data
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <param name="entityEventType">Entity event type</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected override async Task ClearCacheAsync(ProductAttribute entity, EntityEventType entityEventType)
    {
		_logger.LogError(">>> Entering ClearCacheAsync(entity={Entity},entityEventType={EntityEventType})", entity, entityEventType);
        if (entityEventType == EntityEventType.Insert)
            await RemoveAsync(NopCatalogDefaults.ProductAttributeValuesByAttributeCacheKey, entity);

        if (entityEventType == EntityEventType.Delete)
        {
            await RemoveByPrefixAsync(NopCatalogDefaults.ProductAttributeMappingsByProductPrefix);
            await RemoveByPrefixAsync(NopEntityCacheDefaults<ProductAttributeMapping>.ByIdPrefix);
        }
        await base.ClearCacheAsync(entity, entityEventType);
		_logger.LogWarning("<<< Exiting ClearCacheAsync(entity={Entity},entityEventType={EntityEventType})", entity, entityEventType);
    }
}