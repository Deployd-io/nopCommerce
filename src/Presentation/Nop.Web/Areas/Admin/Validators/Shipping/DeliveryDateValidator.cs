using FluentValidation;
using Nop.Core.Domain.Shipping;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Shipping;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Shipping;

public partial class DeliveryDateValidator : BaseNopValidator<DeliveryDateModel>
{
    public DeliveryDateValidator(ILocalizationService localizationService)
    {
		logger.LogDebug(">>> Entering DeliveryDateValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Shipping.DeliveryDates.Fields.Name.Required"));

        SetDatabaseValidationRules<DeliveryDate>();
		logger.LogDebug("<<< Exiting DeliveryDateValidator(localizationService={LocalizationService})", localizationService);
    }
}
