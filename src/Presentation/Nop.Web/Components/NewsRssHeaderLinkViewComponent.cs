using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.News;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components;

public partial class NewsRssHeaderLinkViewComponent : NopViewComponent
{
    protected readonly NewsSettings _newsSettings;

    public NewsRssHeaderLinkViewComponent(NewsSettings newsSettings)
    {
		logger.LogDebug(">>> Entering NewsRssHeaderLinkViewComponent(newsSettings={NewsSettings})", newsSettings);
        _newsSettings = newsSettings;
		logger.LogDebug("<<< Exiting NewsRssHeaderLinkViewComponent(newsSettings={NewsSettings})", newsSettings);
		logger.LogDebug("NewsRssHeaderLinkViewComponent(newsSettings={NewsSettings}): _newsSettings → {_newsSettings}", newsSettings, _newsSettings);
    }

    public IViewComponentResult Invoke(int currentCategoryId, int currentProductId)
    {
		logger.LogDebug(">>> Entering Invoke(currentCategoryId={CurrentCategoryId},currentProductId={CurrentProductId})", currentCategoryId, currentProductId);
        if (!_newsSettings.Enabled || !_newsSettings.ShowHeaderRssUrl)
		logger.LogDebug("<<< Exiting Invoke(currentCategoryId={CurrentCategoryId},currentProductId={CurrentProductId})", currentCategoryId, currentProductId);
            return Content("");

        return View();
    }
}