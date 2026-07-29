using Nop.Core.Domain.Customers;
using Nop.Services.Caching;

namespace Nop.Services.Customers.Caching;

/// <summary>
/// Represents a customer customer role mapping cache event consumer
/// </summary>
public partial class CustomerCustomerRoleMappingCacheEventConsumer : CacheEventConsumer<CustomerCustomerRoleMapping>
{
    /// <summary>
    /// Clear cache data
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected override async Task ClearCacheAsync(CustomerCustomerRoleMapping entity)
    {
		logger.LogDebug(">>> Entering ClearCacheAsync(entity={Entity})", entity);
        await RemoveAsync(NopCustomerServicesDefaults.CustomerRolesCacheKey, entity.CustomerId);
		logger.LogDebug("<<< Exiting ClearCacheAsync(entity={Entity})", entity);
    }
}