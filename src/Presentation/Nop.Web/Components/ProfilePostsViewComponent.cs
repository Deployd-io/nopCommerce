using Microsoft.AspNetCore.Mvc;
using Nop.Services.Customers;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components;

public partial class ProfilePostsViewComponent : NopViewComponent
{
    protected readonly ICustomerService _customerService;
    protected readonly IProfileModelFactory _profileModelFactory;

    public ProfilePostsViewComponent(ICustomerService customerService, IProfileModelFactory profileModelFactory)
    {
		logger.LogDebug(">>> Entering ProfilePostsViewComponent(customerService={CustomerService},profileModelFactory={ProfileModelFactory})", customerService, profileModelFactory);
        _customerService = customerService;
        _profileModelFactory = profileModelFactory;
		logger.LogDebug("<<< Exiting ProfilePostsViewComponent(customerService={CustomerService},profileModelFactory={ProfileModelFactory})", customerService, profileModelFactory);
		logger.LogDebug("ProfilePostsViewComponent(customerService={CustomerService},profileModelFactory={ProfileModelFactory}): _profileModelFactory → {_profileModelFactory}", customerService, profileModelFactory, _profileModelFactory);
		logger.LogDebug("ProfilePostsViewComponent(customerService={CustomerService},profileModelFactory={ProfileModelFactory}): _customerService → {_customerService}", customerService, profileModelFactory, _customerService);
    }

    public async Task<IViewComponentResult> InvokeAsync(int customerProfileId, int pageNumber)
    {
		logger.LogDebug(">>> Entering InvokeAsync(customerProfileId={CustomerProfileId},pageNumber={PageNumber})", customerProfileId, pageNumber);
        var customer = await _customerService.GetCustomerByIdAsync(customerProfileId);
        ArgumentNullException.ThrowIfNull(customer);
		logger.LogDebug("InvokeAsync(customerProfileId={CustomerProfileId},pageNumber={PageNumber}): customer → {Customer}", customerProfileId, pageNumber, customer);

        var model = await _profileModelFactory.PrepareProfilePostsModelAsync(customer, pageNumber);
		logger.LogDebug("InvokeAsync(customerProfileId={CustomerProfileId},pageNumber={PageNumber}): model → {Model}", customerProfileId, pageNumber, model);
		logger.LogDebug("<<< Exiting InvokeAsync(customerProfileId={CustomerProfileId},pageNumber={PageNumber})", customerProfileId, pageNumber);
        return View(model);
    }
}