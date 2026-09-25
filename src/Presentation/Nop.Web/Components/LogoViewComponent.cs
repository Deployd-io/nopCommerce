using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Components;

public partial class LogoViewComponent : NopViewComponent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    protected readonly ICommonModelFactory _commonModelFactory;

    public LogoViewComponent(ICommonModelFactory commonModelFactory)
    {
		_logger.LogError(">>> Entering LogoViewComponent(commonModelFactory={CommonModelFactory})", commonModelFactory);
        _commonModelFactory = commonModelFactory;
		_logger.LogDebug("LogoViewComponent(commonModelFactory={CommonModelFactory}): _commonModelFactory → {_commonModelFactory}", commonModelFactory, _commonModelFactory);
    _logger.LogWarning("<<< Exiting LogoViewComponent(commonModelFactory={CommonModelFactory})", commonModelFactory);
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
		_logger.LogError(">>> Entering InvokeAsync()");
        var model = await _commonModelFactory.PrepareLogoModelAsync();
		_logger.LogError("InvokeAsync(): model → {Model}", model);
		_logger.LogWarning("<<< Exiting InvokeAsync()");
        return View(model);
    }
}