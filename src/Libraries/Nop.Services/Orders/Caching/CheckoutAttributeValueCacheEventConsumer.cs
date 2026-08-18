using Nop.Core.Domain.Orders;
using Nop.Services.Attributes;
using Nop.Services.Caching;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Orders.Caching;

/// <summary>
/// Represents a checkout attribute value cache event consumer
/// </summary>
public partial class CheckoutAttributeValueCacheEventConsumer : CacheEventConsumer<CheckoutAttributeValue>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Clear cache data
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected override async Task ClearCacheAsync(CheckoutAttributeValue entity)
    {
		_logger.LogDebug(">>> Entering ClearCacheAsync(entity={Entity})", entity);
        await RemoveAsync(NopAttributeDefaults.AttributeValuesByAttributeCacheKey, nameof(CheckoutAttribute), entity.AttributeId);
		_logger.LogDebug("<<< Exiting ClearCacheAsync(entity={Entity})", entity);
    }
}