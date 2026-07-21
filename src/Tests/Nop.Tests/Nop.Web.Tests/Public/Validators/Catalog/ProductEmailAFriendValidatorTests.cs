using FluentValidation.TestHelper;
using Nop.Services.Localization;
using Nop.Web.Models.Catalog;
using Nop.Web.Validators.Catalog;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Public.Validators.Catalog;

[TestFixture]
public class ProductEmailAFriendValidatorTests : BaseNopTest
{
    private ProductEmailAFriendValidator _validator;

    [OneTimeSetUp]
    public void Setup()
    {
		logger.LogDebug(">>> Entering Setup()");
        _validator = new ProductEmailAFriendValidator(GetService<ILocalizationService>());
		logger.LogDebug("<<< Exiting Setup()");
		logger.LogDebug("Setup(): _validator → {_validator}", _validator);
    }

    [Test]
    public void ShouldHaveErrorWhenFriendEmailIsNullOrEmpty()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenFriendEmailIsNullOrEmpty()");
        var model = new ProductEmailAFriendModel
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
        var model = new ProductEmailAFriendModel
        {
            FriendEmail = "adminexample.com"
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.FriendEmail);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenFriendEmailIsWrongFormat()");
    }

    [Test]
    public void PublicVoidShouldNotHaveErrorWhenFriendEmailIsCorrectFormat()
    {
		logger.LogDebug(">>> Entering PublicVoidShouldNotHaveErrorWhenFriendEmailIsCorrectFormat()");
        var model = new ProductEmailAFriendModel
        {
            FriendEmail = "admin@example.com"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.FriendEmail);
		logger.LogDebug("<<< Exiting PublicVoidShouldNotHaveErrorWhenFriendEmailIsCorrectFormat()");
    }

    [Test]
    public void ShouldHaveErrorWhenYourEmailAddressIsNullOrEmpty()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenYourEmailAddressIsNullOrEmpty()");
        var model = new ProductEmailAFriendModel
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
        var model = new ProductEmailAFriendModel
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
        var model = new ProductEmailAFriendModel
        {
            YourEmailAddress = "admin@example.com"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.YourEmailAddress);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenYourEmailAddressIsCorrectFormat()");
    }
}
