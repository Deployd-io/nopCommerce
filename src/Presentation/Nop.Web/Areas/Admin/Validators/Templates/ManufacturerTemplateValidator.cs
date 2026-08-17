using FluentValidation;
using Nop.Core.Domain.Catalog;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Templates;
using Nop.Web.Framework.Validators;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Validators.Templates;

public partial class ManufacturerTemplateValidator : BaseNopValidator<ManufacturerTemplateModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public ManufacturerTemplateValidator(ILocalizationService localizationService)
    {
		_logger.LogDebug(">>> Entering ManufacturerTemplateValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.System.Templates.Manufacturer.Name.Required"));
        RuleFor(x => x.ViewPath).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.System.Templates.Manufacturer.ViewPath.Required"));

        SetDatabaseValidationRules<ManufacturerTemplate>();
		_logger.LogDebug("<<< Exiting ManufacturerTemplateValidator(localizationService={LocalizationService})", localizationService);
    }
}