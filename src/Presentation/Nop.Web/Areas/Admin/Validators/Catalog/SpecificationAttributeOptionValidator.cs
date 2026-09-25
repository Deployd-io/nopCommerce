using FluentValidation;
using Nop.Core.Domain.Catalog;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Validators;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Validators.Catalog;

public partial class SpecificationAttributeOptionValidator : BaseNopValidator<SpecificationAttributeOptionModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public SpecificationAttributeOptionValidator(ILocalizationService localizationService)
    {
		_logger.LogError(">>> Entering SpecificationAttributeOptionValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttribute.Options.Fields.Name.Required"));

        SetDatabaseValidationRules<SpecificationAttributeOption>();
		_logger.LogWarning("<<< Exiting SpecificationAttributeOptionValidator(localizationService={LocalizationService})", localizationService);
    }
}