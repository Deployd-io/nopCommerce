using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Nop.Plugin.Payments.PayPalCommerce.Services;
using Nop.Web.Framework.Components;
using Nop.Web.Framework.Infrastructure;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Payments.PayPalCommerce.Components.Public;

/// <summary>
/// Represents the view component to display PayPal logo in the public store
/// </summary>
public class LogoViewComponent : NopViewComponent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    private readonly PayPalCommerceSettings _settings;
    private readonly PayPalCommerceServiceManager _serviceManager;

    #endregion

    #region Ctor

    public LogoViewComponent(PayPalCommerceSettings settings,
        PayPalCommerceServiceManager serviceManager)
    {
		_logger.LogDebug(">>> Entering LogoViewComponent(settings={Settings},serviceManager={ServiceManager})", settings, serviceManager);
        _settings = settings;
        _serviceManager = serviceManager;
		_logger.LogDebug("<<< Exiting LogoViewComponent(settings={Settings},serviceManager={ServiceManager})", settings, serviceManager);
		_logger.LogDebug("LogoViewComponent(settings={Settings},serviceManager={ServiceManager}): _serviceManager → {_serviceManager}", settings, serviceManager, _serviceManager);
		_logger.LogDebug("LogoViewComponent(settings={Settings},serviceManager={ServiceManager}): _settings → {_settings}", settings, serviceManager, _settings);
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
    public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
    {
		_logger.LogDebug(">>> Entering InvokeAsync(widgetZone={WidgetZone},additionalData={AdditionalData})", widgetZone, additionalData);
        var (active, _) = await _serviceManager.IsActiveAsync(_settings);
        if (!active)
            return Content(string.Empty);

        var script = widgetZone.Equals(PublicWidgetZones.HeaderLinksBefore) && _settings.DisplayLogoInHeaderLinks
            ? _settings.LogoInHeaderLinks
            : widgetZone.Equals(PublicWidgetZones.Footer) && _settings.DisplayLogoInFooter
            ? _settings.LogoInFooter
            : null;

		_logger.LogDebug("<<< Exiting InvokeAsync(widgetZone={WidgetZone},additionalData={AdditionalData})", widgetZone, additionalData);
        return new HtmlContentViewComponentResult(new HtmlString(script ?? string.Empty));
    }

    #endregion
}