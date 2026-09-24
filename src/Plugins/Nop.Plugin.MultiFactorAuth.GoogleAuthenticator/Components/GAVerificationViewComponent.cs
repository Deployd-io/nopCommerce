using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.MultiFactorAuth.GoogleAuthenticator.Models;
using Nop.Web.Framework.Components;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.MultiFactorAuth.GoogleAuthenticator.Components;

/// <summary>
/// Represents view component for verification GoogleAuthenticator
/// </summary>
public class GAVerificationViewComponent : NopViewComponent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public GAVerificationViewComponent()
    {
	_logger.LogError(">>> Entering GAVerificationViewComponent()");

	_logger.LogWarning("<<< Exiting GAVerificationViewComponent()");
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
		_logger.LogError(">>> Entering Invoke(widgetZone={WidgetZone},additionalData={AdditionalData})", widgetZone, additionalData);
        var model = new TokenModel();
		_logger.LogError("Invoke(widgetZone={WidgetZone},additionalData={AdditionalData}): model → {Model}", widgetZone, additionalData, model);
		_logger.LogWarning("<<< Exiting Invoke(widgetZone={WidgetZone},additionalData={AdditionalData})", widgetZone, additionalData);
        return View("~/Plugins/MultiFactorAuth.GoogleAuthenticator/Views/Customer/GAVefification.cshtml", model);
    }

    #endregion
}