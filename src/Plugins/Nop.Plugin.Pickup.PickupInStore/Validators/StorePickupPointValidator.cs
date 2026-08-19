using FluentValidation;
using Nop.Plugin.Pickup.PickupInStore.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Pickup.PickupInStore.Validators;

public class StorePickupPointValidator : BaseNopValidator<StorePickupPointModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public StorePickupPointValidator(ILocalizationService localizationService)
    {
		_logger.LogDebug(">>> Entering StorePickupPointValidator(localizationService={LocalizationService})", localizationService);
        // Latitude
        RuleFor(model => model.Latitude)
            .InclusiveBetween(-90, 90)
            .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Pickup.PickupInStore.Fields.Latitude.InvalidRange"))
            .When(model => model.Latitude.HasValue);
        RuleFor(model => model.Latitude)
            .Must(latitude => latitude.HasValue)
            .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Pickup.PickupInStore.Fields.Latitude.IsNullWhenLongitudeHasValue"))
            .When(model => model.Longitude.HasValue);
        RuleFor(model => model.Latitude)
            .PrecisionScale(18, 8, false)
            .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Pickup.PickupInStore.Fields.Latitude.InvalidPrecision"));

        // Longitude
        RuleFor(model => model.Longitude)
            .InclusiveBetween(-180, 180)
            .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Pickup.PickupInStore.Fields.Longitude.InvalidRange"))
            .When(model => model.Longitude.HasValue);
        RuleFor(model => model.Longitude)
            .Must(longitude => longitude.HasValue)
            .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Pickup.PickupInStore.Fields.Longitude.IsNullWhenLatitudeHasValue"))
            .When(model => model.Latitude.HasValue);
        RuleFor(model => model.Longitude)
            .PrecisionScale(18, 8, false)
            .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Pickup.PickupInStore.Fields.Longitude.InvalidPrecision"));
			_logger.LogDebug("<<< Exiting StorePickupPointValidator(localizationService={LocalizationService})", localizationService);
    }
}