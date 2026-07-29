using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Models.Catalog;

public partial record ProductReviewOverviewModel : BaseNopModel
{
    public int ProductId { get; set; }

    public int RatingSum { get; set; }

    public int TotalReviews { get; set; }

    public bool AllowCustomerReviews { get; set; }

    public bool CanAddNewReview { get; set; }
    public bool CanCurrentCustomerLeaveReview { get; set; }
}

public partial record ProductReviewsModel : BaseNopModel
{
    public ProductReviewsModel()
    {
		logger.LogDebug(">>> Entering ProductReviewsModel()");
        Items = new List<ProductReviewModel>();
        AddProductReview = new AddProductReviewModel();
		logger.LogDebug("ProductReviewsModel(): Items → {Items}", Items);
		logger.LogDebug("ProductReviewsModel(): AddProductReview → {AddProductReview}", AddProductReview);
        ReviewTypeList = new List<ReviewTypeModel>();
        AddAdditionalProductReviewList = new List<AddProductReviewReviewTypeMappingModel>();
		logger.LogDebug("<<< Exiting ProductReviewsModel()");
		logger.LogDebug("ProductReviewsModel(): ReviewTypeList → {ReviewTypeList}", ReviewTypeList);
		logger.LogDebug("ProductReviewsModel(): AddAdditionalProductReviewList → {AddAdditionalProductReviewList}", AddAdditionalProductReviewList);
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
    public ProductReviewModel()
    {
		logger.LogDebug(">>> Entering ProductReviewModel()");
        AdditionalProductReviewList = new List<ProductReviewReviewTypeMappingModel>();
		logger.LogDebug("<<< Exiting ProductReviewModel()");
		logger.LogDebug("ProductReviewModel(): AdditionalProductReviewList → {AdditionalProductReviewList}", AdditionalProductReviewList);
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