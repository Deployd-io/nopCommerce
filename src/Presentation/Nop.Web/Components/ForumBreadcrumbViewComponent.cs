using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components;

public partial class ForumBreadcrumbViewComponent : NopViewComponent
{
    protected readonly IForumModelFactory _forumModelFactory;

    public ForumBreadcrumbViewComponent(IForumModelFactory forumModelFactory)
    {
		logger.LogDebug(">>> Entering ForumBreadcrumbViewComponent(forumModelFactory={ForumModelFactory})", forumModelFactory);
        _forumModelFactory = forumModelFactory;
		logger.LogDebug("<<< Exiting ForumBreadcrumbViewComponent(forumModelFactory={ForumModelFactory})", forumModelFactory);
		logger.LogDebug("ForumBreadcrumbViewComponent(forumModelFactory={ForumModelFactory}): _forumModelFactory → {_forumModelFactory}", forumModelFactory, _forumModelFactory);
    }

    public async Task<IViewComponentResult> InvokeAsync(int? forumGroupId, int? forumId, int? forumTopicId)
    {
		logger.LogDebug(">>> Entering InvokeAsync(forumGroupId={ForumGroupId},forumId={ForumId},forumTopicId={ForumTopicId})", forumGroupId, forumId, forumTopicId);
        var model = await _forumModelFactory.PrepareForumBreadcrumbModelAsync(forumGroupId, forumId, forumTopicId);
		logger.LogDebug("InvokeAsync(forumGroupId={ForumGroupId},forumId={ForumId},forumTopicId={ForumTopicId}): model → {Model}", forumGroupId, forumId, forumTopicId, model);
		logger.LogDebug("<<< Exiting InvokeAsync(forumGroupId={ForumGroupId},forumId={ForumId},forumTopicId={ForumTopicId})", forumGroupId, forumId, forumTopicId);
        return View(model);
    }
}
