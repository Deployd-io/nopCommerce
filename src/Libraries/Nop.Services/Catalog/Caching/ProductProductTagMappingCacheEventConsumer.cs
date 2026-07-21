using Nop.Core.Domain.Catalog;
using Nop.Services.Caching;

namespace Nop.Services.Catalog.Caching;

/// <summary>
/// Represents a product-product tag mapping  cache event consumer
/// </summary>
public partial class ProductProductTagMappingCacheEventConsumer : CacheEventConsumer<ProductProductTagMapping>
{
    /// <summary>
    /// Clear cache data
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected override async Task ClearCacheAsync(ProductProductTagMapping entity)
    {
		logger.LogDebug(">>> Entering ClearCacheAsync(entity={Entity})", entity);
        await RemoveAsync(NopCatalogDefaults.ProductTagsByProductCacheKey, entity.ProductId);
		logger.LogDebug("<<< Exiting ClearCacheAsync(entity={Entity})", entity);
    }
}
