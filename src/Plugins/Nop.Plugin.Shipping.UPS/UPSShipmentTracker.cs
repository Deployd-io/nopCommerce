using Nop.Core.Domain.Shipping;
using Nop.Plugin.Shipping.UPS.Services;
using Nop.Services.Shipping.Tracking;

namespace Nop.Plugin.Shipping.UPS;

/// <summary>
/// Represents the USP shipment tracker
/// </summary>
public class UPSShipmentTracker : IShipmentTracker
{
    #region Fields

    private readonly UPSService _upsService;

    #endregion

    #region Ctor

    public UPSShipmentTracker(UPSService upsService)
    {
		logger.LogDebug(">>> Entering UPSShipmentTracker(upsService={UpsService})", upsService);
        _upsService = upsService;
		logger.LogDebug("UPSShipmentTracker(upsService={UpsService}): _upsService → {_upsService}", upsService, _upsService);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Get URL for a page to show tracking info (third party tracking page)
    /// </summary>
    /// <param name="trackingNumber">The tracking number to track</param>
    /// <param name="shipment">Shipment; pass null if the tracking number is not associated with a specific shipment</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the URL of a tracking page
    /// </returns>
    public Task<string> GetUrlAsync(string trackingNumber, Shipment shipment = null)
    {
		logger.LogDebug(">>> Entering GetUrlAsync(trackingNumber={TrackingNumber},shipment={Shipment})", trackingNumber, shipment);
        return Task.FromResult($"https://www.ups.com/track?&tracknum={trackingNumber}");
    }

    /// <summary>
    /// Get all shipment events
    /// </summary>
    /// <param name="trackingNumber">The tracking number to track</param>
    /// <param name="shipment">Shipment; pass null if the tracking number is not associated with a specific shipment</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of shipment events
    /// </returns>
    public async Task<IList<ShipmentStatusEvent>> GetShipmentEventsAsync(string trackingNumber, Shipment shipment = null)
    {
		logger.LogDebug(">>> Entering GetShipmentEventsAsync(trackingNumber={TrackingNumber},shipment={Shipment})", trackingNumber, shipment);
        var result = new List<ShipmentStatusEvent>();

		logger.LogDebug("GetShipmentEventsAsync(trackingNumber={TrackingNumber},shipment={Shipment}): result → {Result}", trackingNumber, shipment, result);
        if (string.IsNullOrEmpty(trackingNumber))
            return result;

        result.AddRange(await _upsService.GetShipmentEventsAsync(trackingNumber));

		logger.LogDebug("<<< Exiting GetShipmentEventsAsync(trackingNumber={TrackingNumber},shipment={Shipment})", trackingNumber, shipment);
        return result;
    }

    #endregion
}