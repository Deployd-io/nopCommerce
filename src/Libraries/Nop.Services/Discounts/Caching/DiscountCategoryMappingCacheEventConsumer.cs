using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Discounts;
using Nop.Services.Caching;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Discounts.Caching;

/// <summary>
/// Represents a discount-category mapping cache event consumer
/// </summary>
public partial class DiscountCategoryMappingCacheEventConsumer : CacheEventConsumer<DiscountCategoryMapping>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    protected override async Task ClearCacheAsync(DiscountCategoryMapping entity)
    {
		_logger.LogDebug(">>> Entering ClearCacheAsync(entity={Entity})", entity);
        await RemoveAsync(NopDiscountDefaults.AppliedDiscountsCacheKey, nameof(Category), entity.EntityId);

        await base.ClearCacheAsync(entity);
		_logger.LogDebug("<<< Exiting ClearCacheAsync(entity={Entity})", entity);
    }
}