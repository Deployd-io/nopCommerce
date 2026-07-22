using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components;

public partial class CustomerNavigationViewComponent : NopViewComponent
{
    protected readonly ICustomerModelFactory _customerModelFactory;

    public CustomerNavigationViewComponent(ICustomerModelFactory customerModelFactory)
    {
		logger.LogDebug(">>> Entering CustomerNavigationViewComponent(customerModelFactory={CustomerModelFactory})", customerModelFactory);
        _customerModelFactory = customerModelFactory;
		logger.LogDebug("<<< Exiting CustomerNavigationViewComponent(customerModelFactory={CustomerModelFactory})", customerModelFactory);
		logger.LogDebug("CustomerNavigationViewComponent(customerModelFactory={CustomerModelFactory}): _customerModelFactory → {_customerModelFactory}", customerModelFactory, _customerModelFactory);
    }

    public async Task<IViewComponentResult> InvokeAsync(int selectedTabId = 0)
    {
		logger.LogDebug(">>> Entering InvokeAsync(selectedTabId={SelectedTabId})", selectedTabId);
        var model = await _customerModelFactory.PrepareCustomerNavigationModelAsync(selectedTabId);
		logger.LogDebug("InvokeAsync(selectedTabId={SelectedTabId}): model → {Model}", selectedTabId, model);
		logger.LogDebug("<<< Exiting InvokeAsync(selectedTabId={SelectedTabId})", selectedTabId);
        return View(model);
    }
}
