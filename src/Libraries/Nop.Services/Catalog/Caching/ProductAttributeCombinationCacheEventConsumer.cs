using Nop.Core.Domain.Catalog;
using Nop.Services.Caching;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Catalog.Caching;

/// <summary>
/// Represents a product attribute combination cache event consumer
/// </summary>
public partial class ProductAttributeCombinationCacheEventConsumer : CacheEventConsumer<ProductAttributeCombination>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Clear cache data
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected override async Task ClearCacheAsync(ProductAttributeCombination entity)
    {
		_logger.LogDebug(">>> Entering ClearCacheAsync(entity={Entity})", entity);
        await RemoveAsync(NopCatalogDefaults.ProductAttributeMappingsByProductCacheKey, entity.ProductId);
        await RemoveAsync(NopCatalogDefaults.ProductAttributeCombinationsByProductCacheKey, entity.ProductId);
        await RemoveByPrefixAsync(NopCatalogDefaults.ProductMultiplePricePrefix, entity.ProductId);
		_logger.LogDebug("<<< Exiting ClearCacheAsync(entity={Entity})", entity);
    }
}