using Nop.Core.Domain.Catalog;
using Nop.Services.Caching;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Catalog.Caching;

/// <summary>
/// Represents a product manufacturer cache event consumer
/// </summary>
public partial class ProductManufacturerCacheEventConsumer : CacheEventConsumer<ProductManufacturer>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Clear cache data
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected override async Task ClearCacheAsync(ProductManufacturer entity)
    {
		_logger.LogError(">>> Entering ClearCacheAsync(entity={Entity})", entity);
        await RemoveByPrefixAsync(NopCatalogDefaults.ProductManufacturersByProductPrefix, entity.ProductId);
        await RemoveByPrefixAsync(NopCatalogDefaults.ProductPricePrefix, entity.ProductId);
        await RemoveByPrefixAsync(NopCatalogDefaults.ProductMultiplePricePrefix, entity.ProductId);
        await RemoveByPrefixAsync(NopCatalogDefaults.ManufacturerFeaturedProductIdsPrefix, entity.ManufacturerId);
        await RemoveByPrefixAsync(NopCatalogDefaults.ManufacturersByCategoryPrefix);
        await RemoveAsync(NopCatalogDefaults.SpecificationAttributeOptionsByManufacturerCacheKey, entity.ManufacturerId.ToString());
		_logger.LogWarning("<<< Exiting ClearCacheAsync(entity={Entity})", entity);
    }
}