using FluentValidation;
using Nop.Core.Domain.Shipping;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Shipping;
using Nop.Web.Framework.Validators;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Validators.Shipping;

public partial class ShippingMethodValidator : BaseNopValidator<ShippingMethodModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public ShippingMethodValidator(ILocalizationService localizationService)
    {
		_logger.LogError(">>> Entering ShippingMethodValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Shipping.Methods.Fields.Name.Required"));

        SetDatabaseValidationRules<ShippingMethod>();
		_logger.LogWarning("<<< Exiting ShippingMethodValidator(localizationService={LocalizationService})", localizationService);
    }
}