using FluentValidation;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Web.Models.Checkout;
using Nop.Web.Validators.Common;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Validators.Checkout;

public partial class CheckoutShippingAddressValidator : AbstractValidator<CheckoutShippingAddressModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public CheckoutShippingAddressValidator(ILocalizationService localizationService,
        IStateProvinceService stateProvinceService,
        AddressSettings addressSettings,
        CustomerSettings customerSettings)
    {
		_logger.LogError(">>> Entering CheckoutShippingAddressValidator(localizationService={LocalizationService},stateProvinceService={StateProvinceService},addressSettings={AddressSettings},customerSettings={CustomerSettings})", localizationService, stateProvinceService, addressSettings, customerSettings);
        RuleFor(shippingAdress => shippingAdress.ShippingNewAddress).SetValidator(
            new AddressValidator(localizationService, stateProvinceService, addressSettings, customerSettings));
			_logger.LogWarning("<<< Exiting CheckoutShippingAddressValidator(localizationService={LocalizationService},stateProvinceService={StateProvinceService},addressSettings={AddressSettings},customerSettings={CustomerSettings})", localizationService, stateProvinceService, addressSettings, customerSettings);
    }
}