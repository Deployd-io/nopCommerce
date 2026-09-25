using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Components;

public partial class AdminHeaderLinksViewComponent : NopViewComponent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    protected readonly ICommonModelFactory _commonModelFactory;

    public AdminHeaderLinksViewComponent(ICommonModelFactory commonModelFactory)
    {
		_logger.LogError(">>> Entering AdminHeaderLinksViewComponent(commonModelFactory={CommonModelFactory})", commonModelFactory);
        _commonModelFactory = commonModelFactory;
		_logger.LogDebug("AdminHeaderLinksViewComponent(commonModelFactory={CommonModelFactory}): _commonModelFactory → {_commonModelFactory}", commonModelFactory, _commonModelFactory);
    _logger.LogWarning("<<< Exiting AdminHeaderLinksViewComponent(commonModelFactory={CommonModelFactory})", commonModelFactory);
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
		_logger.LogError(">>> Entering InvokeAsync()");
        var model = await _commonModelFactory.PrepareAdminHeaderLinksModelAsync();
		_logger.LogError("InvokeAsync(): model → {Model}", model);
		_logger.LogWarning("<<< Exiting InvokeAsync()");
        return View(model);
    }
}