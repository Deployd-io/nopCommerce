using FluentValidation;
using Nop.Core.Domain.Shipping;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Shipping;
using Nop.Web.Framework.Validators;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Validators.Shipping;

public partial class DeliveryDateValidator : BaseNopValidator<DeliveryDateModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public DeliveryDateValidator(ILocalizationService localizationService)
    {
		_logger.LogDebug(">>> Entering DeliveryDateValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Shipping.DeliveryDates.Fields.Name.Required"));

        SetDatabaseValidationRules<DeliveryDate>();
		_logger.LogDebug("<<< Exiting DeliveryDateValidator(localizationService={LocalizationService})", localizationService);
    }
}