using FluentValidation;
using Nop.Core.Domain.Configuration;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Settings;
using Nop.Web.Framework.Validators;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Validators.Settings;

public partial class SettingValidator : BaseNopValidator<SettingModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public SettingValidator(ILocalizationService localizationService)
    {
		_logger.LogDebug(">>> Entering SettingValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Settings.AllSettings.Fields.Name.Required"));

        SetDatabaseValidationRules<Setting>();
		_logger.LogDebug("<<< Exiting SettingValidator(localizationService={LocalizationService})", localizationService);
    }
}