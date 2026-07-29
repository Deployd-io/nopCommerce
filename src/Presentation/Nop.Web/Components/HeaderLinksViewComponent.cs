using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components;

public partial class HeaderLinksViewComponent : NopViewComponent
{
    protected readonly ICommonModelFactory _commonModelFactory;

    public HeaderLinksViewComponent(ICommonModelFactory commonModelFactory)
    {
		logger.LogDebug(">>> Entering HeaderLinksViewComponent(commonModelFactory={CommonModelFactory})", commonModelFactory);
        _commonModelFactory = commonModelFactory;
		logger.LogDebug("<<< Exiting HeaderLinksViewComponent(commonModelFactory={CommonModelFactory})", commonModelFactory);
		logger.LogDebug("HeaderLinksViewComponent(commonModelFactory={CommonModelFactory}): _commonModelFactory → {_commonModelFactory}", commonModelFactory, _commonModelFactory);
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
		logger.LogDebug(">>> Entering InvokeAsync()");
        var model = await _commonModelFactory.PrepareHeaderLinksModelAsync();
		logger.LogDebug("InvokeAsync(): model → {Model}", model);
		logger.LogDebug("<<< Exiting InvokeAsync()");
        return View(model);
    }
}