using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Payments.PayPalCommerce.Services;
using Nop.Web.Framework.Components;
using Nop.Web.Framework.Infrastructure;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Payments.PayPalCommerce.Components.Admin;

/// <summary>
/// Represents the view component to display PayPal on the payment methods page in the admin area
/// </summary>
public class PaymentMethodViewComponent : NopViewComponent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    private readonly PayPalCommerceSettings _settings;
    private readonly PayPalCommerceServiceManager _serviceManager;

    #endregion

    #region Ctor

    public PaymentMethodViewComponent(PayPalCommerceSettings settings,
        PayPalCommerceServiceManager serviceManager)
    {
        _settings = settings;
        _serviceManager = serviceManager;
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
        if (!widgetZone.Equals(AdminWidgetZones.PaymentMethodListTop))
            return Content(string.Empty);

        var (active, _) = await _serviceManager.IsActiveAsync(_settings);

		_logger.LogDebug("<<< Exiting InvokeAsync(widgetZone={WidgetZone},additionalData={AdditionalData})", widgetZone, additionalData);
        return View("~/Plugins/Payments.PayPalCommerce/Views/Admin/_PaymentMethod.cshtml", active);
    }

    #endregion
}