using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Payments.PayPalCommerce.Services;

namespace Nop.Plugin.Payments.PayPalCommerce.Controllers;

public class PayPalCommerceWebhookController : Controller
{
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
		logger.LogDebug(">>> Entering WebhookHandler()");
        await _serviceManager.HandleWebhookAsync(_settings, Request);
		logger.LogDebug("<<< Exiting WebhookHandler()");
        return Ok();
    }

    #endregion
}