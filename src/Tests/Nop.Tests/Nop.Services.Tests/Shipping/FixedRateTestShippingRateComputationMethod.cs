using Nop.Core.Domain.Shipping;
using Nop.Services.Plugins;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Tracking;

namespace Nop.Tests.Nop.Services.Tests.Shipping;

public class FixedRateTestShippingRateComputationMethod : BasePlugin, IShippingRateComputationMethod
{
    private decimal GetRate()
    {
		logger.LogDebug(">>> Entering GetRate()");
        var rate = 10M;
		logger.LogDebug("<<< Exiting GetRate()");
        return rate;
    }

    /// <summary>
    ///  Gets available shipping options
    /// </summary>
    /// <param name="getShippingOptionRequest">A request for getting shipping options</param>
    /// <returns>Represents a response of getting shipping rate options</returns>
    public Task<GetShippingOptionResponse> GetShippingOptionsAsync(GetShippingOptionRequest getShippingOptionRequest)
    {
		logger.LogDebug(">>> Entering GetShippingOptionsAsync(getShippingOptionRequest={GetShippingOptionRequest})", getShippingOptionRequest);
        ArgumentNullException.ThrowIfNull(getShippingOptionRequest);

        var response = new GetShippingOptionResponse();
        response.ShippingOptions.Add(new ShippingOption
        {
		logger.LogDebug("GetShippingOptionsAsync(getShippingOptionRequest={GetShippingOptionRequest}): response → {Response}", getShippingOptionRequest, response);
            Name = "Shipping option 1",
            Description = string.Empty,
            Rate = GetRate()
        });
        response.ShippingOptions.Add(new ShippingOption
        {
            Name = "Shipping option 2",
            Description = string.Empty,
            Rate = GetRate()
        });

		logger.LogDebug("<<< Exiting GetShippingOptionsAsync(getShippingOptionRequest={GetShippingOptionRequest})", getShippingOptionRequest);
        return Task.FromResult(response);
    }

    /// <summary>
    /// Gets fixed shipping rate (if shipping rate computation method allows it and the rate can be calculated before checkout).
    /// </summary>
    /// <param name="getShippingOptionRequest">A request for getting shipping options</param>
    /// <returns>Fixed shipping rate; or null in case there's no fixed shipping rate</returns>
    public Task<decimal?> GetFixedRateAsync(GetShippingOptionRequest getShippingOptionRequest)
    {
		logger.LogDebug(">>> Entering GetFixedRateAsync(getShippingOptionRequest={GetShippingOptionRequest})", getShippingOptionRequest);
        ArgumentNullException.ThrowIfNull(getShippingOptionRequest);

		logger.LogDebug("<<< Exiting GetFixedRateAsync(getShippingOptionRequest={GetShippingOptionRequest})", getShippingOptionRequest);
        return Task.FromResult<decimal?>(GetRate());
    }

    /// <summary>
    /// Get associated shipment tracker
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the shipment tracker
    /// </returns>
    public Task<IShipmentTracker> GetShipmentTrackerAsync()
    {
		logger.LogDebug(">>> Entering GetShipmentTrackerAsync()");
		logger.LogDebug("<<< Exiting GetShipmentTrackerAsync()");
        return Task.FromResult<IShipmentTracker>(null);
    }
}