using FluentValidation.TestHelper;
using Nop.Services.Localization;
using Nop.Web.Models.PrivateMessages;
using Nop.Web.Validators.PrivateMessages;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Web.Tests.Public.Validators.PrivateMessages;

[TestFixture]
public class SendPrivateMessageValidatorTests : BaseNopTest
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    private SendPrivateMessageValidator _validator;

    [OneTimeSetUp]
    public void Setup()
    {
		_logger.LogDebug(">>> Entering Setup()");
        _validator = new SendPrivateMessageValidator(GetService<ILocalizationService>());
		_logger.LogDebug("<<< Exiting Setup()");
		_logger.LogDebug("Setup(): _validator → {_validator}", _validator);
    }

    [Test]
    public void ShouldHaveErrorWhenSubjectIsNullOrEmpty()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenSubjectIsNullOrEmpty()");
        var model = new SendPrivateMessageModel
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
        var model = new SendPrivateMessageModel
        {
            Subject = "some comment"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Subject);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenSubjectIsSpecified()");
    }

    [Test]
    public void ShouldHaveErrorWhenMessageIsNullOrEmpty()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenMessageIsNullOrEmpty()");
        var model = new SendPrivateMessageModel
        {
            Message = null
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Message);
        model.Message = string.Empty;
		_logger.LogDebug("ShouldHaveErrorWhenMessageIsNullOrEmpty(): model.Message → {Arg0}", model.Message);
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Message);
		_logger.LogDebug("<<< Exiting ShouldHaveErrorWhenMessageIsNullOrEmpty()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenMessageIsSpecified()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenMessageIsSpecified()");
        var model = new SendPrivateMessageModel
        {
            Message = "some comment"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Message);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenMessageIsSpecified()");
    }
}