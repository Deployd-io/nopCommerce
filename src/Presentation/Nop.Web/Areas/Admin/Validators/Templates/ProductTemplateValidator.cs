using FluentValidation;
using Nop.Core.Domain.Catalog;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Templates;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Templates;

public partial class ProductTemplateValidator : BaseNopValidator<ProductTemplateModel>
{
    public ProductTemplateValidator(ILocalizationService localizationService)
    {
		logger.LogDebug(">>> Entering ProductTemplateValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.System.Templates.Product.Name.Required"));
        RuleFor(x => x.ViewPath).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.System.Templates.Product.ViewPath.Required"));

        SetDatabaseValidationRules<ProductTemplate>();
		logger.LogDebug("<<< Exiting ProductTemplateValidator(localizationService={LocalizationService})", localizationService);
    }
}
