using Nop.Core.Domain.Orders;
using Nop.Services.Attributes;
using Nop.Services.Caching;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Orders.Caching;

/// <summary>
/// Represents a checkout attribute cache event consumer
/// </summary>
public partial class CheckoutAttributeCacheEventConsumer : CacheEventConsumer<CheckoutAttribute>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Clear cache data
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected override async Task ClearCacheAsync(CheckoutAttribute entity)
    {
		_logger.LogDebug(">>> Entering ClearCacheAsync(entity={Entity})", entity);
        await RemoveAsync(NopAttributeDefaults.AttributeValuesByAttributeCacheKey, nameof(CheckoutAttribute), entity);
		_logger.LogDebug("<<< Exiting ClearCacheAsync(entity={Entity})", entity);
    }
}