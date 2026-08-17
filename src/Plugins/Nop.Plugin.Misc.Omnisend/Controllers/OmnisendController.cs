using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Http;
using Nop.Plugin.Misc.Omnisend.Services;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Web.Framework.Controllers;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Misc.Omnisend.Controllers;

public class OmnisendController : BasePluginController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
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
		_logger.LogDebug(">>> Entering OmnisendController(customerService={CustomerService},genericAttributeService={GenericAttributeService},webHelper={WebHelper},workContext={WorkContext},omnisendService={OmnisendService})", customerService, genericAttributeService, webHelper, workContext, omnisendService);
        _customerService = customerService;
		_logger.LogDebug("OmnisendController(customerService={CustomerService},genericAttributeService={GenericAttributeService},webHelper={WebHelper},workContext={WorkContext},omnisendService={OmnisendService}): _customerService → {_customerService}", customerService, genericAttributeService, webHelper, workContext, omnisendService, _customerService);
        _genericAttributeService = genericAttributeService;
		_logger.LogDebug("OmnisendController(customerService={CustomerService},genericAttributeService={GenericAttributeService},webHelper={WebHelper},workContext={WorkContext},omnisendService={OmnisendService}): _genericAttributeService → {_genericAttributeService}", customerService, genericAttributeService, webHelper, workContext, omnisendService, _genericAttributeService);
        _webHelper = webHelper;
		_logger.LogDebug("OmnisendController(customerService={CustomerService},genericAttributeService={GenericAttributeService},webHelper={WebHelper},workContext={WorkContext},omnisendService={OmnisendService}): _webHelper → {_webHelper}", customerService, genericAttributeService, webHelper, workContext, omnisendService, _webHelper);
        _workContext = workContext;
		_logger.LogDebug("OmnisendController(customerService={CustomerService},genericAttributeService={GenericAttributeService},webHelper={WebHelper},workContext={WorkContext},omnisendService={OmnisendService}): _workContext → {_workContext}", customerService, genericAttributeService, webHelper, workContext, omnisendService, _workContext);
        _omnisendService = omnisendService;
		_logger.LogDebug("<<< Exiting OmnisendController(customerService={CustomerService},genericAttributeService={GenericAttributeService},webHelper={WebHelper},workContext={WorkContext},omnisendService={OmnisendService})", customerService, genericAttributeService, webHelper, workContext, omnisendService);
		_logger.LogDebug("OmnisendController(customerService={CustomerService},genericAttributeService={GenericAttributeService},webHelper={WebHelper},workContext={WorkContext},omnisendService={OmnisendService}): _omnisendService → {_omnisendService}", customerService, genericAttributeService, webHelper, workContext, omnisendService, _omnisendService);
    }

    #endregion

    #region Methods

    public async Task<IActionResult> AbandonedCheckout(string cartId)
    {
		_logger.LogDebug(">>> Entering AbandonedCheckout(cartId={CartId})", cartId);
        var customer = await _workContext.GetCurrentCustomerAsync();
        if (await _customerService.IsGuestAsync(customer))
			_logger.LogDebug("AbandonedCheckout(cartId={CartId}): customer → {Customer}", cartId, customer);
            return RedirectToRoute(NopRouteNames.General.LOGIN, new { ReturnUrl = _webHelper.GetRawUrl(Request) });

        var customerEmail = await _genericAttributeService.GetAttributeAsync<string>(customer, OmnisendDefaults.CustomerEmailAttribute);
		_logger.LogDebug("AbandonedCheckout(cartId={CartId}): customerEmail → {CustomerEmail}", cartId, customerEmail);
        if (!string.IsNullOrEmpty(customerEmail) && !customerEmail.Equals(customer.Email, StringComparison.InvariantCultureIgnoreCase))
            return RedirectToRoute(NopRouteNames.General.LOGIN, new { ReturnUrl = _webHelper.GetRawUrl(Request) });

        await _omnisendService.RestoreShoppingCartAsync(cartId);

		_logger.LogDebug("<<< Exiting AbandonedCheckout(cartId={CartId})", cartId);
        return RedirectToRoute(NopRouteNames.General.CART);
    }

    #endregion
}