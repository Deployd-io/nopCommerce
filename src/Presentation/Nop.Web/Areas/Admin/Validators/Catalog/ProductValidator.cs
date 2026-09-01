using FluentValidation;
using Nop.Core.Domain.Catalog;
using Nop.Services.Localization;
using Nop.Services.Seo;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Validators;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Validators.Catalog;

public partial class ProductValidator : BaseNopValidator<ProductModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public ProductValidator(ILocalizationService localizationService)
    {
		_logger.LogDebug(">>> Entering ProductValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessageAwait(localizationService.GetResourceAsync("Admin.Catalog.Products.Fields.Name.Required"));

        RuleFor(x => x.SeName)
            .Length(0, NopSeoDefaults.SearchEngineNameLength)
            .WithMessageAwait(localizationService.GetResourceAsync("Admin.SEO.SeName.MaxLengthValidation"), NopSeoDefaults.SearchEngineNameLength);

        RuleFor(x => x.RentalPriceLength)
            .GreaterThan(0)
            .WithMessageAwait(localizationService.GetResourceAsync("Admin.Catalog.Products.Fields.RentalPriceLength.ShouldBeGreaterThanZero"))
            .When(x => x.IsRental);

        RuleFor(x => x.MinimumAgeToPurchase)
            .GreaterThan(0)
            .WithMessageAwait(localizationService.GetResourceAsync("Admin.Catalog.Products.Fields.MinimumAgeToPurchase.ShouldBeGreaterThanZero"))
            .When(x => x.AgeVerification);

        SetDatabaseValidationRules<Product>();
		_logger.LogDebug("<<< Exiting ProductValidator(localizationService={LocalizationService})", localizationService);
    }
}