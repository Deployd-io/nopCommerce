using Nop.Core.Domain.Discounts;
using Nop.Services.Caching;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Discounts.Caching;

/// <summary>
/// Represents a discount requirement cache event consumer
/// </summary>
public partial class DiscountRequirementCacheEventConsumer : CacheEventConsumer<DiscountRequirement>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Clear cache data
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected override async Task ClearCacheAsync(DiscountRequirement entity)
    {
		_logger.LogDebug(">>> Entering ClearCacheAsync(entity={Entity})", entity);
        await RemoveAsync(NopDiscountDefaults.DiscountRequirementsByDiscountCacheKey, entity.DiscountId);

        if (entity.ParentId.HasValue)
            await RemoveAsync(NopDiscountDefaults.DiscountRequirementsByParentCacheKey, entity.ParentId);
			_logger.LogDebug("<<< Exiting ClearCacheAsync(entity={Entity})", entity);
    }

    /// <summary>
    /// Clear cache by entity event type
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <param name="entityEventType">Entity event type</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected override async Task ClearCacheAsync(DiscountRequirement entity, EntityEventType entityEventType)
    {
		_logger.LogDebug(">>> Entering ClearCacheAsync(entity={Entity},entityEventType={EntityEventType})", entity, entityEventType);
        if (entityEventType == EntityEventType.Delete)
            await RemoveAsync(NopDiscountDefaults.DiscountRequirementsByParentCacheKey, entity);

        await base.ClearCacheAsync(entity, entityEventType);
		_logger.LogDebug("<<< Exiting ClearCacheAsync(entity={Entity},entityEventType={EntityEventType})", entity, entityEventType);
    }
}