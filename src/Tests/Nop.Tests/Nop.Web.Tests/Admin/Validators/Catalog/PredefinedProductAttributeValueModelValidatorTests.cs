using FluentValidation.TestHelper;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Areas.Admin.Validators.Catalog;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Admin.Validators.Catalog;

[TestFixture]
public class PredefinedProductAttributeValueModelValidatorTests : BaseNopTest
{
    private PredefinedProductAttributeValueModelValidator _validator;

    [OneTimeSetUp]
    public void Setup()
    {
		logger.LogDebug(">>> Entering Setup()");
        _validator = new PredefinedProductAttributeValueModelValidator(GetService<ILocalizationService>());
		logger.LogDebug("<<< Exiting Setup()");
		logger.LogDebug("Setup(): _validator → {_validator}", _validator);
    }

    #region Name Validation Tests

    [Test]
    public void ShouldHaveErrorWhenNameIsNull()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenNameIsNull()");
        var model = new PredefinedProductAttributeValueModel
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
        var model = new PredefinedProductAttributeValueModel
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
        var model = new PredefinedProductAttributeValueModel
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
        var model = new PredefinedProductAttributeValueModel
        {
            Name = "Small"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Name);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenNameIsSpecified()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenNameHasValidValue()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenNameHasValidValue()");
        var model = new PredefinedProductAttributeValueModel
        {
            Name = "Large Size"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Name);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenNameHasValidValue()");
    }

    #endregion

    #region Other Properties Validation Tests

    [Test]
    public void ShouldNotHaveErrorForOtherPropertiesWhenNameIsValid()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorForOtherPropertiesWhenNameIsValid()");
        var model = new PredefinedProductAttributeValueModel
        {
            Name = "Medium",
            ProductAttributeId = 1,
            PriceAdjustment = 10.50m,
            PriceAdjustmentUsePercentage = true,
            WeightAdjustment = 0.5m,
            Cost = 5.25m,
            IsPreSelected = false,
            DisplayOrder = 1
        };

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
		logger.LogDebug("ShouldNotHaveErrorForOtherPropertiesWhenNameIsValid(): result → {Result}", result);
        result.ShouldNotHaveValidationErrorFor(x => x.ProductAttributeId);
        result.ShouldNotHaveValidationErrorFor(x => x.PriceAdjustment);
        result.ShouldNotHaveValidationErrorFor(x => x.PriceAdjustmentUsePercentage);
        result.ShouldNotHaveValidationErrorFor(x => x.WeightAdjustment);
        result.ShouldNotHaveValidationErrorFor(x => x.Cost);
        result.ShouldNotHaveValidationErrorFor(x => x.IsPreSelected);
        result.ShouldNotHaveValidationErrorFor(x => x.DisplayOrder);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorForOtherPropertiesWhenNameIsValid()");
    }

    #endregion

    #region Edge Cases

    [Test]
    public void ShouldNotHaveErrorWhenNameIsMinimalValidValue()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenNameIsMinimalValidValue()");
        var model = new PredefinedProductAttributeValueModel
        {
            Name = "A"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Name);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenNameIsMinimalValidValue()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenNameIsLongValidValue()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenNameIsLongValidValue()");
        var model = new PredefinedProductAttributeValueModel
        {
            Name = new string('A', 400) // Maximum length based on database schema
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Name);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenNameIsLongValidValue()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenNameContainsSpecialCharacters()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenNameContainsSpecialCharacters()");
        var model = new PredefinedProductAttributeValueModel
        {
            Name = "Size: XL (Extra Large) - 100%"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Name);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenNameContainsSpecialCharacters()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenNameContainsNumbers()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenNameContainsNumbers()");
        var model = new PredefinedProductAttributeValueModel
        {
            Name = "Size 42"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Name);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenNameContainsNumbers()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenNameContainsUnicodeCharacters()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenNameContainsUnicodeCharacters()");
        var model = new PredefinedProductAttributeValueModel
        {
            Name = "Größe: Groß"
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
        var model = new PredefinedProductAttributeValueModel
        {
            Id = 1,
            Name = "Extra Large",
            ProductAttributeId = 5,
            PriceAdjustment = 15.99m,
            PriceAdjustmentUsePercentage = false,
            WeightAdjustment = 1.2m,
            Cost = 8.50m,
            IsPreSelected = true,
            DisplayOrder = 10
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
        var model = new PredefinedProductAttributeValueModel
        {
            Id = 1,
            Name = "", // Invalid
            ProductAttributeId = 5,
            PriceAdjustment = 15.99m,
            PriceAdjustmentUsePercentage = false,
            WeightAdjustment = 1.2m,
            Cost = 8.50m,
            IsPreSelected = true,
            DisplayOrder = 10
        };

        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Name);
		logger.LogDebug("ShouldFailValidationOnlyForNameWhenNameIsInvalid(): result → {Result}", result);
        result.ShouldNotHaveValidationErrorFor(x => x.ProductAttributeId);
        result.ShouldNotHaveValidationErrorFor(x => x.PriceAdjustment);
        result.ShouldNotHaveValidationErrorFor(x => x.PriceAdjustmentUsePercentage);
        result.ShouldNotHaveValidationErrorFor(x => x.WeightAdjustment);
        result.ShouldNotHaveValidationErrorFor(x => x.Cost);
        result.ShouldNotHaveValidationErrorFor(x => x.IsPreSelected);
        result.ShouldNotHaveValidationErrorFor(x => x.DisplayOrder);
		logger.LogDebug("<<< Exiting ShouldFailValidationOnlyForNameWhenNameIsInvalid()");
    }

    #endregion
}
