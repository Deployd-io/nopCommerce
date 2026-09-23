using Nop.Core.Domain.Customers;
using Nop.Services.Caching;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Customers.Caching;

/// <summary>
/// Represents a customer customer role mapping cache event consumer
/// </summary>
public partial class CustomerCustomerRoleMappingCacheEventConsumer : CacheEventConsumer<CustomerCustomerRoleMapping>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Clear cache data
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected override async Task ClearCacheAsync(CustomerCustomerRoleMapping entity)
    {
		_logger.LogError(">>> Entering ClearCacheAsync(entity={Entity})", entity);
        await RemoveAsync(NopCustomerServicesDefaults.CustomerRolesCacheKey, entity.CustomerId);
		_logger.LogWarning("<<< Exiting ClearCacheAsync(entity={Entity})", entity);
    }
}