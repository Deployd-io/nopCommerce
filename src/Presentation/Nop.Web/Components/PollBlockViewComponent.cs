using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components;

public partial class PollBlockViewComponent : NopViewComponent
{
    protected readonly IPollModelFactory _pollModelFactory;

    public PollBlockViewComponent(IPollModelFactory pollModelFactory)
    {
		logger.LogDebug(">>> Entering PollBlockViewComponent(pollModelFactory={PollModelFactory})", pollModelFactory);
        _pollModelFactory = pollModelFactory;
		logger.LogDebug("<<< Exiting PollBlockViewComponent(pollModelFactory={PollModelFactory})", pollModelFactory);
		logger.LogDebug("PollBlockViewComponent(pollModelFactory={PollModelFactory}): _pollModelFactory → {_pollModelFactory}", pollModelFactory, _pollModelFactory);
    }

    public async Task<IViewComponentResult> InvokeAsync(string systemKeyword)
    {
	logger.LogDebug(">>> Entering InvokeAsync(systemKeyword={SystemKeyword})", systemKeyword);

        if (string.IsNullOrWhiteSpace(systemKeyword))
            return Content("");

        var model = await _pollModelFactory.PreparePollModelBySystemNameAsync(systemKeyword);
        if (model == null)
			logger.LogDebug("InvokeAsync(systemKeyword={SystemKeyword}): model → {Model}", systemKeyword, model);
            return Content("");

        return View(model);
    }
}