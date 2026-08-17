using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Security;

/// <summary>
/// Represents a permission item model
/// </summary>
public partial record PermissionItemModel : BaseNopEntityModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public PermissionItemModel()
    {
		_logger.LogDebug(">>> Entering PermissionItemModel()");
        SelectedCustomerRoleIds = new List<int>();
        AvailableCustomerRoles = new List<SelectListItem>();
		_logger.LogDebug("<<< Exiting PermissionItemModel()");
		_logger.LogDebug("PermissionItemModel(): AvailableCustomerRoles → {AvailableCustomerRoles}", AvailableCustomerRoles);
		_logger.LogDebug("PermissionItemModel(): SelectedCustomerRoleIds → {SelectedCustomerRoleIds}", SelectedCustomerRoleIds);
    }

    #endregion

    #region Properties

    public string PermissionName { get; set; }

    public string PermissionAppliedFor { get; set; }

    [NopResourceDisplayName("Admin.Customers.CustomerRoles")]
    public IList<int> SelectedCustomerRoleIds { get; set; }

    public IList<SelectListItem> AvailableCustomerRoles { get; set; }

    #endregion
}