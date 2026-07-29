using FluentValidation.TestHelper;
using Nop.Core.Domain.Common;
using Nop.Services.Localization;
using Nop.Web.Models.Common;
using Nop.Web.Validators.Common;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Public.Validators.Common;

[TestFixture]
public class ContactUsValidatorTests : BaseNopTest
{
    private ContactUsValidator _validator;

    [OneTimeSetUp]
    public void Setup()
    {
		logger.LogDebug(">>> Entering Setup()");
        _validator = new ContactUsValidator(GetService<ILocalizationService>(), GetService<CommonSettings>());
		logger.LogDebug("<<< Exiting Setup()");
		logger.LogDebug("Setup(): _validator → {_validator}", _validator);
    }

    [Test]
    public void ShouldHaveErrorWhenEmailIsNullOrEmpty()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenEmailIsNullOrEmpty()");
        var model = new ContactUsModel
        {
            Email = null
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Email);
        model.Email = string.Empty;
		logger.LogDebug("ShouldHaveErrorWhenEmailIsNullOrEmpty(): model.Email → {Arg0}", model.Email);
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Email);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenEmailIsNullOrEmpty()");
    }

    [Test]
    public void ShouldHaveErrorWhenEmailIsWrongFormat()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenEmailIsWrongFormat()");
        var model = new ContactUsModel
        {
            Email = "adminexample.com"
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Email);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenEmailIsWrongFormat()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenEmailIsCorrectFormat()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenEmailIsCorrectFormat()");
        var model = new ContactUsModel
        {
            Email = "admin@example.com"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Email);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenEmailIsCorrectFormat()");
    }

    [Test]
    public void ShouldHaveErrorWhenFullnameIsNullOrEmpty()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenFullnameIsNullOrEmpty()");
        var model = new ContactUsModel
        {
            FullName = null
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.FullName);
        model.FullName = string.Empty;
		logger.LogDebug("ShouldHaveErrorWhenFullnameIsNullOrEmpty(): model.FullName → {Arg0}", model.FullName);
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.FullName);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenFullnameIsNullOrEmpty()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenFullnameIsSpecified()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenFullnameIsSpecified()");
        var model = new ContactUsModel
        {
            FullName = "John Smith"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.FullName);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenFullnameIsSpecified()");
    }

    [Test]
    public void ShouldHaveErrorWhenEnquiryIsNullOrEmpty()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenEnquiryIsNullOrEmpty()");
        var model = new ContactUsModel
        {
            Enquiry = null
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Enquiry);
        model.Enquiry = string.Empty;
		logger.LogDebug("ShouldHaveErrorWhenEnquiryIsNullOrEmpty(): model.Enquiry → {Arg0}", model.Enquiry);
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Enquiry);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenEnquiryIsNullOrEmpty()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenEnquiryIsSpecified()
    {
		logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenEnquiryIsSpecified()");
        var model = new ContactUsModel
        {
            Enquiry = "please call me back"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Enquiry);
		logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenEnquiryIsSpecified()");
    }

    [Test]
    public void ShouldHaveErrorWhenSubjectIsNullOrEmptyAnd()
    {
		logger.LogDebug(">>> Entering ShouldHaveErrorWhenSubjectIsNullOrEmptyAnd()");
        var settings = GetService<CommonSettings>();
        settings.SubjectFieldOnContactUsForm = true;
		logger.LogDebug("ShouldHaveErrorWhenSubjectIsNullOrEmptyAnd(): settings → {Settings}", settings);
		logger.LogDebug("ShouldHaveErrorWhenSubjectIsNullOrEmptyAnd(): settings.SubjectFieldOnContactUsForm → {Arg0}", settings.SubjectFieldOnContactUsForm);
        _validator = new ContactUsValidator(GetService<ILocalizationService>(), settings);

		logger.LogDebug("ShouldHaveErrorWhenSubjectIsNullOrEmptyAnd(): _validator → {_validator}", _validator);
        var model = new ContactUsModel
        {
            Subject = string.Empty
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Subject);
        model.Subject = null;
		logger.LogDebug("ShouldHaveErrorWhenSubjectIsNullOrEmptyAnd(): model.Subject → {Arg0}", model.Subject);
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Subject);
        model.Subject = "Test subject";
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Subject);
		logger.LogDebug("<<< Exiting ShouldHaveErrorWhenSubjectIsNullOrEmptyAnd()");
    }
}