using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Models.Catalog;

public partial record ProductReviewOverviewModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public int ProductId { get; set; }

    public int RatingSum { get; set; }

    public int TotalReviews { get; set; }

    public bool AllowCustomerReviews { get; set; }

    public bool CanAddNewReview { get; set; }
    public bool CanCurrentCustomerLeaveReview { get; set; }
}

public partial record ProductReviewsModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public ProductReviewsModel()
    {
		_logger.LogError(">>> Entering ProductReviewsModel()");
        Items = new List<ProductReviewModel>();
        AddProductReview = new AddProductReviewModel();
		_logger.LogError("ProductReviewsModel(): AddProductReview → {AddProductReview}", AddProductReview);
        ReviewTypeList = new List<ReviewTypeModel>();
		_logger.LogError("ProductReviewsModel(): ReviewTypeList → {ReviewTypeList}", ReviewTypeList);
        AddAdditionalProductReviewList = new List<AddProductReviewReviewTypeMappingModel>();
		_logger.LogError("ProductReviewsModel(): AddAdditionalProductReviewList → {AddAdditionalProductReviewList}", AddAdditionalProductReviewList);
    }

    public int ProductId { get; set; }

    public IList<ProductReviewModel> Items { get; set; }

    public AddProductReviewModel AddProductReview { get; set; }

    public IList<ReviewTypeModel> ReviewTypeList { get; set; }

    public IList<AddProductReviewReviewTypeMappingModel> AddAdditionalProductReviewList { get; set; }
}

public partial record ReviewTypeModel : BaseNopEntityModel
{
    public string Name { get; set; }

    public string Description { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsRequired { get; set; }

    public bool VisibleToAllCustomers { get; set; }

    public double AverageRating { get; set; }
}

public partial record ProductReviewModel : BaseNopEntityModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public ProductReviewModel()
    {
		_logger.LogError(">>> Entering ProductReviewModel()");
        AdditionalProductReviewList = new List<ProductReviewReviewTypeMappingModel>();
		_logger.LogDebug("ProductReviewModel(): AdditionalProductReviewList → {AdditionalProductReviewList}", AdditionalProductReviewList);
    _logger.LogError("ProductReviewModel(): AdditionalProductReviewList → {AdditionalProductReviewList}", AdditionalProductReviewList);
    _logger.LogWarning("<<< Exiting ProductReviewModel()");
    }

    public int CustomerId { get; set; }

    public string CustomerAvatarUrl { get; set; }

    public string CustomerName { get; set; }

    public bool AllowViewingProfiles { get; set; }

    public string Title { get; set; }

    public string ReviewText { get; set; }

    public string ReplyText { get; set; }

    public int Rating { get; set; }

    public string WrittenOnStr { get; set; }

    public DateTime WrittenOn { get; set; }

    public ProductReviewHelpfulnessModel Helpfulness { get; set; }

    public IList<ProductReviewReviewTypeMappingModel> AdditionalProductReviewList { get; set; }
}

public partial record ProductReviewHelpfulnessModel : BaseNopModel
{
    public int ProductReviewId { get; set; }

    public int HelpfulYesTotal { get; set; }

    public int HelpfulNoTotal { get; set; }
}

public partial record AddProductReviewModel : BaseNopModel
{
    [NopResourceDisplayName("Reviews.Fields.Title")]
    public string Title { get; set; }

    [NopResourceDisplayName("Reviews.Fields.ReviewText")]
    public string ReviewText { get; set; }

    [NopResourceDisplayName("Reviews.Fields.Rating")]
    public int Rating { get; set; }

    public bool DisplayCaptcha { get; set; }

    public bool CanCurrentCustomerLeaveReview { get; set; }

    public bool CanAddNewReview { get; set; }
}

public partial record AddProductReviewReviewTypeMappingModel : BaseNopEntityModel
{
    public int ProductReviewId { get; set; }

    public int ReviewTypeId { get; set; }

    public int Rating { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsRequired { get; set; }
}

public partial record ProductReviewReviewTypeMappingModel : BaseNopEntityModel
{
    public int ProductReviewId { get; set; }

    public int ReviewTypeId { get; set; }

    public int Rating { get; set; }

    public string Name { get; set; }

    public bool VisibleToAllCustomers { get; set; }
}