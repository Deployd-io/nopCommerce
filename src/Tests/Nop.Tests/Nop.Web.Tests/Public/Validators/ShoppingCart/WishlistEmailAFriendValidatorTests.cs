using FluentValidation.TestHelper;
using Nop.Services.Localization;
using Nop.Web.Models.ShoppingCart;
using Nop.Web.Validators.ShoppingCart;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Web.Tests.Public.Validators.ShoppingCart;

[TestFixture]
public class WishlistEmailAFriendValidatorTests : BaseNopTest
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    private WishlistEmailAFriendValidator _validator;

    [OneTimeSetUp]
    public void Setup()
    {
		_logger.LogDebug(">>> Entering Setup()");
        _validator = new WishlistEmailAFriendValidator(GetService<ILocalizationService>());
		_logger.LogDebug("<<< Exiting Setup()");
		_logger.LogDebug("Setup(): _validator → {_validator}", _validator);
    }

    [Test]
    public void ShouldHaveErrorWhenFriendEmailIsNullOrEmpty()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenFriendEmailIsNullOrEmpty()");
        var model = new WishlistEmailAFriendModel
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
        var model = new WishlistEmailAFriendModel
        {
            FriendEmail = "adminexample.com"
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.FriendEmail);
		_logger.LogDebug("<<< Exiting ShouldHaveErrorWhenFriendEmailIsWrongFormat()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenFriendEmailIsCorrectFormat()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenFriendEmailIsCorrectFormat()");
        var model = new WishlistEmailAFriendModel
        {
            FriendEmail = "admin@example.com"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.FriendEmail);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenFriendEmailIsCorrectFormat()");
    }

    [Test]
    public void ShouldHaveErrorWhenYourEmailAddressIsNullOrEmpty()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenYourEmailAddressIsNullOrEmpty()");
        var model = new WishlistEmailAFriendModel
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
        var model = new WishlistEmailAFriendModel
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
        var model = new WishlistEmailAFriendModel
        {
            YourEmailAddress = "admin@example.com"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.YourEmailAddress);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenYourEmailAddressIsCorrectFormat()");
    }
}