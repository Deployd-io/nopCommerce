using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Blogs;
using Nop.Web.Framework.Components;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Components;

public partial class BlogRssHeaderLinkViewComponent : NopViewComponent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    protected readonly BlogSettings _blogSettings;

    public BlogRssHeaderLinkViewComponent(BlogSettings blogSettings)
    {
		_logger.LogDebug(">>> Entering BlogRssHeaderLinkViewComponent(blogSettings={BlogSettings})", blogSettings);
        _blogSettings = blogSettings;
		_logger.LogDebug("BlogRssHeaderLinkViewComponent(blogSettings={BlogSettings}): _blogSettings → {_blogSettings}", blogSettings, _blogSettings);
    }

    public IViewComponentResult Invoke(int currentCategoryId, int currentProductId)
    {
		_logger.LogDebug(">>> Entering Invoke(currentCategoryId={CurrentCategoryId},currentProductId={CurrentProductId})", currentCategoryId, currentProductId);
        if (!_blogSettings.Enabled || !_blogSettings.ShowHeaderRssUrl)
            return Content("");

        return View();
    }
}