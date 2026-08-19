using Nop.Core.Domain.Catalog;
using Nop.Services.Caching;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Catalog.Caching;

/// <summary>
/// Represents a specification attribute option cache event consumer
/// </summary>
public partial class SpecificationAttributeOptionCacheEventConsumer : CacheEventConsumer<SpecificationAttributeOption>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Clear cache data
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <param name="entityEventType">Entity event type</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected override async Task ClearCacheAsync(SpecificationAttributeOption entity, EntityEventType entityEventType)
    {
		_logger.LogDebug(">>> Entering ClearCacheAsync(entity={Entity},entityEventType={EntityEventType})", entity, entityEventType);
        await RemoveAsync(NopCatalogDefaults.SpecificationAttributesWithOptionsCacheKey);
        await RemoveAsync(NopCatalogDefaults.SpecificationAttributeOptionsCacheKey, entity.SpecificationAttributeId);
        await RemoveByPrefixAsync(NopCatalogDefaults.ProductSpecificationAttributeAllByProductPrefix);
        await RemoveByPrefixAsync(NopCatalogDefaults.FilterableSpecificationAttributeOptionsPrefix);

        if (entityEventType == EntityEventType.Delete)
            await RemoveByPrefixAsync(NopCatalogDefaults.SpecificationAttributeGroupByProductPrefix);

        await base.ClearCacheAsync(entity, entityEventType);
		_logger.LogDebug("<<< Exiting ClearCacheAsync(entity={Entity},entityEventType={EntityEventType})", entity, entityEventType);
    }
}