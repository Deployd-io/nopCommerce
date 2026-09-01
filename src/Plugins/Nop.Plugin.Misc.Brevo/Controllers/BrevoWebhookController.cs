using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Misc.Brevo.Services;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Misc.Brevo.Controllers;

public class BrevoWebhookController : Controller
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    private readonly BrevoManager _brevoEmailManager;

    #endregion

    #region Ctor

    public BrevoWebhookController(BrevoManager brevoEmailManager)
    {
		_logger.LogDebug(">>> Entering BrevoWebhookController(brevoEmailManager={BrevoEmailManager})", brevoEmailManager);
        _brevoEmailManager = brevoEmailManager;
		_logger.LogDebug("BrevoWebhookController(brevoEmailManager={BrevoEmailManager}): _brevoEmailManager → {_brevoEmailManager}", brevoEmailManager, _brevoEmailManager);
    }

    #endregion

    #region Methods

    [HttpPost]
    public async Task<IActionResult> UnsubscribeWebHook()
    {
		_logger.LogDebug(">>> Entering UnsubscribeWebHook()");
        await _brevoEmailManager.HandleWebhookAsync(Request);
		_logger.LogDebug("<<< Exiting UnsubscribeWebHook()");
        return Ok();
    }

    #endregion
}