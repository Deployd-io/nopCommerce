using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Tax.Avalara.Services;

namespace Nop.Plugin.Tax.Avalara.Controllers;

public class AvalaraWebhookController : Controller
{
    #region Fields

    protected readonly AvalaraTaxManager _avalaraTaxManager;

    #endregion

    #region Ctor

    public AvalaraWebhookController(AvalaraTaxManager avalaraTaxManager)
    {
		logger.LogDebug(">>> Entering AvalaraWebhookController(avalaraTaxManager={AvalaraTaxManager})", avalaraTaxManager);
        _avalaraTaxManager = avalaraTaxManager;
		logger.LogDebug("<<< Exiting AvalaraWebhookController(avalaraTaxManager={AvalaraTaxManager})", avalaraTaxManager);
		logger.LogDebug("AvalaraWebhookController(avalaraTaxManager={AvalaraTaxManager}): _avalaraTaxManager → {_avalaraTaxManager}", avalaraTaxManager, _avalaraTaxManager);
    }

    #endregion

    #region Methods

    [HttpPost]
    public async Task<IActionResult> ItemClassificationWebhook()
    {
		logger.LogDebug(">>> Entering ItemClassificationWebhook()");
        await _avalaraTaxManager.HandleItemClassificationWebhookAsync(Request);
		logger.LogDebug("<<< Exiting ItemClassificationWebhook()");
        return Ok();
    }

    #endregion
}
