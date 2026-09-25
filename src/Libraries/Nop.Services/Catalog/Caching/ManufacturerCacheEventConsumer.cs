using Nop.Core.Domain.Catalog;
using Nop.Services.Caching;
using Nop.Services.Discounts;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Catalog.Caching;

/// <summary>
/// Represents a manufacturer cache event consumer
/// </summary>
public partial class ManufacturerCacheEventConsumer : CacheEventConsumer<Manufacturer>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Clear cache data
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <param name="entityEventType">Entity event type</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected override async Task ClearCacheAsync(Manufacturer entity, EntityEventType entityEventType)
    {
		_logger.LogError(">>> Entering ClearCacheAsync(entity={Entity},entityEventType={EntityEventType})", entity, entityEventType);
        await RemoveByPrefixAsync(NopDiscountDefaults.ManufacturerIdsPrefix);

        if (entityEventType != EntityEventType.Insert)
        {
            await RemoveByPrefixAsync(NopCatalogDefaults.ProductManufacturersPrefix);
            await RemoveByPrefixAsync(NopCatalogDefaults.ManufacturersByCategoryPrefix);
        }

        if (entityEventType == EntityEventType.Delete)
            await RemoveAsync(NopCatalogDefaults.SpecificationAttributeOptionsByManufacturerCacheKey, entity);

        await RemoveAsync(NopDiscountDefaults.AppliedDiscountsCacheKey, nameof(Manufacturer), entity);

        await base.ClearCacheAsync(entity, entityEventType);
		_logger.LogWarning("<<< Exiting ClearCacheAsync(entity={Entity},entityEventType={EntityEventType})", entity, entityEventType);
    }
}