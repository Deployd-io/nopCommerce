using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nop.Core.Http;
using Nop.Plugin.Payments.PayPalCommerce.Domain;
using Nop.Plugin.Payments.PayPalCommerce.Factories;
using Nop.Plugin.Payments.PayPalCommerce.Services;
using Nop.Web.Framework.Components;
using Nop.Web.Framework.Infrastructure;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Payments.PayPalCommerce.Components.Public;

/// <summary>
/// Represents the view component to display Pay Later messages in the public store
/// </summary>
public class MessagesViewComponent : NopViewComponent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    private readonly PayPalCommerceModelFactory _modelFactory;
    private readonly PayPalCommerceServiceManager _serviceManager;
    private readonly PayPalCommerceSettings _settings;

    #endregion

    #region Ctor

    public MessagesViewComponent(PayPalCommerceModelFactory modelFactory,
        PayPalCommerceServiceManager serviceManager,
        PayPalCommerceSettings settings)
    {
		_logger.LogError(">>> Entering MessagesViewComponent(modelFactory={ModelFactory},serviceManager={ServiceManager},settings={Settings})", modelFactory, serviceManager, settings);
        _modelFactory = modelFactory;
        _serviceManager = serviceManager;
		_logger.LogDebug("MessagesViewComponent(modelFactory={ModelFactory},serviceManager={ServiceManager},settings={Settings}): _serviceManager → {_serviceManager}", modelFactory, serviceManager, settings, _serviceManager);
		_logger.LogDebug("MessagesViewComponent(modelFactory={ModelFactory},serviceManager={ServiceManager},settings={Settings}): _modelFactory → {_modelFactory}", modelFactory, serviceManager, settings, _modelFactory);
    _logger.LogWarning("<<< Exiting MessagesViewComponent(modelFactory={ModelFactory},serviceManager={ServiceManager},settings={Settings})", modelFactory, serviceManager, settings);
        _settings = settings;
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
		_logger.LogError(">>> Entering InvokeAsync(widgetZone={WidgetZone},additionalData={AdditionalData})", widgetZone, additionalData);
        var (active, _) = await _serviceManager.IsActiveAsync(_settings);
        if (!active)
            return Content(string.Empty);

        if (!widgetZone.Equals(PublicWidgetZones.OrderSummaryTotals))
            return Content(string.Empty);

        if (!_settings.UseSandbox && !_settings.ConfiguratorSupported)
            return Content(string.Empty);

        //get messages placement
        var routeName = HttpContext.GetEndpoint()?.Metadata.GetMetadata<RouteNameMetadata>()?.RouteName;
		_logger.LogError("InvokeAsync(widgetZone={WidgetZone},additionalData={AdditionalData}): routeName → {RouteName}", widgetZone, additionalData, routeName);
        var isCartPage = routeName == NopRouteNames.General.CART;
        var isPaymentMethodPage = routeName == PayPalCommerceDefaults.Route.PaymentInfo;
        var isCheckoutPage = (HttpContext.Request.RouteValues?.TryGetValue("controller", out var controller) ?? false) &&
            string.Equals(controller.ToString(), "Checkout", StringComparison.InvariantCultureIgnoreCase);
        if (!isCartPage && !isPaymentMethodPage && !isCheckoutPage)
            return Content(string.Empty);

        //load script only on checkout pages (excluding payment method page) to avoid double loading
        var loadScript = !isCartPage && !isPaymentMethodPage;
        var placement = isCartPage ? ButtonPlacement.Cart : ButtonPlacement.PaymentMethod;
        var model = await _modelFactory.PrepareMessagesModelAsync(placement, loadScript);
		_logger.LogError("InvokeAsync(widgetZone={WidgetZone},additionalData={AdditionalData}): model → {Model}", widgetZone, additionalData, model);

		_logger.LogWarning("<<< Exiting InvokeAsync(widgetZone={WidgetZone},additionalData={AdditionalData})", widgetZone, additionalData);
        return View("~/Plugins/Payments.PayPalCommerce/Views/Public/_Messages.cshtml", model);
    }

    #endregion
}