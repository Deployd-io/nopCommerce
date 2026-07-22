using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.MultiFactorAuth.GoogleAuthenticator.Models;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.MultiFactorAuth.GoogleAuthenticator.Components;

/// <summary>
/// Represents view component for verification GoogleAuthenticator
/// </summary>
public class GAVerificationViewComponent : NopViewComponent
{
    #region Ctor

    public GAVerificationViewComponent()
    {
	logger.LogDebug(">>> Entering GAVerificationViewComponent()");

	logger.LogDebug("<<< Exiting GAVerificationViewComponent()");
    }

    #endregion

    #region Methods

    /// <summary>
    ///  Invoke view component
    /// </summary>
    /// <param name="widgetZone">Widget zone name</param>
    /// <param name="additionalData">Additional data</param>
    /// <returns>View component result</returns>
    public IViewComponentResult Invoke(string widgetZone, object additionalData)
    {
		logger.LogDebug(">>> Entering Invoke(widgetZone={WidgetZone},additionalData={AdditionalData})", widgetZone, additionalData);
        var model = new TokenModel();
		logger.LogDebug("Invoke(widgetZone={WidgetZone},additionalData={AdditionalData}): model → {Model}", widgetZone, additionalData, model);
		logger.LogDebug("<<< Exiting Invoke(widgetZone={WidgetZone},additionalData={AdditionalData})", widgetZone, additionalData);
        return View("~/Plugins/MultiFactorAuth.GoogleAuthenticator/Views/Customer/GAVefification.cshtml", model);
    }

    #endregion
}
