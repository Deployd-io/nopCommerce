using FluentValidation.TestHelper;
using Nop.Services.Localization;
using Nop.Web.Models.Boards;
using Nop.Web.Validators.Boards;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Web.Tests.Public.Validators.Boards;

[TestFixture]
public class EditForumPostValidatorTests : BaseNopTest
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    private EditForumPostValidator _validator;

    [OneTimeSetUp]
    public void Setup()
    {
		_logger.LogDebug(">>> Entering Setup()");
        _validator = new EditForumPostValidator(GetService<ILocalizationService>());
		_logger.LogDebug("<<< Exiting Setup()");
		_logger.LogDebug("Setup(): _validator → {_validator}", _validator);
    }

    [Test]
    public void ShouldHaveErrorWhenTextIsNullOrEmpty()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenTextIsNullOrEmpty()");
        var model = new EditForumPostModel
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
        var model = new EditForumPostModel
        {
            Text = "some comment"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Text);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenTextIsSpecified()");
    }
}