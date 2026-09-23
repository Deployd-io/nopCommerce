using FluentValidation;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Web.Models.Customer;
using Nop.Web.Validators.Common;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Validators.Customer;

public partial class CustomerAddressEditValidator : AbstractValidator<CustomerAddressEditModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public CustomerAddressEditValidator(ILocalizationService localizationService,
        IStateProvinceService stateProvinceService,
        AddressSettings addressSettings,
        CustomerSettings customerSettings)
    {
		_logger.LogError(">>> Entering CustomerAddressEditValidator(localizationService={LocalizationService},stateProvinceService={StateProvinceService},addressSettings={AddressSettings},customerSettings={CustomerSettings})", localizationService, stateProvinceService, addressSettings, customerSettings);
        RuleFor(model => model.Address).SetValidator(new AddressValidator(localizationService, stateProvinceService, addressSettings, customerSettings));
		_logger.LogWarning("<<< Exiting CustomerAddressEditValidator(localizationService={LocalizationService},stateProvinceService={StateProvinceService},addressSettings={AddressSettings},customerSettings={CustomerSettings})", localizationService, stateProvinceService, addressSettings, customerSettings);
    }
}