using FluentValidation;
using Nop.Core.Domain.Catalog;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Validators;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Validators.Catalog;

/// <summary>
/// Represent a review type validator
/// </summary>
public partial class ReviewTypeValidator : BaseNopValidator<ReviewTypeModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public ReviewTypeValidator(ILocalizationService localizationService)
    {
		_logger.LogError(">>> Entering ReviewTypeValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Settings.ReviewType.Fields.Name.Required"));
        RuleFor(x => x.Description).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Settings.ReviewType.Fields.Description.Required"));

        SetDatabaseValidationRules<ReviewType>();
		_logger.LogWarning("<<< Exiting ReviewTypeValidator(localizationService={LocalizationService})", localizationService);
    }
}