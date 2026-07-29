using Nop.Web.Framework.Models;
using Nop.Web.Models.Common;

namespace Nop.Web.Models.Customer;

public partial record CustomerAddressEditModel : BaseNopModel
{
    public CustomerAddressEditModel()
    {
		logger.LogDebug(">>> Entering CustomerAddressEditModel()");
        Address = new AddressModel();
		logger.LogDebug("<<< Exiting CustomerAddressEditModel()");
		logger.LogDebug("CustomerAddressEditModel(): Address → {Address}", Address);
    }

    public AddressModel Address { get; set; }
}