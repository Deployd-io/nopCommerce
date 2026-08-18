using FluentValidation.TestHelper;
using Nop.Services.Localization;
using Nop.Web.Models.Catalog;
using Nop.Web.Validators.Catalog;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Web.Tests.Public.Validators.Catalog;

[TestFixture]
public class ProductEmailAFriendValidatorTests : BaseNopTest
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    private ProductEmailAFriendValidator _validator;

    [OneTimeSetUp]
    public void Setup()
    {
		_logger.LogDebug(">>> Entering Setup()");
        _validator = new ProductEmailAFriendValidator(GetService<ILocalizationService>());
		_logger.LogDebug("Setup(): _validator → {_validator}", _validator);
		_logger.LogDebug("<<< Exiting Setup()");
    }

    [Test]
    public void ShouldHaveErrorWhenFriendEmailIsNullOrEmpty()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenFriendEmailIsNullOrEmpty()");
        var model = new ProductEmailAFriendModel
        {
            FriendEmail = null
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.FriendEmail);
        model.FriendEmail = string.Empty;
		_logger.LogDebug("ShouldHaveErrorWhenFriendEmailIsNullOrEmpty(): model.FriendEmail → {Arg0}", model.FriendEmail);
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.FriendEmail);
		_logger.LogDebug("<<< Exiting ShouldHaveErrorWhenFriendEmailIsNullOrEmpty()");
    }

    [Test]
    public void ShouldHaveErrorWhenFriendEmailIsWrongFormat()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenFriendEmailIsWrongFormat()");
        var model = new ProductEmailAFriendModel
        {
            FriendEmail = "adminexample.com"
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.FriendEmail);
		_logger.LogDebug("<<< Exiting ShouldHaveErrorWhenFriendEmailIsWrongFormat()");
    }

    [Test]
    public void PublicVoidShouldNotHaveErrorWhenFriendEmailIsCorrectFormat()
    {
		_logger.LogDebug(">>> Entering PublicVoidShouldNotHaveErrorWhenFriendEmailIsCorrectFormat()");
        var model = new ProductEmailAFriendModel
        {
            FriendEmail = "admin@example.com"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.FriendEmail);
		_logger.LogDebug("<<< Exiting PublicVoidShouldNotHaveErrorWhenFriendEmailIsCorrectFormat()");
    }

    [Test]
    public void ShouldHaveErrorWhenYourEmailAddressIsNullOrEmpty()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenYourEmailAddressIsNullOrEmpty()");
        var model = new ProductEmailAFriendModel
        {
            YourEmailAddress = null
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.YourEmailAddress);
        model.YourEmailAddress = string.Empty;
		_logger.LogDebug("ShouldHaveErrorWhenYourEmailAddressIsNullOrEmpty(): model.YourEmailAddress → {Arg0}", model.YourEmailAddress);
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.YourEmailAddress);
		_logger.LogDebug("<<< Exiting ShouldHaveErrorWhenYourEmailAddressIsNullOrEmpty()");
    }

    [Test]
    public void ShouldHaveErrorWhenYourEmailAddressIsWrongFormat()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenYourEmailAddressIsWrongFormat()");
        var model = new ProductEmailAFriendModel
        {
            YourEmailAddress = "adminexample.com"
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.YourEmailAddress);
		_logger.LogDebug("<<< Exiting ShouldHaveErrorWhenYourEmailAddressIsWrongFormat()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenYourEmailAddressIsCorrectFormat()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenYourEmailAddressIsCorrectFormat()");
        var model = new ProductEmailAFriendModel
        {
            YourEmailAddress = "admin@example.com"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.YourEmailAddress);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenYourEmailAddressIsCorrectFormat()");
    }
}