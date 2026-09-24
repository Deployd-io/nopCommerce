using FluentValidation;
using Nop.Core.Domain.Localization;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Localization;
using Nop.Web.Framework.Validators;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Validators.Localization;

public partial class LanguageResourceValidator : BaseNopValidator<LocaleResourceModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public LanguageResourceValidator(ILocalizationService localizationService)
    {
		_logger.LogError(">>> Entering LanguageResourceValidator(localizationService={LocalizationService})", localizationService);
        //if validation without this set rule is applied, in this case nothing will be validated
        //it's used to prevent auto-validation of child models
        RuleSet(NopValidationDefaults.ValidationRuleSet, () =>
        {
            RuleFor(model => model.ResourceName)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Languages.Resources.Fields.Name.Required"));

            RuleFor(model => model.ResourceValue)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Languages.Resources.Fields.Value.Required"));

            SetDatabaseValidationRules<LocaleStringResource>();
        });
		_logger.LogWarning("<<< Exiting LanguageResourceValidator(localizationService={LocalizationService})", localizationService);
    }
}