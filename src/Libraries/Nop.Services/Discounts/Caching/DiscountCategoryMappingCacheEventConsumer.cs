using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Discounts;
using Nop.Services.Caching;

namespace Nop.Services.Discounts.Caching;

/// <summary>
/// Represents a discount-category mapping cache event consumer
/// </summary>
public partial class DiscountCategoryMappingCacheEventConsumer : CacheEventConsumer<DiscountCategoryMapping>
{
    protected override async Task ClearCacheAsync(DiscountCategoryMapping entity)
    {
		logger.LogDebug(">>> Entering ClearCacheAsync(entity={Entity})", entity);
        await RemoveAsync(NopDiscountDefaults.AppliedDiscountsCacheKey, nameof(Category), entity.EntityId);

        await base.ClearCacheAsync(entity);
		logger.LogDebug("<<< Exiting ClearCacheAsync(entity={Entity})", entity);
    }
}
