using FluentValidation;
using Nop.Plugin.Widgets.Swiper.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Widgets.Swiper.Validators;

/// <summary>
/// Represents slide model validator
/// </summary>
public class SlidePictureValidator : BaseNopValidator<SlidePictureModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public SlidePictureValidator(ILocalizationService localizationService)
    {
		_logger.LogError(">>> Entering SlidePictureValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(model => model.PictureId)
            .GreaterThan(0)
            .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Widgets.Swiper.Picture.Required"));
			_logger.LogWarning("<<< Exiting SlidePictureValidator(localizationService={LocalizationService})", localizationService);
    }

    #endregion
}
