using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Components;

public partial class LanguageSelectorViewComponent : NopViewComponent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    protected readonly ICommonModelFactory _commonModelFactory;

    public LanguageSelectorViewComponent(ICommonModelFactory commonModelFactory)
    {
		_logger.LogDebug(">>> Entering LanguageSelectorViewComponent(commonModelFactory={CommonModelFactory})", commonModelFactory);
        _commonModelFactory = commonModelFactory;
		_logger.LogDebug("LanguageSelectorViewComponent(commonModelFactory={CommonModelFactory}): _commonModelFactory → {_commonModelFactory}", commonModelFactory, _commonModelFactory);
		_logger.LogDebug("<<< Exiting LanguageSelectorViewComponent(commonModelFactory={CommonModelFactory})", commonModelFactory);
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
		_logger.LogDebug(">>> Entering InvokeAsync()");
        var model = await _commonModelFactory.PrepareLanguageSelectorModelAsync();

		_logger.LogDebug("InvokeAsync(): model → {Model}", model);
        if (model.AvailableLanguages.Count == 1)
            return Content("");

        return View(model);
    }
}