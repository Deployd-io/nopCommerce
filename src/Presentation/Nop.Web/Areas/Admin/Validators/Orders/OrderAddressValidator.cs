using FluentValidation;
using Nop.Core.Domain.Common;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Areas.Admin.Validators.Common;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Validators.Orders;

public partial class OrderAddressValidator : AbstractValidator<OrderAddressModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public OrderAddressValidator(ILocalizationService localizationService,
        AddressSettings addressSettings)
    {
		_logger.LogError(">>> Entering OrderAddressValidator(localizationService={LocalizationService},addressSettings={AddressSettings})", localizationService, addressSettings);
        RuleFor(model => model.Address).SetValidator(new AddressValidator(addressSettings, localizationService));
		_logger.LogWarning("<<< Exiting OrderAddressValidator(localizationService={LocalizationService},addressSettings={AddressSettings})", localizationService, addressSettings);
    }
}