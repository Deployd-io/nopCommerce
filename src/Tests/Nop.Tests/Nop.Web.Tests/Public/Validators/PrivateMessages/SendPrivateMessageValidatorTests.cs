using FluentValidation.TestHelper;
using Nop.Services.Localization;
using Nop.Web.Models.PrivateMessages;
using Nop.Web.Validators.PrivateMessages;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Public.Validators.PrivateMessages;

[TestFixture]
public class SendPrivateMessageValidatorTests : BaseNopTest
{
    private SendPrivateMessageValidator _validator;

    [OneTimeSetUp]
    public void Setup()
    {
		logger.LogDebug(">>> Entering Setup()");
        _validator = new SendPrivateMessageValidator(GetService<ILocalizationService>());
		logger.LogDebug("<<< Exiting Setup()");
		logger.LogDebug("Setup(): _validator → {_validator}", _validator);
    }

    [Test]
    public void ShouldHaveErrorWhenSubjectIsNullOrEmpty()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenSubjectIsNullOrEmpty()");
        var model = new SendPrivateMessageModel
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
        var model = new SendPrivateMessageModel
        {
            Subject = "some comment"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Subject);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenSubjectIsSpecified()");
    }

    [Test]
    public void ShouldHaveErrorWhenMessageIsNullOrEmpty()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenMessageIsNullOrEmpty()");
        var model = new SendPrivateMessageModel
        {
            Message = null
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Message);
        model.Message = string.Empty;
		logger.LogDebug("ShouldHaveErrorWhenMessageIsNullOrEmpty(): model.Message → {Arg0}", model.Message);
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Message);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenMessageIsNullOrEmpty()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenMessageIsSpecified()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenMessageIsSpecified()");
        var model = new SendPrivateMessageModel
        {
            Message = "some comment"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Message);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenMessageIsSpecified()");
    }
}
