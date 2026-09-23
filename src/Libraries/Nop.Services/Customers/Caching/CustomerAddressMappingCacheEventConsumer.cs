using Nop.Core.Domain.Customers;
using Nop.Services.Caching;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Customers.Caching;

/// <summary>
/// Represents a customer address mapping cache event consumer
/// </summary>
public partial class CustomerAddressMappingCacheEventConsumer : CacheEventConsumer<CustomerAddressMapping>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Clear cache by entity event type
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <param name="entityEventType">Entity event type</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected override async Task ClearCacheAsync(CustomerAddressMapping entity, EntityEventType entityEventType)
    {
		_logger.LogError(">>> Entering ClearCacheAsync(entity={Entity},entityEventType={EntityEventType})", entity, entityEventType);
        await RemoveAsync(NopCustomerServicesDefaults.CustomerAddressesCacheKey, entity.CustomerId);

        if (entityEventType == EntityEventType.Delete)
            await RemoveAsync(NopCustomerServicesDefaults.CustomerAddressCacheKey, entity.CustomerId, entity.AddressId);

        await base.ClearCacheAsync(entity, entityEventType);
		_logger.LogWarning("<<< Exiting ClearCacheAsync(entity={Entity},entityEventType={EntityEventType})", entity, entityEventType);
    }
}