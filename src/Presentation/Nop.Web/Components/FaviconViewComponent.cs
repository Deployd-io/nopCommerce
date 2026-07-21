using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components;

public partial class FaviconViewComponent : NopViewComponent
{
    protected readonly ICommonModelFactory _commonModelFactory;

    public FaviconViewComponent(ICommonModelFactory commonModelFactory)
    {
		logger.LogDebug(">>> Entering FaviconViewComponent(commonModelFactory={CommonModelFactory})", commonModelFactory);
        _commonModelFactory = commonModelFactory;
		logger.LogDebug("<<< Exiting FaviconViewComponent(commonModelFactory={CommonModelFactory})", commonModelFactory);
		logger.LogDebug("FaviconViewComponent(commonModelFactory={CommonModelFactory}): _commonModelFactory → {_commonModelFactory}", commonModelFactory, _commonModelFactory);
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
		logger.LogDebug(">>> Entering InvokeAsync()");
        var model = await _commonModelFactory.PrepareFaviconAndAppIconsModelAsync();
        if (string.IsNullOrEmpty(model.HeadCode))
			logger.LogDebug("InvokeAsync(): model → {Model}", model);
            return Content("");
        return View(model);
    }
}
