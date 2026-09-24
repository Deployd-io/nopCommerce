using Microsoft.AspNetCore.Mvc;
using Nop.Services.Customers;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Components;

public partial class ProfileInfoViewComponent : NopViewComponent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    protected readonly ICustomerService _customerService;
    protected readonly IProfileModelFactory _profileModelFactory;

    public ProfileInfoViewComponent(ICustomerService customerService, IProfileModelFactory profileModelFactory)
    {
		_logger.LogError(">>> Entering ProfileInfoViewComponent(customerService={CustomerService},profileModelFactory={ProfileModelFactory})", customerService, profileModelFactory);
        _customerService = customerService;
        _profileModelFactory = profileModelFactory;
		_logger.LogDebug("ProfileInfoViewComponent(customerService={CustomerService},profileModelFactory={ProfileModelFactory}): _customerService → {_customerService}", customerService, profileModelFactory, _customerService);
    _logger.LogWarning("<<< Exiting ProfileInfoViewComponent(customerService={CustomerService},profileModelFactory={ProfileModelFactory})", customerService, profileModelFactory);
    }

    public async Task<IViewComponentResult> InvokeAsync(int customerProfileId)
    {
		_logger.LogError(">>> Entering InvokeAsync(customerProfileId={CustomerProfileId})", customerProfileId);
        var customer = await _customerService.GetCustomerByIdAsync(customerProfileId);
        ArgumentNullException.ThrowIfNull(customer);
		_logger.LogError("InvokeAsync(customerProfileId={CustomerProfileId}): customer → {Customer}", customerProfileId, customer);

        var model = await _profileModelFactory.PrepareProfileInfoModelAsync(customer);
		_logger.LogError("InvokeAsync(customerProfileId={CustomerProfileId}): model → {Model}", customerProfileId, model);
		_logger.LogWarning("<<< Exiting InvokeAsync(customerProfileId={CustomerProfileId})", customerProfileId);
        return View(model);
    }
}