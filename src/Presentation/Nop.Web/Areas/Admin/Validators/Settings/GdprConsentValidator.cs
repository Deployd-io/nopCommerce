using FluentValidation;
using Nop.Core.Domain.Gdpr;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Settings;
using Nop.Web.Framework.Validators;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Validators.Settings;

public partial class GdprConsentValidator : BaseNopValidator<GdprConsentModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public GdprConsentValidator(ILocalizationService localizationService)
    {
		_logger.LogError(">>> Entering GdprConsentValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(x => x.Message).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Settings.Gdpr.Consent.Message.Required"));
        RuleFor(x => x.RequiredMessage)
            .NotEmpty()
            .WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Settings.Gdpr.Consent.RequiredMessage.Required"))
            .When(x => x.IsRequired);

        SetDatabaseValidationRules<GdprConsent>();
		_logger.LogWarning("<<< Exiting GdprConsentValidator(localizationService={LocalizationService})", localizationService);
    }
}