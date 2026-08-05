using FluentValidation;
using Nop.Core.Domain.Directory;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Directory;
using Nop.Web.Framework.Validators;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Validators.Directory;

public partial class MeasureDimensionValidator : BaseNopValidator<MeasureDimensionModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public MeasureDimensionValidator(ILocalizationService localizationService)
    {
		_logger.LogDebug(">>> Entering MeasureDimensionValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Shipping.Measures.Dimensions.Fields.Name.Required"));
        RuleFor(x => x.SystemKeyword).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Shipping.Measures.Dimensions.Fields.SystemKeyword.Required"));

        SetDatabaseValidationRules<MeasureDimension>();
		_logger.LogDebug("<<< Exiting MeasureDimensionValidator(localizationService={LocalizationService})", localizationService);
    }
}