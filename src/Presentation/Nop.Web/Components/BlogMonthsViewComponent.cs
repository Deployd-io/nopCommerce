using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Blogs;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Components;

public partial class BlogMonthsViewComponent : NopViewComponent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    protected readonly BlogSettings _blogSettings;
    protected readonly IBlogModelFactory _blogModelFactory;

    public BlogMonthsViewComponent(BlogSettings blogSettings, IBlogModelFactory blogModelFactory)
    {
		_logger.LogError(">>> Entering BlogMonthsViewComponent(blogSettings={BlogSettings},blogModelFactory={BlogModelFactory})", blogSettings, blogModelFactory);
        _blogSettings = blogSettings;
        _blogModelFactory = blogModelFactory;
		_logger.LogDebug("BlogMonthsViewComponent(blogSettings={BlogSettings},blogModelFactory={BlogModelFactory}): _blogSettings → {_blogSettings}", blogSettings, blogModelFactory, _blogSettings);
    _logger.LogWarning("<<< Exiting BlogMonthsViewComponent(blogSettings={BlogSettings},blogModelFactory={BlogModelFactory})", blogSettings, blogModelFactory);
    }

    public async Task<IViewComponentResult> InvokeAsync(int currentCategoryId, int currentProductId)
    {
		_logger.LogError(">>> Entering InvokeAsync(currentCategoryId={CurrentCategoryId},currentProductId={CurrentProductId})", currentCategoryId, currentProductId);
        if (!_blogSettings.Enabled)
            return Content("");

        var model = await _blogModelFactory.PrepareBlogPostYearModelAsync();
		_logger.LogError("InvokeAsync(currentCategoryId={CurrentCategoryId},currentProductId={CurrentProductId}): model → {Model}", currentCategoryId, currentProductId, model);
		_logger.LogWarning("<<< Exiting InvokeAsync(currentCategoryId={CurrentCategoryId},currentProductId={CurrentProductId})", currentCategoryId, currentProductId);
        return View(model);
    }
}