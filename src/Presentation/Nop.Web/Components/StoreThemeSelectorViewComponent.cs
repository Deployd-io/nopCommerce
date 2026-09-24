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
		_logger.LogError(">>> Entering StoreThemeSelectorViewComponent(commonModelFactory={CommonModelFactory},storeInformationSettings={StoreInformationSettings})", commonModelFactory, storeInformationSettings);
        _commonModelFactory = commonModelFactory;
        _storeInformationSettings = storeInformationSettings;
		_logger.LogDebug("StoreThemeSelectorViewComponent(commonModelFactory={CommonModelFactory},storeInformationSettings={StoreInformationSettings}): _commonModelFactory → {_commonModelFactory}", commonModelFactory, storeInformationSettings, _commonModelFactory);
    _logger.LogWarning("<<< Exiting StoreThemeSelectorViewComponent(commonModelFactory={CommonModelFactory},storeInformationSettings={StoreInformationSettings})", commonModelFactory, storeInformationSettings);
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
		_logger.LogError(">>> Entering InvokeAsync()");
        if (!_storeInformationSettings.AllowCustomerToSelectTheme)
            return Content("");

        var model = await _commonModelFactory.PrepareStoreThemeSelectorModelAsync();
		_logger.LogError("InvokeAsync(): model → {Model}", model);
		_logger.LogWarning("<<< Exiting InvokeAsync()");
        return View(model);
    }
}