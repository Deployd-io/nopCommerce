using Nop.Core.Domain.Customers;
using Nop.Services.Caching;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Tax.Avalara.Services.Caching;

/// <summary>
/// Represents a customer cache event consumer
/// </summary>
public class CustomerCacheEventConsumer : CacheEventConsumer<Customer>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Methods

    /// <summary>
    /// Clear cache data
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected override async Task ClearCacheAsync(Customer entity)
    {
		_logger.LogDebug(">>> Entering ClearCacheAsync(entity={Entity})", entity);
        await RemoveByPrefixAsync(AvalaraTaxDefaults.TaxRateCacheKeyByCustomerPrefix, entity);
		_logger.LogDebug("<<< Exiting ClearCacheAsync(entity={Entity})", entity);
    }

    #endregion
}