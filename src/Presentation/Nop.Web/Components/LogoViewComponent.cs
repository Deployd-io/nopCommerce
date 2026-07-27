using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components;

public partial class LogoViewComponent : NopViewComponent
{
    protected readonly ICommonModelFactory _commonModelFactory;

    public LogoViewComponent(ICommonModelFactory commonModelFactory)
    {
		logger.LogDebug(">>> Entering LogoViewComponent(commonModelFactory={CommonModelFactory})", commonModelFactory);
        _commonModelFactory = commonModelFactory;
		logger.LogDebug("<<< Exiting LogoViewComponent(commonModelFactory={CommonModelFactory})", commonModelFactory);
		logger.LogDebug("LogoViewComponent(commonModelFactory={CommonModelFactory}): _commonModelFactory → {_commonModelFactory}", commonModelFactory, _commonModelFactory);
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
		logger.LogDebug(">>> Entering InvokeAsync()");
        var model = await _commonModelFactory.PrepareLogoModelAsync();
		logger.LogDebug("InvokeAsync(): model → {Model}", model);
		logger.LogDebug("<<< Exiting InvokeAsync()");
        return View(model);
    }
}