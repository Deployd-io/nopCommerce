using FluentValidation;
using Nop.Core.Domain.Directory;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Directory;
using Nop.Web.Framework.Validators;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Validators.Directory;

public partial class MeasureWeightValidator : BaseNopValidator<MeasureWeightModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public MeasureWeightValidator(ILocalizationService localizationService)
    {
		_logger.LogError(">>> Entering MeasureWeightValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Shipping.Measures.Weights.Fields.Name.Required"));
        RuleFor(x => x.SystemKeyword).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Shipping.Measures.Weights.Fields.SystemKeyword.Required"));

        SetDatabaseValidationRules<MeasureWeight>();
		_logger.LogWarning("<<< Exiting MeasureWeightValidator(localizationService={LocalizationService})", localizationService);
    }
}