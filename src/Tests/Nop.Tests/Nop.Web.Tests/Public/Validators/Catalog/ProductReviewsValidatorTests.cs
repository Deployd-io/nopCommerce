using FluentValidation.TestHelper;
using Nop.Services.Localization;
using Nop.Web.Models.Catalog;
using Nop.Web.Validators.Catalog;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Public.Validators.Catalog;

[TestFixture]
public class ProductReviewsValidatorTests : BaseNopTest
{
    private ProductReviewsValidator _validator;

    [OneTimeSetUp]
    public void Setup()
    {
		logger.LogDebug(">>> Entering Setup()");
        _validator = new ProductReviewsValidator(GetService<ILocalizationService>());
		logger.LogDebug("<<< Exiting Setup()");
		logger.LogDebug("Setup(): _validator → {_validator}", _validator);
    }

    [Test]
    public void ShouldHaveErrorWhenTitleIsNullOrEmpty()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenTitleIsNullOrEmpty()");
        var model = new ProductReviewsModel { AddProductReview = { Title = null } };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.AddProductReview.Title);
        model.AddProductReview.Title = string.Empty;
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.AddProductReview.Title);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenTitleIsNullOrEmpty()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenTitleIsSpecified()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenTitleIsSpecified()");
        var model = new ProductReviewsModel { AddProductReview = { Title = "some comment" } };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.AddProductReview.Title);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenTitleIsSpecified()");
		logger.LogDebug("ShouldNotHaveErrorWhenTitleIsSpecified(): model → {Model}", model);
    }

    [Test]
    public void ShouldHaveErrorWhenReviewTextIsNullOrEmpty()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenReviewTextIsNullOrEmpty()");
        var model = new ProductReviewsModel { AddProductReview = { ReviewText = null } };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.AddProductReview.ReviewText);
        model.AddProductReview.ReviewText = string.Empty;
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.AddProductReview.ReviewText);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenReviewTextIsNullOrEmpty()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenReviewTextIsSpecified()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenReviewTextIsSpecified()");
        var model = new ProductReviewsModel { AddProductReview = { ReviewText = "some comment" } };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.AddProductReview.ReviewText);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenReviewTextIsSpecified()");
		logger.LogDebug("ShouldNotHaveErrorWhenReviewTextIsSpecified(): model → {Model}", model);
    }
}