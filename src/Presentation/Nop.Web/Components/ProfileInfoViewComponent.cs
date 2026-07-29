using Microsoft.AspNetCore.Mvc;
using Nop.Services.Customers;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components;

public partial class ProfileInfoViewComponent : NopViewComponent
{
    protected readonly ICustomerService _customerService;
    protected readonly IProfileModelFactory _profileModelFactory;

    public ProfileInfoViewComponent(ICustomerService customerService, IProfileModelFactory profileModelFactory)
    {
		logger.LogDebug(">>> Entering ProfileInfoViewComponent(customerService={CustomerService},profileModelFactory={ProfileModelFactory})", customerService, profileModelFactory);
        _customerService = customerService;
        _profileModelFactory = profileModelFactory;
		logger.LogDebug("<<< Exiting ProfileInfoViewComponent(customerService={CustomerService},profileModelFactory={ProfileModelFactory})", customerService, profileModelFactory);
		logger.LogDebug("ProfileInfoViewComponent(customerService={CustomerService},profileModelFactory={ProfileModelFactory}): _profileModelFactory → {_profileModelFactory}", customerService, profileModelFactory, _profileModelFactory);
		logger.LogDebug("ProfileInfoViewComponent(customerService={CustomerService},profileModelFactory={ProfileModelFactory}): _customerService → {_customerService}", customerService, profileModelFactory, _customerService);
    }

    public async Task<IViewComponentResult> InvokeAsync(int customerProfileId)
    {
		logger.LogDebug(">>> Entering InvokeAsync(customerProfileId={CustomerProfileId})", customerProfileId);
        var customer = await _customerService.GetCustomerByIdAsync(customerProfileId);
        ArgumentNullException.ThrowIfNull(customer);
		logger.LogDebug("InvokeAsync(customerProfileId={CustomerProfileId}): customer → {Customer}", customerProfileId, customer);

        var model = await _profileModelFactory.PrepareProfileInfoModelAsync(customer);
		logger.LogDebug("InvokeAsync(customerProfileId={CustomerProfileId}): model → {Model}", customerProfileId, model);
		logger.LogDebug("<<< Exiting InvokeAsync(customerProfileId={CustomerProfileId})", customerProfileId);
        return View(model);
    }
}