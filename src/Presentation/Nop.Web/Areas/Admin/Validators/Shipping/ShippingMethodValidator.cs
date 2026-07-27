using FluentValidation;
using Nop.Core.Domain.Shipping;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Shipping;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Shipping;

public partial class ShippingMethodValidator : BaseNopValidator<ShippingMethodModel>
{
    public ShippingMethodValidator(ILocalizationService localizationService)
    {
		logger.LogDebug(">>> Entering ShippingMethodValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Shipping.Methods.Fields.Name.Required"));

        SetDatabaseValidationRules<ShippingMethod>();
		logger.LogDebug("<<< Exiting ShippingMethodValidator(localizationService={LocalizationService})", localizationService);
    }
}