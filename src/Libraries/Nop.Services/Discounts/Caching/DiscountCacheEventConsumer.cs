using Nop.Core.Domain.Discounts;
using Nop.Services.Caching;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Discounts.Caching;

/// <summary>
/// Represents a discount cache event consumer
/// </summary>
public partial class DiscountCacheEventConsumer : CacheEventConsumer<Discount>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Clear cache data
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected override async Task ClearCacheAsync(Discount entity)
    {
		_logger.LogDebug(">>> Entering ClearCacheAsync(entity={Entity})", entity);
        await RemoveAsync(NopDiscountDefaults.DiscountRequirementsByDiscountCacheKey, entity);
        await RemoveByPrefixAsync(NopDiscountDefaults.CategoryIdsByDiscountPrefix, entity);
        await RemoveByPrefixAsync(NopDiscountDefaults.ManufacturerIdsByDiscountPrefix, entity);
        await RemoveByPrefixAsync(NopDiscountDefaults.AppliedDiscountsCachePrefix);
		_logger.LogDebug("<<< Exiting ClearCacheAsync(entity={Entity})", entity);
    }
}