using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Misc.Zettle.Services;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Misc.Zettle.Controllers;

public class ZettleWebhookController : Controller
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly ZettleService _zettleService;

    #endregion

    #region Ctor

    public ZettleWebhookController(ZettleService zettleService)
    {
		_logger.LogDebug(">>> Entering ZettleWebhookController(zettleService={ZettleService})", zettleService);
        _zettleService = zettleService;
		_logger.LogDebug("<<< Exiting ZettleWebhookController(zettleService={ZettleService})", zettleService);
		_logger.LogDebug("ZettleWebhookController(zettleService={ZettleService}): _zettleService → {_zettleService}", zettleService, _zettleService);
    }

    #endregion

    #region Methods

    [HttpPost]
    public async Task<IActionResult> Webhook()
    {
		_logger.LogDebug(">>> Entering Webhook()");
        await _zettleService.HandleWebhookAsync(Request);
		_logger.LogDebug("<<< Exiting Webhook()");
        return Ok();
    }

    #endregion
}