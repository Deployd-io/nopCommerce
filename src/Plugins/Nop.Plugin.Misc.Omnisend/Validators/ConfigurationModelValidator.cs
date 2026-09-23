using FluentValidation;
using Nop.Plugin.Misc.Omnisend.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Misc.Omnisend.Validators;

/// <summary>
/// Represents configuration model validator
/// </summary>
public class ConfigurationModelValidator : BaseNopValidator<ConfigurationModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public ConfigurationModelValidator(ILocalizationService localizationService)
    {
		_logger.LogError(">>> Entering ConfigurationModelValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(model => model.ApiKey)
            .NotEmpty()
            .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Misc.Omnisend.Fields.ApiKey.Required"));
			_logger.LogWarning("<<< Exiting ConfigurationModelValidator(localizationService={LocalizationService})", localizationService);
    }

    #endregion
}