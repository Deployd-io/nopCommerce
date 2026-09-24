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
		_logger.LogError(">>> Entering CustomerNavigationViewComponent(customerModelFactory={CustomerModelFactory})", customerModelFactory);
        _customerModelFactory = customerModelFactory;
		_logger.LogDebug("CustomerNavigationViewComponent(customerModelFactory={CustomerModelFactory}): _customerModelFactory → {_customerModelFactory}", customerModelFactory, _customerModelFactory);
    _logger.LogWarning("<<< Exiting CustomerNavigationViewComponent(customerModelFactory={CustomerModelFactory})", customerModelFactory);
    }

    public async Task<IViewComponentResult> InvokeAsync(int selectedTabId = 0)
    {
		_logger.LogError(">>> Entering InvokeAsync(selectedTabId={SelectedTabId})", selectedTabId);
        var model = await _customerModelFactory.PrepareCustomerNavigationModelAsync(selectedTabId);
		_logger.LogError("InvokeAsync(selectedTabId={SelectedTabId}): model → {Model}", selectedTabId, model);
		_logger.LogWarning("<<< Exiting InvokeAsync(selectedTabId={SelectedTabId})", selectedTabId);
        return View(model);
    }
}