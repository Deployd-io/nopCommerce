using Microsoft.Extensions.Logging;
namespace Nop.Core.Domain.Catalog;

/// <summary>
/// Product review approved event
/// </summary>
public partial class ProductReviewApprovedEvent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="productReview">Product review</param>
    public ProductReviewApprovedEvent(ProductReview productReview)
    {
		_logger.LogDebug(">>> Entering ProductReviewApprovedEvent(productReview={ProductReview})", productReview);
        ProductReview = productReview;
		_logger.LogDebug("ProductReviewApprovedEvent(productReview={ProductReview}): ProductReview → {ProductReview1}", productReview, ProductReview);
		_logger.LogDebug("<<< Exiting ProductReviewApprovedEvent(productReview={ProductReview})", productReview);
    }

    /// <summary>
    /// Product review
    /// </summary>
    public ProductReview ProductReview { get; }
}