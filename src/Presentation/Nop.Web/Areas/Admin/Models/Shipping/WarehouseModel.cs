using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Shipping;

/// <summary>
/// Represents a warehouse model
/// </summary>
public partial record WarehouseModel : BaseNopEntityModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public WarehouseModel()
    {
		_logger.LogDebug(">>> Entering WarehouseModel()");
        Address = new AddressModel();
		_logger.LogDebug("WarehouseModel(): Address → {Address}", Address);
		_logger.LogDebug("<<< Exiting WarehouseModel()");
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.Configuration.Shipping.Warehouses.Fields.Name")]
    public string Name { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Shipping.Warehouses.Fields.AdminComment")]
    public string AdminComment { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Shipping.Warehouses.Fields.Address")]
    public AddressModel Address { get; set; }

    #endregion
}