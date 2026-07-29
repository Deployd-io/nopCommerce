using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Customers;

/// <summary>
/// Represents a customer address model
/// </summary>
public partial record CustomerAddressModel : BaseNopModel
{
    #region Ctor

    public CustomerAddressModel()
    {
		logger.LogDebug(">>> Entering CustomerAddressModel()");
        Address = new AddressModel();
		logger.LogDebug("<<< Exiting CustomerAddressModel()");
		logger.LogDebug("CustomerAddressModel(): Address → {Address}", Address);
    }

    #endregion

    #region Properties

    public int CustomerId { get; set; }

    public AddressModel Address { get; set; }

    #endregion
}