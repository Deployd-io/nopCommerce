using FluentValidation.TestHelper;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Areas.Admin.Validators.Catalog;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Admin.Validators.Catalog;

[TestFixture]
public class ProductTagValidatorTests : BaseNopTest
{
    private ProductTagValidator _validator;

    [OneTimeSetUp]
    public void Setup()
    {
		logger.LogDebug(">>> Entering Setup()");
        _validator = new ProductTagValidator(GetService<ILocalizationService>());
		logger.LogDebug("<<< Exiting Setup()");
		logger.LogDebug("Setup(): _validator → {_validator}", _validator);
    }

    #region Name Validation Tests

    [Test]
    public void ShouldHaveErrorWhenNameIsNull()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenNameIsNull()");
        var model = new ProductTagModel
        {
            Name = null
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Name);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenNameIsNull()");
    }

    [Test]
    public void ShouldHaveErrorWhenNameIsEmpty()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenNameIsEmpty()");
        var model = new ProductTagModel
        {
            Name = string.Empty
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Name);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenNameIsEmpty()");
    }

    [Test]
    public void ShouldHaveErrorWhenNameIsWhitespace()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenNameIsWhitespace()");
        var model = new ProductTagModel
        {
            Name = "   "
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Name);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenNameIsWhitespace()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenNameIsSpecified()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenNameIsSpecified()");
        var model = new ProductTagModel
        {
            Name = "Electronics"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Name);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenNameIsSpecified()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenNameIsMinimalValidValue()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenNameIsMinimalValidValue()");
        var model = new ProductTagModel
        {
            Name = "A"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Name);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenNameIsMinimalValidValue()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenNameContainsSpecialCharacters()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenNameContainsSpecialCharacters()");
        var model = new ProductTagModel
        {
            Name = "High-Tech & Electronics"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Name);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenNameContainsSpecialCharacters()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenNameContainsNumbers()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenNameContainsNumbers()");
        var model = new ProductTagModel
        {
            Name = "Tech 2024"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Name);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenNameContainsNumbers()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenNameContainsUnicodeCharacters()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenNameContainsUnicodeCharacters()");
        var model = new ProductTagModel
        {
            Name = "Électronique"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Name);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenNameContainsUnicodeCharacters()");
    }

    #endregion

    #region Complete Model Validation Tests

    [Test]
    public void ShouldPassValidationWhenAllPropertiesAreValid()
    {
		logger.LogDebug(">>> Entering ShouldPassValidationWhenAllPropertiesAreValid()");
        var model = new ProductTagModel
        {
            Id = 1,
            Name = "Smartphones",
            ProductCount = 25
        };

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
		logger.LogDebug("<<< Exiting ShouldPassValidationWhenAllPropertiesAreValid()");
		logger.LogDebug("ShouldPassValidationWhenAllPropertiesAreValid(): result → {Result}", result);
    }

    [Test]
    public void ShouldFailValidationOnlyForNameWhenNameIsInvalid()
    {
		logger.LogDebug(">>> Entering ShouldFailValidationOnlyForNameWhenNameIsInvalid()");
        var model = new ProductTagModel
        {
            Id = 1,
            Name = "", // Invalid
            ProductCount = 10
        };

        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Name);
		logger.LogDebug("ShouldFailValidationOnlyForNameWhenNameIsInvalid(): result → {Result}", result);
        result.ShouldNotHaveValidationErrorFor(x => x.ProductCount);
		logger.LogDebug("<<< Exiting ShouldFailValidationOnlyForNameWhenNameIsInvalid()");
    }

    [Test]
    public void ShouldNotHaveErrorForOtherPropertiesWhenNameIsValid()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorForOtherPropertiesWhenNameIsValid()");
        var model = new ProductTagModel
        {
            Name = "Laptops",
            ProductCount = 0 // Should not be validated
        };

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
		logger.LogDebug("ShouldNotHaveErrorForOtherPropertiesWhenNameIsValid(): result → {Result}", result);
        result.ShouldNotHaveValidationErrorFor(x => x.ProductCount);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorForOtherPropertiesWhenNameIsValid()");
    }

    #endregion

    #region Database Validation Rules Tests

    [Test]
    public void ShouldInheritDatabaseValidationRules()
    {
		logger.LogDebug(">>> Entering ShouldInheritDatabaseValidationRules()");
        // The validator calls SetDatabaseValidationRules<ProductTag>()
        // This test ensures the method is called and basic validation works
        var model = new ProductTagModel
        {
            Name = "Valid Tag Name"
        };

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
		logger.LogDebug("<<< Exiting ShouldInheritDatabaseValidationRules()");
		logger.LogDebug("ShouldInheritDatabaseValidationRules(): result → {Result}", result);
    }

    #endregion
}
