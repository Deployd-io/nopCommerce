using FluentValidation;
using Nop.Core.Domain.Catalog;
using Nop.Services.Localization;
using Nop.Services.Seo;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Validators;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Validators.Catalog;

public partial class CategoryValidator : BaseNopValidator<CategoryModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public CategoryValidator(ILocalizationService localizationService)
    {
		_logger.LogDebug(">>> Entering CategoryValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Catalog.Categories.Fields.Name.Required"));
        RuleFor(x => x.PageSizeOptions).Must(ValidatorUtilities.PageSizeOptionsValidator).WithMessageAwait(localizationService.GetResourceAsync("Admin.Catalog.Categories.Fields.PageSizeOptions.ShouldHaveUniqueItems"));
        RuleFor(x => x.PageSize).Must((x, context) =>
        {
            if (!x.AllowCustomersToSelectPageSize && x.PageSize <= 0)
                return false;

            return true;
        }).WithMessageAwait(localizationService.GetResourceAsync("Admin.Catalog.Categories.Fields.PageSize.Positive"));
        RuleFor(x => x.SeName).Length(0, NopSeoDefaults.SearchEngineNameLength)
            .WithMessageAwait(localizationService.GetResourceAsync("Admin.SEO.SeName.MaxLengthValidation"), NopSeoDefaults.SearchEngineNameLength);

        RuleFor(x => x.PriceFrom)
            .GreaterThanOrEqualTo(0)
            .WithMessageAwait(localizationService.GetResourceAsync("Admin.Catalog.Categories.Fields.PriceFrom.GreaterThanOrEqualZero"))
            .When(x => x.PriceRangeFiltering && x.ManuallyPriceRange);

        RuleFor(x => x.PriceTo)
            .GreaterThan(x => x.PriceFrom > decimal.Zero ? x.PriceFrom : decimal.Zero)
            .WithMessage(x => string.Format(localizationService.GetResourceAsync("Admin.Catalog.Categories.Fields.PriceTo.GreaterThanZeroOrPriceFrom").Result, x.PriceFrom > decimal.Zero ? x.PriceFrom : decimal.Zero))
            .When(x => x.PriceRangeFiltering && x.ManuallyPriceRange);

        SetDatabaseValidationRules<Category>();
		_logger.LogDebug("<<< Exiting CategoryValidator(localizationService={LocalizationService})", localizationService);
    }
}