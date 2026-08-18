using FluentValidation.TestHelper;
using Nop.Core.Domain.ArtificialIntelligence;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Areas.Admin.Validators.Catalog;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Web.Tests.Admin.Validators.Catalog;

[TestFixture]
public class ArtificialIntelligenceFullDescriptionValidatorTests : BaseNopTest
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    private ArtificialIntelligenceFullDescriptionValidator _validator;

    [OneTimeSetUp]
    public void Setup()
    {
		_logger.LogDebug(">>> Entering Setup()");
        _validator = new ArtificialIntelligenceFullDescriptionValidator(GetService<ILocalizationService>());
		_logger.LogDebug("Setup(): _validator → {_validator}", _validator);
		_logger.LogDebug("<<< Exiting Setup()");
    }

    #region ProductName Validation Tests

    [Test]
    public void ShouldHaveErrorWhenProductNameIsNull()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenProductNameIsNull()");
        var model = new ArtificialIntelligenceFullDescriptionModel
        {
            ProductName = null
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.ProductName);
		_logger.LogDebug("<<< Exiting ShouldHaveErrorWhenProductNameIsNull()");
    }

    [Test]
    public void ShouldHaveErrorWhenProductNameIsEmpty()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenProductNameIsEmpty()");
        var model = new ArtificialIntelligenceFullDescriptionModel
        {
            ProductName = string.Empty
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.ProductName);
		_logger.LogDebug("<<< Exiting ShouldHaveErrorWhenProductNameIsEmpty()");
    }

    [Test]
    public void ShouldHaveErrorWhenProductNameIsWhitespace()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenProductNameIsWhitespace()");
        var model = new ArtificialIntelligenceFullDescriptionModel
        {
            ProductName = "   "
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.ProductName);
		_logger.LogDebug("<<< Exiting ShouldHaveErrorWhenProductNameIsWhitespace()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenProductNameIsSpecified()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenProductNameIsSpecified()");
        var model = new ArtificialIntelligenceFullDescriptionModel
        {
            ProductName = "Sample Product"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.ProductName);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenProductNameIsSpecified()");
    }

    #endregion

    #region Keywords Validation Tests

    [Test]
    public void ShouldHaveErrorWhenKeywordsIsNull()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenKeywordsIsNull()");
        var model = new ArtificialIntelligenceFullDescriptionModel
        {
            Keywords = null
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Keywords);
		_logger.LogDebug("<<< Exiting ShouldHaveErrorWhenKeywordsIsNull()");
    }

    [Test]
    public void ShouldHaveErrorWhenKeywordsIsEmpty()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenKeywordsIsEmpty()");
        var model = new ArtificialIntelligenceFullDescriptionModel
        {
            Keywords = string.Empty
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Keywords);
		_logger.LogDebug("<<< Exiting ShouldHaveErrorWhenKeywordsIsEmpty()");
    }

    [Test]
    public void ShouldHaveErrorWhenKeywordsIsWhitespace()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenKeywordsIsWhitespace()");
        var model = new ArtificialIntelligenceFullDescriptionModel
        {
            Keywords = "   "
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Keywords);
		_logger.LogDebug("<<< Exiting ShouldHaveErrorWhenKeywordsIsWhitespace()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenKeywordsIsSpecified()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenKeywordsIsSpecified()");
        var model = new ArtificialIntelligenceFullDescriptionModel
        {
            Keywords = "electronics, smartphone, mobile"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Keywords);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenKeywordsIsSpecified()");
    }

    #endregion

    #region CustomToneOfVoice Validation Tests

    [Test]
    public void ShouldHaveErrorWhenCustomToneOfVoiceIsNullAndToneOfVoiceIsCustom()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenCustomToneOfVoiceIsNullAndToneOfVoiceIsCustom()");
        var model = new ArtificialIntelligenceFullDescriptionModel
        {
            ToneOfVoiceId = (int)ToneOfVoiceType.Custom,
            CustomToneOfVoice = null
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.CustomToneOfVoice);
		_logger.LogDebug("<<< Exiting ShouldHaveErrorWhenCustomToneOfVoiceIsNullAndToneOfVoiceIsCustom()");
    }

    [Test]
    public void ShouldHaveErrorWhenCustomToneOfVoiceIsEmptyAndToneOfVoiceIsCustom()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenCustomToneOfVoiceIsEmptyAndToneOfVoiceIsCustom()");
        var model = new ArtificialIntelligenceFullDescriptionModel
        {
            ToneOfVoiceId = (int)ToneOfVoiceType.Custom,
            CustomToneOfVoice = string.Empty
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.CustomToneOfVoice);
		_logger.LogDebug("<<< Exiting ShouldHaveErrorWhenCustomToneOfVoiceIsEmptyAndToneOfVoiceIsCustom()");
    }

    [Test]
    public void ShouldHaveErrorWhenCustomToneOfVoiceIsWhitespaceAndToneOfVoiceIsCustom()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenCustomToneOfVoiceIsWhitespaceAndToneOfVoiceIsCustom()");
        var model = new ArtificialIntelligenceFullDescriptionModel
        {
            ToneOfVoiceId = (int)ToneOfVoiceType.Custom,
            CustomToneOfVoice = "   "
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.CustomToneOfVoice);
		_logger.LogDebug("<<< Exiting ShouldHaveErrorWhenCustomToneOfVoiceIsWhitespaceAndToneOfVoiceIsCustom()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenCustomToneOfVoiceIsSpecifiedAndToneOfVoiceIsCustom()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenCustomToneOfVoiceIsSpecifiedAndToneOfVoiceIsCustom()");
        var model = new ArtificialIntelligenceFullDescriptionModel
        {
            ToneOfVoiceId = (int)ToneOfVoiceType.Custom,
            CustomToneOfVoice = "Professional and informative"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.CustomToneOfVoice);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenCustomToneOfVoiceIsSpecifiedAndToneOfVoiceIsCustom()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenCustomToneOfVoiceIsNullAndToneOfVoiceIsNotCustom()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenCustomToneOfVoiceIsNullAndToneOfVoiceIsNotCustom()");
        var model = new ArtificialIntelligenceFullDescriptionModel
        {
            ToneOfVoiceId = (int)ToneOfVoiceType.Expert,
            CustomToneOfVoice = null
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.CustomToneOfVoice);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenCustomToneOfVoiceIsNullAndToneOfVoiceIsNotCustom()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenCustomToneOfVoiceIsEmptyAndToneOfVoiceIsNotCustom()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenCustomToneOfVoiceIsEmptyAndToneOfVoiceIsNotCustom()");
        var model = new ArtificialIntelligenceFullDescriptionModel
        {
            ToneOfVoiceId = (int)ToneOfVoiceType.Supportive,
            CustomToneOfVoice = string.Empty
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.CustomToneOfVoice);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenCustomToneOfVoiceIsEmptyAndToneOfVoiceIsNotCustom()");
    }

    #endregion

    #region Combined Validation Tests

    [Test]
    public void ShouldHaveMultipleErrorsWhenMultipleFieldsAreInvalid()
    {
		_logger.LogDebug(">>> Entering ShouldHaveMultipleErrorsWhenMultipleFieldsAreInvalid()");
        var model = new ArtificialIntelligenceFullDescriptionModel
        {
            ProductName = null,
            Keywords = string.Empty,
            ToneOfVoiceId = (int)ToneOfVoiceType.Custom,
            CustomToneOfVoice = "   "
        };

        var result = _validator.TestValidate(model);
		_logger.LogDebug("ShouldHaveMultipleErrorsWhenMultipleFieldsAreInvalid(): result → {Result}", result);
        result.ShouldHaveValidationErrorFor(x => x.ProductName);
        result.ShouldHaveValidationErrorFor(x => x.Keywords);
        result.ShouldHaveValidationErrorFor(x => x.CustomToneOfVoice);
		_logger.LogDebug("<<< Exiting ShouldHaveMultipleErrorsWhenMultipleFieldsAreInvalid()");
    }

    [Test]
    public void ShouldNotHaveErrorsWhenAllFieldsAreValidWithCustomTone()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorsWhenAllFieldsAreValidWithCustomTone()");
        var model = new ArtificialIntelligenceFullDescriptionModel
        {
            ProductName = "Smartphone XYZ",
            Keywords = "smartphone, mobile, electronics, communication",
            ToneOfVoiceId = (int)ToneOfVoiceType.Custom,
            CustomToneOfVoice = "Professional and engaging"
        };

        var result = _validator.TestValidate(model);
		_logger.LogDebug("ShouldNotHaveErrorsWhenAllFieldsAreValidWithCustomTone(): result → {Result}", result);
        result.ShouldNotHaveValidationErrorFor(x => x.ProductName);
        result.ShouldNotHaveValidationErrorFor(x => x.Keywords);
        result.ShouldNotHaveValidationErrorFor(x => x.CustomToneOfVoice);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorsWhenAllFieldsAreValidWithCustomTone()");
    }

    [Test]
    public void ShouldNotHaveErrorsWhenAllFieldsAreValidWithNonCustomTone()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorsWhenAllFieldsAreValidWithNonCustomTone()");
        var model = new ArtificialIntelligenceFullDescriptionModel
        {
            ProductName = "Laptop ABC",
            Keywords = "laptop, computer, technology, productivity",
            ToneOfVoiceId = (int)ToneOfVoiceType.Expert,
            CustomToneOfVoice = null
        };

        var result = _validator.TestValidate(model);
		_logger.LogDebug("ShouldNotHaveErrorsWhenAllFieldsAreValidWithNonCustomTone(): result → {Result}", result);
        result.ShouldNotHaveValidationErrorFor(x => x.ProductName);
        result.ShouldNotHaveValidationErrorFor(x => x.Keywords);
        result.ShouldNotHaveValidationErrorFor(x => x.CustomToneOfVoice);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorsWhenAllFieldsAreValidWithNonCustomTone()");
    }

    #endregion
}
