using Microsoft.Extensions.Logging;
﻿using Nop.Core.Domain.Shipping;

namespace Nop.Services.Shipping.Pickup;

/// <summary>
/// Represents a response of getting pickup points
/// </summary>
public partial class GetPickupPointsResponse : BaseNopResult
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public GetPickupPointsResponse()
    {
		_logger.LogDebug(">>> Entering GetPickupPointsResponse()");
        PickupPoints = new List<PickupPoint>();
		_logger.LogDebug("<<< Exiting GetPickupPointsResponse()");
		_logger.LogDebug("GetPickupPointsResponse(): PickupPoints → {PickupPoints}", PickupPoints);
    }

    /// <summary>
    /// Gets or sets a list of pickup points
    /// </summary>
    public IList<PickupPoint> PickupPoints { get; set; }
}