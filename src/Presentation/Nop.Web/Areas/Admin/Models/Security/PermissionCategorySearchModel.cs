using Microsoft.Extensions.Logging;
﻿using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Security;

public partial record PermissionCategorySearchModel : BaseSearchModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public PermissionCategorySearchModel()
    {
		_logger.LogDebug(">>> Entering PermissionCategorySearchModel()");
        PermissionItemSearchModel = new PermissionItemSearchModel();
		_logger.LogDebug("<<< Exiting PermissionCategorySearchModel()");
		_logger.LogDebug("PermissionCategorySearchModel(): PermissionItemSearchModel → {PermissionItemSearchModel}", PermissionItemSearchModel);
    }

    #endregion

    #region Properties

    public PermissionItemSearchModel PermissionItemSearchModel { get; set; }

    #endregion
}