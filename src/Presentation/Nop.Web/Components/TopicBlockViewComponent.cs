using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Components;

public partial class TopicBlockViewComponent : NopViewComponent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    protected readonly ITopicModelFactory _topicModelFactory;

    public TopicBlockViewComponent(ITopicModelFactory topicModelFactory)
    {
		_logger.LogDebug(">>> Entering TopicBlockViewComponent(topicModelFactory={TopicModelFactory})", topicModelFactory);
        _topicModelFactory = topicModelFactory;
		_logger.LogDebug("<<< Exiting TopicBlockViewComponent(topicModelFactory={TopicModelFactory})", topicModelFactory);
		_logger.LogDebug("TopicBlockViewComponent(topicModelFactory={TopicModelFactory}): _topicModelFactory → {_topicModelFactory}", topicModelFactory, _topicModelFactory);
    }

    public async Task<IViewComponentResult> InvokeAsync(string systemName)
    {
		_logger.LogDebug(">>> Entering InvokeAsync(systemName={SystemName})", systemName);
        var model = await _topicModelFactory.PrepareTopicModelBySystemNameAsync(systemName);
        if (model == null)
			_logger.LogDebug("InvokeAsync(systemName={SystemName}): model → {Model}", systemName, model);
            return Content("");
        return View(model);
    }
}