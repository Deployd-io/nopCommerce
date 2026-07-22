using FluentValidation;
using Nop.Core.Domain.Shipping;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Shipping;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Shipping;

public partial class ProductAvailabilityRangeValidator : BaseNopValidator<ProductAvailabilityRangeModel>
{
    public ProductAvailabilityRangeValidator(ILocalizationService localizationService)
    {
		logger.LogDebug(">>> Entering ProductAvailabilityRangeValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Shipping.ProductAvailabilityRanges.Fields.Name.Required"));

        SetDatabaseValidationRules<ProductAvailabilityRange>();
		logger.LogDebug("<<< Exiting ProductAvailabilityRangeValidator(localizationService={LocalizationService})", localizationService);
    }
}
