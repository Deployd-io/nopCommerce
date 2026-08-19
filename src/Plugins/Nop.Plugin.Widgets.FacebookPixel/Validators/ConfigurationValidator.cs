using FluentValidation;
using Nop.Plugin.Widgets.FacebookPixel.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Widgets.FacebookPixel.Validators;

/// <summary>
/// Represents configuration model validator
/// </summary>
public class ConfigurationValidator : BaseNopValidator<FacebookPixelModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public ConfigurationValidator(ILocalizationService localizationService)
    {
		_logger.LogDebug(">>> Entering ConfigurationValidator(localizationService={LocalizationService})", localizationService);
        //set validation rules
        RuleFor(model => model.PixelId)
            .NotEmpty()
            .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Widgets.FacebookPixel.Configuration.Fields.PixelId.Required"));
        RuleFor(model => model.AccessToken)
            .NotEmpty()
            .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Widgets.FacebookPixel.Configuration.Fields.AccessToken.Required"))
            .When(model => model.ConversionsApiEnabled);
        RuleFor(model => model.UseAdvancedMatching)
            .NotEqual(true)
            .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Widgets.FacebookPixel.Configuration.Fields.UseAdvancedMatching.Forbidden"))
            .When(model => model.PassUserProperties);
        RuleFor(model => model.PassUserProperties)
            .NotEqual(true)
            .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Widgets.FacebookPixel.Configuration.Fields.PassUserProperties.Forbidden"))
            .When(model => model.UseAdvancedMatching);
			_logger.LogDebug("<<< Exiting ConfigurationValidator(localizationService={LocalizationService})", localizationService);
    }

    #endregion
}