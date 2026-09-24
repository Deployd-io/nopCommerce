using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;
using Nop.Web.Framework.Factories;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Components;

public partial class WidgetViewComponent : NopViewComponent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    protected readonly IWidgetModelFactory _widgetModelFactory;

    public WidgetViewComponent(IWidgetModelFactory widgetModelFactory)
    {
		_logger.LogError(">>> Entering WidgetViewComponent(widgetModelFactory={WidgetModelFactory})", widgetModelFactory);
        _widgetModelFactory = widgetModelFactory;
		_logger.LogDebug("WidgetViewComponent(widgetModelFactory={WidgetModelFactory}): _widgetModelFactory → {_widgetModelFactory}", widgetModelFactory, _widgetModelFactory);
    _logger.LogWarning("<<< Exiting WidgetViewComponent(widgetModelFactory={WidgetModelFactory})", widgetModelFactory);
    }

    public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData = null)
    {
		_logger.LogError(">>> Entering InvokeAsync(widgetZone={WidgetZone},additionalData={AdditionalData})", widgetZone, additionalData);
        var model = await _widgetModelFactory.PrepareRenderWidgetModelAsync(widgetZone, additionalData);

		_logger.LogError("InvokeAsync(widgetZone={WidgetZone},additionalData={AdditionalData}): model → {Model}", widgetZone, additionalData, model);
        //no data?
        if (!model.Any())
            return Content("");

        return View(model);
    }
}