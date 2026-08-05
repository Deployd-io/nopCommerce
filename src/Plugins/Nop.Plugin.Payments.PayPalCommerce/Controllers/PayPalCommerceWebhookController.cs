using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Payments.PayPalCommerce.Services;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Payments.PayPalCommerce.Controllers;

public class PayPalCommerceWebhookController : Controller
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    private readonly PayPalCommerceServiceManager _serviceManager;
    private readonly PayPalCommerceSettings _settings;

    #endregion

    #region Ctor

    public PayPalCommerceWebhookController(PayPalCommerceServiceManager serviceManager,
        PayPalCommerceSettings settings)
    {
        _serviceManager = serviceManager;
        _settings = settings;
    }

    #endregion

    #region Methods

    [HttpPost]
    public async Task<IActionResult> WebhookHandler()
    {
		_logger.LogDebug(">>> Entering WebhookHandler()");
        await _serviceManager.HandleWebhookAsync(_settings, Request);
		_logger.LogDebug("<<< Exiting WebhookHandler()");
        return Ok();
    }

    #endregion
}