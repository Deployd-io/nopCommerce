using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Catalog;

public partial class PredefinedProductAttributeValueModelValidator : BaseNopValidator<PredefinedProductAttributeValueModel>
{
    public PredefinedProductAttributeValueModelValidator(ILocalizationService localizationService)
    {
		logger.LogDebug(">>> Entering PredefinedProductAttributeValueModelValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessageAwait(localizationService.GetResourceAsync("Admin.Catalog.Attributes.ProductAttributes.PredefinedValues.Fields.Name.Required"));
			logger.LogDebug("<<< Exiting PredefinedProductAttributeValueModelValidator(localizationService={LocalizationService})", localizationService);
    }
}