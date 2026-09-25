using FluentValidation;
using Nop.Core.Domain.Discounts;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Discounts;
using Nop.Web.Framework.Validators;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Validators.Discounts;

public partial class DiscountValidator : BaseNopValidator<DiscountModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public DiscountValidator(ILocalizationService localizationService)
    {
		_logger.LogError(">>> Entering DiscountValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Promotions.Discounts.Fields.Name.Required"));

        SetDatabaseValidationRules<Discount>();
		_logger.LogWarning("<<< Exiting DiscountValidator(localizationService={LocalizationService})", localizationService);
    }
}