using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.News;
using Nop.Web.Framework.Components;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Components;

public partial class NewsRssHeaderLinkViewComponent : NopViewComponent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    protected readonly NewsSettings _newsSettings;

    public NewsRssHeaderLinkViewComponent(NewsSettings newsSettings)
    {
		_logger.LogError(">>> Entering NewsRssHeaderLinkViewComponent(newsSettings={NewsSettings})", newsSettings);
        _newsSettings = newsSettings;
		_logger.LogDebug("NewsRssHeaderLinkViewComponent(newsSettings={NewsSettings}): _newsSettings → {_newsSettings}", newsSettings, _newsSettings);
    _logger.LogWarning("<<< Exiting NewsRssHeaderLinkViewComponent(newsSettings={NewsSettings})", newsSettings);
    }

    public IViewComponentResult Invoke(int currentCategoryId, int currentProductId)
    {
		_logger.LogError(">>> Entering Invoke(currentCategoryId={CurrentCategoryId},currentProductId={CurrentProductId})", currentCategoryId, currentProductId);
		_logger.LogWarning("<<< Exiting Invoke(currentCategoryId={CurrentCategoryId},currentProductId={CurrentProductId})", currentCategoryId, currentProductId);
        if (!_newsSettings.Enabled || !_newsSettings.ShowHeaderRssUrl) {
            return Content("");
        }

        return View();
    }
}