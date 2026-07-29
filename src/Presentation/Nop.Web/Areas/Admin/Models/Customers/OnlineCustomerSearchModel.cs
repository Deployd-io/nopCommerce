using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Customers;

/// <summary>
/// Represents an online customer search model
/// </summary>
public partial record OnlineCustomerSearchModel : BaseSearchModel, IAclSupportedModel
{
    #region Ctor

    public OnlineCustomerSearchModel()
    {
		logger.LogDebug(">>> Entering OnlineCustomerSearchModel()");
        SelectedCustomerRoleIds = new List<int>();
        AvailableCustomerRoles = new List<SelectListItem>();
		logger.LogDebug("<<< Exiting OnlineCustomerSearchModel()");
		logger.LogDebug("OnlineCustomerSearchModel(): SelectedCustomerRoleIds → {SelectedCustomerRoleIds}", SelectedCustomerRoleIds);
		logger.LogDebug("OnlineCustomerSearchModel(): AvailableCustomerRoles → {AvailableCustomerRoles}", AvailableCustomerRoles);
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.Customers.Customers.List.CustomerRoles")]
    public IList<int> SelectedCustomerRoleIds { get; set; }

    public IList<SelectListItem> AvailableCustomerRoles { get; set; }

    #endregion
}