using Nop.Core.Domain.Catalog;
using Nop.Core.Events;
using Nop.Services.Caching;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Catalog.Caching;

/// <summary>
/// Represents a review type cache event consumer
/// </summary>
public partial class ReviewTypeCacheEventConsumer : CacheEventConsumer<ReviewType>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public override async Task HandleEventAsync(EntityDeletedEvent<ReviewType> eventMessage)
    {
		_logger.LogError(">>> Entering HandleEventAsync(eventMessage={EventMessage})", eventMessage);
        await RemoveByPrefixAsync(NopCatalogDefaults.ProductReviewTypeMappingByReviewIdPrefix);
        await base.HandleEventAsync(eventMessage);
		_logger.LogWarning("<<< Exiting HandleEventAsync(eventMessage={EventMessage})", eventMessage);
    }
}