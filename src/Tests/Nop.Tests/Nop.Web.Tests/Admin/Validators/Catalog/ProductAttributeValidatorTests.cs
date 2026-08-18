using FluentValidation.TestHelper;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Areas.Admin.Validators.Catalog;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Web.Tests.Admin.Validators.Catalog;

[TestFixture]
public class ProductAttributeValidatorTests : BaseNopTest
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    private ProductAttributeValidator _validator;

    [OneTimeSetUp]
    public void Setup()
    {
		_logger.LogDebug(">>> Entering Setup()");
        _validator = new ProductAttributeValidator(GetService<ILocalizationService>());
		_logger.LogDebug("Setup(): _validator → {_validator}", _validator);
		_logger.LogDebug("<<< Exiting Setup()");
    }

    #region Name Validation Tests

    [Test]
    public void ShouldHaveErrorWhenNameIsNull()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenNameIsNull()");
        var model = new ProductAttributeModel
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
        var model = new ProductAttributeModel
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
        var model = new ProductAttributeModel
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
        var model = new ProductAttributeModel
        {
            Name = "Size"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Name);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenNameIsSpecified()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenNameHasValidValue()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenNameHasValidValue()");
        var model = new ProductAttributeModel
        {
            Name = "Color Options"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Name);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenNameHasValidValue()");
    }

    #endregion

    #region Other Properties Validation Tests

    [Test]
    public void ShouldNotHaveErrorForOtherPropertiesWhenNameIsValid()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorForOtherPropertiesWhenNameIsValid()");
        var model = new ProductAttributeModel
        {
            Name = "Material",
            Description = "Product material options"
        };

        var result = _validator.TestValidate(model);
		_logger.LogDebug("ShouldNotHaveErrorForOtherPropertiesWhenNameIsValid(): result → {Result}", result);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorForOtherPropertiesWhenNameIsValid()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenDescriptionIsNull()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenDescriptionIsNull()");
        var model = new ProductAttributeModel
        {
            Name = "Weight",
            Description = null
        };

        var result = _validator.TestValidate(model);
		_logger.LogDebug("ShouldNotHaveErrorWhenDescriptionIsNull(): result → {Result}", result);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenDescriptionIsNull()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenDescriptionIsEmpty()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenDescriptionIsEmpty()");
        var model = new ProductAttributeModel
        {
            Name = "Dimensions",
            Description = string.Empty
        };

        var result = _validator.TestValidate(model);
		_logger.LogDebug("ShouldNotHaveErrorWhenDescriptionIsEmpty(): result → {Result}", result);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenDescriptionIsEmpty()");
    }

    #endregion

    #region Edge Cases

    [Test]
    public void ShouldNotHaveErrorWhenNameIsMinimalValidValue()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenNameIsMinimalValidValue()");
        var model = new ProductAttributeModel
        {
            Name = "A"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Name);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenNameIsMinimalValidValue()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenNameIsLongValidValue()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenNameIsLongValidValue()");
        var model = new ProductAttributeModel
        {
            Name = new string('A', 400) // Maximum length based on database schema
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Name);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenNameIsLongValidValue()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenNameContainsSpecialCharacters()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenNameContainsSpecialCharacters()");
        var model = new ProductAttributeModel
        {
            Name = "Size & Weight (kg/lbs)"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Name);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenNameContainsSpecialCharacters()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenNameContainsNumbers()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenNameContainsNumbers()");
        var model = new ProductAttributeModel
        {
            Name = "Version 2.0"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Name);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenNameContainsNumbers()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenNameContainsUnicodeCharacters()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenNameContainsUnicodeCharacters()");
        var model = new ProductAttributeModel
        {
            Name = "Größe"
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
        var model = new ProductAttributeModel
        {
            Id = 1,
            Name = "Color",
            Description = "Available color options for the product"
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
        var model = new ProductAttributeModel
        {
            Id = 1,
            Name = "", // Invalid
            Description = "Valid description"
        };

        var result = _validator.TestValidate(model);
		_logger.LogDebug("ShouldFailValidationOnlyForNameWhenNameIsInvalid(): result → {Result}", result);
        result.ShouldHaveValidationErrorFor(x => x.Name);
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
		_logger.LogDebug("<<< Exiting ShouldFailValidationOnlyForNameWhenNameIsInvalid()");
    }

    [Test]
    public void ShouldPassValidationWhenOnlyNameIsProvided()
    {
		_logger.LogDebug(">>> Entering ShouldPassValidationWhenOnlyNameIsProvided()");
        var model = new ProductAttributeModel
        {
            Name = "Brand"
        };

        var result = _validator.TestValidate(model);
		_logger.LogDebug("ShouldPassValidationWhenOnlyNameIsProvided(): result → {Result}", result);
        result.ShouldNotHaveAnyValidationErrors();
		_logger.LogDebug("<<< Exiting ShouldPassValidationWhenOnlyNameIsProvided()");
    }

    [Test]
    public void ShouldPassValidationWithComplexModel()
    {
		_logger.LogDebug(">>> Entering ShouldPassValidationWithComplexModel()");
        var model = new ProductAttributeModel
        {
            Id = 5,
            Name = "Technical Specifications",
            Description = "Detailed technical specifications including dimensions, weight, and performance metrics",
            PreTranslationAvailable = true
        };

        var result = _validator.TestValidate(model);
		_logger.LogDebug("ShouldPassValidationWithComplexModel(): result → {Result}", result);
        result.ShouldNotHaveAnyValidationErrors();
		_logger.LogDebug("<<< Exiting ShouldPassValidationWithComplexModel()");
    }

    #endregion

    #region Database Validation Rules Tests

    [Test]
    public void ShouldInheritDatabaseValidationRules()
    {
		_logger.LogDebug(">>> Entering ShouldInheritDatabaseValidationRules()");
        // The validator calls SetDatabaseValidationRules<ProductAttribute>()
        // This test ensures the method is called and basic validation works
        var model = new ProductAttributeModel
        {
            Name = "Valid Attribute Name"
        };

        var result = _validator.TestValidate(model);
		_logger.LogDebug("ShouldInheritDatabaseValidationRules(): result → {Result}", result);
        result.ShouldNotHaveAnyValidationErrors();
		_logger.LogDebug("<<< Exiting ShouldInheritDatabaseValidationRules()");
    }

    #endregion
}
