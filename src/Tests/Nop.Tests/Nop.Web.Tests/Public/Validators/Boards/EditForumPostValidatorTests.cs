using FluentValidation.TestHelper;
using Nop.Services.Localization;
using Nop.Web.Models.Boards;
using Nop.Web.Validators.Boards;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Public.Validators.Boards;

[TestFixture]
public class EditForumPostValidatorTests : BaseNopTest
{
    private EditForumPostValidator _validator;

    [OneTimeSetUp]
    public void Setup()
    {
		logger.LogDebug(">>> Entering Setup()");
        _validator = new EditForumPostValidator(GetService<ILocalizationService>());
		logger.LogDebug("<<< Exiting Setup()");
		logger.LogDebug("Setup(): _validator → {_validator}", _validator);
    }

    [Test]
    public void ShouldHaveErrorWhenTextIsNullOrEmpty()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenTextIsNullOrEmpty()");
        var model = new EditForumPostModel
        {
            Text = null
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Text);
        model.Text = string.Empty;
		logger.LogDebug("ShouldHaveErrorWhenTextIsNullOrEmpty(): model.Text → {Arg0}", model.Text);
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Text);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenTextIsNullOrEmpty()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenTextIsSpecified()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenTextIsSpecified()");
        var model = new EditForumPostModel
        {
            Text = "some comment"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Text);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenTextIsSpecified()");
    }
}
