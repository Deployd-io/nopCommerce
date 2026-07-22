using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components;

public partial class ForumActiveDiscussionsSmallViewComponent : NopViewComponent
{
    protected readonly IForumModelFactory _forumModelFactory;

    public ForumActiveDiscussionsSmallViewComponent(IForumModelFactory forumModelFactory)
    {
		logger.LogDebug(">>> Entering ForumActiveDiscussionsSmallViewComponent(forumModelFactory={ForumModelFactory})", forumModelFactory);
        _forumModelFactory = forumModelFactory;
		logger.LogDebug("<<< Exiting ForumActiveDiscussionsSmallViewComponent(forumModelFactory={ForumModelFactory})", forumModelFactory);
		logger.LogDebug("ForumActiveDiscussionsSmallViewComponent(forumModelFactory={ForumModelFactory}): _forumModelFactory → {_forumModelFactory}", forumModelFactory, _forumModelFactory);
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
		logger.LogDebug(">>> Entering InvokeAsync()");
        var model = await _forumModelFactory.PrepareActiveDiscussionsModelAsync();
        if (!model.ForumTopics.Any())
			logger.LogDebug("InvokeAsync(): model → {Model}", model);
            return Content("");

        return View(model);
    }
}
