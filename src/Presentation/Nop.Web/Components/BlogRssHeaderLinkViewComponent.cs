using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Blogs;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components;

public partial class BlogRssHeaderLinkViewComponent : NopViewComponent
{
    protected readonly BlogSettings _blogSettings;

    public BlogRssHeaderLinkViewComponent(BlogSettings blogSettings)
    {
		logger.LogDebug(">>> Entering BlogRssHeaderLinkViewComponent(blogSettings={BlogSettings})", blogSettings);
        _blogSettings = blogSettings;
		logger.LogDebug("<<< Exiting BlogRssHeaderLinkViewComponent(blogSettings={BlogSettings})", blogSettings);
		logger.LogDebug("BlogRssHeaderLinkViewComponent(blogSettings={BlogSettings}): _blogSettings → {_blogSettings}", blogSettings, _blogSettings);
    }

    public IViewComponentResult Invoke(int currentCategoryId, int currentProductId)
    {
		logger.LogDebug(">>> Entering Invoke(currentCategoryId={CurrentCategoryId},currentProductId={CurrentProductId})", currentCategoryId, currentProductId);
        if (!_blogSettings.Enabled || !_blogSettings.ShowHeaderRssUrl)
		logger.LogDebug("<<< Exiting Invoke(currentCategoryId={CurrentCategoryId},currentProductId={CurrentProductId})", currentCategoryId, currentProductId);
            return Content("");

        return View();
    }
}