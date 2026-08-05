using FluentValidation;
using Nop.Core.Domain.Shipping;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Shipping;
using Nop.Web.Framework.Validators;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Validators.Shipping;

public partial class WarehouseValidator : BaseNopValidator<WarehouseModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public WarehouseValidator(ILocalizationService localizationService)
    {
		_logger.LogDebug(">>> Entering WarehouseValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Shipping.Warehouses.Fields.Name.Required"));

        SetDatabaseValidationRules<Warehouse>();
		_logger.LogDebug("<<< Exiting WarehouseValidator(localizationService={LocalizationService})", localizationService);
    }
}