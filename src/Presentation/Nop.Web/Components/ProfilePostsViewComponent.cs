using Microsoft.AspNetCore.Mvc;
using Nop.Services.Customers;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Components;

public partial class ProfilePostsViewComponent : NopViewComponent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    protected readonly ICustomerService _customerService;
    protected readonly IProfileModelFactory _profileModelFactory;

    public ProfilePostsViewComponent(ICustomerService customerService, IProfileModelFactory profileModelFactory)
    {
		_logger.LogDebug(">>> Entering ProfilePostsViewComponent(customerService={CustomerService},profileModelFactory={ProfileModelFactory})", customerService, profileModelFactory);
        _customerService = customerService;
        _profileModelFactory = profileModelFactory;
		_logger.LogDebug("<<< Exiting ProfilePostsViewComponent(customerService={CustomerService},profileModelFactory={ProfileModelFactory})", customerService, profileModelFactory);
		_logger.LogDebug("ProfilePostsViewComponent(customerService={CustomerService},profileModelFactory={ProfileModelFactory}): _profileModelFactory → {_profileModelFactory}", customerService, profileModelFactory, _profileModelFactory);
		_logger.LogDebug("ProfilePostsViewComponent(customerService={CustomerService},profileModelFactory={ProfileModelFactory}): _customerService → {_customerService}", customerService, profileModelFactory, _customerService);
    }

    public async Task<IViewComponentResult> InvokeAsync(int customerProfileId, int pageNumber)
    {
		_logger.LogDebug(">>> Entering InvokeAsync(customerProfileId={CustomerProfileId},pageNumber={PageNumber})", customerProfileId, pageNumber);
        var customer = await _customerService.GetCustomerByIdAsync(customerProfileId);
        ArgumentNullException.ThrowIfNull(customer);
		_logger.LogDebug("InvokeAsync(customerProfileId={CustomerProfileId},pageNumber={PageNumber}): customer → {Customer}", customerProfileId, pageNumber, customer);

        var model = await _profileModelFactory.PrepareProfilePostsModelAsync(customer, pageNumber);
		_logger.LogDebug("InvokeAsync(customerProfileId={CustomerProfileId},pageNumber={PageNumber}): model → {Model}", customerProfileId, pageNumber, model);
		_logger.LogDebug("<<< Exiting InvokeAsync(customerProfileId={CustomerProfileId},pageNumber={PageNumber})", customerProfileId, pageNumber);
        return View(model);
    }
}