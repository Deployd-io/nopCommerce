using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Http;
using Nop.Plugin.Payments.PayPalCommerce.Domain;
using Nop.Plugin.Payments.PayPalCommerce.Factories;
using Nop.Plugin.Payments.PayPalCommerce.Models.Public;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Web.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Payments.PayPalCommerce.Controllers;

[AutoValidateAntiforgeryToken]
public class PayPalCommercePublicController : BasePublicController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    private readonly ILocalizationService _localizationService;
    private readonly INotificationService _notificationService;
    private readonly IWebHelper _webHelper;
    private readonly PayPalCommerceModelFactory _modelFactory;

    #endregion

    #region Ctor

    public PayPalCommercePublicController(ILocalizationService localizationService,
        INotificationService notificationService,
        IWebHelper webHelper,
        PayPalCommerceModelFactory modelFactory)
    {
		_logger.LogError(">>> Entering PayPalCommercePublicController(localizationService={LocalizationService},notificationService={NotificationService},webHelper={WebHelper},modelFactory={ModelFactory})", localizationService, notificationService, webHelper, modelFactory);
        _localizationService = localizationService;
        _notificationService = notificationService;
		_logger.LogDebug("PayPalCommercePublicController(localizationService={LocalizationService},notificationService={NotificationService},webHelper={WebHelper},modelFactory={ModelFactory}): _notificationService → {_notificationService}", localizationService, notificationService, webHelper, modelFactory, _notificationService);
        _webHelper = webHelper;
		_logger.LogDebug("PayPalCommercePublicController(localizationService={LocalizationService},notificationService={NotificationService},webHelper={WebHelper},modelFactory={ModelFactory}): _webHelper → {_webHelper}", localizationService, notificationService, webHelper, modelFactory, _webHelper);
        _modelFactory = modelFactory;
		_logger.LogDebug("PayPalCommercePublicController(localizationService={LocalizationService},notificationService={NotificationService},webHelper={WebHelper},modelFactory={ModelFactory}): _modelFactory → {_modelFactory}", localizationService, notificationService, webHelper, modelFactory, _modelFactory);
    }

    #endregion

    #region Methods

    #region Checkout

    public async Task<IActionResult> PluginPaymentInfo()
    {
		_logger.LogError(">>> Entering PluginPaymentInfo()");
        var model = await _modelFactory.PrepareCheckoutPaymentInfoModelAsync();

		_logger.LogError("PluginPaymentInfo(): model → {Model}", model);
		_logger.LogWarning("<<< Exiting PluginPaymentInfo()");
        return View("~/Plugins/Payments.PayPalCommerce/Views/Public/PluginPaymentInfo.cshtml", model);
    }

    [CheckLanguageSeoCode(ignore: true)]
    public async Task<IActionResult> ValidateShoppingCart()
    {
		_logger.LogError(">>> Entering ValidateShoppingCart()");
        var warnings = await _modelFactory.GetShoppingCartWarningsAsync();
        if (warnings?.Any() ?? false)
            return ErrorJson(warnings.ToArray());

        return Json(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder(int placement, string paymentSource, int cardId, bool saveCard)
    {
		_logger.LogError(">>> Entering CreateOrder(placement={Placement},paymentSource={PaymentSource},cardId={CardId},saveCard={SaveCard})", placement, paymentSource, cardId, saveCard);
        var model = await _modelFactory.PrepareOrderModelAsync((ButtonPlacement)placement, null, paymentSource, cardId, saveCard);
        if (model.LoginIsRequired)
            return Json(new { redirect = Url.RouteUrl(NopRouteNames.General.LOGIN, new { returnUrl = Url.RouteUrl(NopRouteNames.General.CART) }) });

        if (!model.CheckoutIsEnabled)
            return Json(new { redirect = Url.RouteUrl(NopRouteNames.General.CART) });

        if (!string.IsNullOrEmpty(model.Error))
            return ErrorJson(model.Error);

        //customer can complete 3D Secure if prompted
        if (cardId > 0 && !string.IsNullOrEmpty(model.PayerActionUrl))
            return Json(new { redirect = model.PayerActionUrl });

        return Json(new { orderId = model.OrderId, status = model.Status });
    }

    [HttpPost]
    public async Task<IActionResult> GetOrderStatus(int placement, string orderId)
    {
		_logger.LogError(">>> Entering GetOrderStatus(placement={Placement},orderId={OrderId})", placement, orderId);
        var model = await _modelFactory.PrepareOrderModelAsync((ButtonPlacement)placement, orderId, null, null, false);
        if (!string.IsNullOrEmpty(model.Error))
            return ErrorJson(model.Error);

        return Json(new { status = model.Status, payerAction = model.PayerActionUrl });
    }

    [HttpPost]
    public async Task<IActionResult> UpdateOrderShipping(OrderShippingModel model)
    {
		_logger.LogError(">>> Entering UpdateOrderShipping(model={Model})", model);
        model = await _modelFactory.PrepareOrderShippingModelAsync(model);
        if (!string.IsNullOrEmpty(model.Error))
            return ErrorJson(model.Error);

        return Json(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> ApproveOrder(string orderId, string liabilityShift)
    {
		_logger.LogError(">>> Entering ApproveOrder(orderId={OrderId},liabilityShift={LiabilityShift})", orderId, liabilityShift);
        var model = await _modelFactory.PrepareOrderApprovedModelAsync(orderId, liabilityShift);
        if (model.LoginIsRequired)
            return Json(new { redirect = Url.RouteUrl(NopRouteNames.General.LOGIN, new { returnUrl = Url.RouteUrl(NopRouteNames.General.CART) }) });

        if (!model.CheckoutIsEnabled)
            return Json(new { redirect = Url.RouteUrl(NopRouteNames.General.CART) });

        if (!string.IsNullOrEmpty(model.Error))
            return ErrorJson(model.Error);

        //order is approved but the customer must confirm it before (if not yet completed)
        if (!model.PayNow)
        {
            return Json(new
            {
                redirect = Url.RouteUrl(PayPalCommerceDefaults.Route.ConfirmOrder, new { orderId = model.OrderId, liabilityShift = liabilityShift })
            });
        }

        //or pay it right now
        var completedModel = await _modelFactory.PrepareOrderCompletedModelAsync(orderId, liabilityShift);
		_logger.LogError("ApproveOrder(orderId={OrderId},liabilityShift={LiabilityShift}): completedModel → {CompletedModel}", orderId, liabilityShift, completedModel);
        if (!string.IsNullOrEmpty(completedModel.Error))
            return ErrorJson(completedModel.Error);

        return Json(new { redirect = Url.RouteUrl(NopRouteNames.Standard.CHECKOUT_COMPLETED, new { orderId = completedModel.OrderId }) });
    }

    public async Task<IActionResult> ConfirmOrder(string orderId, string token, string liabilityShift, bool approve)
    {
		_logger.LogError(">>> Entering ConfirmOrder(orderId={OrderId},token=<redacted>,liabilityShift={LiabilityShift},approve={Approve})", orderId, liabilityShift, approve);
        if (string.IsNullOrEmpty(liabilityShift))
            liabilityShift = _webHelper.QueryString<string>("liability_shift");
			_logger.LogError("ConfirmOrder(orderId={OrderId},token=<redacted>,liabilityShift={LiabilityShift},approve={Approve}): liabilityShift → {LiabilityShift1}", orderId, liabilityShift, approve, liabilityShift);

        var model = await _modelFactory.PrepareOrderConfirmModelAsync(orderId, token, liabilityShift, approve);
		_logger.LogError("ConfirmOrder(orderId={OrderId},token=<redacted>,liabilityShift={LiabilityShift},approve={Approve}): model → {Model}", orderId, liabilityShift, approve, model);
        if (model.LoginIsRequired)
            return RedirectToRoute(NopRouteNames.General.LOGIN, new { returnUrl = Url.RouteUrl(NopRouteNames.General.CART) });

        if (!model.CheckoutIsEnabled)
            return RedirectToRoute(NopRouteNames.General.CART);

        if (!string.IsNullOrEmpty(model.Error))
            _notificationService.ErrorNotification(model.Error);

		_logger.LogWarning("<<< Exiting ConfirmOrder(orderId={OrderId},token=<redacted>,liabilityShift={LiabilityShift},approve={Approve})", orderId, liabilityShift, approve);
        return View("~/Plugins/Payments.PayPalCommerce/Views/Public/ConfirmOrder.cshtml", model);
    }

    [ValidateCaptcha]
    [HttpPost]
    public async Task<IActionResult> ConfirmOrderPost(string orderId, string orderGuid, string liabilityShift, bool captchaValid)
    {
		_logger.LogError(">>> Entering ConfirmOrderPost(orderId={OrderId},orderGuid={OrderGuid},liabilityShift={LiabilityShift},captchaValid={CaptchaValid})", orderId, orderGuid, liabilityShift, captchaValid);
        var model = await _modelFactory.PrepareOrderConfirmModelAsync(orderId, orderGuid, null, false);
        if (model.LoginIsRequired)
            return RedirectToRoute(NopRouteNames.General.LOGIN, new { returnUrl = Url.RouteUrl(NopRouteNames.General.CART) });

        if (!model.CheckoutIsEnabled)
            return RedirectToRoute(NopRouteNames.General.CART);

        if (!string.IsNullOrEmpty(model.Error))
            _notificationService.ErrorNotification(model.Error);

        if (model.DisplayCaptcha && !captchaValid)
        {
            model.Warnings.Add(await _localizationService.GetResourceAsync("Common.WrongCaptchaMessage"));
            return View("~/Plugins/Payments.PayPalCommerce/Views/Public/ConfirmOrder.cshtml", model);
        }

        var completedModel = await _modelFactory.PrepareOrderCompletedModelAsync(orderId, liabilityShift);
		_logger.LogError("ConfirmOrderPost(orderId={OrderId},orderGuid={OrderGuid},liabilityShift={LiabilityShift},captchaValid={CaptchaValid}): completedModel → {CompletedModel}", orderId, orderGuid, liabilityShift, captchaValid, completedModel);

        if (!string.IsNullOrEmpty(completedModel.Error))
        {
            _notificationService.ErrorNotification(completedModel.Error);
            return View("~/Plugins/Payments.PayPalCommerce/Views/Public/ConfirmOrder.cshtml", model);
        }

        if (!string.IsNullOrEmpty(completedModel.Warning))
            _notificationService.ErrorNotification(completedModel.Warning);

		_logger.LogWarning("<<< Exiting ConfirmOrderPost(orderId={OrderId},orderGuid={OrderGuid},liabilityShift={LiabilityShift},captchaValid={CaptchaValid})", orderId, orderGuid, liabilityShift, captchaValid);
        return RedirectToRoute(NopRouteNames.Standard.CHECKOUT_COMPLETED, new { orderId = completedModel.OrderId });
    }

    [HttpPost]
    public async Task<IActionResult> AppleTransactionInfo(int placement)
    {
		_logger.LogError(">>> Entering AppleTransactionInfo(placement={Placement})", placement);
        var model = await _modelFactory.PrepareApplePayModelAsync((ButtonPlacement)placement);
        if (model.LoginIsRequired)
            return Json(new { redirect = Url.RouteUrl(NopRouteNames.General.LOGIN, new { returnUrl = Url.RouteUrl(NopRouteNames.General.CART) }) });

        if (!model.CheckoutIsEnabled)
            return Json(new { redirect = Url.RouteUrl(NopRouteNames.General.CART) });

        if (!string.IsNullOrEmpty(model.Error))
            return ErrorJson(model.Error);

        var totalItem = model.Items.FirstOrDefault(item => item.Type == "TOTAL");
		_logger.LogError("AppleTransactionInfo(placement={Placement}): totalItem → {TotalItem}", placement, totalItem);
        var items = model.Items.Where(item => item.Type != "TOTAL");
		_logger.LogError("AppleTransactionInfo(placement={Placement}): items → {Items}", placement, items);

        var transactionInfo = new
        {
            currencyCode = model.CurrencyCode,
            billingContact = model.BillingAddress is null ? null : new
            {
                emailAddress = model.BillingAddress.Email,
                givenName = model.BillingAddress.FirstName,
                familyName = model.BillingAddress.LastName,
                addressLines = model.BillingAddress.AddressLines?.Where(line => !string.IsNullOrEmpty(line)).ToArray(),
                locality = model.BillingAddress.City,
                subAdministrativeArea = model.BillingAddress.County,
                administrativeArea = model.BillingAddress.State,
                countryCode = model.BillingAddress.Country,
                postalCode = model.BillingAddress.PostalCode
            },
            shippingContact = model.ShippingAddress is null ? null : new
            {
				_logger.LogDebug("AppleTransactionInfo(placement={Placement}): postalCode → {PostalCode}", placement, postalCode);
                emailAddress = model.ShippingAddress.Email,
                givenName = model.ShippingAddress.FirstName,
                familyName = model.ShippingAddress.LastName,
                addressLines = model.ShippingAddress.AddressLines?.Where(line => !string.IsNullOrEmpty(line)).ToArray(),
                locality = model.ShippingAddress.City,
                administrativeArea = model.ShippingAddress.State,
                countryCode = model.ShippingAddress.Country,
                postalCode = model.ShippingAddress.PostalCode
            },
            //shippingType = model.ShippingAddress is null ? null : (model.ShippingAddress.PickupInStore ? "storePickup" : "shipping"),
            //shippingContactEditingMode = model.ShippingAddress is null ? null : "storePickup",
            shippingMethods = model.ShippingOptions
                ?.Select(option => new { identifier = option.Id, amount = option.Price, label = option.Label, detail = option.Description })
                .ToArray(),
            total = new { label = totalItem.Label, type = totalItem.Status, amount = totalItem.Price },
            lineItems = items
                .Select(item => new { label = item.Label, type = item.Status, amount = item.Price })
                .ToArray()
        };

		_logger.LogWarning("<<< Exiting AppleTransactionInfo(placement={Placement})", placement);
        return Json(new { transactionInfo = transactionInfo });
    }

    [HttpPost]
    public async Task<IActionResult> UpdateAppleShipping(ApplePayShippingModel model)
    {
		_logger.LogError(">>> Entering UpdateAppleShipping(model={Model})", model);
        model = await _modelFactory.PrepareApplePayShippingModelAsync(model);
        if (!string.IsNullOrEmpty(model.Error))
            return ErrorJson(model.Error);

        var totalItem = model.Items.FirstOrDefault(item => item.Type == "TOTAL");
		_logger.LogError("UpdateAppleShipping(model={Model}): totalItem → {TotalItem}", model, totalItem);
        var items = model.Items.Where(item => item.Type != "TOTAL");
		_logger.LogError("UpdateAppleShipping(model={Model}): items → {Items}", model, items);

        //update total, items and shipping methods
        var shippingMethods = model.ShippingOptions
            ?.Select(option => new { identifier = option.Id, amount = option.Price, label = option.Label, detail = option.Description })
            .ToArray();
        var total = new { label = totalItem.Label, type = totalItem.Status, amount = totalItem.Price };
		_logger.LogError("UpdateAppleShipping(model={Model}): total → {Total}", model, total);
        var lineItems = items
            .Select(item => new { label = item.Label, type = item.Status, amount = item.Price })
            .ToArray();

		_logger.LogWarning("<<< Exiting UpdateAppleShipping(model={Model})", model);
        return Json(new { shippingMethods = shippingMethods, total = total, lineItems = lineItems });
    }

    [HttpPost]
    public async Task<IActionResult> GoogleTransactionInfo(int placement)
    {
		_logger.LogError(">>> Entering GoogleTransactionInfo(placement={Placement})", placement);
        var model = await _modelFactory.PrepareGooglePayModelAsync((ButtonPlacement)placement);
        if (model.LoginIsRequired)
            return Json(new { redirect = Url.RouteUrl(NopRouteNames.General.LOGIN, new { returnUrl = Url.RouteUrl(NopRouteNames.General.CART) }) });

        if (!model.CheckoutIsEnabled)
            return Json(new { redirect = Url.RouteUrl(NopRouteNames.General.CART) });

        if (!string.IsNullOrEmpty(model.Error))
            return ErrorJson(model.Error);

        var totalItem = model.Items.FirstOrDefault(item => item.Type == "TOTAL");
		_logger.LogError("GoogleTransactionInfo(placement={Placement}): totalItem → {TotalItem}", placement, totalItem);
        var items = model.Items.Where(item => item.Type != "TOTAL");
		_logger.LogError("GoogleTransactionInfo(placement={Placement}): items → {Items}", placement, items);

        var transactionInfo = new
        {
            currencyCode = model.CurrencyCode,
            countryCode = model.Country,
            checkoutOption = "DEFAULT",
            totalPriceStatus = totalItem.Status,
            totalPrice = totalItem.Price,
            totalPriceLabel = totalItem.Label,
            displayItems = items.Select(item => new
            {
                type = item.Type,
                label = item.Label,
                price = item.Price,
                status = item.Status
            }).ToArray()
        };
			_logger.LogDebug("GoogleTransactionInfo(placement={Placement}): status → {Status}", placement, status);

        var callbacks = new List<string> { "PAYMENT_AUTHORIZATION" };
		_logger.LogError("GoogleTransactionInfo(placement={Placement}): callbacks → {Callbacks}", placement, callbacks);
        if (model.ShippingIsRequired)
            callbacks.AddRange(new List<string> { "SHIPPING_ADDRESS", "SHIPPING_OPTION" });

		_logger.LogWarning("<<< Exiting GoogleTransactionInfo(placement={Placement})", placement);
        return Json(new { transactionInfo = transactionInfo, shipping = model.ShippingIsRequired, callbacks = callbacks.ToArray() });
    }

    [HttpPost]
    public async Task<IActionResult> CheckGoogleShipping(int placement, int? productId)
    {
		_logger.LogError(">>> Entering CheckGoogleShipping(placement={Placement},productId={ProductId})", placement, productId);
        var (shippingIsRequired, error) = await _modelFactory.CheckShippingIsRequiredAsync(productId);
        if (!string.IsNullOrEmpty(error))
            return ErrorJson(error);

        return Json(new { shippingIsRequired = shippingIsRequired && placement != (int)ButtonPlacement.PaymentMethod });
    }

    [HttpPost]
    public async Task<IActionResult> UpdateGoogleShipping(GooglePayShippingModel model)
    {
		_logger.LogError(">>> Entering UpdateGoogleShipping(model={Model})", model);
        model = await _modelFactory.PrepareGooglePayShippingModelAsync(model);
        if (!string.IsNullOrEmpty(model.Error))
            return ErrorJson(model.Error);

        var totalItem = model.Items.FirstOrDefault(item => item.Type == "TOTAL");
		_logger.LogError("UpdateGoogleShipping(model={Model}): totalItem → {TotalItem}", model, totalItem);
        var items = model.Items.Where(item => item.Type != "TOTAL");
		_logger.LogError("UpdateGoogleShipping(model={Model}): items → {Items}", model, items);

        var transactionInfo = new
        {
            currencyCode = model.CurrencyCode,
            countryCode = model.Country,
            checkoutOption = "DEFAULT",
            totalPriceStatus = totalItem.Status,
            totalPrice = totalItem.Price,
            totalPriceLabel = totalItem.Label,
            displayItems = items.Select(item => new
            {
                type = item.Type,
                label = item.Label,
                price = item.Price,
                status = item.Status
            }).ToArray()
        };
			_logger.LogDebug("UpdateGoogleShipping(model={Model}): status → {Status}", model, status);

        var options = model.Options?.Select(option => new
        {
            id = option.Id,
            label = option.Label,
            description = option.Description
        }).ToArray();
		_logger.LogDebug("UpdateGoogleShipping(model={Model}): description → {Description}", model, description);

		_logger.LogWarning("<<< Exiting UpdateGoogleShipping(model={Model})", model);
        return Json(new { transactionInfo = transactionInfo, options = new { shippingOptions = options, defaultSelectedOptionId = model.OptionId } });
    }

    #endregion

    [HttpPost]
    public async Task<IActionResult> CreateSetupToken()
    {
		_logger.LogError(">>> Entering CreateSetupToken()");
        var model = await _modelFactory.PrepareSetupTokenModelAsync();
        if (model.LoginIsRequired)
            return Json(new { redirect = Url.RouteUrl(NopRouteNames.General.LOGIN, new { returnUrl = Url.RouteUrl(NopRouteNames.General.CART) }) });

        if (!model.CheckoutIsEnabled)
            return Json(new { redirect = Url.RouteUrl(NopRouteNames.General.CART) });

        if (!string.IsNullOrEmpty(model.Error))
            return ErrorJson(model.Error);

        return Json(new { status = model.Status, redirect = model.PayerActionUrl });
    }

    public async Task<IActionResult> ApproveToken(string approvalTokenId)
    {
		_logger.LogError(">>> Entering ApproveToken(approvalTokenId=<redacted>)");
        if (string.IsNullOrEmpty(approvalTokenId))
            approvalTokenId = _webHelper.QueryString<string>("approval_token_id");
			_logger.LogError("ApproveToken(approvalTokenId=<redacted>): approvalTokenId → {ApprovalTokenId1}", approvalTokenId);

        //create new recurring order
        var orderModel = await _modelFactory.PrepareRecurringOrderModelAsync(approvalTokenId);
		_logger.LogError("ApproveToken(approvalTokenId=<redacted>): orderModel → {OrderModel}", orderModel);
        if (orderModel.LoginIsRequired)
            return RedirectToRoute(NopRouteNames.General.LOGIN, new { returnUrl = Url.RouteUrl(NopRouteNames.General.CART) });

        if (!orderModel.CheckoutIsEnabled)
            return RedirectToRoute(NopRouteNames.General.CART);

        if (!string.IsNullOrEmpty(orderModel.Error))
        {
            _notificationService.ErrorNotification(orderModel.Error);
            return RedirectToRoute(NopRouteNames.General.CART);
        }

        //order is created, let's approve it
        var liabilityShift = string.Empty;
        var approvedModel = await _modelFactory.PrepareOrderApprovedModelAsync(orderModel.OrderId, liabilityShift);
		_logger.LogError("ApproveToken(approvalTokenId=<redacted>): approvedModel → {ApprovedModel}", approvedModel);

        if (!string.IsNullOrEmpty(approvedModel.Error))
        {
            _notificationService.ErrorNotification(approvedModel.Error);
            return RedirectToRoute(NopRouteNames.General.CART);
        }

        //order is approved but the customer must confirm it before (if not yet completed)
        if (!approvedModel.PayNow)
            return RedirectToRoute(PayPalCommerceDefaults.Route.ConfirmOrder, new { orderId = approvedModel.OrderId, liabilityShift = liabilityShift });

        //or pay it right now
        var completedModel = await _modelFactory.PrepareOrderCompletedModelAsync(orderModel.OrderId, liabilityShift);
		_logger.LogError("ApproveToken(approvalTokenId=<redacted>): completedModel → {CompletedModel}", completedModel);
        if (!string.IsNullOrEmpty(completedModel.Error))
        {
            _notificationService.ErrorNotification(completedModel.Error);
            return RedirectToRoute(NopRouteNames.General.CART);
        }

		_logger.LogWarning("<<< Exiting ApproveToken(approvalTokenId=<redacted>)");
        return RedirectToRoute(NopRouteNames.Standard.CHECKOUT_COMPLETED, new { orderId = completedModel.OrderId });
    }

    #region Payment tokens

    public async Task<IActionResult> PaymentTokens()
    {
		_logger.LogError(">>> Entering PaymentTokens()");
        var model = await _modelFactory.PreparePaymentTokenListModelAsync();
        if (!model.VaultIsEnabled)
            return RedirectToRoute(NopRouteNames.General.CUSTOMER_INFO);

        if (!string.IsNullOrEmpty(model.Error))
            _notificationService.ErrorNotification(model.Error);

		_logger.LogWarning("<<< Exiting PaymentTokens()");
        return View("~/Plugins/Payments.PayPalCommerce/Views/Public/PaymentTokens.cshtml", model);

    }

    [HttpPost]
    public async Task<IActionResult> PaymentTokensDelete(int tokenId)
    {
		_logger.LogError(">>> Entering PaymentTokensDelete(tokenId=<redacted>)");
        var model = await _modelFactory.PreparePaymentTokenListModelAsync(deleteTokenId: tokenId);
        if (!model.VaultIsEnabled)
            return Json(new { redirect = Url.RouteUrl(NopRouteNames.General.CUSTOMER_INFO) });

        if (!string.IsNullOrEmpty(model.Error))
            return ErrorJson(model.Error);

        return Json(new { redirect = Url.RouteUrl(PayPalCommerceDefaults.Route.PaymentTokens) });
    }

    [HttpPost]
    public async Task<IActionResult> PaymentTokensMarkDefault(int tokenId)
    {
		_logger.LogError(">>> Entering PaymentTokensMarkDefault(tokenId=<redacted>)");
        var model = await _modelFactory.PreparePaymentTokenListModelAsync(defaultTokenId: tokenId);
        if (!model.VaultIsEnabled)
            return Json(new { redirect = Url.RouteUrl(NopRouteNames.General.CUSTOMER_INFO) });

        if (!string.IsNullOrEmpty(model.Error))
            return ErrorJson(model.Error);

        return Json(new { redirect = Url.RouteUrl(PayPalCommerceDefaults.Route.PaymentTokens) });
    }

    [HttpPost]
    public async Task<IActionResult> GetSavedCards(int placement)
    {
		_logger.LogError(">>> Entering GetSavedCards(placement={Placement})", placement);
        var model = await _modelFactory.PrepareSavedCardListModelAsync((ButtonPlacement)placement);
        if (!string.IsNullOrEmpty(model.Error))
            return ErrorJson(model.Error);

        if (model.PaymentTokens?.Any() != true)
            return Json(new { });

        var cards = model.PaymentTokens.Select(token => new { id = token.Id, label = token.Title }).ToArray();
		_logger.LogError("GetSavedCards(placement={Placement}): cards → {Cards}", placement, cards);

		_logger.LogWarning("<<< Exiting GetSavedCards(placement={Placement})", placement);
        return Json(new { cards = cards, defaultId = model.PaymentTokens.FirstOrDefault().Id });
    }

    #endregion

    #endregion
}