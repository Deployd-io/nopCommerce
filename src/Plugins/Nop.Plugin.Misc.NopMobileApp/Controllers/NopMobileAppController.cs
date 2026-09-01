using Microsoft.AspNetCore.Mvc;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Misc.NopMobileApp.Controllers;

[AutoValidateAntiforgeryToken]
[AuthorizeAdmin]
[Area(AreaNames.ADMIN)]
public class NopMobileAppController : BasePluginController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    private readonly IPermissionService _permissionService;

    #endregion

    #region Ctor 

    public NopMobileAppController(IPermissionService permissionService)
    {
		_logger.LogDebug(">>> Entering NopMobileAppController(permissionService={PermissionService})", permissionService);
        _permissionService = permissionService;
		_logger.LogDebug("NopMobileAppController(permissionService={PermissionService}): _permissionService → {_permissionService}", permissionService, _permissionService);
    }

    #endregion

    #region Methods

    [CheckPermission(StandardPermission.Configuration.MANAGE_PLUGINS)]
    public virtual IActionResult Configure()
    {
		_logger.LogDebug(">>> Entering Configure()");
        return View("~/Plugins/Misc.NopMobileApp/Views/Configure.cshtml");
    }

    #endregion
}