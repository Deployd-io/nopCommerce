using FluentValidation.TestHelper;
using Nop.Core.Domain.ArtificialIntelligence;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Areas.Admin.Validators.Catalog;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Admin.Validators.Catalog;

[TestFixture]
public class ArtificialIntelligenceFullDescriptionValidatorTests : BaseNopTest
{
    private ArtificialIntelligenceFullDescriptionValidator _validator;

    [OneTimeSetUp]
    public void Setup()
    {
		logger.LogDebug(">>> Entering Setup()");
        _validator = new ArtificialIntelligenceFullDescriptionValidator(GetService<ILocalizationService>());
		logger.LogDebug("<<< Exiting Setup()");
		logger.LogDebug("Setup(): _validator → {_validator}", _validator);
    }

    #region ProductName Validation Tests

    [Test]
    public void ShouldHaveErrorWhenProductNameIsNull()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenProductNameIsNull()");
        var model = new ArtificialIntelligenceFullDescriptionModel
        {
            ProductName = null
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.ProductName);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenProductNameIsNull()");
    }

    [Test]
    public void ShouldHaveErrorWhenProductNameIsEmpty()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenProductNameIsEmpty()");
        var model = new ArtificialIntelligenceFullDescriptionModel
        {
            ProductName = string.Empty
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.ProductName);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenProductNameIsEmpty()");
    }

    [Test]
    public void ShouldHaveErrorWhenProductNameIsWhitespace()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenProductNameIsWhitespace()");
        var model = new ArtificialIntelligenceFullDescriptionModel
        {
            ProductName = "   "
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.ProductName);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenProductNameIsWhitespace()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenProductNameIsSpecified()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenProductNameIsSpecified()");
        var model = new ArtificialIntelligenceFullDescriptionModel
        {
            ProductName = "Sample Product"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.ProductName);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenProductNameIsSpecified()");
    }

    #endregion

    #region Keywords Validation Tests

    [Test]
    public void ShouldHaveErrorWhenKeywordsIsNull()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenKeywordsIsNull()");
        var model = new ArtificialIntelligenceFullDescriptionModel
        {
            Keywords = null
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Keywords);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenKeywordsIsNull()");
    }

    [Test]
    public void ShouldHaveErrorWhenKeywordsIsEmpty()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenKeywordsIsEmpty()");
        var model = new ArtificialIntelligenceFullDescriptionModel
        {
            Keywords = string.Empty
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Keywords);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenKeywordsIsEmpty()");
    }

    [Test]
    public void ShouldHaveErrorWhenKeywordsIsWhitespace()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenKeywordsIsWhitespace()");
        var model = new ArtificialIntelligenceFullDescriptionModel
        {
            Keywords = "   "
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Keywords);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenKeywordsIsWhitespace()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenKeywordsIsSpecified()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenKeywordsIsSpecified()");
        var model = new ArtificialIntelligenceFullDescriptionModel
        {
            Keywords = "electronics, smartphone, mobile"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Keywords);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenKeywordsIsSpecified()");
    }

    #endregion

    #region CustomToneOfVoice Validation Tests

    [Test]
    public void ShouldHaveErrorWhenCustomToneOfVoiceIsNullAndToneOfVoiceIsCustom()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenCustomToneOfVoiceIsNullAndToneOfVoiceIsCustom()");
        var model = new ArtificialIntelligenceFullDescriptionModel
        {
            ToneOfVoiceId = (int)ToneOfVoiceType.Custom,
            CustomToneOfVoice = null
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.CustomToneOfVoice);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenCustomToneOfVoiceIsNullAndToneOfVoiceIsCustom()");
    }

    [Test]
    public void ShouldHaveErrorWhenCustomToneOfVoiceIsEmptyAndToneOfVoiceIsCustom()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenCustomToneOfVoiceIsEmptyAndToneOfVoiceIsCustom()");
        var model = new ArtificialIntelligenceFullDescriptionModel
        {
            ToneOfVoiceId = (int)ToneOfVoiceType.Custom,
            CustomToneOfVoice = string.Empty
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.CustomToneOfVoice);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenCustomToneOfVoiceIsEmptyAndToneOfVoiceIsCustom()");
    }

    [Test]
    public void ShouldHaveErrorWhenCustomToneOfVoiceIsWhitespaceAndToneOfVoiceIsCustom()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenCustomToneOfVoiceIsWhitespaceAndToneOfVoiceIsCustom()");
        var model = new ArtificialIntelligenceFullDescriptionModel
        {
            ToneOfVoiceId = (int)ToneOfVoiceType.Custom,
            CustomToneOfVoice = "   "
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.CustomToneOfVoice);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenCustomToneOfVoiceIsWhitespaceAndToneOfVoiceIsCustom()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenCustomToneOfVoiceIsSpecifiedAndToneOfVoiceIsCustom()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenCustomToneOfVoiceIsSpecifiedAndToneOfVoiceIsCustom()");
        var model = new ArtificialIntelligenceFullDescriptionModel
        {
            ToneOfVoiceId = (int)ToneOfVoiceType.Custom,
            CustomToneOfVoice = "Professional and informative"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.CustomToneOfVoice);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenCustomToneOfVoiceIsSpecifiedAndToneOfVoiceIsCustom()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenCustomToneOfVoiceIsNullAndToneOfVoiceIsNotCustom()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenCustomToneOfVoiceIsNullAndToneOfVoiceIsNotCustom()");
        var model = new ArtificialIntelligenceFullDescriptionModel
        {
            ToneOfVoiceId = (int)ToneOfVoiceType.Expert,
            CustomToneOfVoice = null
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.CustomToneOfVoice);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenCustomToneOfVoiceIsNullAndToneOfVoiceIsNotCustom()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenCustomToneOfVoiceIsEmptyAndToneOfVoiceIsNotCustom()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenCustomToneOfVoiceIsEmptyAndToneOfVoiceIsNotCustom()");
        var model = new ArtificialIntelligenceFullDescriptionModel
        {
            ToneOfVoiceId = (int)ToneOfVoiceType.Supportive,
            CustomToneOfVoice = string.Empty
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.CustomToneOfVoice);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenCustomToneOfVoiceIsEmptyAndToneOfVoiceIsNotCustom()");
    }

    #endregion

    #region Combined Validation Tests

    [Test]
    public void ShouldHaveMultipleErrorsWhenMultipleFieldsAreInvalid()
    {
		logger.LogDebug(">>> Entering ShouldHaveMultipleErrorsWhenMultipleFieldsAreInvalid()");
        var model = new ArtificialIntelligenceFullDescriptionModel
        {
            ProductName = null,
            Keywords = string.Empty,
            ToneOfVoiceId = (int)ToneOfVoiceType.Custom,
            CustomToneOfVoice = "   "
        };

        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.ProductName);
		logger.LogDebug("ShouldHaveMultipleErrorsWhenMultipleFieldsAreInvalid(): result → {Result}", result);
        result.ShouldHaveValidationErrorFor(x => x.Keywords);
        result.ShouldHaveValidationErrorFor(x => x.CustomToneOfVoice);
		logger.LogDebug("<<< Exiting ShouldHaveMultipleErrorsWhenMultipleFieldsAreInvalid()");
    }

    [Test]
    public void ShouldNotHaveErrorsWhenAllFieldsAreValidWithCustomTone()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorsWhenAllFieldsAreValidWithCustomTone()");
        var model = new ArtificialIntelligenceFullDescriptionModel
        {
            ProductName = "Smartphone XYZ",
            Keywords = "smartphone, mobile, electronics, communication",
            ToneOfVoiceId = (int)ToneOfVoiceType.Custom,
            CustomToneOfVoice = "Professional and engaging"
        };

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.ProductName);
		logger.LogDebug("ShouldNotHaveErrorsWhenAllFieldsAreValidWithCustomTone(): result → {Result}", result);
        result.ShouldNotHaveValidationErrorFor(x => x.Keywords);
        result.ShouldNotHaveValidationErrorFor(x => x.CustomToneOfVoice);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorsWhenAllFieldsAreValidWithCustomTone()");
    }

    [Test]
    public void ShouldNotHaveErrorsWhenAllFieldsAreValidWithNonCustomTone()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorsWhenAllFieldsAreValidWithNonCustomTone()");
        var model = new ArtificialIntelligenceFullDescriptionModel
        {
            ProductName = "Laptop ABC",
            Keywords = "laptop, computer, technology, productivity",
            ToneOfVoiceId = (int)ToneOfVoiceType.Expert,
            CustomToneOfVoice = null
        };

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.ProductName);
		logger.LogDebug("ShouldNotHaveErrorsWhenAllFieldsAreValidWithNonCustomTone(): result → {Result}", result);
        result.ShouldNotHaveValidationErrorFor(x => x.Keywords);
        result.ShouldNotHaveValidationErrorFor(x => x.CustomToneOfVoice);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorsWhenAllFieldsAreValidWithNonCustomTone()");
    }

    #endregion
}
