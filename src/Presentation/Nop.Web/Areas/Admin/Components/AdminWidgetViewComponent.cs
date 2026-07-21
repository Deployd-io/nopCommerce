using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;
using Nop.Web.Framework.Factories;

namespace Nop.Web.Areas.Admin.Components;

/// <summary>
/// Represents a view component that displays an admin widgets
/// </summary>
public partial class AdminWidgetViewComponent : NopViewComponent
{
    #region Fields

    protected readonly IWidgetModelFactory _widgetModelFactory;

    #endregion

    #region Ctor

    public AdminWidgetViewComponent(IWidgetModelFactory widgetModelFactory)
    {
		logger.LogDebug(">>> Entering AdminWidgetViewComponent(widgetModelFactory={WidgetModelFactory})", widgetModelFactory);
        _widgetModelFactory = widgetModelFactory;
		logger.LogDebug("<<< Exiting AdminWidgetViewComponent(widgetModelFactory={WidgetModelFactory})", widgetModelFactory);
		logger.LogDebug("AdminWidgetViewComponent(widgetModelFactory={WidgetModelFactory}): _widgetModelFactory → {_widgetModelFactory}", widgetModelFactory, _widgetModelFactory);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Invoke view component
    /// </summary>
    /// <param name="widgetZone">Widget zone name</param>
    /// <param name="additionalData">Additional data</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the view component result
    /// </returns>
    public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData = null)
    {
		logger.LogDebug(">>> Entering InvokeAsync(widgetZone={WidgetZone},additionalData={AdditionalData})", widgetZone, additionalData);
        //prepare model
        var models = await _widgetModelFactory.PrepareRenderWidgetModelAsync(widgetZone, additionalData, false);

		logger.LogDebug("InvokeAsync(widgetZone={WidgetZone},additionalData={AdditionalData}): models → {Models}", widgetZone, additionalData, models);
        //no data?
        if (!models.Any())
		logger.LogDebug("<<< Exiting InvokeAsync(widgetZone={WidgetZone},additionalData={AdditionalData})", widgetZone, additionalData);
            return Content(string.Empty);

        return View(models);
    }

    #endregion
}
