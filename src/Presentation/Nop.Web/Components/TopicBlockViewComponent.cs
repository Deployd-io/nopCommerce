using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components;

public partial class TopicBlockViewComponent : NopViewComponent
{
    protected readonly ITopicModelFactory _topicModelFactory;

    public TopicBlockViewComponent(ITopicModelFactory topicModelFactory)
    {
		logger.LogDebug(">>> Entering TopicBlockViewComponent(topicModelFactory={TopicModelFactory})", topicModelFactory);
        _topicModelFactory = topicModelFactory;
		logger.LogDebug("<<< Exiting TopicBlockViewComponent(topicModelFactory={TopicModelFactory})", topicModelFactory);
		logger.LogDebug("TopicBlockViewComponent(topicModelFactory={TopicModelFactory}): _topicModelFactory → {_topicModelFactory}", topicModelFactory, _topicModelFactory);
    }

    public async Task<IViewComponentResult> InvokeAsync(string systemName)
    {
		logger.LogDebug(">>> Entering InvokeAsync(systemName={SystemName})", systemName);
        var model = await _topicModelFactory.PrepareTopicModelBySystemNameAsync(systemName);
        if (model == null)
			logger.LogDebug("InvokeAsync(systemName={SystemName}): model → {Model}", systemName, model);
            return Content("");
        return View(model);
    }
}