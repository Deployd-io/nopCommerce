using FluentValidation;
using Nop.Core.Domain.Catalog;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Validators;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Validators.Catalog;

public partial class SpecificationAttributeValidator : BaseNopValidator<SpecificationAttributeModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public SpecificationAttributeValidator(ILocalizationService localizationService)
    {
		_logger.LogError(">>> Entering SpecificationAttributeValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttribute.Fields.Name.Required"));

        SetDatabaseValidationRules<SpecificationAttribute>();
		_logger.LogWarning("<<< Exiting SpecificationAttributeValidator(localizationService={LocalizationService})", localizationService);
    }
}