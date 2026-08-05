using FluentValidation.TestHelper;
using Nop.Core.Domain.Common;
using Nop.Services.Localization;
using Nop.Web.Models.Common;
using Nop.Web.Validators.Common;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Web.Tests.Public.Validators.Common;

[TestFixture]
public class ContactVendorValidatorTests : BaseNopTest
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    private ContactVendorValidator _validator;

    [OneTimeSetUp]
    public void Setup()
    {
		_logger.LogDebug(">>> Entering Setup()");
        _validator = new ContactVendorValidator(GetService<ILocalizationService>(), GetService<CommonSettings>());
		_logger.LogDebug("<<< Exiting Setup()");
		_logger.LogDebug("Setup(): _validator → {_validator}", _validator);
    }

    [Test]
    public void ShouldHaveErrorWhenEmailIsNullOrEmpty()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenEmailIsNullOrEmpty()");
        var model = new ContactVendorModel
        {
            Email = null
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Email);
        model.Email = string.Empty;
		_logger.LogDebug("ShouldHaveErrorWhenEmailIsNullOrEmpty(): model.Email → {Arg0}", model.Email);
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Email);
		_logger.LogDebug("<<< Exiting ShouldHaveErrorWhenEmailIsNullOrEmpty()");
    }

    [Test]
    public void ShouldHaveErrorWhenEmailIsWrongFormat()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenEmailIsWrongFormat()");
        var model = new ContactVendorModel
        {
            Email = "adminexample.com"
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Email);
		_logger.LogDebug("<<< Exiting ShouldHaveErrorWhenEmailIsWrongFormat()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenEmailIsCorrectFormat()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenEmailIsCorrectFormat()");
        var model = new ContactVendorModel
        {
            Email = "admin@example.com"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Email);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenEmailIsCorrectFormat()");
    }

    [Test]
    public void ShouldHaveErrorWhenFullnameIsNullOrEmpty()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenFullnameIsNullOrEmpty()");
        var model = new ContactVendorModel
        {
            FullName = null
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.FullName);
        model.FullName = string.Empty;
		_logger.LogDebug("ShouldHaveErrorWhenFullnameIsNullOrEmpty(): model.FullName → {Arg0}", model.FullName);
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.FullName);
		_logger.LogDebug("<<< Exiting ShouldHaveErrorWhenFullnameIsNullOrEmpty()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenFullnameIsSpecified()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenFullnameIsSpecified()");
        var model = new ContactVendorModel
        {
            FullName = "John Smith"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.FullName);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenFullnameIsSpecified()");
    }

    [Test]
    public void ShouldHaveErrorWhenEnquiryIsNullOrEmpty()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenEnquiryIsNullOrEmpty()");
        var model = new ContactVendorModel
        {
            Enquiry = null
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Enquiry);
        model.Enquiry = string.Empty;
		_logger.LogDebug("ShouldHaveErrorWhenEnquiryIsNullOrEmpty(): model.Enquiry → {Arg0}", model.Enquiry);
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Enquiry);
		_logger.LogDebug("<<< Exiting ShouldHaveErrorWhenEnquiryIsNullOrEmpty()");
    }

    [Test]
    public void ShouldNotHaveErrorWhenEnquiryIsSpecified()
    {
		_logger.LogDebug(">>> Entering ShouldNotHaveErrorWhenEnquiryIsSpecified()");
        var model = new ContactVendorModel
        {
            Enquiry = "please call me back"
        };
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Enquiry);
		_logger.LogDebug("<<< Exiting ShouldNotHaveErrorWhenEnquiryIsSpecified()");
    }

    [Test]
    public void ShouldHaveErrorWhenSubjectIsNullOrEmptyAnd()
    {
		_logger.LogDebug(">>> Entering ShouldHaveErrorWhenSubjectIsNullOrEmptyAnd()");
        var settings = GetService<CommonSettings>();
        settings.SubjectFieldOnContactUsForm = true;
		_logger.LogDebug("ShouldHaveErrorWhenSubjectIsNullOrEmptyAnd(): settings → {Settings}", settings);
		_logger.LogDebug("ShouldHaveErrorWhenSubjectIsNullOrEmptyAnd(): settings.SubjectFieldOnContactUsForm → {Arg0}", settings.SubjectFieldOnContactUsForm);
        _validator = new ContactVendorValidator(GetService<ILocalizationService>(), settings);

		_logger.LogDebug("ShouldHaveErrorWhenSubjectIsNullOrEmptyAnd(): _validator → {_validator}", _validator);
        var model = new ContactVendorModel
        {
            Subject = string.Empty
        };
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Subject);
        model.Subject = null;
		_logger.LogDebug("ShouldHaveErrorWhenSubjectIsNullOrEmptyAnd(): model.Subject → {Arg0}", model.Subject);
        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Subject);
        model.Subject = "Test subject";
        _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Subject);
		_logger.LogDebug("<<< Exiting ShouldHaveErrorWhenSubjectIsNullOrEmptyAnd()");
    }
}