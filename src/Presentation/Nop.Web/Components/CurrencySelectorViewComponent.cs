using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Components;

public partial class CurrencySelectorViewComponent : NopViewComponent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    protected readonly ICommonModelFactory _commonModelFactory;

    public CurrencySelectorViewComponent(ICommonModelFactory commonModelFactory)
    {
		_logger.LogError(">>> Entering CurrencySelectorViewComponent(commonModelFactory={CommonModelFactory})", commonModelFactory);
        _commonModelFactory = commonModelFactory;
		_logger.LogDebug("CurrencySelectorViewComponent(commonModelFactory={CommonModelFactory}): _commonModelFactory → {_commonModelFactory}", commonModelFactory, _commonModelFactory);
    _logger.LogWarning("<<< Exiting CurrencySelectorViewComponent(commonModelFactory={CommonModelFactory})", commonModelFactory);
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
		_logger.LogError(">>> Entering InvokeAsync()");
        var model = await _commonModelFactory.PrepareCurrencySelectorModelAsync();
			_logger.LogError("InvokeAsync(): model → {Model}", model);
		_logger.LogWarning("<<< Exiting InvokeAsync()");
        if (model.AvailableCurrencies.Count == 1) {
            return Content("");
        }

        return View(model);
    }
}