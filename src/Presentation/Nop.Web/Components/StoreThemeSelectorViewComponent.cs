using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Components;

public partial class StoreThemeSelectorViewComponent : NopViewComponent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    protected readonly ICommonModelFactory _commonModelFactory;
    protected readonly StoreInformationSettings _storeInformationSettings;

    public StoreThemeSelectorViewComponent(ICommonModelFactory commonModelFactory,
        StoreInformationSettings storeInformationSettings)
    {
        _commonModelFactory = commonModelFactory;
        _storeInformationSettings = storeInformationSettings;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
		_logger.LogDebug(">>> Entering InvokeAsync()");
        if (!_storeInformationSettings.AllowCustomerToSelectTheme)
            return Content("");

        var model = await _commonModelFactory.PrepareStoreThemeSelectorModelAsync();
		_logger.LogDebug("InvokeAsync(): model → {Model}", model);
		_logger.LogDebug("<<< Exiting InvokeAsync()");
        return View(model);
    }
}