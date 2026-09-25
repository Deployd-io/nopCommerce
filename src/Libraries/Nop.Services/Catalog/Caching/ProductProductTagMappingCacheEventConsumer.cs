using Nop.Core.Domain.Catalog;
using Nop.Services.Caching;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Catalog.Caching;

/// <summary>
/// Represents a product-product tag mapping  cache event consumer
/// </summary>
public partial class ProductProductTagMappingCacheEventConsumer : CacheEventConsumer<ProductProductTagMapping>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Clear cache data
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected override async Task ClearCacheAsync(ProductProductTagMapping entity)
    {
		_logger.LogError(">>> Entering ClearCacheAsync(entity={Entity})", entity);
        await RemoveAsync(NopCatalogDefaults.ProductTagsByProductCacheKey, entity.ProductId);
		_logger.LogWarning("<<< Exiting ClearCacheAsync(entity={Entity})", entity);
    }
}