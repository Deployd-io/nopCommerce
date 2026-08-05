using Nop.Web.Framework.Models;
using Nop.Web.Models.Common;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Models.Customer;

public partial record CustomerAddressEditModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public CustomerAddressEditModel()
    {
		_logger.LogDebug(">>> Entering CustomerAddressEditModel()");
        Address = new AddressModel();
		_logger.LogDebug("<<< Exiting CustomerAddressEditModel()");
		_logger.LogDebug("CustomerAddressEditModel(): Address → {Address}", Address);
    }

    public AddressModel Address { get; set; }
}