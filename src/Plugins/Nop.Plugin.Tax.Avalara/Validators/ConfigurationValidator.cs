using FluentValidation;
using Nop.Plugin.Tax.Avalara.Models.Configuration;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Tax.Avalara.Validators;

/// <summary>
/// Represents configuration model validator
/// </summary>
public class ConfigurationValidator : BaseNopValidator<ConfigurationModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public ConfigurationValidator(ILocalizationService localizationService)
    {
		_logger.LogDebug(">>> Entering ConfigurationValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(model => model.AccountId)
            .NotEmpty()
            .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Tax.Avalara.Fields.AccountId.Required"))
            .When(model => !model.UseSandbox);
        RuleFor(model => model.LicenseKey)
            .NotEmpty()
            .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Tax.Avalara.Fields.LicenseKey.Required"))
            .When(model => !model.UseSandbox);
        RuleFor(model => model.SelectedCountryIds)
            .NotEmpty()
            .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Tax.Avalara.Fields.Countries.Required"))
            .When(model => model.UseItemClassification);
			_logger.LogDebug("<<< Exiting ConfigurationValidator(localizationService={LocalizationService})", localizationService);
    }

    #endregion
}