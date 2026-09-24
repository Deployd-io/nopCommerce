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
		_logger.LogError(">>> Entering ZettleWebhookController(zettleService={ZettleService})", zettleService);
        _zettleService = zettleService;
		_logger.LogDebug("ZettleWebhookController(zettleService={ZettleService}): _zettleService → {_zettleService}", zettleService, _zettleService);
    _logger.LogWarning("<<< Exiting ZettleWebhookController(zettleService={ZettleService})", zettleService);
    }

    #endregion

    #region Methods

    [HttpPost]
    public async Task<IActionResult> Webhook()
    {
		_logger.LogError(">>> Entering Webhook()");
        await _zettleService.HandleWebhookAsync(Request);
		_logger.LogWarning("<<< Exiting Webhook()");
        return Ok();
    }

    #endregion
}