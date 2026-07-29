using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Http;
using Nop.Plugin.Misc.Omnisend.Services;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Misc.Omnisend.Controllers;

public class OmnisendController : BasePluginController
{
    #region Fields

    private readonly ICustomerService _customerService;
    private readonly IGenericAttributeService _genericAttributeService;
    private readonly IWebHelper _webHelper;
    private readonly IWorkContext _workContext;
    private readonly OmnisendService _omnisendService;

    #endregion

    #region Ctor

    public OmnisendController(ICustomerService customerService,
        IGenericAttributeService genericAttributeService,
        IWebHelper webHelper,
        IWorkContext workContext,
        OmnisendService omnisendService)
    {
        _customerService = customerService;
        _genericAttributeService = genericAttributeService;
        _webHelper = webHelper;
        _workContext = workContext;
        _omnisendService = omnisendService;
    }

    #endregion

    #region Methods

    public async Task<IActionResult> AbandonedCheckout(string cartId)
    {
		logger.LogDebug(">>> Entering AbandonedCheckout(cartId={CartId})", cartId);
        var customer = await _workContext.GetCurrentCustomerAsync();
        if (await _customerService.IsGuestAsync(customer))
			logger.LogDebug("AbandonedCheckout(cartId={CartId}): customer → {Customer}", cartId, customer);
            return RedirectToRoute(NopRouteNames.General.LOGIN, new { ReturnUrl = _webHelper.GetRawUrl(Request) });

        var customerEmail = await _genericAttributeService.GetAttributeAsync<string>(customer, OmnisendDefaults.CustomerEmailAttribute);
        if (!string.IsNullOrEmpty(customerEmail) && !customerEmail.Equals(customer.Email, StringComparison.InvariantCultureIgnoreCase))
			logger.LogDebug("AbandonedCheckout(cartId={CartId}): customerEmail → {CustomerEmail}", cartId, customerEmail);
            return RedirectToRoute(NopRouteNames.General.LOGIN, new { ReturnUrl = _webHelper.GetRawUrl(Request) });

        await _omnisendService.RestoreShoppingCartAsync(cartId);

		logger.LogDebug("<<< Exiting AbandonedCheckout(cartId={CartId})", cartId);
        return RedirectToRoute(NopRouteNames.General.CART);
    }

    #endregion
}