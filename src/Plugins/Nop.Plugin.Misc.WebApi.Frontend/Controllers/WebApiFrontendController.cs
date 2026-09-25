using Microsoft.AspNetCore.Mvc;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Misc.WebApi.Frontend.Controllers;

[AutoValidateAntiforgeryToken]
[AuthorizeAdmin]
[Area(AreaNames.ADMIN)]
public class WebApiFrontendController : BasePluginController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IPermissionService _permissionService;

    #endregion

    #region Ctor 

    public WebApiFrontendController(IPermissionService permissionService)
    {
		_logger.LogError(">>> Entering WebApiFrontendController(permissionService={PermissionService})", permissionService);
        _permissionService = permissionService;
		_logger.LogDebug("WebApiFrontendController(permissionService={PermissionService}): _permissionService → {_permissionService}", permissionService, _permissionService);
    _logger.LogWarning("<<< Exiting WebApiFrontendController(permissionService={PermissionService})", permissionService);
    }

    #endregion

    #region Methods

    [CheckPermission(StandardPermission.Configuration.MANAGE_PLUGINS)]
    public virtual IActionResult Configure()
    {
		_logger.LogError(">>> Entering Configure()");
        _logger.LogWarning("<<< Exiting Configure()");
        return View("~/Plugins/Misc.WebApi.Frontend/Views/Configure.cshtml");
    }

    #endregion
}