using FluentValidation;
using Nop.Core.Domain.Discounts;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Discounts;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Discounts;

public partial class DiscountValidator : BaseNopValidator<DiscountModel>
{
    public DiscountValidator(ILocalizationService localizationService)
    {
		logger.LogDebug(">>> Entering DiscountValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Promotions.Discounts.Fields.Name.Required"));

        SetDatabaseValidationRules<Discount>();
		logger.LogDebug("<<< Exiting DiscountValidator(localizationService={LocalizationService})", localizationService);
    }
}