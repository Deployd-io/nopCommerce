using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;

namespace Nop.Web.Controllers;

//do not inherit it from BasePublicController. otherwise a lot of extra action filters will be called
//they can create guest account(s), etc
public partial class ErrorController : Controller
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public virtual IActionResult Error()
    {
		_logger.LogDebug(">>> Entering Error()");
        Response.StatusCode = StatusCodes.Status500InternalServerError;
		_logger.LogDebug("<<< Exiting Error()");
        return File("ErrorPage.htm", "text/html");
    }
}