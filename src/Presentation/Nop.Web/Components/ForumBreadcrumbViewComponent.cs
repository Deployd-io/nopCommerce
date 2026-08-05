using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Components;

public partial class ForumBreadcrumbViewComponent : NopViewComponent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    protected readonly IForumModelFactory _forumModelFactory;

    public ForumBreadcrumbViewComponent(IForumModelFactory forumModelFactory)
    {
		_logger.LogDebug(">>> Entering ForumBreadcrumbViewComponent(forumModelFactory={ForumModelFactory})", forumModelFactory);
        _forumModelFactory = forumModelFactory;
		_logger.LogDebug("<<< Exiting ForumBreadcrumbViewComponent(forumModelFactory={ForumModelFactory})", forumModelFactory);
		_logger.LogDebug("ForumBreadcrumbViewComponent(forumModelFactory={ForumModelFactory}): _forumModelFactory → {_forumModelFactory}", forumModelFactory, _forumModelFactory);
    }

    public async Task<IViewComponentResult> InvokeAsync(int? forumGroupId, int? forumId, int? forumTopicId)
    {
		_logger.LogDebug(">>> Entering InvokeAsync(forumGroupId={ForumGroupId},forumId={ForumId},forumTopicId={ForumTopicId})", forumGroupId, forumId, forumTopicId);
        var model = await _forumModelFactory.PrepareForumBreadcrumbModelAsync(forumGroupId, forumId, forumTopicId);
		_logger.LogDebug("InvokeAsync(forumGroupId={ForumGroupId},forumId={ForumId},forumTopicId={ForumTopicId}): model → {Model}", forumGroupId, forumId, forumTopicId, model);
		_logger.LogDebug("<<< Exiting InvokeAsync(forumGroupId={ForumGroupId},forumId={ForumId},forumTopicId={ForumTopicId})", forumGroupId, forumId, forumTopicId);
        return View(model);
    }
}