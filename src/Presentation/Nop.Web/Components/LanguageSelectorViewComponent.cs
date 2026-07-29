using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components;

public partial class LanguageSelectorViewComponent : NopViewComponent
{
    protected readonly ICommonModelFactory _commonModelFactory;

    public LanguageSelectorViewComponent(ICommonModelFactory commonModelFactory)
    {
		logger.LogDebug(">>> Entering LanguageSelectorViewComponent(commonModelFactory={CommonModelFactory})", commonModelFactory);
        _commonModelFactory = commonModelFactory;
		logger.LogDebug("<<< Exiting LanguageSelectorViewComponent(commonModelFactory={CommonModelFactory})", commonModelFactory);
		logger.LogDebug("LanguageSelectorViewComponent(commonModelFactory={CommonModelFactory}): _commonModelFactory → {_commonModelFactory}", commonModelFactory, _commonModelFactory);
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
		logger.LogDebug(">>> Entering InvokeAsync()");
        var model = await _commonModelFactory.PrepareLanguageSelectorModelAsync();

		logger.LogDebug("InvokeAsync(): model → {Model}", model);
        if (model.AvailableLanguages.Count == 1)
		logger.LogDebug("<<< Exiting InvokeAsync()");
            return Content("");

        return View(model);
    }
}