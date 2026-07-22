using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components;

public partial class SocialButtonsViewComponent : NopViewComponent
{
    protected readonly ICommonModelFactory _commonModelFactory;

    public SocialButtonsViewComponent(ICommonModelFactory commonModelFactory)
    {
		logger.LogDebug(">>> Entering SocialButtonsViewComponent(commonModelFactory={CommonModelFactory})", commonModelFactory);
        _commonModelFactory = commonModelFactory;
		logger.LogDebug("<<< Exiting SocialButtonsViewComponent(commonModelFactory={CommonModelFactory})", commonModelFactory);
		logger.LogDebug("SocialButtonsViewComponent(commonModelFactory={CommonModelFactory}): _commonModelFactory → {_commonModelFactory}", commonModelFactory, _commonModelFactory);
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
		logger.LogDebug(">>> Entering InvokeAsync()");
        var model = await _commonModelFactory.PrepareSocialModelAsync();
		logger.LogDebug("InvokeAsync(): model → {Model}", model);
		logger.LogDebug("<<< Exiting InvokeAsync()");
        return View(model);
    }
}
