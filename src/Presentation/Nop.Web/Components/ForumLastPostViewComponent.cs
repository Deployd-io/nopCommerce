using Microsoft.AspNetCore.Mvc;
using Nop.Services.Forums;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components;

public partial class ForumLastPostViewComponent : NopViewComponent
{
    protected readonly IForumModelFactory _forumModelFactory;
    protected readonly IForumService _forumService;

    public ForumLastPostViewComponent(IForumModelFactory forumModelFactory, IForumService forumService)
    {
		logger.LogDebug(">>> Entering ForumLastPostViewComponent(forumModelFactory={ForumModelFactory},forumService={ForumService})", forumModelFactory, forumService);
        _forumModelFactory = forumModelFactory;
        _forumService = forumService;
		logger.LogDebug("<<< Exiting ForumLastPostViewComponent(forumModelFactory={ForumModelFactory},forumService={ForumService})", forumModelFactory, forumService);
		logger.LogDebug("ForumLastPostViewComponent(forumModelFactory={ForumModelFactory},forumService={ForumService}): _forumService → {_forumService}", forumModelFactory, forumService, _forumService);
		logger.LogDebug("ForumLastPostViewComponent(forumModelFactory={ForumModelFactory},forumService={ForumService}): _forumModelFactory → {_forumModelFactory}", forumModelFactory, forumService, _forumModelFactory);
    }

    public async Task<IViewComponentResult> InvokeAsync(int forumPostId, bool showTopic)
    {
		logger.LogDebug(">>> Entering InvokeAsync(forumPostId={ForumPostId},showTopic={ShowTopic})", forumPostId, showTopic);
        var forumPost = await _forumService.GetPostByIdAsync(forumPostId);
        var model = await _forumModelFactory.PrepareLastPostModelAsync(forumPost, showTopic);
		logger.LogDebug("InvokeAsync(forumPostId={ForumPostId},showTopic={ShowTopic}): forumPost → {ForumPost}", forumPostId, showTopic, forumPost);
		logger.LogDebug("InvokeAsync(forumPostId={ForumPostId},showTopic={ShowTopic}): model → {Model}", forumPostId, showTopic, model);

		logger.LogDebug("<<< Exiting InvokeAsync(forumPostId={ForumPostId},showTopic={ShowTopic})", forumPostId, showTopic);
        return View(model);
    }
}
