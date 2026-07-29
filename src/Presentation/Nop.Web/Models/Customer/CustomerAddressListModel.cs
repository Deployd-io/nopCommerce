using Nop.Web.Framework.Models;
using Nop.Web.Models.Common;

namespace Nop.Web.Models.Customer;

public partial record CustomerAddressListModel : BaseNopModel
{
    public CustomerAddressListModel()
    {
		logger.LogDebug(">>> Entering CustomerAddressListModel()");
        Addresses = new List<AddressModel>();
		logger.LogDebug("<<< Exiting CustomerAddressListModel()");
		logger.LogDebug("CustomerAddressListModel(): Addresses → {Addresses}", Addresses);
    }

    public IList<AddressModel> Addresses { get; set; }
}