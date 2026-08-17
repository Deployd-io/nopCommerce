using FluentValidation.TestHelper;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Areas.Admin.Validators.Catalog;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Web.Tests.Admin.Validators.Catalog;

[TestFixture]
public class ProductTagValidatorTests : BaseNopTest
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    private ProductTagValidator _validator;

    [OneTimeSetUp]
    public void Setup()
    {
		_logger.LogDebug(">>> Entering Setup()");
        _validator = new ProductTagValidator(GetService<ILocalizationService>());
		_logger.LogDebug("<<< Exiting Setup()");
		_logger.LogDebug("Setup(): _validator → {_validator}", _validator);
    }

    #region Name Validation Tests

    [Test]
    public void ShouldHaveErrorWhenNameIsNull()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenNameIsNull()");
        var model = new ProductTagModel
        {
            Name = null
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Name);
		_logger.LogDebug("<<< Exiting ShouldHaveErrorWhenNameIsNull()");
    }

    [Test]
    public void ShouldHaveErrorWhenNameIsEmpty()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenNameIsEmpty()");
        var model = new ProductTagModel
        {
            Name = string.Empty
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Name);
		_logger.LogDebug("<<< Exiting ShouldHaveErrorWhenNameIsEmpty()");
    }

    [Test]
    public void ShouldHaveErrorWhenNameIsWhitespace()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenNameIsWhitespace()");
        var model = new ProductTagModel
        {
            Name = "   "
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Name);
		_logger.LogDebug("<<< Exiting ShouldHaveErrorWhenNameIsWhitespace()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenNameIsSpecified()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenNameIsSpecified()");
        var model = new ProductTagModel
        {
            Name = "Electronics"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Name);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenNameIsSpecified()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenNameIsMinimalValidValue()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenNameIsMinimalValidValue()");
        var model = new ProductTagModel
        {
            Name = "A"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Name);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenNameIsMinimalValidValue()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenNameContainsSpecialCharacters()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenNameContainsSpecialCharacters()");
        var model = new ProductTagModel
        {
            Name = "High-Tech & Electronics"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Name);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenNameContainsSpecialCharacters()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenNameContainsNumbers()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenNameContainsNumbers()");
        var model = new ProductTagModel
        {
            Name = "Tech 2024"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Name);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenNameContainsNumbers()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenNameContainsUnicodeCharacters()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenNameContainsUnicodeCharacters()");
        var model = new ProductTagModel
        {
            Name = "Électronique"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Name);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenNameContainsUnicodeCharacters()");
    }

    #endregion

    #region Complete Model Validation Tests

    [Test]
    public void ShouldPassValidationWhenAllPropertiesAreValid()
    {
		_logger.LogDebug(">>> Entering ShouldPassValidationWhenAllPropertiesAreValid()");
        var model = new ProductTagModel
        {
            Id = 1,
            Name = "Smartphones",
            ProductCount = 25
        };

        var result = _validator.TestValidate(model);
		_logger.LogDebug("ShouldPassValidationWhenAllPropertiesAreValid(): result → {Result}", result);
        result.ShouldNotHaveAnyValidationErrors();
		_logger.LogDebug("<<< Exiting ShouldPassValidationWhenAllPropertiesAreValid()");
    }

    [Test]
    public void ShouldFailValidationOnlyForNameWhenNameIsInvalid()
    {
		_logger.LogDebug(">>> Entering ShouldFailValidationOnlyForNameWhenNameIsInvalid()");
        var model = new ProductTagModel
        {
            Id = 1,
            Name = "", // Invalid
            ProductCount = 10
        };

        var result = _validator.TestValidate(model);
		_logger.LogDebug("ShouldFailValidationOnlyForNameWhenNameIsInvalid(): result → {Result}", result);
        result.ShouldHaveValidationErrorFor(x => x.Name);
        result.ShouldNotHaveValidationErrorFor(x => x.ProductCount);
		_logger.LogDebug("<<< Exiting ShouldFailValidationOnlyForNameWhenNameIsInvalid()");
    }

    [Test]
    public void ShouldNotHaveErrorForOtherPropertiesWhenNameIsValid()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorForOtherPropertiesWhenNameIsValid()");
        var model = new ProductTagModel
        {
            Name = "Laptops",
            ProductCount = 0 // Should not be validated
        };

        var result = _validator.TestValidate(model);
		_logger.LogDebug("ShouldNotHaveErrorForOtherPropertiesWhenNameIsValid(): result → {Result}", result);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
        result.ShouldNotHaveValidationErrorFor(x => x.ProductCount);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorForOtherPropertiesWhenNameIsValid()");
    }

    #endregion

    #region Database Validation Rules Tests

    [Test]
    public void ShouldInheritDatabaseValidationRules()
    {
		_logger.LogDebug(">>> Entering ShouldInheritDatabaseValidationRules()");
        // The validator calls SetDatabaseValidationRules<ProductTag>()
        // This test ensures the method is called and basic validation works
        var model = new ProductTagModel
        {
            Name = "Valid Tag Name"
        };

        var result = _validator.TestValidate(model);
		_logger.LogDebug("ShouldInheritDatabaseValidationRules(): result → {Result}", result);
        result.ShouldNotHaveAnyValidationErrors();
		_logger.LogDebug("<<< Exiting ShouldInheritDatabaseValidationRules()");
    }

    #endregion
}
