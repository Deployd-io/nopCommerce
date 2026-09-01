using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Blogs;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Components;

public partial class BlogTagsViewComponent : NopViewComponent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    protected readonly BlogSettings _blogSettings;
    protected readonly IBlogModelFactory _blogModelFactory;

    public BlogTagsViewComponent(BlogSettings blogSettings, IBlogModelFactory blogModelFactory)
    {
		_logger.LogDebug(">>> Entering BlogTagsViewComponent(blogSettings={BlogSettings},blogModelFactory={BlogModelFactory})", blogSettings, blogModelFactory);
        _blogSettings = blogSettings;
		_logger.LogDebug("BlogTagsViewComponent(blogSettings={BlogSettings},blogModelFactory={BlogModelFactory}): _blogSettings → {_blogSettings}", blogSettings, blogModelFactory, _blogSettings);
        _blogModelFactory = blogModelFactory;
    }

    public async Task<IViewComponentResult> InvokeAsync(int currentCategoryId, int currentProductId)
    {
		_logger.LogDebug(">>> Entering InvokeAsync(currentCategoryId={CurrentCategoryId},currentProductId={CurrentProductId})", currentCategoryId, currentProductId);
        if (!_blogSettings.Enabled)
            return Content("");

        var model = await _blogModelFactory.PrepareBlogPostTagListModelAsync();
		_logger.LogDebug("InvokeAsync(currentCategoryId={CurrentCategoryId},currentProductId={CurrentProductId}): model → {Model}", currentCategoryId, currentProductId, model);
		_logger.LogDebug("<<< Exiting InvokeAsync(currentCategoryId={CurrentCategoryId},currentProductId={CurrentProductId})", currentCategoryId, currentProductId);
        return View(model);
    }
}