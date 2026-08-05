using FluentValidation.TestHelper;
using Nop.Services.Localization;
using Nop.Services.Seo;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Areas.Admin.Validators.Catalog;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Web.Tests.Admin.Validators.Catalog;

[TestFixture]
public class ProductValidatorTests : BaseNopTest
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    private ProductValidator _validator;

    [OneTimeSetUp]
    public void Setup()
    {
		_logger.LogDebug(">>> Entering Setup()");
        _validator = new ProductValidator(GetService<ILocalizationService>());
		_logger.LogDebug("<<< Exiting Setup()");
		_logger.LogDebug("Setup(): _validator → {_validator}", _validator);
    }

    #region Name Validation Tests

    [Test]
    public void ShouldHaveErrorWhenNameIsNull()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenNameIsNull()");
        var model = new ProductModel
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
        var model = new ProductModel
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
        var model = new ProductModel
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
        var model = new ProductModel
        {
            Name = "Sample Product"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Name);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenNameIsSpecified()");
    }

    #endregion

    #region SeName Validation Tests

    [Test]
    public void ShouldNotHaveErrorWhenSeNameIsNull()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenSeNameIsNull()");
        var model = new ProductModel
        {
            Name = "Valid Product",
            SeName = null
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.SeName);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenSeNameIsNull()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenSeNameIsEmpty()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenSeNameIsEmpty()");
        var model = new ProductModel
        {
            Name = "Valid Product",
            SeName = string.Empty
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.SeName);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenSeNameIsEmpty()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenSeNameIsWithinMaxLength()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenSeNameIsWithinMaxLength()");
        var model = new ProductModel
        {
            Name = "Valid Product",
            SeName = new string('a', NopSeoDefaults.SearchEngineNameLength) // 200 characters
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.SeName);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenSeNameIsWithinMaxLength()");
    }

    [Test]
    public void ShouldHaveErrorWhenSeNameExceedsMaxLength()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenSeNameExceedsMaxLength()");
        var model = new ProductModel
        {
            Name = "Valid Product",
            SeName = new string('a', NopSeoDefaults.SearchEngineNameLength + 1) // 201 characters
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.SeName);
		_logger.LogDebug("<<< Exiting ShouldHaveErrorWhenSeNameExceedsMaxLength()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenSeNameIsValidLength()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenSeNameIsValidLength()");
        var model = new ProductModel
        {
            Name = "Valid Product",
            SeName = "sample-product-sename"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.SeName);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenSeNameIsValidLength()");
    }

    #endregion

    #region RentalPriceLength Validation Tests

    [Test]
    public void ShouldHaveErrorWhenRentalPriceLengthIsZeroAndIsRental()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenRentalPriceLengthIsZeroAndIsRental()");
        var model = new ProductModel
        {
            Name = "Valid Product",
            IsRental = true,
            RentalPriceLength = 0
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.RentalPriceLength);
		_logger.LogDebug("<<< Exiting ShouldHaveErrorWhenRentalPriceLengthIsZeroAndIsRental()");
    }

    [Test]
    public void ShouldHaveErrorWhenRentalPriceLengthIsNegativeAndIsRental()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenRentalPriceLengthIsNegativeAndIsRental()");
        var model = new ProductModel
        {
            Name = "Valid Product",
            IsRental = true,
            RentalPriceLength = -1
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.RentalPriceLength);
		_logger.LogDebug("<<< Exiting ShouldHaveErrorWhenRentalPriceLengthIsNegativeAndIsRental()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenRentalPriceLengthIsPositiveAndIsRental()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenRentalPriceLengthIsPositiveAndIsRental()");
        var model = new ProductModel
        {
            Name = "Valid Product",
            IsRental = true,
            RentalPriceLength = 7
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.RentalPriceLength);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenRentalPriceLengthIsPositiveAndIsRental()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenRentalPriceLengthIsZeroAndIsNotRental()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenRentalPriceLengthIsZeroAndIsNotRental()");
        var model = new ProductModel
        {
            Name = "Valid Product",
            IsRental = false,
            RentalPriceLength = 0
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.RentalPriceLength);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenRentalPriceLengthIsZeroAndIsNotRental()");
    }

    #endregion

    #region MinimumAgeToPurchase Validation Tests

    [Test]
    public void ShouldHaveErrorWhenMinimumAgeToPurchaseIsZeroAndAgeVerificationIsEnabled()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenMinimumAgeToPurchaseIsZeroAndAgeVerificationIsEnabled()");
        var model = new ProductModel
        {
            Name = "Valid Product",
            AgeVerification = true,
            MinimumAgeToPurchase = 0
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.MinimumAgeToPurchase);
		_logger.LogDebug("<<< Exiting ShouldHaveErrorWhenMinimumAgeToPurchaseIsZeroAndAgeVerificationIsEnabled()");
    }

    [Test]
    public void ShouldHaveErrorWhenMinimumAgeToPurchaseIsNegativeAndAgeVerificationIsEnabled()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenMinimumAgeToPurchaseIsNegativeAndAgeVerificationIsEnabled()");
        var model = new ProductModel
        {
            Name = "Valid Product",
            AgeVerification = true,
            MinimumAgeToPurchase = -1
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.MinimumAgeToPurchase);
		_logger.LogDebug("<<< Exiting ShouldHaveErrorWhenMinimumAgeToPurchaseIsNegativeAndAgeVerificationIsEnabled()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenMinimumAgeToPurchaseIsPositiveAndAgeVerificationIsEnabled()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenMinimumAgeToPurchaseIsPositiveAndAgeVerificationIsEnabled()");
        var model = new ProductModel
        {
            Name = "Valid Product",
            AgeVerification = true,
            MinimumAgeToPurchase = 18
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.MinimumAgeToPurchase);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenMinimumAgeToPurchaseIsPositiveAndAgeVerificationIsEnabled()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenMinimumAgeToPurchaseIsZeroAndAgeVerificationIsDisabled()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenMinimumAgeToPurchaseIsZeroAndAgeVerificationIsDisabled()");
        var model = new ProductModel
        {
            Name = "Valid Product",
            AgeVerification = false,
            MinimumAgeToPurchase = 0
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.MinimumAgeToPurchase);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenMinimumAgeToPurchaseIsZeroAndAgeVerificationIsDisabled()");
    }

    #endregion

    #region Combined Validation Tests

    [Test]
    public void ShouldHaveMultipleErrorsWhenMultipleFieldsAreInvalid()
    {
		_logger.LogDebug(">>> Entering ShouldHaveMultipleErrorsWhenMultipleFieldsAreInvalid()");
        var model = new ProductModel
        {
            Name = null, // Invalid
            SeName = new string('a', NopSeoDefaults.SearchEngineNameLength + 1), // Invalid
            IsRental = true,
            RentalPriceLength = 0, // Invalid
            AgeVerification = true,
            MinimumAgeToPurchase = 0 // Invalid
        };

        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Name);
		_logger.LogDebug("ShouldHaveMultipleErrorsWhenMultipleFieldsAreInvalid(): result → {Result}", result);
        result.ShouldHaveValidationErrorFor(x => x.SeName);
        result.ShouldHaveValidationErrorFor(x => x.RentalPriceLength);
        result.ShouldHaveValidationErrorFor(x => x.MinimumAgeToPurchase);
		_logger.LogDebug("<<< Exiting ShouldHaveMultipleErrorsWhenMultipleFieldsAreInvalid()");
    }

    [Test]
    public void ShouldNotHaveErrorsWhenAllFieldsAreValid()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorsWhenAllFieldsAreValid()");
        var model = new ProductModel
        {
            Name = "Premium Product",
            SeName = "premium-product",
            IsRental = true,
            RentalPriceLength = 30,
            AgeVerification = true,
            MinimumAgeToPurchase = 21
        };

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
		_logger.LogDebug("ShouldNotHaveErrorsWhenAllFieldsAreValid(): result → {Result}", result);
        result.ShouldNotHaveValidationErrorFor(x => x.SeName);
        result.ShouldNotHaveValidationErrorFor(x => x.RentalPriceLength);
        result.ShouldNotHaveValidationErrorFor(x => x.MinimumAgeToPurchase);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorsWhenAllFieldsAreValid()");
    }

    [Test]
    public void ShouldNotHaveErrorsWhenOptionalValidationsAreDisabled()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorsWhenOptionalValidationsAreDisabled()");
        var model = new ProductModel
        {
            Name = "Standard Product",
            SeName = "standard-product",
            IsRental = false, // Rental validation disabled
            RentalPriceLength = 0,
            AgeVerification = false, // Age verification disabled
            MinimumAgeToPurchase = 0
        };

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
		_logger.LogDebug("ShouldNotHaveErrorsWhenOptionalValidationsAreDisabled(): result → {Result}", result);
        result.ShouldNotHaveValidationErrorFor(x => x.SeName);
        result.ShouldNotHaveValidationErrorFor(x => x.RentalPriceLength);
        result.ShouldNotHaveValidationErrorFor(x => x.MinimumAgeToPurchase);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorsWhenOptionalValidationsAreDisabled()");
    }

    #endregion
}
