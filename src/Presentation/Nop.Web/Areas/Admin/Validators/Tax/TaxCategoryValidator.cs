using FluentValidation;
using Nop.Core.Domain.Tax;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Tax;
using Nop.Web.Framework.Validators;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Validators.Tax;

public partial class TaxCategoryValidator : BaseNopValidator<TaxCategoryModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public TaxCategoryValidator(ILocalizationService localizationService)
    {
		_logger.LogDebug(">>> Entering TaxCategoryValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Tax.Categories.Fields.Name.Required"));

        SetDatabaseValidationRules<TaxCategory>();
		_logger.LogDebug("<<< Exiting TaxCategoryValidator(localizationService={LocalizationService})", localizationService);
    }
}