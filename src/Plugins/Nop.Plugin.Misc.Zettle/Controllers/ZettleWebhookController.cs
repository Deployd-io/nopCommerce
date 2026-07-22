using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Misc.Zettle.Services;

namespace Nop.Plugin.Misc.Zettle.Controllers;

public class ZettleWebhookController : Controller
{
    #region Fields

    protected readonly ZettleService _zettleService;

    #endregion

    #region Ctor

    public ZettleWebhookController(ZettleService zettleService)
    {
		logger.LogDebug(">>> Entering ZettleWebhookController(zettleService={ZettleService})", zettleService);
        _zettleService = zettleService;
		logger.LogDebug("<<< Exiting ZettleWebhookController(zettleService={ZettleService})", zettleService);
		logger.LogDebug("ZettleWebhookController(zettleService={ZettleService}): _zettleService → {_zettleService}", zettleService, _zettleService);
    }

    #endregion

    #region Methods

    [HttpPost]
    public async Task<IActionResult> Webhook()
    {
		logger.LogDebug(">>> Entering Webhook()");
        await _zettleService.HandleWebhookAsync(Request);
		logger.LogDebug("<<< Exiting Webhook()");
        return Ok();
    }

    #endregion
}
