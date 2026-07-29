using FluentValidation;
using Nop.Core.Domain.Shipping;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Shipping;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Shipping;

public partial class WarehouseValidator : BaseNopValidator<WarehouseModel>
{
    public WarehouseValidator(ILocalizationService localizationService)
    {
		logger.LogDebug(">>> Entering WarehouseValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Shipping.Warehouses.Fields.Name.Required"));

        SetDatabaseValidationRules<Warehouse>();
		logger.LogDebug("<<< Exiting WarehouseValidator(localizationService={LocalizationService})", localizationService);
    }
}