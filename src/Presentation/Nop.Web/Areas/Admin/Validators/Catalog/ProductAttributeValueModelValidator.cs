using FluentValidation;
using Nop.Core.Domain.Catalog;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Validators;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Validators.Catalog;

public partial class ProductAttributeValueModelValidator : BaseNopValidator<ProductAttributeValueModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public ProductAttributeValueModelValidator(ILocalizationService localizationService)
    {
		_logger.LogDebug(">>> Entering ProductAttributeValueModelValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessageAwait(localizationService.GetResourceAsync("Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.Name.Required"));

        RuleFor(x => x.Quantity)
            .GreaterThanOrEqualTo(1)
            .WithMessageAwait(localizationService.GetResourceAsync("Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.Quantity.GreaterThanOrEqualTo1"))
            .When(x => x.AttributeValueTypeId == (int)AttributeValueType.AssociatedToProduct && !x.CustomerEntersQty);

        RuleFor(x => x.AssociatedProductId)
            .GreaterThanOrEqualTo(1)
            .WithMessageAwait(localizationService.GetResourceAsync("Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.AssociatedProduct.Choose"))
            .When(x => x.AttributeValueTypeId == (int)AttributeValueType.AssociatedToProduct);

        SetDatabaseValidationRules<ProductAttributeValue>();
		_logger.LogDebug("<<< Exiting ProductAttributeValueModelValidator(localizationService={LocalizationService})", localizationService);
    }
}