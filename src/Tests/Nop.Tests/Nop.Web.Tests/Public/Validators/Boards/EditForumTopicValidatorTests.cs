using FluentValidation.TestHelper;
using Nop.Services.Localization;
using Nop.Web.Models.Boards;
using Nop.Web.Validators.Boards;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Web.Tests.Public.Validators.Boards;

[TestFixture]
public class EditForumTopicValidatorTests : BaseNopTest
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    private EditForumTopicValidator _validator;

    [OneTimeSetUp]
    public void Setup()
    {
		_logger.LogDebug(">>> Entering Setup()");
        _validator = new EditForumTopicValidator(GetService<ILocalizationService>());
		_logger.LogDebug("Setup(): _validator → {_validator}", _validator);
		_logger.LogDebug("<<< Exiting Setup()");
    }

    [Test]
    public void ShouldHaveErrorWhenSubjectIsNullOrEmpty()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenSubjectIsNullOrEmpty()");
        var model = new EditForumTopicModel
        {
            Subject = null
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Subject);
        model.Subject = string.Empty;
		_logger.LogDebug("ShouldHaveErrorWhenSubjectIsNullOrEmpty(): model.Subject → {Arg0}", model.Subject);
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Subject);
		_logger.LogDebug("<<< Exiting ShouldHaveErrorWhenSubjectIsNullOrEmpty()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenSubjectIsSpecified()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenSubjectIsSpecified()");
        var model = new EditForumTopicModel
        {
            Subject = "some comment"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Subject);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenSubjectIsSpecified()");
    }

    [Test]
    public void ShouldHaveErrorWhenTextIsNullOrEmpty()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenTextIsNullOrEmpty()");
        var model = new EditForumTopicModel
        {
            Text = null
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Text);
        model.Text = string.Empty;
		_logger.LogDebug("ShouldHaveErrorWhenTextIsNullOrEmpty(): model.Text → {Arg0}", model.Text);
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Text);
		_logger.LogDebug("<<< Exiting ShouldHaveErrorWhenTextIsNullOrEmpty()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenTextIsSpecified()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenTextIsSpecified()");
        var model = new EditForumTopicModel
        {
            Text = "some comment"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Text);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenTextIsSpecified()");
    }
}