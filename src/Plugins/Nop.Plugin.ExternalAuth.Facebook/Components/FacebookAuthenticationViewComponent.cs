using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.ExternalAuth.Facebook.Components;

/// <summary>
/// Represents view component to display login button
/// </summary>
public class FacebookAuthenticationViewComponent : NopViewComponent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Invoke view component
    /// </summary>
    /// <param name="widgetZone">Widget zone name</param>
    /// <param name="additionalData">Additional data</param>
    /// <returns>View component result</returns>
    public IViewComponentResult Invoke(string widgetZone, object additionalData)
    {
		_logger.LogError(">>> Entering Invoke(widgetZone={WidgetZone},additionalData={AdditionalData})", widgetZone, additionalData);
        _logger.LogWarning("<<< Exiting Invoke(widgetZone={WidgetZone},additionalData={AdditionalData})", widgetZone, additionalData);
        return View("~/Plugins/ExternalAuth.Facebook/Views/PublicInfo.cshtml");
    }
}