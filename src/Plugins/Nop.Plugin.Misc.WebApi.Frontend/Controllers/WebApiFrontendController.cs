using Microsoft.AspNetCore.Mvc;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Misc.WebApi.Frontend.Controllers;

[AutoValidateAntiforgeryToken]
[AuthorizeAdmin]
[Area(AreaNames.ADMIN)]
public class WebApiFrontendController : BasePluginController
{
    #region Fields

    protected readonly IPermissionService _permissionService;

    #endregion

    #region Ctor 

    public WebApiFrontendController(IPermissionService permissionService)
    {
		logger.LogDebug(">>> Entering WebApiFrontendController(permissionService={PermissionService})", permissionService);
        _permissionService = permissionService;
		logger.LogDebug("<<< Exiting WebApiFrontendController(permissionService={PermissionService})", permissionService);
		logger.LogDebug("WebApiFrontendController(permissionService={PermissionService}): _permissionService → {_permissionService}", permissionService, _permissionService);
    }

    #endregion

    #region Methods

    [CheckPermission(StandardPermission.Configuration.MANAGE_PLUGINS)]
    public virtual IActionResult Configure()
    {
		logger.LogDebug(">>> Entering Configure()");
		logger.LogDebug("<<< Exiting Configure()");
        return View("~/Plugins/Misc.WebApi.Frontend/Views/Configure.cshtml");
    }

    #endregion
}