using Nop.Core.Domain.Customers;
using Nop.Services.Caching;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Customers.Caching;

/// <summary>
/// Represents a customer role cache event consumer
/// </summary>
public partial class CustomerRoleCacheEventConsumer : CacheEventConsumer<CustomerRole>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Clear cache by entity event type
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <param name="entityEventType">Entity event type</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected override async Task ClearCacheAsync(CustomerRole entity, EntityEventType entityEventType)
    {
		_logger.LogError(">>> Entering ClearCacheAsync(entity={Entity},entityEventType={EntityEventType})", entity, entityEventType);
        switch (entityEventType)
        {
            case EntityEventType.Update:
                await RemoveByPrefixAsync(NopCustomerServicesDefaults.CustomerRolesBySystemNamePrefix);
                break;
            case EntityEventType.Delete:
                await RemoveAsync(NopCustomerServicesDefaults.CustomerRolesBySystemNameCacheKey, entity.SystemName);
                break;
        }

        if (entityEventType != EntityEventType.Insert)
            await RemoveByPrefixAsync(NopCustomerServicesDefaults.CustomerCustomerRolesPrefix);

        await base.ClearCacheAsync(entity, entityEventType);
		_logger.LogWarning("<<< Exiting ClearCacheAsync(entity={Entity},entityEventType={EntityEventType})", entity, entityEventType);
    }
}