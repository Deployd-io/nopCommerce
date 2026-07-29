using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Blogs;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components;

public partial class BlogMonthsViewComponent : NopViewComponent
{
    protected readonly BlogSettings _blogSettings;
    protected readonly IBlogModelFactory _blogModelFactory;

    public BlogMonthsViewComponent(BlogSettings blogSettings, IBlogModelFactory blogModelFactory)
    {
		logger.LogDebug(">>> Entering BlogMonthsViewComponent(blogSettings={BlogSettings},blogModelFactory={BlogModelFactory})", blogSettings, blogModelFactory);
        _blogSettings = blogSettings;
        _blogModelFactory = blogModelFactory;
		logger.LogDebug("<<< Exiting BlogMonthsViewComponent(blogSettings={BlogSettings},blogModelFactory={BlogModelFactory})", blogSettings, blogModelFactory);
		logger.LogDebug("BlogMonthsViewComponent(blogSettings={BlogSettings},blogModelFactory={BlogModelFactory}): _blogModelFactory → {_blogModelFactory}", blogSettings, blogModelFactory, _blogModelFactory);
		logger.LogDebug("BlogMonthsViewComponent(blogSettings={BlogSettings},blogModelFactory={BlogModelFactory}): _blogSettings → {_blogSettings}", blogSettings, blogModelFactory, _blogSettings);
    }

    public async Task<IViewComponentResult> InvokeAsync(int currentCategoryId, int currentProductId)
    {
		logger.LogDebug(">>> Entering InvokeAsync(currentCategoryId={CurrentCategoryId},currentProductId={CurrentProductId})", currentCategoryId, currentProductId);
        if (!_blogSettings.Enabled)
            return Content("");

        var model = await _blogModelFactory.PrepareBlogPostYearModelAsync();
		logger.LogDebug("InvokeAsync(currentCategoryId={CurrentCategoryId},currentProductId={CurrentProductId}): model → {Model}", currentCategoryId, currentProductId, model);
		logger.LogDebug("<<< Exiting InvokeAsync(currentCategoryId={CurrentCategoryId},currentProductId={CurrentProductId})", currentCategoryId, currentProductId);
        return View(model);
    }
}