using Nop.Core.Domain.Customers;
using Nop.Services.Caching;
using Nop.Services.Events;
using Nop.Services.Orders;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Customers.Caching;

/// <summary>
/// Represents a customer cache event consumer
/// </summary>
public partial class CustomerCacheEventConsumer : CacheEventConsumer<Customer>, IConsumer<CustomerPasswordChangedEvent>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Methods

    /// <summary>
    /// Handle password changed event
    /// </summary>
    /// <param name="eventMessage">Event message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task HandleEventAsync(CustomerPasswordChangedEvent eventMessage)
    {
		_logger.LogDebug(">>> Entering HandleEventAsync(eventMessage={EventMessage})", eventMessage);
        await RemoveAsync(NopCustomerServicesDefaults.CustomerPasswordLifetimeCacheKey, eventMessage.Password.CustomerId);
		_logger.LogDebug("<<< Exiting HandleEventAsync(eventMessage={EventMessage})", eventMessage);
    }

    /// <summary>
    /// Clear cache by entity event type
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <param name="entityEventType">Entity event type</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected override async Task ClearCacheAsync(Customer entity, EntityEventType entityEventType)
    {
		_logger.LogDebug(">>> Entering ClearCacheAsync(entity={Entity},entityEventType={EntityEventType})", entity, entityEventType);
        if (entityEventType == EntityEventType.Delete)
        {
            await RemoveAsync(NopCustomerServicesDefaults.CustomerAddressesCacheKey, entity);
            await RemoveByPrefixAsync(NopCustomerServicesDefaults.CustomerAddressesByCustomerPrefix, entity);
        }

        await base.ClearCacheAsync(entity, entityEventType);
		_logger.LogDebug(">>> Entering ClearCacheAsync(entity={Entity})", entity);
		_logger.LogDebug("<<< Exiting ClearCacheAsync(entity={Entity},entityEventType={EntityEventType})", entity, entityEventType);
		_logger.LogDebug("<<< Exiting ClearCacheAsync(entity={Entity})", entity);
    }

    /// <summary>
    /// Clear cache data
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected override async Task ClearCacheAsync(Customer entity)
    {
        await RemoveAsync(NopCustomerServicesDefaults.CustomerRolesCacheKey, entity);
        await RemoveByPrefixAsync(NopOrderDefaults.ShoppingCartItemsByCustomerPrefix, entity);
        await RemoveAsync(NopCustomerServicesDefaults.CustomerByGuidCacheKey, entity.CustomerGuid);

        if (string.IsNullOrEmpty(entity.SystemName))
            return;

        await RemoveAsync(NopCustomerServicesDefaults.CustomerBySystemNameCacheKey, entity.SystemName);
    }

    #endregion
}