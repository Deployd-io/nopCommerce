using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;
using Nop.Web.Framework.Factories;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Components;

/// <summary>
/// Represents a view component that displays an admin widgets
/// </summary>
public partial class AdminWidgetViewComponent : NopViewComponent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IWidgetModelFactory _widgetModelFactory;

    #endregion

    #region Ctor

    public AdminWidgetViewComponent(IWidgetModelFactory widgetModelFactory)
    {
		_logger.LogDebug(">>> Entering AdminWidgetViewComponent(widgetModelFactory={WidgetModelFactory})", widgetModelFactory);
        _widgetModelFactory = widgetModelFactory;
		_logger.LogDebug("AdminWidgetViewComponent(widgetModelFactory={WidgetModelFactory}): _widgetModelFactory → {_widgetModelFactory}", widgetModelFactory, _widgetModelFactory);
		_logger.LogDebug("<<< Exiting AdminWidgetViewComponent(widgetModelFactory={WidgetModelFactory})", widgetModelFactory);
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
		_logger.LogDebug(">>> Entering InvokeAsync(widgetZone={WidgetZone},additionalData={AdditionalData})", widgetZone, additionalData);
        //prepare model
        var models = await _widgetModelFactory.PrepareRenderWidgetModelAsync(widgetZone, additionalData, false);
		_logger.LogDebug("InvokeAsync(widgetZone={WidgetZone},additionalData={AdditionalData}): models → {Models}", widgetZone, additionalData, models);

        //no data?
        if (!models.Any())
            return Content(string.Empty);

        return View(models);
    }

    #endregion
}