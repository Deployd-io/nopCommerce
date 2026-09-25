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
		_logger.LogError(">>> Entering HomepageNewsViewComponent(newsModelFactory={NewsModelFactory},newsSettings={NewsSettings})", newsModelFactory, newsSettings);
        _newsModelFactory = newsModelFactory;
        _newsSettings = newsSettings;
		_logger.LogDebug("HomepageNewsViewComponent(newsModelFactory={NewsModelFactory},newsSettings={NewsSettings}): _newsSettings → {_newsSettings}", newsModelFactory, newsSettings, _newsSettings);
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
		_logger.LogError(">>> Entering InvokeAsync()");
        if (!_newsSettings.Enabled || !_newsSettings.ShowNewsOnMainPage)
            return Content("");

        var model = await _newsModelFactory.PrepareHomepageNewsItemsModelAsync();
		_logger.LogError("InvokeAsync(): model → {Model}", model);
		_logger.LogWarning("<<< Exiting InvokeAsync()");
        return View(model);
    }
}