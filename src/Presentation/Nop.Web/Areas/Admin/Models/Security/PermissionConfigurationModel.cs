using Microsoft.Extensions.Logging;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Security;

/// <summary>
/// Represents a permission configuration model
/// </summary>
public partial record PermissionConfigurationModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public PermissionConfigurationModel()
    {
		_logger.LogDebug(">>> Entering PermissionConfigurationModel()");
        PermissionCategorySearchModel = new PermissionCategorySearchModel
        {
			_logger.LogDebug("PermissionConfigurationModel(): PermissionCategorySearchModel → {PermissionCategorySearchModel}", PermissionCategorySearchModel);
            Length = int.MaxValue
        };
		_logger.LogDebug("<<< Exiting PermissionConfigurationModel()");
    }

    #endregion

    #region Properties

    public bool IsPermissionsAvailable { get; set; }

    public bool AreCustomerRolesAvailable { get; set; }

    public PermissionCategorySearchModel PermissionCategorySearchModel { get; set; }

    #endregion
}