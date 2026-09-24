using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Nop.Web.Models.Customer;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Validators.Customer;

public partial class GiftCardValidator : BaseNopValidator<CheckGiftCardBalanceModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public GiftCardValidator(ILocalizationService localizationService)
    {
		_logger.LogError(">>> Entering GiftCardValidator(localizationService={LocalizationService})", localizationService);
        RuleFor(x => x.GiftCardCode).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("CheckGiftCardBalance.GiftCardCouponCode.Empty"));
		_logger.LogWarning("<<< Exiting GiftCardValidator(localizationService={LocalizationService})", localizationService);
    }
}