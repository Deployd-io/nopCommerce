using Microsoft.AspNetCore.Mvc;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Misc.OmnibusDirective.Controllers;

[AuthorizeAdmin]
[Area(AreaNames.ADMIN)]
[AutoValidateAntiforgeryToken]
public class OmnibusDirectiveController : BasePluginController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Methods

    [CheckPermission(StandardPermission.Configuration.MANAGE_PLUGINS)]
    public IActionResult Configure()
    {
		_logger.LogDebug(">>> Entering Configure()");
		_logger.LogDebug("<<< Exiting Configure()");
        return View("~/Plugins/Misc.OmnibusDirective/Views/Configure.cshtml");
    }

    #endregion
}
