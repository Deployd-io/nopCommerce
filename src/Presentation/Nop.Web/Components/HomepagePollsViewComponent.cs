using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Components;

public partial class HomepagePollsViewComponent : NopViewComponent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    protected readonly IPollModelFactory _pollModelFactory;

    public HomepagePollsViewComponent(IPollModelFactory pollModelFactory)
    {
		_logger.LogError(">>> Entering HomepagePollsViewComponent(pollModelFactory={PollModelFactory})", pollModelFactory);
        _pollModelFactory = pollModelFactory;
		_logger.LogDebug("HomepagePollsViewComponent(pollModelFactory={PollModelFactory}): _pollModelFactory → {_pollModelFactory}", pollModelFactory, _pollModelFactory);
    _logger.LogWarning("<<< Exiting HomepagePollsViewComponent(pollModelFactory={PollModelFactory})", pollModelFactory);
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
		_logger.LogError(">>> Entering InvokeAsync()");
        var model = await _pollModelFactory.PrepareHomepagePollModelsAsync();
        if (!model.Any())
            return Content("");

        return View(model);
    }
}