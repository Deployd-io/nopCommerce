using FluentValidation;
using Nop.Core.Domain.Catalog;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Templates;
using Nop.Web.Framework.Validators;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Validators.Templates;

public partial class CategoryTemplateValidator : BaseNopValidator<CategoryTemplateModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public CategoryTemplateValidator(ILocalizationService localizationService)
    {
		_logger.LogError(">>> Entering CategoryTemplateValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.System.Templates.Category.Name.Required"));
        RuleFor(x => x.ViewPath).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.System.Templates.Category.ViewPath.Required"));

        SetDatabaseValidationRules<CategoryTemplate>();
		_logger.LogWarning("<<< Exiting CategoryTemplateValidator(localizationService={LocalizationService})", localizationService);
    }
}