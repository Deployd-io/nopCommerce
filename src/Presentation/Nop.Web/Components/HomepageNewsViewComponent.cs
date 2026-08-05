using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.News;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Components;

public partial class HomepageNewsViewComponent : NopViewComponent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    protected readonly INewsModelFactory _newsModelFactory;
    protected readonly NewsSettings _newsSettings;

    public HomepageNewsViewComponent(INewsModelFactory newsModelFactory, NewsSettings newsSettings)
    {
		_logger.LogDebug(">>> Entering HomepageNewsViewComponent(newsModelFactory={NewsModelFactory},newsSettings={NewsSettings})", newsModelFactory, newsSettings);
        _newsModelFactory = newsModelFactory;
        _newsSettings = newsSettings;
		_logger.LogDebug("<<< Exiting HomepageNewsViewComponent(newsModelFactory={NewsModelFactory},newsSettings={NewsSettings})", newsModelFactory, newsSettings);
		_logger.LogDebug("HomepageNewsViewComponent(newsModelFactory={NewsModelFactory},newsSettings={NewsSettings}): _newsSettings → {_newsSettings}", newsModelFactory, newsSettings, _newsSettings);
		_logger.LogDebug("HomepageNewsViewComponent(newsModelFactory={NewsModelFactory},newsSettings={NewsSettings}): _newsModelFactory → {_newsModelFactory}", newsModelFactory, newsSettings, _newsModelFactory);
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
		_logger.LogDebug(">>> Entering InvokeAsync()");
        if (!_newsSettings.Enabled || !_newsSettings.ShowNewsOnMainPage)
            return Content("");

        var model = await _newsModelFactory.PrepareHomepageNewsItemsModelAsync();
		_logger.LogDebug("InvokeAsync(): model → {Model}", model);
		_logger.LogDebug("<<< Exiting InvokeAsync()");
        return View(model);
    }
}