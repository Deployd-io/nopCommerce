using Nop.Web.Framework.Models;
using Nop.Web.Models.Common;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Models.Customer;

public partial record CustomerAddressEditModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public CustomerAddressEditModel()
    {
		_logger.LogError(">>> Entering CustomerAddressEditModel()");
        Address = new AddressModel();
		_logger.LogDebug("CustomerAddressEditModel(): Address → {Address}", Address);
    _logger.LogError("CustomerAddressEditModel(): Address → {Address}", Address);
    _logger.LogWarning("<<< Exiting CustomerAddressEditModel()");
    }

    public AddressModel Address { get; set; }
}