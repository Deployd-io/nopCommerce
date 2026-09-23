using FluentValidation;
using Nop.Core.Domain.Catalog;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Validators;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Validators.Catalog;

public partial class ProductTagValidator : BaseNopValidator<ProductTagModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public ProductTagValidator(ILocalizationService localizationService)
    {
		_logger.LogError(">>> Entering ProductTagValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Catalog.ProductTags.Fields.Name.Required"));

        SetDatabaseValidationRules<ProductTag>();
		_logger.LogWarning("<<< Exiting ProductTagValidator(localizationService={LocalizationService})", localizationService);
    }
}