using Nop.Core.Domain.Catalog;
using Nop.Services.Caching;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Catalog.Caching;

/// <summary>
/// Represents a product attribute combination picture cache event consumer
/// </summary>
public partial class ProductAttributeCombinationPictureCacheEventConsumer : CacheEventConsumer<ProductAttributeCombinationPicture>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Clear cache data
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected override async Task ClearCacheAsync(ProductAttributeCombinationPicture entity)
    {
		_logger.LogDebug(">>> Entering ClearCacheAsync(entity={Entity})", entity);
        await RemoveAsync(NopCatalogDefaults.ProductAttributeCombinationPicturesByCombinationCacheKey, entity.ProductAttributeCombinationId);
		_logger.LogDebug("<<< Exiting ClearCacheAsync(entity={Entity})", entity);
    }
}