using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Framework.Models;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Customers;

/// <summary>
/// Represents a customer address model
/// </summary>
public partial record CustomerAddressModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public CustomerAddressModel()
    {
		_logger.LogDebug(">>> Entering CustomerAddressModel()");
        Address = new AddressModel();
		_logger.LogDebug("CustomerAddressModel(): Address → {Address}", Address);
		_logger.LogDebug("<<< Exiting CustomerAddressModel()");
    }

    #endregion

    #region Properties

    public int CustomerId { get; set; }

    public AddressModel Address { get; set; }

    #endregion
}