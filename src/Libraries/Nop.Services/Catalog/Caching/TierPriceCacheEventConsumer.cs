using Nop.Core.Domain.Catalog;
using Nop.Services.Caching;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Catalog.Caching;

/// <summary>
/// Represents a tier price cache event consumer
/// </summary>
public partial class TierPriceCacheEventConsumer : CacheEventConsumer<TierPrice>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Clear cache data
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected override async Task ClearCacheAsync(TierPrice entity)
    {
		_logger.LogError(">>> Entering ClearCacheAsync(entity={Entity})", entity);
        await RemoveAsync(NopCatalogDefaults.TierPricesByProductCacheKey, entity.ProductId);
        await RemoveByPrefixAsync(NopCatalogDefaults.ProductPricePrefix, entity.ProductId);
        await RemoveByPrefixAsync(NopCatalogDefaults.ProductMultiplePricePrefix, entity.ProductId);
		_logger.LogWarning("<<< Exiting ClearCacheAsync(entity={Entity})", entity);
    }
}