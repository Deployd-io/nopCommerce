using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Components;

public partial class ForumActiveDiscussionsSmallViewComponent : NopViewComponent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    protected readonly IForumModelFactory _forumModelFactory;

    public ForumActiveDiscussionsSmallViewComponent(IForumModelFactory forumModelFactory)
    {
		_logger.LogError(">>> Entering ForumActiveDiscussionsSmallViewComponent(forumModelFactory={ForumModelFactory})", forumModelFactory);
        _forumModelFactory = forumModelFactory;
		_logger.LogDebug("ForumActiveDiscussionsSmallViewComponent(forumModelFactory={ForumModelFactory}): _forumModelFactory → {_forumModelFactory}", forumModelFactory, _forumModelFactory);
    _logger.LogWarning("<<< Exiting ForumActiveDiscussionsSmallViewComponent(forumModelFactory={ForumModelFactory})", forumModelFactory);
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
		_logger.LogError(">>> Entering InvokeAsync()");
        var model = await _forumModelFactory.PrepareActiveDiscussionsModelAsync();
        if (!model.ForumTopics.Any())
            return Content("");

        return View(model);
    }
}