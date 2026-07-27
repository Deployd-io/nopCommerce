using FluentValidation.TestHelper;
using Nop.Services.Localization;
using Nop.Web.Models.Boards;
using Nop.Web.Validators.Boards;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Public.Validators.Boards;

[TestFixture]
public class EditForumTopicValidatorTests : BaseNopTest
{
    private EditForumTopicValidator _validator;

    [OneTimeSetUp]
    public void Setup()
    {
		logger.LogDebug(">>> Entering Setup()");
        _validator = new EditForumTopicValidator(GetService<ILocalizationService>());
		logger.LogDebug("<<< Exiting Setup()");
		logger.LogDebug("Setup(): _validator → {_validator}", _validator);
    }

    [Test]
    public void ShouldHaveErrorWhenSubjectIsNullOrEmpty()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenSubjectIsNullOrEmpty()");
        var model = new EditForumTopicModel
        {
            Subject = null
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Subject);
        model.Subject = string.Empty;
		logger.LogDebug("ShouldHaveErrorWhenSubjectIsNullOrEmpty(): model.Subject → {Arg0}", model.Subject);
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Subject);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenSubjectIsNullOrEmpty()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenSubjectIsSpecified()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenSubjectIsSpecified()");
        var model = new EditForumTopicModel
        {
            Subject = "some comment"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Subject);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenSubjectIsSpecified()");
    }

    [Test]
    public void ShouldHaveErrorWhenTextIsNullOrEmpty()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenTextIsNullOrEmpty()");
        var model = new EditForumTopicModel
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
        var model = new EditForumTopicModel
        {
            Text = "some comment"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Text);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenTextIsSpecified()");
    }
}