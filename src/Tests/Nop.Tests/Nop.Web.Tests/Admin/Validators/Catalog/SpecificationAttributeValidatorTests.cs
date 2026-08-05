using FluentValidation.TestHelper;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Areas.Admin.Validators.Catalog;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Web.Tests.Admin.Validators.Catalog;

[TestFixture]
public class SpecificationAttributeValidatorTests : BaseNopTest
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    private SpecificationAttributeValidator _validator;

    [OneTimeSetUp]
    public void Setup()
    {
        _validator = new SpecificationAttributeValidator(GetService<ILocalizationService>());
    }

    #region Name Validation Tests

    [Test]
    public void ShouldHaveErrorWhenNameIsNull()
    {
        var model = new SpecificationAttributeModel
        {
            Name = null
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public void ShouldHaveErrorWhenNameIsEmpty()
    {
        var model = new SpecificationAttributeModel
        {
            Name = string.Empty
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public void ShouldHaveErrorWhenNameIsWhitespace()
    {
        var model = new SpecificationAttributeModel
        {
            Name = "   "
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public void ShouldNotHaveErrorWhenNameIsSpecified()
    {
        var model = new SpecificationAttributeModel
        {
            Name = "Screen Size"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Name);
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenNameIsSpecified()");
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenNameIsSpecified()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenNameIsMinimalValidValue()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenNameIsMinimalValidValue()");
        var model = new SpecificationAttributeModel
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
        var model = new SpecificationAttributeModel
        {
            Name = "CPU Speed (GHz)"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Name);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenNameContainsSpecialCharacters()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenNameContainsNumbers()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenNameContainsNumbers()");
        var model = new SpecificationAttributeModel
        {
            Name = "USB 3.0 Ports"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Name);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenNameContainsNumbers()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenNameContainsUnicodeCharacters()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenNameContainsUnicodeCharacters()");
        var model = new SpecificationAttributeModel
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
        var model = new SpecificationAttributeModel
        {
            Id = 1,
            Name = "Memory Capacity",
            DisplayOrder = 1
        };

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void ShouldFailValidationOnlyForNameWhenNameIsInvalid()
    {
		_logger.LogDebug(">>> Entering ShouldFailValidationOnlyForNameWhenNameIsInvalid()");
        var model = new SpecificationAttributeModel
        {
            Id = 1,
            Name = "", // Invalid
            DisplayOrder = 5
        };

        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Name);
		_logger.LogDebug("ShouldFailValidationOnlyForNameWhenNameIsInvalid(): result → {Result}", result);
        result.ShouldNotHaveValidationErrorFor(x => x.DisplayOrder);
		_logger.LogDebug("<<< Exiting ShouldFailValidationOnlyForNameWhenNameIsInvalid()");
    }

    [Test]
    public void ShouldNotHaveErrorForOtherPropertiesWhenNameIsValid()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorForOtherPropertiesWhenNameIsValid()");
        var model = new SpecificationAttributeModel
        {
            Name = "Battery Life",
            DisplayOrder = 0 // Should not be validated
        };

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
		_logger.LogDebug("ShouldNotHaveErrorForOtherPropertiesWhenNameIsValid(): result → {Result}", result);
        result.ShouldNotHaveValidationErrorFor(x => x.DisplayOrder);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorForOtherPropertiesWhenNameIsValid()");
    }

    #endregion

    #region Database Validation Rules Tests

    [Test]
    public void ShouldInheritDatabaseValidationRules()
    {
		_logger.LogDebug(">>> Entering ShouldInheritDatabaseValidationRules()");
        // The validator calls SetDatabaseValidationRules<SpecificationAttribute>()
        // This test ensures the method is called and basic validation works
        var model = new SpecificationAttributeModel
        {
            Name = "Valid Specification Attribute"
        };

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
		_logger.LogDebug("<<< Exiting ShouldInheritDatabaseValidationRules()");
		_logger.LogDebug("ShouldInheritDatabaseValidationRules(): result → {Result}", result);
    }

    #endregion
}

[TestFixture]
public class SpecificationAttributeGroupValidatorTests : BaseNopTest
{
    private SpecificationAttributeGroupValidator _validator;

    [OneTimeSetUp]
    public void Setup()
    {
        _validator = new SpecificationAttributeGroupValidator(GetService<ILocalizationService>());
		_logger.LogDebug(">>> Entering Setup()");
		_logger.LogDebug("<<< Exiting Setup()");
    }

    #region Name Validation Tests

    [Test]
    public void ShouldHaveErrorWhenNameIsNull()
    {
        var model = new SpecificationAttributeGroupModel
        {
            Name = null
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Name);
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenNameIsNull()");
		_logger.LogDebug("<<< Exiting ShouldHaveErrorWhenNameIsNull()");
    }

    [Test]
    public void ShouldHaveErrorWhenNameIsEmpty()
    {
        var model = new SpecificationAttributeGroupModel
        {
            Name = string.Empty
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Name);
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenNameIsEmpty()");
		_logger.LogDebug("<<< Exiting ShouldHaveErrorWhenNameIsEmpty()");
    }

    [Test]
    public void ShouldHaveErrorWhenNameIsWhitespace()
    {
        var model = new SpecificationAttributeGroupModel
        {
            Name = "   "
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Name);
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenNameIsWhitespace()");
		_logger.LogDebug("<<< Exiting ShouldHaveErrorWhenNameIsWhitespace()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenNameIsSpecified()
    {
        var model = new SpecificationAttributeGroupModel
        {
            Name = "Technical Specifications"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public void ShouldPassValidationWhenAllPropertiesAreValid()
    {
        var model = new SpecificationAttributeGroupModel
        {
            Id = 1,
            Name = "Hardware Specifications",
            DisplayOrder = 1
        };

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
		_logger.LogDebug(">>> Entering ShouldPassValidationWhenAllPropertiesAreValid()");
		_logger.LogDebug("<<< Exiting ShouldPassValidationWhenAllPropertiesAreValid()");
    }

    #endregion
}

[TestFixture]
public class SpecificationAttributeOptionValidatorTests : BaseNopTest
{
    private SpecificationAttributeOptionValidator _validator;

    [OneTimeSetUp]
    public void Setup()
    {
        _validator = new SpecificationAttributeOptionValidator(GetService<ILocalizationService>());
		_logger.LogDebug("Setup(): _validator → {_validator}", _validator);
    }

    #region Name Validation Tests

    [Test]
    public void ShouldHaveErrorWhenNameIsNull()
    {
        var model = new SpecificationAttributeOptionModel
        {
            Name = null
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public void ShouldHaveErrorWhenNameIsEmpty()
    {
        var model = new SpecificationAttributeOptionModel
        {
            Name = string.Empty
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public void ShouldHaveErrorWhenNameIsWhitespace()
    {
        var model = new SpecificationAttributeOptionModel
        {
            Name = "   "
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public void ShouldNotHaveErrorWhenNameIsSpecified()
    {
        var model = new SpecificationAttributeOptionModel
        {
            Name = "15 inches"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public void ShouldPassValidationWhenAllPropertiesAreValid()
    {
        var model = new SpecificationAttributeOptionModel
        {
            Id = 1,
            Name = "8GB",
            DisplayOrder = 1,
            ColorSquaresRgb = "#FF0000"
        };

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
		_logger.LogDebug("ShouldPassValidationWhenAllPropertiesAreValid(): result → {Result}", result);
    }

    #endregion
}
