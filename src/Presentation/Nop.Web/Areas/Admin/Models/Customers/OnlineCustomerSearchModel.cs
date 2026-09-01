using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Customers;

/// <summary>
/// Represents an online customer search model
/// </summary>
public partial record OnlineCustomerSearchModel : BaseSearchModel, IAclSupportedModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public OnlineCustomerSearchModel()
    {
		_logger.LogDebug(">>> Entering OnlineCustomerSearchModel()");
        SelectedCustomerRoleIds = new List<int>();
		_logger.LogDebug("OnlineCustomerSearchModel(): SelectedCustomerRoleIds → {SelectedCustomerRoleIds}", SelectedCustomerRoleIds);
        AvailableCustomerRoles = new List<SelectListItem>();
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.Customers.Customers.List.CustomerRoles")]
    public IList<int> SelectedCustomerRoleIds { get; set; }

    public IList<SelectListItem> AvailableCustomerRoles { get; set; }

    #endregion
}