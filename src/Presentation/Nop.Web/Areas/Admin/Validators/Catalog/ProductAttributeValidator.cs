using FluentValidation;
using Nop.Core.Domain.Catalog;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Validators;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Validators.Catalog;

public partial class ProductAttributeValidator : BaseNopValidator<ProductAttributeModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public ProductAttributeValidator(ILocalizationService localizationService)
    {
		_logger.LogError(">>> Entering ProductAttributeValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Catalog.Attributes.ProductAttributes.Fields.Name.Required"));
        SetDatabaseValidationRules<ProductAttribute>();
		_logger.LogWarning("<<< Exiting ProductAttributeValidator(localizationService={LocalizationService})", localizationService);
    }
}