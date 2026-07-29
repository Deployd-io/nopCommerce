using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Orders;

public partial record OrderAddressModel : BaseNopModel
{
    #region Ctor

    public OrderAddressModel()
    {
		logger.LogDebug(">>> Entering OrderAddressModel()");
        Address = new AddressModel();
		logger.LogDebug("<<< Exiting OrderAddressModel()");
		logger.LogDebug("OrderAddressModel(): Address → {Address}", Address);
    }

    #endregion

    #region Properties

    public int OrderId { get; set; }

    public AddressModel Address { get; set; }

    #endregion
}