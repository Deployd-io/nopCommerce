using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Components;

public partial class PollBlockViewComponent : NopViewComponent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    protected readonly IPollModelFactory _pollModelFactory;

    public PollBlockViewComponent(IPollModelFactory pollModelFactory)
    {
		_logger.LogDebug(">>> Entering PollBlockViewComponent(pollModelFactory={PollModelFactory})", pollModelFactory);
        _pollModelFactory = pollModelFactory;
		_logger.LogDebug("PollBlockViewComponent(pollModelFactory={PollModelFactory}): _pollModelFactory → {_pollModelFactory}", pollModelFactory, _pollModelFactory);
    }

    public async Task<IViewComponentResult> InvokeAsync(string systemKeyword)
    {
	_logger.LogDebug(">>> Entering InvokeAsync(systemKeyword={SystemKeyword})", systemKeyword);

        if (string.IsNullOrWhiteSpace(systemKeyword))
            return Content("");

        var model = await _pollModelFactory.PreparePollModelBySystemNameAsync(systemKeyword);
		_logger.LogDebug("InvokeAsync(systemKeyword={SystemKeyword}): model → {Model}", systemKeyword, model);
        if (model == null)
            return Content("");

        return View(model);
    }
}