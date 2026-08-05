using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.MultiFactorAuth.GoogleAuthenticator.Factories;
using Nop.Plugin.MultiFactorAuth.GoogleAuthenticator.Models;
using Nop.Web.Framework.Components;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.MultiFactorAuth.GoogleAuthenticator.Components;

/// <summary>
/// Represents view component for setting GoogleAuthenticator
/// </summary>
public class GAAuthenticationViewComponent : NopViewComponent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly AuthenticationModelFactory _authenticationModelFactory;

    #endregion

    #region Ctor

    public GAAuthenticationViewComponent(AuthenticationModelFactory authenticationModelFactory)
    {
		_logger.LogDebug(">>> Entering GAAuthenticationViewComponent(authenticationModelFactory={AuthenticationModelFactory})", authenticationModelFactory);
        _authenticationModelFactory = authenticationModelFactory;
		_logger.LogDebug("<<< Exiting GAAuthenticationViewComponent(authenticationModelFactory={AuthenticationModelFactory})", authenticationModelFactory);
		_logger.LogDebug("GAAuthenticationViewComponent(authenticationModelFactory={AuthenticationModelFactory}): _authenticationModelFactory → {_authenticationModelFactory}", authenticationModelFactory, _authenticationModelFactory);
    }

    #endregion

    #region Methods

    /// <summary>
    ///  Invoke view component
    /// </summary>
    /// <param name="widgetZone">Widget zone name</param>
    /// <param name="additionalData">Additional data</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the view component result
    /// </returns>
    public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
    {
		_logger.LogDebug(">>> Entering InvokeAsync(widgetZone={WidgetZone},additionalData={AdditionalData})", widgetZone, additionalData);
        var model = new AuthModel();
        model = await _authenticationModelFactory.PrepareAuthModel(model);
		_logger.LogDebug("InvokeAsync(widgetZone={WidgetZone},additionalData={AdditionalData}): model → {Model}", widgetZone, additionalData, model);

		_logger.LogDebug("<<< Exiting InvokeAsync(widgetZone={WidgetZone},additionalData={AdditionalData})", widgetZone, additionalData);
        return View("~/Plugins/MultiFactorAuth.GoogleAuthenticator/Views/Customer/GAAuthentication.cshtml", model);
    }

    #endregion
}