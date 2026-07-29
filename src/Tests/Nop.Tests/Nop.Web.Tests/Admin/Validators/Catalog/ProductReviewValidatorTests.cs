using FluentValidation.TestHelper;
using Nop.Core;
using Nop.Core.Domain.Vendors;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Areas.Admin.Validators.Catalog;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Admin.Validators.Catalog;

[TestFixture]
public class ProductReviewValidatorTests : BaseNopTest
{
    private ProductReviewValidator _validator;
    private IWorkContext _workContext;

    [OneTimeSetUp]
    public void Setup()
    {
		logger.LogDebug(">>> Entering Setup()");
        _workContext = GetService<IWorkContext>();
        _validator = new ProductReviewValidator(GetService<ILocalizationService>(), _workContext);
		logger.LogDebug("<<< Exiting Setup()");
		logger.LogDebug("Setup(): _workContext → {_workContext}", _workContext);
		logger.LogDebug("Setup(): _validator → {_validator}", _validator);
    }

    #region Title Validation Tests (Non-Vendor)

    [Test]
    public void ShouldHaveErrorWhenTitleIsNullForNonVendor()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenTitleIsNullForNonVendor()");
        // Ensure we're not logged in as vendor
        var model = new ProductReviewModel
        {
            Title = null,
            ReviewText = "Valid review text"
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Title);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenTitleIsNullForNonVendor()");
    }

    [Test]
    public void ShouldHaveErrorWhenTitleIsEmptyForNonVendor()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenTitleIsEmptyForNonVendor()");
        var model = new ProductReviewModel
        {
            Title = string.Empty,
            ReviewText = "Valid review text"
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Title);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenTitleIsEmptyForNonVendor()");
    }

    [Test]
    public void ShouldHaveErrorWhenTitleIsWhitespaceForNonVendor()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenTitleIsWhitespaceForNonVendor()");
        var model = new ProductReviewModel
        {
            Title = "   ",
            ReviewText = "Valid review text"
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Title);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenTitleIsWhitespaceForNonVendor()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenTitleIsSpecifiedForNonVendor()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenTitleIsSpecifiedForNonVendor()");
        var model = new ProductReviewModel
        {
            Title = "Great Product",
            ReviewText = "Valid review text"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Title);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenTitleIsSpecifiedForNonVendor()");
    }

    #endregion

    #region ReviewText Validation Tests (Non-Vendor)

    [Test]
    public void ShouldHaveErrorWhenReviewTextIsNullForNonVendor()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenReviewTextIsNullForNonVendor()");
        var model = new ProductReviewModel
        {
            Title = "Valid Title",
            ReviewText = null
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.ReviewText);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenReviewTextIsNullForNonVendor()");
    }

    [Test]
    public void ShouldHaveErrorWhenReviewTextIsEmptyForNonVendor()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenReviewTextIsEmptyForNonVendor()");
        var model = new ProductReviewModel
        {
            Title = "Valid Title",
            ReviewText = string.Empty
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.ReviewText);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenReviewTextIsEmptyForNonVendor()");
    }

    [Test]
    public void ShouldHaveErrorWhenReviewTextIsWhitespaceForNonVendor()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenReviewTextIsWhitespaceForNonVendor()");
        var model = new ProductReviewModel
        {
            Title = "Valid Title",
            ReviewText = "   "
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.ReviewText);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenReviewTextIsWhitespaceForNonVendor()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenReviewTextIsSpecifiedForNonVendor()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenReviewTextIsSpecifiedForNonVendor()");
        var model = new ProductReviewModel
        {
            Title = "Valid Title",
            ReviewText = "This is a detailed review of the product."
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.ReviewText);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenReviewTextIsSpecifiedForNonVendor()");
    }

    #endregion

    #region Combined Validation Tests

    [Test]
    public void ShouldHaveMultipleErrorsWhenBothFieldsAreInvalidForNonVendor()
    {
		logger.LogDebug(">>> Entering ShouldHaveMultipleErrorsWhenBothFieldsAreInvalidForNonVendor()");
        var model = new ProductReviewModel
        {
            Title = null,
            ReviewText = string.Empty
        };

        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Title);
		logger.LogDebug("ShouldHaveMultipleErrorsWhenBothFieldsAreInvalidForNonVendor(): result → {Result}", result);
        result.ShouldHaveValidationErrorFor(x => x.ReviewText);
		logger.LogDebug("<<< Exiting ShouldHaveMultipleErrorsWhenBothFieldsAreInvalidForNonVendor()");
    }

    [Test]
    public void ShouldNotHaveErrorsWhenAllFieldsAreValidForNonVendor()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorsWhenAllFieldsAreValidForNonVendor()");
        var model = new ProductReviewModel
        {
            Title = "Excellent Product Quality",
            ReviewText = "I am very satisfied with this product. The quality exceeds my expectations and the delivery was fast.",
            Rating = 5,
            IsApproved = true
        };

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Title);
		logger.LogDebug("ShouldNotHaveErrorsWhenAllFieldsAreValidForNonVendor(): result → {Result}", result);
        result.ShouldNotHaveValidationErrorFor(x => x.ReviewText);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorsWhenAllFieldsAreValidForNonVendor()");
    }

    #endregion

    #region Other Properties Validation Tests

    [Test]
    public void ShouldNotHaveErrorForOtherPropertiesWhenRequiredFieldsAreValid()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorForOtherPropertiesWhenRequiredFieldsAreValid()");
        var model = new ProductReviewModel
        {
            Title = "Good Product",
            ReviewText = "Satisfied with the purchase",
            Rating = 4,
            IsApproved = false,
            ReplyText = "Thank you for your review"
        };

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Rating);
		logger.LogDebug("ShouldNotHaveErrorForOtherPropertiesWhenRequiredFieldsAreValid(): result → {Result}", result);
        result.ShouldNotHaveValidationErrorFor(x => x.IsApproved);
        result.ShouldNotHaveValidationErrorFor(x => x.ReplyText);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorForOtherPropertiesWhenRequiredFieldsAreValid()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenOptionalFieldsAreNull()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenOptionalFieldsAreNull()");
        var model = new ProductReviewModel
        {
            Title = "Average Product",
            ReviewText = "The product is okay",
            ReplyText = null
        };

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Title);
		logger.LogDebug("ShouldNotHaveErrorWhenOptionalFieldsAreNull(): result → {Result}", result);
        result.ShouldNotHaveValidationErrorFor(x => x.ReviewText);
        result.ShouldNotHaveValidationErrorFor(x => x.ReplyText);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenOptionalFieldsAreNull()");
    }

    #endregion

    #region Database Validation Rules Tests

    [Test]
    public void ShouldInheritDatabaseValidationRules()
    {
		logger.LogDebug(">>> Entering ShouldInheritDatabaseValidationRules()");
        // The validator calls SetDatabaseValidationRules<ProductReview>()
        // This test ensures the method is called and basic validation works
        var model = new ProductReviewModel
        {
            Title = "Valid Review Title",
            ReviewText = "Valid review content"
        };

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
		logger.LogDebug("<<< Exiting ShouldInheritDatabaseValidationRules()");
		logger.LogDebug("ShouldInheritDatabaseValidationRules(): result → {Result}", result);
    }

    #endregion

    #region Edge Cases

    [Test]
    public void ShouldNotHaveErrorWhenTitleAndReviewTextAreMinimalValidValues()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenTitleAndReviewTextAreMinimalValidValues()");
        var model = new ProductReviewModel
        {
            Title = "A",
            ReviewText = "B"
        };

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Title);
		logger.LogDebug("ShouldNotHaveErrorWhenTitleAndReviewTextAreMinimalValidValues(): result → {Result}", result);
        result.ShouldNotHaveValidationErrorFor(x => x.ReviewText);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenTitleAndReviewTextAreMinimalValidValues()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenTitleAndReviewTextContainSpecialCharacters()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenTitleAndReviewTextContainSpecialCharacters()");
        var model = new ProductReviewModel
        {
            Title = "Great Product! 5/5 ⭐⭐⭐⭐⭐",
            ReviewText = "This product is amazing! I would definitely recommend it to others. Price: $99.99"
        };

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Title);
		logger.LogDebug("ShouldNotHaveErrorWhenTitleAndReviewTextContainSpecialCharacters(): result → {Result}", result);
        result.ShouldNotHaveValidationErrorFor(x => x.ReviewText);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenTitleAndReviewTextContainSpecialCharacters()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenTitleAndReviewTextContainUnicodeCharacters()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenTitleAndReviewTextContainUnicodeCharacters()");
        var model = new ProductReviewModel
        {
            Title = "Ausgezeichnetes Produkt",
            ReviewText = "Ich bin sehr zufrieden mit diesem Produkt. Die Qualität ist hervorragend."
        };

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Title);
		logger.LogDebug("ShouldNotHaveErrorWhenTitleAndReviewTextContainUnicodeCharacters(): result → {Result}", result);
        result.ShouldNotHaveValidationErrorFor(x => x.ReviewText);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenTitleAndReviewTextContainUnicodeCharacters()");
    }

    #endregion
}
