using FluentValidation;
using Nop.Core.Domain.Shipping;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Shipping;
using Nop.Web.Framework.Validators;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Validators.Shipping;

public partial class ProductAvailabilityRangeValidator : BaseNopValidator<ProductAvailabilityRangeModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public ProductAvailabilityRangeValidator(ILocalizationService localizationService)
    {
		_logger.LogError(">>> Entering ProductAvailabilityRangeValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Shipping.ProductAvailabilityRanges.Fields.Name.Required"));

        SetDatabaseValidationRules<ProductAvailabilityRange>();
		_logger.LogWarning("<<< Exiting ProductAvailabilityRangeValidator(localizationService={LocalizationService})", localizationService);
    }
}