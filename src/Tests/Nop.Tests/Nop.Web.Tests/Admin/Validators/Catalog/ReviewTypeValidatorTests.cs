using FluentValidation.TestHelper;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Areas.Admin.Validators.Catalog;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Web.Tests.Admin.Validators.Catalog;

[TestFixture]
public class ReviewTypeValidatorTests : BaseNopTest
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    private ReviewTypeValidator _validator;

    [OneTimeSetUp]
    public void Setup()
    {
		_logger.LogDebug(">>> Entering Setup()");
        _validator = new ReviewTypeValidator(GetService<ILocalizationService>());
		_logger.LogDebug("Setup(): _validator → {_validator}", _validator);
		_logger.LogDebug("<<< Exiting Setup()");
    }

    #region Name Validation Tests

    [Test]
    public void ShouldHaveErrorWhenNameIsNull()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenNameIsNull()");
        var model = new ReviewTypeModel
        {
            Name = null,
            Description = "Valid description"
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Name);
		_logger.LogDebug("<<< Exiting ShouldHaveErrorWhenNameIsNull()");
    }

    [Test]
    public void ShouldHaveErrorWhenNameIsEmpty()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenNameIsEmpty()");
        var model = new ReviewTypeModel
        {
            Name = string.Empty,
            Description = "Valid description"
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Name);
		_logger.LogDebug("<<< Exiting ShouldHaveErrorWhenNameIsEmpty()");
    }

    [Test]
    public void ShouldHaveErrorWhenNameIsWhitespace()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenNameIsWhitespace()");
        var model = new ReviewTypeModel
        {
            Name = "   ",
            Description = "Valid description"
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Name);
		_logger.LogDebug("<<< Exiting ShouldHaveErrorWhenNameIsWhitespace()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenNameIsSpecified()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenNameIsSpecified()");
        var model = new ReviewTypeModel
        {
            Name = "Quality Rating",
            Description = "Valid description"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Name);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenNameIsSpecified()");
    }

    #endregion

    #region Description Validation Tests

    [Test]
    public void ShouldHaveErrorWhenDescriptionIsNull()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenDescriptionIsNull()");
        var model = new ReviewTypeModel
        {
            Name = "Valid Name",
            Description = null
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Description);
		_logger.LogDebug("<<< Exiting ShouldHaveErrorWhenDescriptionIsNull()");
    }

    [Test]
    public void ShouldHaveErrorWhenDescriptionIsEmpty()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenDescriptionIsEmpty()");
        var model = new ReviewTypeModel
        {
            Name = "Valid Name",
            Description = string.Empty
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Description);
		_logger.LogDebug("<<< Exiting ShouldHaveErrorWhenDescriptionIsEmpty()");
    }

    [Test]
    public void ShouldHaveErrorWhenDescriptionIsWhitespace()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenDescriptionIsWhitespace()");
        var model = new ReviewTypeModel
        {
            Name = "Valid Name",
            Description = "   "
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Description);
		_logger.LogDebug("<<< Exiting ShouldHaveErrorWhenDescriptionIsWhitespace()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenDescriptionIsSpecified()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenDescriptionIsSpecified()");
        var model = new ReviewTypeModel
        {
            Name = "Valid Name",
            Description = "Rate the overall quality of the product"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Description);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenDescriptionIsSpecified()");
    }

    #endregion

    #region Combined Validation Tests

    [Test]
    public void ShouldHaveMultipleErrorsWhenBothFieldsAreInvalid()
    {
		_logger.LogDebug(">>> Entering ShouldHaveMultipleErrorsWhenBothFieldsAreInvalid()");
        var model = new ReviewTypeModel
        {
            Name = null,
            Description = string.Empty
        };

        var result = _validator.TestValidate(model);
		_logger.LogDebug("ShouldHaveMultipleErrorsWhenBothFieldsAreInvalid(): result → {Result}", result);
        result.ShouldHaveValidationErrorFor(x => x.Name);
        result.ShouldHaveValidationErrorFor(x => x.Description);
		_logger.LogDebug("<<< Exiting ShouldHaveMultipleErrorsWhenBothFieldsAreInvalid()");
    }

    [Test]
    public void ShouldNotHaveErrorsWhenAllFieldsAreValid()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorsWhenAllFieldsAreValid()");
        var model = new ReviewTypeModel
        {
            Name = "Service Quality",
            Description = "Rate the quality of customer service received",
            DisplayOrder = 1,
            VisibleToAllCustomers = true
        };

        var result = _validator.TestValidate(model);
		_logger.LogDebug("ShouldNotHaveErrorsWhenAllFieldsAreValid(): result → {Result}", result);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorsWhenAllFieldsAreValid()");
    }

    [Test]
    public void ShouldNotHaveErrorForOtherPropertiesWhenRequiredFieldsAreValid()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorForOtherPropertiesWhenRequiredFieldsAreValid()");
        var model = new ReviewTypeModel
        {
            Name = "Delivery Speed",
            Description = "Rate the speed of product delivery",
            DisplayOrder = 5,
            VisibleToAllCustomers = false
        };

        var result = _validator.TestValidate(model);
		_logger.LogDebug("ShouldNotHaveErrorForOtherPropertiesWhenRequiredFieldsAreValid(): result → {Result}", result);
        result.ShouldNotHaveValidationErrorFor(x => x.DisplayOrder);
        result.ShouldNotHaveValidationErrorFor(x => x.VisibleToAllCustomers);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorForOtherPropertiesWhenRequiredFieldsAreValid()");
    }

    #endregion

    #region Edge Cases

    [Test]
    public void ShouldNotHaveErrorWhenNameAndDescriptionAreMinimalValidValues()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenNameAndDescriptionAreMinimalValidValues()");
        var model = new ReviewTypeModel
        {
            Name = "A",
            Description = "B"
        };

        var result = _validator.TestValidate(model);
		_logger.LogDebug("ShouldNotHaveErrorWhenNameAndDescriptionAreMinimalValidValues(): result → {Result}", result);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenNameAndDescriptionAreMinimalValidValues()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenNameAndDescriptionContainSpecialCharacters()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenNameAndDescriptionContainSpecialCharacters()");
        var model = new ReviewTypeModel
        {
            Name = "Price/Value Ratio",
            Description = "Rate the price-to-value ratio (1-5 stars)"
        };

        var result = _validator.TestValidate(model);
		_logger.LogDebug("ShouldNotHaveErrorWhenNameAndDescriptionContainSpecialCharacters(): result → {Result}", result);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenNameAndDescriptionContainSpecialCharacters()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenNameAndDescriptionContainUnicodeCharacters()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenNameAndDescriptionContainUnicodeCharacters()");
        var model = new ReviewTypeModel
        {
            Name = "Qualité",
            Description = "Évaluez la qualité du produit"
        };

        var result = _validator.TestValidate(model);
		_logger.LogDebug("ShouldNotHaveErrorWhenNameAndDescriptionContainUnicodeCharacters(): result → {Result}", result);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenNameAndDescriptionContainUnicodeCharacters()");
    }

    #endregion

    #region Database Validation Rules Tests

    [Test]
    public void ShouldInheritDatabaseValidationRules()
    {
		_logger.LogDebug(">>> Entering ShouldInheritDatabaseValidationRules()");
        // The validator calls SetDatabaseValidationRules<ReviewType>()
        // This test ensures the method is called and basic validation works
        var model = new ReviewTypeModel
        {
            Name = "Valid Review Type",
            Description = "Valid description for the review type"
        };

        var result = _validator.TestValidate(model);
		_logger.LogDebug("ShouldInheritDatabaseValidationRules(): result → {Result}", result);
        result.ShouldNotHaveAnyValidationErrors();
		_logger.LogDebug("<<< Exiting ShouldInheritDatabaseValidationRules()");
    }

    #endregion
}
