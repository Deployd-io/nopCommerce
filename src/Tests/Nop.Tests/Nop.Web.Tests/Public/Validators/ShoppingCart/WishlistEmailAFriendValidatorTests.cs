using FluentValidation.TestHelper;
using Nop.Services.Localization;
using Nop.Web.Models.ShoppingCart;
using Nop.Web.Validators.ShoppingCart;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Public.Validators.ShoppingCart;

[TestFixture]
public class WishlistEmailAFriendValidatorTests : BaseNopTest
{
    private WishlistEmailAFriendValidator _validator;

    [OneTimeSetUp]
    public void Setup()
    {
		logger.LogDebug(">>> Entering Setup()");
        _validator = new WishlistEmailAFriendValidator(GetService<ILocalizationService>());
		logger.LogDebug("<<< Exiting Setup()");
		logger.LogDebug("Setup(): _validator → {_validator}", _validator);
    }

    [Test]
    public void ShouldHaveErrorWhenFriendEmailIsNullOrEmpty()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenFriendEmailIsNullOrEmpty()");
        var model = new WishlistEmailAFriendModel
        {
            FriendEmail = null
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.FriendEmail);
        model.FriendEmail = string.Empty;
		logger.LogDebug("ShouldHaveErrorWhenFriendEmailIsNullOrEmpty(): model.FriendEmail → {Arg0}", model.FriendEmail);
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.FriendEmail);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenFriendEmailIsNullOrEmpty()");
    }

    [Test]
    public void ShouldHaveErrorWhenFriendEmailIsWrongFormat()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenFriendEmailIsWrongFormat()");
        var model = new WishlistEmailAFriendModel
        {
            FriendEmail = "adminexample.com"
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.FriendEmail);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenFriendEmailIsWrongFormat()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenFriendEmailIsCorrectFormat()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenFriendEmailIsCorrectFormat()");
        var model = new WishlistEmailAFriendModel
        {
            FriendEmail = "admin@example.com"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.FriendEmail);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenFriendEmailIsCorrectFormat()");
    }

    [Test]
    public void ShouldHaveErrorWhenYourEmailAddressIsNullOrEmpty()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenYourEmailAddressIsNullOrEmpty()");
        var model = new WishlistEmailAFriendModel
        {
            YourEmailAddress = null
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.YourEmailAddress);
        model.YourEmailAddress = string.Empty;
		logger.LogDebug("ShouldHaveErrorWhenYourEmailAddressIsNullOrEmpty(): model.YourEmailAddress → {Arg0}", model.YourEmailAddress);
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.YourEmailAddress);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenYourEmailAddressIsNullOrEmpty()");
    }

    [Test]
    public void ShouldHaveErrorWhenYourEmailAddressIsWrongFormat()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenYourEmailAddressIsWrongFormat()");
        var model = new WishlistEmailAFriendModel
        {
            YourEmailAddress = "adminexample.com"
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.YourEmailAddress);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenYourEmailAddressIsWrongFormat()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenYourEmailAddressIsCorrectFormat()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenYourEmailAddressIsCorrectFormat()");
        var model = new WishlistEmailAFriendModel
        {
            YourEmailAddress = "admin@example.com"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.YourEmailAddress);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenYourEmailAddressIsCorrectFormat()");
    }
}
