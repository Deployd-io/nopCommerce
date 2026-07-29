using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Security;

public partial record PermissionCategorySearchModel : BaseSearchModel
{
    #region Ctor

    public PermissionCategorySearchModel()
    {
		logger.LogDebug(">>> Entering PermissionCategorySearchModel()");
        PermissionItemSearchModel = new PermissionItemSearchModel();
		logger.LogDebug("<<< Exiting PermissionCategorySearchModel()");
		logger.LogDebug("PermissionCategorySearchModel(): PermissionItemSearchModel → {PermissionItemSearchModel}", PermissionItemSearchModel);
    }

    #endregion

    #region Properties

    public PermissionItemSearchModel PermissionItemSearchModel { get; set; }

    #endregion
}