using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Controllers;

[WwwRequirement]
[CheckLanguageSeoCode]
[CheckAccessPublicStore]
[CheckAccessClosedStore]
[CheckDiscountCoupon]
[CheckAffiliate]
public abstract partial class BasePublicController : BaseController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    protected virtual IActionResult InvokeHttp404()
    {
		_logger.LogDebug(">>> Entering InvokeHttp404()");
        Response.StatusCode = 404;
		_logger.LogDebug("InvokeHttp404(): Response.StatusCode → {Arg0}", Response.StatusCode);
		_logger.LogDebug("<<< Exiting InvokeHttp404()");
        return new EmptyResult();
    }
}