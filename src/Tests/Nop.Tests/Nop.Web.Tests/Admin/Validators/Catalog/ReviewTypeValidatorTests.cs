using FluentValidation.TestHelper;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Areas.Admin.Validators.Catalog;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Admin.Validators.Catalog;

[TestFixture]
public class ReviewTypeValidatorTests : BaseNopTest
{
    private ReviewTypeValidator _validator;

    [OneTimeSetUp]
    public void Setup()
    {
		logger.LogDebug(">>> Entering Setup()");
        _validator = new ReviewTypeValidator(GetService<ILocalizationService>());
		logger.LogDebug("<<< Exiting Setup()");
		logger.LogDebug("Setup(): _validator → {_validator}", _validator);
    }

    #region Name Validation Tests

    [Test]
    public void ShouldHaveErrorWhenNameIsNull()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenNameIsNull()");
        var model = new ReviewTypeModel
        {
            Name = null,
            Description = "Valid description"
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Name);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenNameIsNull()");
    }

    [Test]
    public void ShouldHaveErrorWhenNameIsEmpty()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenNameIsEmpty()");
        var model = new ReviewTypeModel
        {
            Name = string.Empty,
            Description = "Valid description"
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Name);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenNameIsEmpty()");
    }

    [Test]
    public void ShouldHaveErrorWhenNameIsWhitespace()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenNameIsWhitespace()");
        var model = new ReviewTypeModel
        {
            Name = "   ",
            Description = "Valid description"
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Name);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenNameIsWhitespace()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenNameIsSpecified()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenNameIsSpecified()");
        var model = new ReviewTypeModel
        {
            Name = "Quality Rating",
            Description = "Valid description"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Name);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenNameIsSpecified()");
    }

    #endregion

    #region Description Validation Tests

    [Test]
    public void ShouldHaveErrorWhenDescriptionIsNull()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenDescriptionIsNull()");
        var model = new ReviewTypeModel
        {
            Name = "Valid Name",
            Description = null
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Description);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenDescriptionIsNull()");
    }

    [Test]
    public void ShouldHaveErrorWhenDescriptionIsEmpty()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenDescriptionIsEmpty()");
        var model = new ReviewTypeModel
        {
            Name = "Valid Name",
            Description = string.Empty
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Description);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenDescriptionIsEmpty()");
    }

    [Test]
    public void ShouldHaveErrorWhenDescriptionIsWhitespace()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenDescriptionIsWhitespace()");
        var model = new ReviewTypeModel
        {
            Name = "Valid Name",
            Description = "   "
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Description);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenDescriptionIsWhitespace()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenDescriptionIsSpecified()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenDescriptionIsSpecified()");
        var model = new ReviewTypeModel
        {
            Name = "Valid Name",
            Description = "Rate the overall quality of the product"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Description);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenDescriptionIsSpecified()");
    }

    #endregion

    #region Combined Validation Tests

    [Test]
    public void ShouldHaveMultipleErrorsWhenBothFieldsAreInvalid()
    {
		logger.LogDebug(">>> Entering ShouldHaveMultipleErrorsWhenBothFieldsAreInvalid()");
        var model = new ReviewTypeModel
        {
            Name = null,
            Description = string.Empty
        };

        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Name);
		logger.LogDebug("ShouldHaveMultipleErrorsWhenBothFieldsAreInvalid(): result → {Result}", result);
        result.ShouldHaveValidationErrorFor(x => x.Description);
		logger.LogDebug("<<< Exiting ShouldHaveMultipleErrorsWhenBothFieldsAreInvalid()");
    }

    [Test]
    public void ShouldNotHaveErrorsWhenAllFieldsAreValid()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorsWhenAllFieldsAreValid()");
        var model = new ReviewTypeModel
        {
            Name = "Service Quality",
            Description = "Rate the quality of customer service received",
            DisplayOrder = 1,
            VisibleToAllCustomers = true
        };

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
		logger.LogDebug("ShouldNotHaveErrorsWhenAllFieldsAreValid(): result → {Result}", result);
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorsWhenAllFieldsAreValid()");
    }

    [Test]
    public void ShouldNotHaveErrorForOtherPropertiesWhenRequiredFieldsAreValid()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorForOtherPropertiesWhenRequiredFieldsAreValid()");
        var model = new ReviewTypeModel
        {
            Name = "Delivery Speed",
            Description = "Rate the speed of product delivery",
            DisplayOrder = 5,
            VisibleToAllCustomers = false
        };

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.DisplayOrder);
		logger.LogDebug("ShouldNotHaveErrorForOtherPropertiesWhenRequiredFieldsAreValid(): result → {Result}", result);
        result.ShouldNotHaveValidationErrorFor(x => x.VisibleToAllCustomers);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorForOtherPropertiesWhenRequiredFieldsAreValid()");
    }

    #endregion

    #region Edge Cases

    [Test]
    public void ShouldNotHaveErrorWhenNameAndDescriptionAreMinimalValidValues()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenNameAndDescriptionAreMinimalValidValues()");
        var model = new ReviewTypeModel
        {
            Name = "A",
            Description = "B"
        };

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
		logger.LogDebug("ShouldNotHaveErrorWhenNameAndDescriptionAreMinimalValidValues(): result → {Result}", result);
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenNameAndDescriptionAreMinimalValidValues()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenNameAndDescriptionContainSpecialCharacters()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenNameAndDescriptionContainSpecialCharacters()");
        var model = new ReviewTypeModel
        {
            Name = "Price/Value Ratio",
            Description = "Rate the price-to-value ratio (1-5 stars)"
        };

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
		logger.LogDebug("ShouldNotHaveErrorWhenNameAndDescriptionContainSpecialCharacters(): result → {Result}", result);
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenNameAndDescriptionContainSpecialCharacters()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenNameAndDescriptionContainUnicodeCharacters()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenNameAndDescriptionContainUnicodeCharacters()");
        var model = new ReviewTypeModel
        {
            Name = "Qualité",
            Description = "Évaluez la qualité du produit"
        };

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
		logger.LogDebug("ShouldNotHaveErrorWhenNameAndDescriptionContainUnicodeCharacters(): result → {Result}", result);
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenNameAndDescriptionContainUnicodeCharacters()");
    }

    #endregion

    #region Database Validation Rules Tests

    [Test]
    public void ShouldInheritDatabaseValidationRules()
    {
		logger.LogDebug(">>> Entering ShouldInheritDatabaseValidationRules()");
        // The validator calls SetDatabaseValidationRules<ReviewType>()
        // This test ensures the method is called and basic validation works
        var model = new ReviewTypeModel
        {
            Name = "Valid Review Type",
            Description = "Valid description for the review type"
        };

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
		logger.LogDebug("<<< Exiting ShouldInheritDatabaseValidationRules()");
		logger.LogDebug("ShouldInheritDatabaseValidationRules(): result → {Result}", result);
    }

    #endregion
}
