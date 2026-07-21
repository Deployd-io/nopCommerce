using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components;

public partial class HomepagePollsViewComponent : NopViewComponent
{
    protected readonly IPollModelFactory _pollModelFactory;

    public HomepagePollsViewComponent(IPollModelFactory pollModelFactory)
    {
		logger.LogDebug(">>> Entering HomepagePollsViewComponent(pollModelFactory={PollModelFactory})", pollModelFactory);
        _pollModelFactory = pollModelFactory;
		logger.LogDebug("<<< Exiting HomepagePollsViewComponent(pollModelFactory={PollModelFactory})", pollModelFactory);
		logger.LogDebug("HomepagePollsViewComponent(pollModelFactory={PollModelFactory}): _pollModelFactory → {_pollModelFactory}", pollModelFactory, _pollModelFactory);
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
		logger.LogDebug(">>> Entering InvokeAsync()");
        var model = await _pollModelFactory.PrepareHomepagePollModelsAsync();
        if (!model.Any())
			logger.LogDebug("InvokeAsync(): model → {Model}", model);
            return Content("");

        return View(model);
    }
}
