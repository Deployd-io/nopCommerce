using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Misc.Brevo.Services;

namespace Nop.Plugin.Misc.Brevo.Controllers;

public class BrevoWebhookController : Controller
{
    #region Fields

    private readonly BrevoManager _brevoEmailManager;

    #endregion

    #region Ctor

    public BrevoWebhookController(BrevoManager brevoEmailManager)
    {
		logger.LogDebug(">>> Entering BrevoWebhookController(brevoEmailManager={BrevoEmailManager})", brevoEmailManager);
        _brevoEmailManager = brevoEmailManager;
		logger.LogDebug("<<< Exiting BrevoWebhookController(brevoEmailManager={BrevoEmailManager})", brevoEmailManager);
		logger.LogDebug("BrevoWebhookController(brevoEmailManager={BrevoEmailManager}): _brevoEmailManager → {_brevoEmailManager}", brevoEmailManager, _brevoEmailManager);
    }

    #endregion

    #region Methods

    [HttpPost]
    public async Task<IActionResult> UnsubscribeWebHook()
    {
		logger.LogDebug(">>> Entering UnsubscribeWebHook()");
        await _brevoEmailManager.HandleWebhookAsync(Request);
		logger.LogDebug("<<< Exiting UnsubscribeWebHook()");
        return Ok();
    }

    #endregion
}
