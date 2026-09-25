using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Nop.Web.Models.Catalog;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Validators.Catalog;

public partial class ProductReviewsValidator : BaseNopValidator<ProductReviewsModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public ProductReviewsValidator(ILocalizationService localizationService)
    {
		_logger.LogError(">>> Entering ProductReviewsValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(x => x.AddProductReview.Title).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Reviews.Fields.Title.Required")).When(x => x.AddProductReview != null);
        RuleFor(x => x.AddProductReview.Title).Length(1, 200).WithMessageAwait(localizationService.GetResourceAsync("Reviews.Fields.Title.MaxLengthValidation"), 200).When(x => x.AddProductReview != null && !string.IsNullOrEmpty(x.AddProductReview.Title));
        RuleFor(x => x.AddProductReview.ReviewText).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Reviews.Fields.ReviewText.Required")).When(x => x.AddProductReview != null);
		_logger.LogWarning("<<< Exiting ProductReviewsValidator(localizationService={LocalizationService})", localizationService);
    }
}