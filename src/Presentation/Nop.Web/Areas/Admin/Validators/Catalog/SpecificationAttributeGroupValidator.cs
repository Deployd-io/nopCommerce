using FluentValidation;
using Nop.Core.Domain.Catalog;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Validators;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Validators.Catalog;

/// <summary>
/// Represents a validator for <see cref="SpecificationAttributeGroupModel"/>
/// </summary>
public partial class SpecificationAttributeGroupValidator : BaseNopValidator<SpecificationAttributeGroupModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public SpecificationAttributeGroupValidator(ILocalizationService localizationService)
    {
		_logger.LogDebug(">>> Entering SpecificationAttributeGroupValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessageAwait(localizationService.GetResourceAsync("Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttributeGroup.Fields.Name.Required"));

        SetDatabaseValidationRules<SpecificationAttributeGroup>();
		_logger.LogDebug("<<< Exiting SpecificationAttributeGroupValidator(localizationService={LocalizationService})", localizationService);
    }
}