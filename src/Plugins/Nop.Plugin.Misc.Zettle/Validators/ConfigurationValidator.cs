using FluentValidation;
using Nop.Plugin.Misc.Zettle.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Misc.Zettle.Validators;

/// <summary>
/// Represents configuration model validator
/// </summary>
public class ConfigurationValidator : BaseNopValidator<ConfigurationModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public ConfigurationValidator(ILocalizationService localizationService)
    {
		_logger.LogError(">>> Entering ConfigurationValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(model => model.ApiKey)
            .NotEmpty()
            .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Misc.Zettle.Configuration.Fields.ApiKey.Required"))
            .When(model => !string.IsNullOrEmpty(model.ClientId));

        RuleFor(model => model.AutoSyncPeriod)
            .NotEmpty()
            .GreaterThan(0)
            .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Misc.Zettle.Configuration.Fields.AutoSyncPeriod.Invalid"))
            .When(model => model.AutoSyncEnabled);
			_logger.LogWarning("<<< Exiting ConfigurationValidator(localizationService={LocalizationService})", localizationService);
    }

    #endregion
}