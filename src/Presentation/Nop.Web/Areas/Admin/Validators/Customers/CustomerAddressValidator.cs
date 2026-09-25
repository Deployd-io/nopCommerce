using FluentValidation;
using Nop.Core.Domain.Common;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Customers;
using Nop.Web.Areas.Admin.Validators.Common;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Validators.Customers;

public partial class CustomerAddressValidator : AbstractValidator<CustomerAddressModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public CustomerAddressValidator(ILocalizationService localizationService,
        AddressSettings addressSettings)
    {
		_logger.LogError(">>> Entering CustomerAddressValidator(localizationService={LocalizationService},addressSettings={AddressSettings})", localizationService, addressSettings);
        RuleFor(model => model.Address).SetValidator(new AddressValidator(addressSettings, localizationService));
		_logger.LogWarning("<<< Exiting CustomerAddressValidator(localizationService={LocalizationService},addressSettings={AddressSettings})", localizationService, addressSettings);
    }
}