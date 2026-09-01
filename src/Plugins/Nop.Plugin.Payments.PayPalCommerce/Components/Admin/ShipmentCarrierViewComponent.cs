using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Payments.PayPalCommerce.Models.Admin;
using Nop.Plugin.Payments.PayPalCommerce.Services;
using Nop.Services.Common;
using Nop.Services.Shipping;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Framework.Components;
using Nop.Web.Framework.Infrastructure;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Payments.PayPalCommerce.Components.Admin;

/// <summary>
/// Represents the view component to render an additional input on the shipment details page in the admin area
/// </summary>
public class ShipmentCarrierViewComponent : NopViewComponent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    private readonly IGenericAttributeService _genericAttributeService;
    private readonly IShipmentService _shipmentService;
    private readonly PayPalCommerceServiceManager _serviceManager;
    private readonly PayPalCommerceSettings _settings;

    #endregion

    #region Ctor

    public ShipmentCarrierViewComponent(IGenericAttributeService genericAttributeService,
        IShipmentService shipmentService,
        PayPalCommerceServiceManager serviceManager,
        PayPalCommerceSettings settings)
    {
		_logger.LogDebug(">>> Entering ShipmentCarrierViewComponent(genericAttributeService={GenericAttributeService},shipmentService={ShipmentService},serviceManager={ServiceManager},settings={Settings})", genericAttributeService, shipmentService, serviceManager, settings);
        _genericAttributeService = genericAttributeService;
		_logger.LogDebug("ShipmentCarrierViewComponent(genericAttributeService={GenericAttributeService},shipmentService={ShipmentService},serviceManager={ServiceManager},settings={Settings}): _genericAttributeService → {_genericAttributeService}", genericAttributeService, shipmentService, serviceManager, settings, _genericAttributeService);
        _shipmentService = shipmentService;
		_logger.LogDebug("ShipmentCarrierViewComponent(genericAttributeService={GenericAttributeService},shipmentService={ShipmentService},serviceManager={ServiceManager},settings={Settings}): _shipmentService → {_shipmentService}", genericAttributeService, shipmentService, serviceManager, settings, _shipmentService);
        _serviceManager = serviceManager;
		_logger.LogDebug("ShipmentCarrierViewComponent(genericAttributeService={GenericAttributeService},shipmentService={ShipmentService},serviceManager={ServiceManager},settings={Settings}): _serviceManager → {_serviceManager}", genericAttributeService, shipmentService, serviceManager, settings, _serviceManager);
        _settings = settings;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Invoke the widget view component
    /// </summary>
    /// <param name="widgetZone">Widget zone</param>
    /// <param name="additionalData">Additional parameters</param>
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

        if (!_settings.UseShipmentTracking)
            return Content(string.Empty);

        if (!widgetZone.Equals(AdminWidgetZones.OrderShipmentDetailsButtons) && !widgetZone.Equals(AdminWidgetZones.OrderShipmentAddButtons))
            return Content(string.Empty);

        if (additionalData is not ShipmentModel shipmentModel)
            return Content(string.Empty);

        var shipment = await _shipmentService.GetShipmentByIdAsync(shipmentModel.Id);
		_logger.LogDebug("InvokeAsync(widgetZone={WidgetZone},additionalData={AdditionalData}): shipment → {Shipment}", widgetZone, additionalData, shipment);
        var model = new ShipmentCarrierModel
        {
            TrackingNumber = shipmentModel.TrackingNumber,
            PayPalCommerceShipmentCarrier = shipment is not null
                ? await _genericAttributeService.GetAttributeAsync<string>(shipment, PayPalCommerceDefaults.ShipmentCarrierAttribute)
                : null
        };

		_logger.LogDebug("<<< Exiting InvokeAsync(widgetZone={WidgetZone},additionalData={AdditionalData})", widgetZone, additionalData);
        return View("~/Plugins/Payments.PayPalCommerce/Views/Admin/_ShipmentCarrier.cshtml", model);
    }

    #endregion
}