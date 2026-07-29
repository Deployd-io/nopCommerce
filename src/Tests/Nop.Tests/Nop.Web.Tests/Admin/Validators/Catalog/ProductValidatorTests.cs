using FluentValidation.TestHelper;
using Nop.Services.Localization;
using Nop.Services.Seo;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Areas.Admin.Validators.Catalog;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Admin.Validators.Catalog;

[TestFixture]
public class ProductValidatorTests : BaseNopTest
{
    private ProductValidator _validator;

    [OneTimeSetUp]
    public void Setup()
    {
		logger.LogDebug(">>> Entering Setup()");
        _validator = new ProductValidator(GetService<ILocalizationService>());
		logger.LogDebug("<<< Exiting Setup()");
		logger.LogDebug("Setup(): _validator → {_validator}", _validator);
    }

    #region Name Validation Tests

    [Test]
    public void ShouldHaveErrorWhenNameIsNull()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenNameIsNull()");
        var model = new ProductModel
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
        var model = new ProductModel
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
        var model = new ProductModel
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
        var model = new ProductModel
        {
            Name = "Sample Product"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Name);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenNameIsSpecified()");
    }

    #endregion

    #region SeName Validation Tests

    [Test]
    public void ShouldNotHaveErrorWhenSeNameIsNull()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenSeNameIsNull()");
        var model = new ProductModel
        {
            Name = "Valid Product",
            SeName = null
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.SeName);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenSeNameIsNull()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenSeNameIsEmpty()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenSeNameIsEmpty()");
        var model = new ProductModel
        {
            Name = "Valid Product",
            SeName = string.Empty
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.SeName);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenSeNameIsEmpty()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenSeNameIsWithinMaxLength()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenSeNameIsWithinMaxLength()");
        var model = new ProductModel
        {
            Name = "Valid Product",
            SeName = new string('a', NopSeoDefaults.SearchEngineNameLength) // 200 characters
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.SeName);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenSeNameIsWithinMaxLength()");
    }

    [Test]
    public void ShouldHaveErrorWhenSeNameExceedsMaxLength()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenSeNameExceedsMaxLength()");
        var model = new ProductModel
        {
            Name = "Valid Product",
            SeName = new string('a', NopSeoDefaults.SearchEngineNameLength + 1) // 201 characters
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.SeName);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenSeNameExceedsMaxLength()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenSeNameIsValidLength()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenSeNameIsValidLength()");
        var model = new ProductModel
        {
            Name = "Valid Product",
            SeName = "sample-product-sename"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.SeName);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenSeNameIsValidLength()");
    }

    #endregion

    #region RentalPriceLength Validation Tests

    [Test]
    public void ShouldHaveErrorWhenRentalPriceLengthIsZeroAndIsRental()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenRentalPriceLengthIsZeroAndIsRental()");
        var model = new ProductModel
        {
            Name = "Valid Product",
            IsRental = true,
            RentalPriceLength = 0
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.RentalPriceLength);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenRentalPriceLengthIsZeroAndIsRental()");
    }

    [Test]
    public void ShouldHaveErrorWhenRentalPriceLengthIsNegativeAndIsRental()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenRentalPriceLengthIsNegativeAndIsRental()");
        var model = new ProductModel
        {
            Name = "Valid Product",
            IsRental = true,
            RentalPriceLength = -1
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.RentalPriceLength);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenRentalPriceLengthIsNegativeAndIsRental()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenRentalPriceLengthIsPositiveAndIsRental()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenRentalPriceLengthIsPositiveAndIsRental()");
        var model = new ProductModel
        {
            Name = "Valid Product",
            IsRental = true,
            RentalPriceLength = 7
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.RentalPriceLength);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenRentalPriceLengthIsPositiveAndIsRental()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenRentalPriceLengthIsZeroAndIsNotRental()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenRentalPriceLengthIsZeroAndIsNotRental()");
        var model = new ProductModel
        {
            Name = "Valid Product",
            IsRental = false,
            RentalPriceLength = 0
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.RentalPriceLength);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenRentalPriceLengthIsZeroAndIsNotRental()");
    }

    #endregion

    #region MinimumAgeToPurchase Validation Tests

    [Test]
    public void ShouldHaveErrorWhenMinimumAgeToPurchaseIsZeroAndAgeVerificationIsEnabled()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenMinimumAgeToPurchaseIsZeroAndAgeVerificationIsEnabled()");
        var model = new ProductModel
        {
            Name = "Valid Product",
            AgeVerification = true,
            MinimumAgeToPurchase = 0
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.MinimumAgeToPurchase);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenMinimumAgeToPurchaseIsZeroAndAgeVerificationIsEnabled()");
    }

    [Test]
    public void ShouldHaveErrorWhenMinimumAgeToPurchaseIsNegativeAndAgeVerificationIsEnabled()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenMinimumAgeToPurchaseIsNegativeAndAgeVerificationIsEnabled()");
        var model = new ProductModel
        {
            Name = "Valid Product",
            AgeVerification = true,
            MinimumAgeToPurchase = -1
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.MinimumAgeToPurchase);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenMinimumAgeToPurchaseIsNegativeAndAgeVerificationIsEnabled()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenMinimumAgeToPurchaseIsPositiveAndAgeVerificationIsEnabled()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenMinimumAgeToPurchaseIsPositiveAndAgeVerificationIsEnabled()");
        var model = new ProductModel
        {
            Name = "Valid Product",
            AgeVerification = true,
            MinimumAgeToPurchase = 18
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.MinimumAgeToPurchase);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenMinimumAgeToPurchaseIsPositiveAndAgeVerificationIsEnabled()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenMinimumAgeToPurchaseIsZeroAndAgeVerificationIsDisabled()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenMinimumAgeToPurchaseIsZeroAndAgeVerificationIsDisabled()");
        var model = new ProductModel
        {
            Name = "Valid Product",
            AgeVerification = false,
            MinimumAgeToPurchase = 0
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.MinimumAgeToPurchase);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenMinimumAgeToPurchaseIsZeroAndAgeVerificationIsDisabled()");
    }

    #endregion

    #region Combined Validation Tests

    [Test]
    public void ShouldHaveMultipleErrorsWhenMultipleFieldsAreInvalid()
    {
		logger.LogDebug(">>> Entering ShouldHaveMultipleErrorsWhenMultipleFieldsAreInvalid()");
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
		logger.LogDebug("ShouldHaveMultipleErrorsWhenMultipleFieldsAreInvalid(): result → {Result}", result);
        result.ShouldHaveValidationErrorFor(x => x.SeName);
        result.ShouldHaveValidationErrorFor(x => x.RentalPriceLength);
        result.ShouldHaveValidationErrorFor(x => x.MinimumAgeToPurchase);
		logger.LogDebug("<<< Exiting ShouldHaveMultipleErrorsWhenMultipleFieldsAreInvalid()");
    }

    [Test]
    public void ShouldNotHaveErrorsWhenAllFieldsAreValid()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorsWhenAllFieldsAreValid()");
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
		logger.LogDebug("ShouldNotHaveErrorsWhenAllFieldsAreValid(): result → {Result}", result);
        result.ShouldNotHaveValidationErrorFor(x => x.SeName);
        result.ShouldNotHaveValidationErrorFor(x => x.RentalPriceLength);
        result.ShouldNotHaveValidationErrorFor(x => x.MinimumAgeToPurchase);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorsWhenAllFieldsAreValid()");
    }

    [Test]
    public void ShouldNotHaveErrorsWhenOptionalValidationsAreDisabled()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorsWhenOptionalValidationsAreDisabled()");
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
		logger.LogDebug("ShouldNotHaveErrorsWhenOptionalValidationsAreDisabled(): result → {Result}", result);
        result.ShouldNotHaveValidationErrorFor(x => x.SeName);
        result.ShouldNotHaveValidationErrorFor(x => x.RentalPriceLength);
        result.ShouldNotHaveValidationErrorFor(x => x.MinimumAgeToPurchase);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorsWhenOptionalValidationsAreDisabled()");
    }

    #endregion
}
