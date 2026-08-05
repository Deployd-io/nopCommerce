using Microsoft.AspNetCore.Mvc;
using Nop.Services.Forums;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Components;

public partial class ForumLastPostViewComponent : NopViewComponent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    protected readonly IForumModelFactory _forumModelFactory;
    protected readonly IForumService _forumService;

    public ForumLastPostViewComponent(IForumModelFactory forumModelFactory, IForumService forumService)
    {
		_logger.LogDebug(">>> Entering ForumLastPostViewComponent(forumModelFactory={ForumModelFactory},forumService={ForumService})", forumModelFactory, forumService);
        _forumModelFactory = forumModelFactory;
        _forumService = forumService;
		_logger.LogDebug("<<< Exiting ForumLastPostViewComponent(forumModelFactory={ForumModelFactory},forumService={ForumService})", forumModelFactory, forumService);
		_logger.LogDebug("ForumLastPostViewComponent(forumModelFactory={ForumModelFactory},forumService={ForumService}): _forumService → {_forumService}", forumModelFactory, forumService, _forumService);
		_logger.LogDebug("ForumLastPostViewComponent(forumModelFactory={ForumModelFactory},forumService={ForumService}): _forumModelFactory → {_forumModelFactory}", forumModelFactory, forumService, _forumModelFactory);
    }

    public async Task<IViewComponentResult> InvokeAsync(int forumPostId, bool showTopic)
    {
		_logger.LogDebug(">>> Entering InvokeAsync(forumPostId={ForumPostId},showTopic={ShowTopic})", forumPostId, showTopic);
        var forumPost = await _forumService.GetPostByIdAsync(forumPostId);
        var model = await _forumModelFactory.PrepareLastPostModelAsync(forumPost, showTopic);
		_logger.LogDebug("InvokeAsync(forumPostId={ForumPostId},showTopic={ShowTopic}): forumPost → {ForumPost}", forumPostId, showTopic, forumPost);
		_logger.LogDebug("InvokeAsync(forumPostId={ForumPostId},showTopic={ShowTopic}): model → {Model}", forumPostId, showTopic, model);

		_logger.LogDebug("<<< Exiting InvokeAsync(forumPostId={ForumPostId},showTopic={ShowTopic})", forumPostId, showTopic);
        return View(model);
    }
}