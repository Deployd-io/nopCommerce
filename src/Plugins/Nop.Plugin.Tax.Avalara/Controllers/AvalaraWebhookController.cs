using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Tax.Avalara.Services;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Tax.Avalara.Controllers;

public class AvalaraWebhookController : Controller
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly AvalaraTaxManager _avalaraTaxManager;

    #endregion

    #region Ctor

    public AvalaraWebhookController(AvalaraTaxManager avalaraTaxManager)
    {
		_logger.LogDebug(">>> Entering AvalaraWebhookController(avalaraTaxManager={AvalaraTaxManager})", avalaraTaxManager);
        _avalaraTaxManager = avalaraTaxManager;
		_logger.LogDebug("AvalaraWebhookController(avalaraTaxManager={AvalaraTaxManager}): _avalaraTaxManager → {_avalaraTaxManager}", avalaraTaxManager, _avalaraTaxManager);
		_logger.LogDebug("<<< Exiting AvalaraWebhookController(avalaraTaxManager={AvalaraTaxManager})", avalaraTaxManager);
    }

    #endregion

    #region Methods

    [HttpPost]
    public async Task<IActionResult> ItemClassificationWebhook()
    {
		_logger.LogDebug(">>> Entering ItemClassificationWebhook()");
        await _avalaraTaxManager.HandleItemClassificationWebhookAsync(Request);
		_logger.LogDebug("<<< Exiting ItemClassificationWebhook()");
        return Ok();
    }

    #endregion
}