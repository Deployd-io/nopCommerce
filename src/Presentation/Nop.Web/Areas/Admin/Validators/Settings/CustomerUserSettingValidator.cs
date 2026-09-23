using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Settings;
using Nop.Web.Framework.Validators;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Validators.Settings;

public partial class CustomerUserSettingValidator : BaseNopValidator<CustomerUserSettingsModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public CustomerUserSettingValidator(ILocalizationService localizationService)
    {
		_logger.LogError(">>> Entering CustomerUserSettingValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(x => x.CustomerSettings.PasswordMinLength)
            .GreaterThan(0)
            .WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Settings.CustomerUser.PasswordMinLength.GreaterThanZero"));

        RuleFor(x => x.CustomerSettings.PasswordMaxLength)
            .GreaterThanOrEqualTo(x => x.CustomerSettings.PasswordMinLength)
            .WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Settings.CustomerUser.PasswordMaxLength.GreaterThanOrEqualMinLength"));
			_logger.LogWarning("<<< Exiting CustomerUserSettingValidator(localizationService={LocalizationService})", localizationService);
    }
}
