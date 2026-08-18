using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Validators;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Validators.Catalog;

public partial class PredefinedProductAttributeValueModelValidator : BaseNopValidator<PredefinedProductAttributeValueModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public PredefinedProductAttributeValueModelValidator(ILocalizationService localizationService)
    {
		_logger.LogDebug(">>> Entering PredefinedProductAttributeValueModelValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessageAwait(localizationService.GetResourceAsync("Admin.Catalog.Attributes.ProductAttributes.PredefinedValues.Fields.Name.Required"));
			_logger.LogDebug("<<< Exiting PredefinedProductAttributeValueModelValidator(localizationService={LocalizationService})", localizationService);
    }
}