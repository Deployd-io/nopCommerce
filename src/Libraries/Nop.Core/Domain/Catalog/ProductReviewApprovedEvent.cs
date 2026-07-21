namespace Nop.Core.Domain.Catalog;

/// <summary>
/// Product review approved event
/// </summary>
public partial class ProductReviewApprovedEvent
{
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="productReview">Product review</param>
    public ProductReviewApprovedEvent(ProductReview productReview)
    {
		logger.LogDebug(">>> Entering ProductReviewApprovedEvent(productReview={ProductReview})", productReview);
        ProductReview = productReview;
		logger.LogDebug("<<< Exiting ProductReviewApprovedEvent(productReview={ProductReview})", productReview);
		logger.LogDebug("ProductReviewApprovedEvent(productReview={ProductReview}): ProductReview → {ProductReview1}", productReview, ProductReview);
    }

    /// <summary>
    /// Product review
    /// </summary>
    public ProductReview ProductReview { get; }
}
