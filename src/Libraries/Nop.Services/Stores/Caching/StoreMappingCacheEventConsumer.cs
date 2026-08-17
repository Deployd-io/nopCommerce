using DocumentFormat.OpenXml.Drawing.Diagrams;
using Nop.Core.Domain.Stores;
using Nop.Services.Caching;
using Nop.Services.Catalog;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Stores.Caching;

/// <summary>
/// Represents a store mapping cache event consumer
/// </summary>
public partial class StoreMappingCacheEventConsumer : CacheEventConsumer<StoreMapping>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Clear cache data
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected override async Task ClearCacheAsync(StoreMapping entity)
    {
		_logger.LogDebug(">>> Entering ClearCacheAsync(entity={Entity})", entity);
        await RemoveAsync(NopStoreDefaults.StoreMappingsCacheKey, entity.EntityId, entity.EntityName);
        await RemoveAsync(NopStoreDefaults.StoreMappingIdsCacheKey, entity.EntityId, entity.EntityName);
        await RemoveAsync(NopStoreDefaults.StoreMappingExistsCacheKey, entity.EntityName);

        if (entity.EntityName.Equals(nameof(Category)))
            await RemoveByPrefixAsync(NopCatalogDefaults.ChildCategoryIdLookupByStorePrefix, entity.StoreId);
			_logger.LogDebug("<<< Exiting ClearCacheAsync(entity={Entity})", entity);
    }
}