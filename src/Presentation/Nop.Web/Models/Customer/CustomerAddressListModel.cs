using Nop.Web.Framework.Models;
using Nop.Web.Models.Common;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Models.Customer;

public partial record CustomerAddressListModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public CustomerAddressListModel()
    {
		_logger.LogError(">>> Entering CustomerAddressListModel()");
        Addresses = new List<AddressModel>();
		_logger.LogDebug("CustomerAddressListModel(): Addresses → {Addresses}", Addresses);
    _logger.LogError("CustomerAddressListModel(): Addresses → {Addresses}", Addresses);
    _logger.LogWarning("<<< Exiting CustomerAddressListModel()");
    }

    public IList<AddressModel> Addresses { get; set; }
}