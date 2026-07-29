using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.ExternalAuth.Facebook.Components;

/// <summary>
/// Represents view component to display login button
/// </summary>
public class FacebookAuthenticationViewComponent : NopViewComponent
{
    /// <summary>
    /// Invoke view component
    /// </summary>
    /// <param name="widgetZone">Widget zone name</param>
    /// <param name="additionalData">Additional data</param>
    /// <returns>View component result</returns>
    public IViewComponentResult Invoke(string widgetZone, object additionalData)
    {
		logger.LogDebug(">>> Entering Invoke(widgetZone={WidgetZone},additionalData={AdditionalData})", widgetZone, additionalData);
		logger.LogDebug("<<< Exiting Invoke(widgetZone={WidgetZone},additionalData={AdditionalData})", widgetZone, additionalData);
        return View("~/Plugins/ExternalAuth.Facebook/Views/PublicInfo.cshtml");
    }
}