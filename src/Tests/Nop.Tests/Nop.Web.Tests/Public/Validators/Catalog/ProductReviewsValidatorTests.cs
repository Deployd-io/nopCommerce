using FluentValidation.TestHelper;
using Nop.Services.Localization;
using Nop.Web.Models.Catalog;
using Nop.Web.Validators.Catalog;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Web.Tests.Public.Validators.Catalog;

[TestFixture]
public class ProductReviewsValidatorTests : BaseNopTest
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    private ProductReviewsValidator _validator;

    [OneTimeSetUp]
    public void Setup()
    {
		_logger.LogDebug(">>> Entering Setup()");
        _validator = new ProductReviewsValidator(GetService<ILocalizationService>());
		_logger.LogDebug("Setup(): _validator → {_validator}", _validator);
		_logger.LogDebug("<<< Exiting Setup()");
    }

    [Test]
    public void ShouldHaveErrorWhenTitleIsNullOrEmpty()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenTitleIsNullOrEmpty()");
        var model = new ProductReviewsModel { AddProductReview = { Title = null } };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.AddProductReview.Title);
        model.AddProductReview.Title = string.Empty;
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.AddProductReview.Title);
		_logger.LogDebug("<<< Exiting ShouldHaveErrorWhenTitleIsNullOrEmpty()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenTitleIsSpecified()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenTitleIsSpecified()");
        var model = new ProductReviewsModel { AddProductReview = { Title = "some comment" } };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.AddProductReview.Title);
		_logger.LogDebug("ShouldNotHaveErrorWhenTitleIsSpecified(): model → {Model}", model);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenTitleIsSpecified()");
    }

    [Test]
    public void ShouldHaveErrorWhenReviewTextIsNullOrEmpty()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenReviewTextIsNullOrEmpty()");
        var model = new ProductReviewsModel { AddProductReview = { ReviewText = null } };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.AddProductReview.ReviewText);
        model.AddProductReview.ReviewText = string.Empty;
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.AddProductReview.ReviewText);
		_logger.LogDebug("<<< Exiting ShouldHaveErrorWhenReviewTextIsNullOrEmpty()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenReviewTextIsSpecified()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenReviewTextIsSpecified()");
        var model = new ProductReviewsModel { AddProductReview = { ReviewText = "some comment" } };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.AddProductReview.ReviewText);
		_logger.LogDebug("ShouldNotHaveErrorWhenReviewTextIsSpecified(): model → {Model}", model);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenReviewTextIsSpecified()");
    }
}