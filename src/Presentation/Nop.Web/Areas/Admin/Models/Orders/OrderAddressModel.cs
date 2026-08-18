using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Framework.Models;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Orders;

public partial record OrderAddressModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public OrderAddressModel()
    {
		_logger.LogDebug(">>> Entering OrderAddressModel()");
        Address = new AddressModel();
		_logger.LogDebug("OrderAddressModel(): Address → {Address}", Address);
		_logger.LogDebug("<<< Exiting OrderAddressModel()");
    }

    #endregion

    #region Properties

    public int OrderId { get; set; }

    public AddressModel Address { get; set; }

    #endregion
}