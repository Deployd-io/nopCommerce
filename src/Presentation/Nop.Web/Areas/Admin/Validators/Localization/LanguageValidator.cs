using System.Globalization;
using FluentValidation;
using Nop.Core.Domain.Localization;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Localization;
using Nop.Web.Framework.Validators;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Validators.Localization;

public partial class LanguageValidator : BaseNopValidator<LanguageModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public LanguageValidator(ILocalizationService localizationService)
    {
		_logger.LogDebug(">>> Entering LanguageValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Languages.Fields.Name.Required"));
        RuleFor(x => x.LanguageCulture)
            .Must(x =>
            {
                try
                {
                    //let's try to create a CultureInfo object
                    //if "DisplayLocale" is wrong, then exception will be thrown
                    var unused = new CultureInfo(x);
					_logger.LogDebug("LanguageValidator(localizationService={LocalizationService}): unused → {Unused}", localizationService, unused);
                    return true;
                }
                catch
                {
					_logger.LogError("Exception in LanguageValidator(localizationService={LocalizationService}): {Arg1}", localizationService, ex.Message);
                    return false;
                }
            })
            .WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Languages.Fields.LanguageCulture.Validation"));

        RuleFor(x => x.UniqueSeoCode).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Languages.Fields.UniqueSeoCode.Required"));
        RuleFor(x => x.UniqueSeoCode).Length(2).WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Languages.Fields.UniqueSeoCode.Length"));

        SetDatabaseValidationRules<Language>("UniqueSeoCode");
		_logger.LogDebug("<<< Exiting LanguageValidator(localizationService={LocalizationService})", localizationService);
    }
}