using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.News;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components;

public partial class HomepageNewsViewComponent : NopViewComponent
{
    protected readonly INewsModelFactory _newsModelFactory;
    protected readonly NewsSettings _newsSettings;

    public HomepageNewsViewComponent(INewsModelFactory newsModelFactory, NewsSettings newsSettings)
    {
		logger.LogDebug(">>> Entering HomepageNewsViewComponent(newsModelFactory={NewsModelFactory},newsSettings={NewsSettings})", newsModelFactory, newsSettings);
        _newsModelFactory = newsModelFactory;
        _newsSettings = newsSettings;
		logger.LogDebug("<<< Exiting HomepageNewsViewComponent(newsModelFactory={NewsModelFactory},newsSettings={NewsSettings})", newsModelFactory, newsSettings);
		logger.LogDebug("HomepageNewsViewComponent(newsModelFactory={NewsModelFactory},newsSettings={NewsSettings}): _newsSettings → {_newsSettings}", newsModelFactory, newsSettings, _newsSettings);
		logger.LogDebug("HomepageNewsViewComponent(newsModelFactory={NewsModelFactory},newsSettings={NewsSettings}): _newsModelFactory → {_newsModelFactory}", newsModelFactory, newsSettings, _newsModelFactory);
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
		logger.LogDebug(">>> Entering InvokeAsync()");
        if (!_newsSettings.Enabled || !_newsSettings.ShowNewsOnMainPage)
            return Content("");

        var model = await _newsModelFactory.PrepareHomepageNewsItemsModelAsync();
		logger.LogDebug("InvokeAsync(): model → {Model}", model);
		logger.LogDebug("<<< Exiting InvokeAsync()");
        return View(model);
    }
}
