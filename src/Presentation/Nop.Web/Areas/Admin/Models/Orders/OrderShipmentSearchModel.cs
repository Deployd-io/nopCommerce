using Microsoft.Extensions.Logging;
﻿using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Orders;

/// <summary>
/// Represents an order shipment search model
/// </summary>
public partial record OrderShipmentSearchModel : BaseSearchModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public OrderShipmentSearchModel()
    {
		_logger.LogDebug(">>> Entering OrderShipmentSearchModel()");
        ShipmentItemSearchModel = new ShipmentItemSearchModel();
		_logger.LogDebug("OrderShipmentSearchModel(): ShipmentItemSearchModel → {ShipmentItemSearchModel}", ShipmentItemSearchModel);
		_logger.LogDebug("<<< Exiting OrderShipmentSearchModel()");
    }

    #endregion

    #region Properties

    public int OrderId { get; set; }

    public ShipmentItemSearchModel ShipmentItemSearchModel { get; set; }

    #endregion
}