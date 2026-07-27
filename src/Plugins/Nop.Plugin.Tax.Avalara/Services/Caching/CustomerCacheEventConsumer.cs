using Nop.Core.Domain.Customers;
using Nop.Services.Caching;

namespace Nop.Plugin.Tax.Avalara.Services.Caching;

/// <summary>
/// Represents a customer cache event consumer
/// </summary>
public class CustomerCacheEventConsumer : CacheEventConsumer<Customer>
{
    #region Methods

    /// <summary>
    /// Clear cache data
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected override async Task ClearCacheAsync(Customer entity)
    {
		logger.LogDebug(">>> Entering ClearCacheAsync(entity={Entity})", entity);
        await RemoveByPrefixAsync(AvalaraTaxDefaults.TaxRateCacheKeyByCustomerPrefix, entity);
		logger.LogDebug("<<< Exiting ClearCacheAsync(entity={Entity})", entity);
    }

    #endregion
}