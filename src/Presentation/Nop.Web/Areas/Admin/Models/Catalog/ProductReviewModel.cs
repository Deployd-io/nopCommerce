using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Catalog;

/// <summary>
/// Represents a product review model
/// </summary>
public partial record ProductReviewModel : BaseNopEntityModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public ProductReviewModel()
    {
		_logger.LogDebug(">>> Entering ProductReviewModel()");
        ProductReviewReviewTypeMappingSearchModel = new ProductReviewReviewTypeMappingSearchModel();
		_logger.LogDebug("<<< Exiting ProductReviewModel()");
		_logger.LogDebug("ProductReviewModel(): ProductReviewReviewTypeMappingSearchModel → {ProductReviewReviewTypeMappingSearchModel}", ProductReviewReviewTypeMappingSearchModel);
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.Catalog.ProductReviews.Fields.Store")]
    public string StoreName { get; set; }
    public bool ShowStoreName { get; set; }

    [NopResourceDisplayName("Admin.Catalog.ProductReviews.Fields.Product")]
    public int ProductId { get; set; }

    [NopResourceDisplayName("Admin.Catalog.ProductReviews.Fields.Product")]
    public string ProductName { get; set; }

    [NopResourceDisplayName("Admin.Catalog.ProductReviews.Fields.Customer")]
    public int CustomerId { get; set; }

    [NopResourceDisplayName("Admin.Catalog.ProductReviews.Fields.Customer")]
    public string CustomerInfo { get; set; }

    [NopResourceDisplayName("Admin.Catalog.ProductReviews.Fields.Title")]
    public string Title { get; set; }

    [NopResourceDisplayName("Admin.Catalog.ProductReviews.Fields.ReviewText")]
    public string ReviewText { get; set; }

    [NopResourceDisplayName("Admin.Catalog.ProductReviews.Fields.ReplyText")]
    public string ReplyText { get; set; }

    [NopResourceDisplayName("Admin.Catalog.ProductReviews.Fields.Rating")]
    public int Rating { get; set; }

    [NopResourceDisplayName("Admin.Catalog.ProductReviews.Fields.IsApproved")]
    public bool IsApproved { get; set; }

    [NopResourceDisplayName("Admin.Catalog.ProductReviews.Fields.CreatedOn")]
    public DateTime CreatedOn { get; set; }

    //vendor
    public bool IsLoggedInAsVendor { get; set; }

    public ProductReviewReviewTypeMappingSearchModel ProductReviewReviewTypeMappingSearchModel { get; set; }

    #endregion
}