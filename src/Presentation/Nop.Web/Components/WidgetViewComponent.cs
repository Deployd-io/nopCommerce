using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;
using Nop.Web.Framework.Factories;

namespace Nop.Web.Components;

public partial class WidgetViewComponent : NopViewComponent
{
    protected readonly IWidgetModelFactory _widgetModelFactory;

    public WidgetViewComponent(IWidgetModelFactory widgetModelFactory)
    {
		logger.LogDebug(">>> Entering WidgetViewComponent(widgetModelFactory={WidgetModelFactory})", widgetModelFactory);
        _widgetModelFactory = widgetModelFactory;
		logger.LogDebug("<<< Exiting WidgetViewComponent(widgetModelFactory={WidgetModelFactory})", widgetModelFactory);
		logger.LogDebug("WidgetViewComponent(widgetModelFactory={WidgetModelFactory}): _widgetModelFactory → {_widgetModelFactory}", widgetModelFactory, _widgetModelFactory);
    }

    public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData = null)
    {
		logger.LogDebug(">>> Entering InvokeAsync(widgetZone={WidgetZone},additionalData={AdditionalData})", widgetZone, additionalData);
        var model = await _widgetModelFactory.PrepareRenderWidgetModelAsync(widgetZone, additionalData);

		logger.LogDebug("InvokeAsync(widgetZone={WidgetZone},additionalData={AdditionalData}): model → {Model}", widgetZone, additionalData, model);
        //no data?
        if (!model.Any())
		logger.LogDebug("<<< Exiting InvokeAsync(widgetZone={WidgetZone},additionalData={AdditionalData})", widgetZone, additionalData);
            return Content("");

        return View(model);
    }
}