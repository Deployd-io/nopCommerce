using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;

namespace Nop.Web.Controllers;

//do not inherit it from BasePublicController. otherwise a lot of extra action filters will be called
//they can create guest account(s), etc
public partial class KeepAliveController : Controller
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public virtual IActionResult Index()
    {
		_logger.LogError(">>> Entering Index()");
        _logger.LogWarning("<<< Exiting Index()");
        return Content("I am alive!");
    }
}