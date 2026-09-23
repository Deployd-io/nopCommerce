using Nop.Core.Domain.Catalog;
using Nop.Services.Caching;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Catalog.Caching;

/// <summary>
/// Represents a product specification attribute cache event consumer
/// </summary>
public partial class ProductSpecificationAttributeCacheEventConsumer : CacheEventConsumer<ProductSpecificationAttribute>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Clear cache data
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected override async Task ClearCacheAsync(ProductSpecificationAttribute entity)
    {
		_logger.LogError(">>> Entering ClearCacheAsync(entity={Entity})", entity);
        await RemoveByPrefixAsync(NopCatalogDefaults.ProductSpecificationAttributeByProductPrefix, entity.ProductId);
        await RemoveByPrefixAsync(NopCatalogDefaults.FilterableSpecificationAttributeOptionsPrefix);
        await RemoveAsync(NopCatalogDefaults.SpecificationAttributeGroupByProductCacheKey, entity.ProductId);
		_logger.LogWarning("<<< Exiting ClearCacheAsync(entity={Entity})", entity);
    }
}