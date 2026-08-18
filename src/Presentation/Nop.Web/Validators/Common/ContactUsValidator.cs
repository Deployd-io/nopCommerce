using FluentValidation;
using Nop.Core.Domain.Common;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Nop.Web.Models.Common;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Validators.Common;

public partial class ContactUsValidator : BaseNopValidator<ContactUsModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public ContactUsValidator(ILocalizationService localizationService, CommonSettings commonSettings)
    {
		_logger.LogDebug(">>> Entering ContactUsValidator(localizationService={LocalizationService},commonSettings={CommonSettings})", localizationService, commonSettings);
        RuleFor(x => x.Email).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("ContactUs.Email.Required"));
        RuleFor(x => x.Email)
            .IsEmailAddress()
            .WithMessageAwait(localizationService.GetResourceAsync("Common.WrongEmail"));
        RuleFor(x => x.FullName).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("ContactUs.FullName.Required"));
        if (commonSettings.SubjectFieldOnContactUsForm)
        {
            RuleFor(x => x.Subject).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("ContactUs.Subject.Required"));
        }
        RuleFor(x => x.Enquiry).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("ContactUs.Enquiry.Required"));
		_logger.LogDebug("<<< Exiting ContactUsValidator(localizationService={LocalizationService},commonSettings={CommonSettings})", localizationService, commonSettings);
    }
}