using FluentValidation;
using Nop.Core.Domain.Directory;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Directory;
using Nop.Web.Framework.Validators;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Validators.Directory;

public partial class CountryValidator : BaseNopValidator<CountryModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public CountryValidator(ILocalizationService localizationService)
    {
		_logger.LogDebug(">>> Entering CountryValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Countries.Fields.Name.Required"));
        RuleFor(p => p.Name).Length(1, 100);

        RuleFor(x => x.TwoLetterIsoCode)
            .NotEmpty()
            .WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Countries.Fields.TwoLetterIsoCode.Required"));
        RuleFor(x => x.TwoLetterIsoCode)
            .Length(2)
            .WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Countries.Fields.TwoLetterIsoCode.Length"));

        RuleFor(x => x.ThreeLetterIsoCode)
            .NotEmpty()
            .WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Countries.Fields.ThreeLetterIsoCode.Required"));
        RuleFor(x => x.ThreeLetterIsoCode)
            .Length(3)
            .WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Countries.Fields.ThreeLetterIsoCode.Length"));

        SetDatabaseValidationRules<Country>();
		_logger.LogDebug("<<< Exiting CountryValidator(localizationService={LocalizationService})", localizationService);
    }
}