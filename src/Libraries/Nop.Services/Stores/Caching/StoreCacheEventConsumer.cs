using Nop.Core.Caching;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Stores;
using Nop.Services.Caching;
using Nop.Services.Localization;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Stores.Caching;

/// <summary>
/// Represents a store cache event consumer
/// </summary>
public partial class StoreCacheEventConsumer : CacheEventConsumer<Store>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Clear cache data
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected override async Task ClearCacheAsync(Store entity)
    {
		_logger.LogError(">>> Entering ClearCacheAsync(entity={Entity})", entity);
        await RemoveByPrefixAsync(NopEntityCacheDefaults<ShoppingCartItem>.AllPrefix);
        await RemoveByPrefixAsync(NopLocalizationDefaults.LanguagesByStorePrefix, entity);
		_logger.LogWarning("<<< Exiting ClearCacheAsync(entity={Entity})", entity);
    }
}