using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Components;

public partial class CustomerNavigationViewComponent : NopViewComponent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    protected readonly ICustomerModelFactory _customerModelFactory;

    public CustomerNavigationViewComponent(ICustomerModelFactory customerModelFactory)
    {
		_logger.LogDebug(">>> Entering CustomerNavigationViewComponent(customerModelFactory={CustomerModelFactory})", customerModelFactory);
        _customerModelFactory = customerModelFactory;
		_logger.LogDebug("<<< Exiting CustomerNavigationViewComponent(customerModelFactory={CustomerModelFactory})", customerModelFactory);
		_logger.LogDebug("CustomerNavigationViewComponent(customerModelFactory={CustomerModelFactory}): _customerModelFactory → {_customerModelFactory}", customerModelFactory, _customerModelFactory);
    }

    public async Task<IViewComponentResult> InvokeAsync(int selectedTabId = 0)
    {
		_logger.LogDebug(">>> Entering InvokeAsync(selectedTabId={SelectedTabId})", selectedTabId);
        var model = await _customerModelFactory.PrepareCustomerNavigationModelAsync(selectedTabId);
		_logger.LogDebug("InvokeAsync(selectedTabId={SelectedTabId}): model → {Model}", selectedTabId, model);
		_logger.LogDebug("<<< Exiting InvokeAsync(selectedTabId={SelectedTabId})", selectedTabId);
        return View(model);
    }
}