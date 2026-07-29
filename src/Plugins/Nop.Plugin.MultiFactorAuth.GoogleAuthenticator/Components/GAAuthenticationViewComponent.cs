using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.MultiFactorAuth.GoogleAuthenticator.Factories;
using Nop.Plugin.MultiFactorAuth.GoogleAuthenticator.Models;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.MultiFactorAuth.GoogleAuthenticator.Components;

/// <summary>
/// Represents view component for setting GoogleAuthenticator
/// </summary>
public class GAAuthenticationViewComponent : NopViewComponent
{
    #region Fields

    protected readonly AuthenticationModelFactory _authenticationModelFactory;

    #endregion

    #region Ctor

    public GAAuthenticationViewComponent(AuthenticationModelFactory authenticationModelFactory)
    {
		logger.LogDebug(">>> Entering GAAuthenticationViewComponent(authenticationModelFactory={AuthenticationModelFactory})", authenticationModelFactory);
        _authenticationModelFactory = authenticationModelFactory;
		logger.LogDebug("<<< Exiting GAAuthenticationViewComponent(authenticationModelFactory={AuthenticationModelFactory})", authenticationModelFactory);
		logger.LogDebug("GAAuthenticationViewComponent(authenticationModelFactory={AuthenticationModelFactory}): _authenticationModelFactory → {_authenticationModelFactory}", authenticationModelFactory, _authenticationModelFactory);
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
		logger.LogDebug(">>> Entering InvokeAsync(widgetZone={WidgetZone},additionalData={AdditionalData})", widgetZone, additionalData);
        var model = new AuthModel();
        model = await _authenticationModelFactory.PrepareAuthModel(model);
		logger.LogDebug("InvokeAsync(widgetZone={WidgetZone},additionalData={AdditionalData}): model → {Model}", widgetZone, additionalData, model);

		logger.LogDebug("<<< Exiting InvokeAsync(widgetZone={WidgetZone},additionalData={AdditionalData})", widgetZone, additionalData);
        return View("~/Plugins/MultiFactorAuth.GoogleAuthenticator/Views/Customer/GAAuthentication.cshtml", model);
    }

    #endregion
}