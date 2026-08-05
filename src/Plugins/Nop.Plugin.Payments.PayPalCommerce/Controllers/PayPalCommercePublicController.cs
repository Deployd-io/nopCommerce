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
        _localizationService = localizationService;
        _notificationService = notificationService;
        _webHelper = webHelper;
        _modelFactory = modelFactory;
    }

    #endregion

    #region Methods

    #region Checkout

    public async Task<IActionResult> PluginPaymentInfo()
    {
		_logger.LogDebug(">>> Entering PluginPaymentInfo()");
        var model = await _modelFactory.PrepareCheckoutPaymentInfoModelAsync();

		_logger.LogDebug("PluginPaymentInfo(): model → {Model}", model);
		_logger.LogDebug("<<< Exiting PluginPaymentInfo()");
        return View("~/Plugins/Payments.PayPalCommerce/Views/Public/PluginPaymentInfo.cshtml", model);
    }

    [CheckLanguageSeoCode(ignore: true)]
    public async Task<IActionResult> ValidateShoppingCart()
    {
		_logger.LogDebug(">>> Entering ValidateShoppingCart()");
        var warnings = await _modelFactory.GetShoppingCartWarningsAsync();
        if (warnings?.Any() ?? false)
			_logger.LogDebug("ValidateShoppingCart(): warnings → {Warnings}", warnings);
            return ErrorJson(warnings.ToArray());

        return Json(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder(int placement, string paymentSource, int cardId, bool saveCard)
    {
		_logger.LogDebug(">>> Entering CreateOrder(placement={Placement},paymentSource={PaymentSource},cardId={CardId},saveCard={SaveCard})", placement, paymentSource, cardId, saveCard);
        var model = await _modelFactory.PrepareOrderModelAsync((ButtonPlacement)placement, null, paymentSource, cardId, saveCard);
        if (model.LoginIsRequired)
			_logger.LogDebug("CreateOrder(placement={Placement},paymentSource={PaymentSource},cardId={CardId},saveCard={SaveCard}): model → {Model}", placement, paymentSource, cardId, saveCard, model);
            return Json(new { redirect = Url.RouteUrl(NopRouteNames.General.LOGIN, new { returnUrl = Url.RouteUrl(NopRouteNames.General.CART) }) });

        if (!model.CheckoutIsEnabled)
            return Json(new { redirect = Url.RouteUrl(NopRouteNames.General.CART) });

        if (!string.IsNullOrEmpty(model.Error))
            return ErrorJson(model.Error);

        //customer can complete 3D Secure if prompted
        if (cardId > 0 && !string.IsNullOrEmpty(model.PayerActionUrl))
		_logger.LogDebug("<<< Exiting CreateOrder(placement={Placement},paymentSource={PaymentSource},cardId={CardId},saveCard={SaveCard})", placement, paymentSource, cardId, saveCard);
            return Json(new { redirect = model.PayerActionUrl });

        return Json(new { orderId = model.OrderId, status = model.Status });
    }

    [HttpPost]
    public async Task<IActionResult> GetOrderStatus(int placement, string orderId)
    {
		_logger.LogDebug(">>> Entering GetOrderStatus(placement={Placement},orderId={OrderId})", placement, orderId);
        var model = await _modelFactory.PrepareOrderModelAsync((ButtonPlacement)placement, orderId, null, null, false);
        if (!string.IsNullOrEmpty(model.Error))
			_logger.LogDebug("GetOrderStatus(placement={Placement},orderId={OrderId}): model → {Model}", placement, orderId, model);
            return ErrorJson(model.Error);

        return Json(new { status = model.Status, payerAction = model.PayerActionUrl });
    }

    [HttpPost]
    public async Task<IActionResult> UpdateOrderShipping(OrderShippingModel model)
    {
        model = await _modelFactory.PrepareOrderShippingModelAsync(model);
        if (!string.IsNullOrEmpty(model.Error))
			_logger.LogDebug("UpdateOrderShipping(model={Model}): model → {Model1}", model, model);
            return ErrorJson(model.Error);

        return Json(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> ApproveOrder(string orderId, string liabilityShift)
    {
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
        if (!string.IsNullOrEmpty(completedModel.Error))
			_logger.LogDebug("ApproveOrder(orderId={OrderId},liabilityShift={LiabilityShift}): completedModel → {CompletedModel}", orderId, liabilityShift, completedModel);
            return ErrorJson(completedModel.Error);

        return Json(new { redirect = Url.RouteUrl(NopRouteNames.Standard.CHECKOUT_COMPLETED, new { orderId = completedModel.OrderId }) });
    }

    public async Task<IActionResult> ConfirmOrder(string orderId, string token, string liabilityShift, bool approve)
    {
        if (string.IsNullOrEmpty(liabilityShift))
            liabilityShift = _webHelper.QueryString<string>("liability_shift");

		_logger.LogDebug("ConfirmOrder(orderId={OrderId},token={Token},liabilityShift={LiabilityShift},approve={Approve}): liabilityShift → {LiabilityShift1}", orderId, token, liabilityShift, approve, liabilityShift);
        var model = await _modelFactory.PrepareOrderConfirmModelAsync(orderId, token, liabilityShift, approve);
        if (model.LoginIsRequired)
			_logger.LogDebug("ConfirmOrder(orderId={OrderId},token={Token},liabilityShift={LiabilityShift},approve={Approve}): model → {Model}", orderId, token, liabilityShift, approve, model);
            return RedirectToRoute(NopRouteNames.General.LOGIN, new { returnUrl = Url.RouteUrl(NopRouteNames.General.CART) });

        if (!model.CheckoutIsEnabled)
            return RedirectToRoute(NopRouteNames.General.CART);

        if (!string.IsNullOrEmpty(model.Error))
            _notificationService.ErrorNotification(model.Error);

		_logger.LogDebug("<<< Exiting ConfirmOrder(orderId={OrderId},token={Token},liabilityShift={LiabilityShift},approve={Approve})", orderId, token, liabilityShift, approve);
        return View("~/Plugins/Payments.PayPalCommerce/Views/Public/ConfirmOrder.cshtml", model);
    }

    [ValidateCaptcha]
    [HttpPost]
    public async Task<IActionResult> ConfirmOrderPost(string orderId, string orderGuid, string liabilityShift, bool captchaValid)
    {
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

        if (!string.IsNullOrEmpty(completedModel.Error))
        {
            _notificationService.ErrorNotification(completedModel.Error);
            return View("~/Plugins/Payments.PayPalCommerce/Views/Public/ConfirmOrder.cshtml", model);
        }

        if (!string.IsNullOrEmpty(completedModel.Warning))
            _notificationService.ErrorNotification(completedModel.Warning);

        return RedirectToRoute(NopRouteNames.Standard.CHECKOUT_COMPLETED, new { orderId = completedModel.OrderId });
    }

    [HttpPost]
    public async Task<IActionResult> AppleTransactionInfo(int placement)
    {
		_logger.LogDebug(">>> Entering AppleTransactionInfo(placement={Placement})", placement);
        var model = await _modelFactory.PrepareApplePayModelAsync((ButtonPlacement)placement);
        if (model.LoginIsRequired)
			_logger.LogDebug("AppleTransactionInfo(placement={Placement}): model → {Model}", placement, model);
            return Json(new { redirect = Url.RouteUrl(NopRouteNames.General.LOGIN, new { returnUrl = Url.RouteUrl(NopRouteNames.General.CART) }) });

        if (!model.CheckoutIsEnabled)
            return Json(new { redirect = Url.RouteUrl(NopRouteNames.General.CART) });

        if (!string.IsNullOrEmpty(model.Error))
            return ErrorJson(model.Error);

        var totalItem = model.Items.FirstOrDefault(item => item.Type == "TOTAL");
        var items = model.Items.Where(item => item.Type != "TOTAL");
		_logger.LogDebug("AppleTransactionInfo(placement={Placement}): totalItem → {TotalItem}", placement, totalItem);
		_logger.LogDebug("AppleTransactionInfo(placement={Placement}): items → {Items}", placement, items);

        var transactionInfo = new
        {
            currencyCode = model.CurrencyCode,
            billingContact = model.BillingAddress is null ? null : new
			_logger.LogDebug("AppleTransactionInfo(placement={Placement}): currencyCode → {CurrencyCode}", placement, currencyCode);
			_logger.LogDebug("AppleTransactionInfo(placement={Placement}): billingContact → {BillingContact}", placement, billingContact);
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
				_logger.LogDebug("AppleTransactionInfo(placement={Placement}): emailAddress → {EmailAddress}", placement, emailAddress);
				_logger.LogDebug("AppleTransactionInfo(placement={Placement}): givenName → {GivenName}", placement, givenName);
				_logger.LogDebug("AppleTransactionInfo(placement={Placement}): familyName → {FamilyName}", placement, familyName);
				_logger.LogDebug("AppleTransactionInfo(placement={Placement}): locality → {Locality}", placement, locality);
				_logger.LogDebug("AppleTransactionInfo(placement={Placement}): subAdministrativeArea → {SubAdministrativeArea}", placement, subAdministrativeArea);
				_logger.LogDebug("AppleTransactionInfo(placement={Placement}): administrativeArea → {AdministrativeArea}", placement, administrativeArea);
				_logger.LogDebug("AppleTransactionInfo(placement={Placement}): countryCode → {CountryCode}", placement, countryCode);
				_logger.LogDebug("AppleTransactionInfo(placement={Placement}): postalCode → {PostalCode}", placement, postalCode);
            },
            shippingContact = model.ShippingAddress is null ? null : new
			_logger.LogDebug("AppleTransactionInfo(placement={Placement}): shippingContact → {ShippingContact}", placement, shippingContact);
            {
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
				_logger.LogDebug("AppleTransactionInfo(placement={Placement}): shippingMethods → {ShippingMethods}", placement, shippingMethods);
                ?.Select(option => new { identifier = option.Id, amount = option.Price, label = option.Label, detail = option.Description })
                .ToArray(),
            total = new { label = totalItem.Label, type = totalItem.Status, amount = totalItem.Price },
            lineItems = items
			_logger.LogDebug("AppleTransactionInfo(placement={Placement}): total → {Total}", placement, total);
			_logger.LogDebug("AppleTransactionInfo(placement={Placement}): lineItems → {LineItems}", placement, lineItems);
                .Select(item => new { label = item.Label, type = item.Status, amount = item.Price })
                .ToArray()
        };

		_logger.LogDebug("<<< Exiting AppleTransactionInfo(placement={Placement})", placement);
        return Json(new { transactionInfo = transactionInfo });
    }

    [HttpPost]
    public async Task<IActionResult> UpdateAppleShipping(ApplePayShippingModel model)
    {
        model = await _modelFactory.PrepareApplePayShippingModelAsync(model);
        if (!string.IsNullOrEmpty(model.Error))
			_logger.LogDebug("UpdateAppleShipping(model={Model}): model → {Model1}", model, model);
            return ErrorJson(model.Error);

        var totalItem = model.Items.FirstOrDefault(item => item.Type == "TOTAL");
        var items = model.Items.Where(item => item.Type != "TOTAL");
		_logger.LogDebug("UpdateAppleShipping(model={Model}): totalItem → {TotalItem}", model, totalItem);
		_logger.LogDebug("UpdateAppleShipping(model={Model}): items → {Items}", model, items);

        //update total, items and shipping methods
        var shippingMethods = model.ShippingOptions
            ?.Select(option => new { identifier = option.Id, amount = option.Price, label = option.Label, detail = option.Description })
            .ToArray();
        var total = new { label = totalItem.Label, type = totalItem.Status, amount = totalItem.Price };
        var lineItems = items
			_logger.LogDebug("UpdateAppleShipping(model={Model}): total → {Total}", model, total);
            .Select(item => new { label = item.Label, type = item.Status, amount = item.Price })
            .ToArray();

		_logger.LogDebug("<<< Exiting UpdateAppleShipping(model={Model})", model);
        return Json(new { shippingMethods = shippingMethods, total = total, lineItems = lineItems });
    }

    [HttpPost]
    public async Task<IActionResult> GoogleTransactionInfo(int placement)
    {
		_logger.LogDebug(">>> Entering GoogleTransactionInfo(placement={Placement})", placement);
        var model = await _modelFactory.PrepareGooglePayModelAsync((ButtonPlacement)placement);
        if (model.LoginIsRequired)
			_logger.LogDebug("GoogleTransactionInfo(placement={Placement}): model → {Model}", placement, model);
            return Json(new { redirect = Url.RouteUrl(NopRouteNames.General.LOGIN, new { returnUrl = Url.RouteUrl(NopRouteNames.General.CART) }) });

        if (!model.CheckoutIsEnabled)
            return Json(new { redirect = Url.RouteUrl(NopRouteNames.General.CART) });

        if (!string.IsNullOrEmpty(model.Error))
            return ErrorJson(model.Error);

        var totalItem = model.Items.FirstOrDefault(item => item.Type == "TOTAL");
        var items = model.Items.Where(item => item.Type != "TOTAL");
		_logger.LogDebug("GoogleTransactionInfo(placement={Placement}): totalItem → {TotalItem}", placement, totalItem);
		_logger.LogDebug("GoogleTransactionInfo(placement={Placement}): items → {Items}", placement, items);

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
			_logger.LogDebug("GoogleTransactionInfo(placement={Placement}): currencyCode → {CurrencyCode}", placement, currencyCode);
			_logger.LogDebug("GoogleTransactionInfo(placement={Placement}): countryCode → {CountryCode}", placement, countryCode);
			_logger.LogDebug("GoogleTransactionInfo(placement={Placement}): checkoutOption → {CheckoutOption}", placement, checkoutOption);
			_logger.LogDebug("GoogleTransactionInfo(placement={Placement}): totalPriceStatus → {TotalPriceStatus}", placement, totalPriceStatus);
			_logger.LogDebug("GoogleTransactionInfo(placement={Placement}): totalPrice → {TotalPrice}", placement, totalPrice);
			_logger.LogDebug("GoogleTransactionInfo(placement={Placement}): totalPriceLabel → {TotalPriceLabel}", placement, totalPriceLabel);
                type = item.Type,
                label = item.Label,
                price = item.Price,
                status = item.Status
				_logger.LogDebug("GoogleTransactionInfo(placement={Placement}): type → {Type}", placement, type);
				_logger.LogDebug("GoogleTransactionInfo(placement={Placement}): label → {Label}", placement, label);
				_logger.LogDebug("GoogleTransactionInfo(placement={Placement}): price → {Price}", placement, price);
				_logger.LogDebug("GoogleTransactionInfo(placement={Placement}): status → {Status}", placement, status);
            }).ToArray()
        };

        var callbacks = new List<string> { "PAYMENT_AUTHORIZATION" };
        if (model.ShippingIsRequired)
			_logger.LogDebug("GoogleTransactionInfo(placement={Placement}): callbacks → {Callbacks}", placement, callbacks);
            callbacks.AddRange(new List<string> { "SHIPPING_ADDRESS", "SHIPPING_OPTION" });

		_logger.LogDebug("<<< Exiting GoogleTransactionInfo(placement={Placement})", placement);
        return Json(new { transactionInfo = transactionInfo, shipping = model.ShippingIsRequired, callbacks = callbacks.ToArray() });
    }

    [HttpPost]
    public async Task<IActionResult> CheckGoogleShipping(int placement, int? productId)
    {
		_logger.LogDebug(">>> Entering CheckGoogleShipping(placement={Placement},productId={ProductId})", placement, productId);
        var (shippingIsRequired, error) = await _modelFactory.CheckShippingIsRequiredAsync(productId);
        if (!string.IsNullOrEmpty(error))
		_logger.LogDebug("<<< Exiting CheckGoogleShipping(placement={Placement},productId={ProductId})", placement, productId);
            return ErrorJson(error);

        return Json(new { shippingIsRequired = shippingIsRequired && placement != (int)ButtonPlacement.PaymentMethod });
    }

    [HttpPost]
    public async Task<IActionResult> UpdateGoogleShipping(GooglePayShippingModel model)
    {
        model = await _modelFactory.PrepareGooglePayShippingModelAsync(model);
        if (!string.IsNullOrEmpty(model.Error))
			_logger.LogDebug("UpdateGoogleShipping(model={Model}): model → {Model1}", model, model);
            return ErrorJson(model.Error);

        var totalItem = model.Items.FirstOrDefault(item => item.Type == "TOTAL");
        var items = model.Items.Where(item => item.Type != "TOTAL");
		_logger.LogDebug("UpdateGoogleShipping(model={Model}): totalItem → {TotalItem}", model, totalItem);
		_logger.LogDebug("UpdateGoogleShipping(model={Model}): items → {Items}", model, items);

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
			_logger.LogDebug("UpdateGoogleShipping(model={Model}): currencyCode → {CurrencyCode}", model, currencyCode);
			_logger.LogDebug("UpdateGoogleShipping(model={Model}): countryCode → {CountryCode}", model, countryCode);
			_logger.LogDebug("UpdateGoogleShipping(model={Model}): checkoutOption → {CheckoutOption}", model, checkoutOption);
			_logger.LogDebug("UpdateGoogleShipping(model={Model}): totalPriceStatus → {TotalPriceStatus}", model, totalPriceStatus);
			_logger.LogDebug("UpdateGoogleShipping(model={Model}): totalPrice → {TotalPrice}", model, totalPrice);
			_logger.LogDebug("UpdateGoogleShipping(model={Model}): totalPriceLabel → {TotalPriceLabel}", model, totalPriceLabel);
                type = item.Type,
                label = item.Label,
                price = item.Price,
                status = item.Status
				_logger.LogDebug("UpdateGoogleShipping(model={Model}): type → {Type}", model, type);
				_logger.LogDebug("UpdateGoogleShipping(model={Model}): label → {Label}", model, label);
				_logger.LogDebug("UpdateGoogleShipping(model={Model}): price → {Price}", model, price);
				_logger.LogDebug("UpdateGoogleShipping(model={Model}): status → {Status}", model, status);
            }).ToArray()
        };

        var options = model.Options?.Select(option => new
        {
            id = option.Id,
            label = option.Label,
            description = option.Description
			_logger.LogDebug("UpdateGoogleShipping(model={Model}): description → {Description}", model, description);
        }).ToArray();

		_logger.LogDebug("<<< Exiting UpdateGoogleShipping(model={Model})", model);
        return Json(new { transactionInfo = transactionInfo, options = new { shippingOptions = options, defaultSelectedOptionId = model.OptionId } });
    }

    #endregion

    [HttpPost]
    public async Task<IActionResult> CreateSetupToken()
    {
		_logger.LogDebug(">>> Entering CreateSetupToken()");
        var model = await _modelFactory.PrepareSetupTokenModelAsync();
        if (model.LoginIsRequired)
			_logger.LogDebug("CreateSetupToken(): model → {Model}", model);
            return Json(new { redirect = Url.RouteUrl(NopRouteNames.General.LOGIN, new { returnUrl = Url.RouteUrl(NopRouteNames.General.CART) }) });

        if (!model.CheckoutIsEnabled)
            return Json(new { redirect = Url.RouteUrl(NopRouteNames.General.CART) });

        if (!string.IsNullOrEmpty(model.Error))
		_logger.LogDebug("<<< Exiting CreateSetupToken()");
            return ErrorJson(model.Error);

        return Json(new { status = model.Status, redirect = model.PayerActionUrl });
    }

    public async Task<IActionResult> ApproveToken(string approvalTokenId)
    {
		_logger.LogDebug(">>> Entering ApproveToken(approvalTokenId={ApprovalTokenId})", approvalTokenId);
        if (string.IsNullOrEmpty(approvalTokenId))
            approvalTokenId = _webHelper.QueryString<string>("approval_token_id");

		_logger.LogDebug("ApproveToken(approvalTokenId={ApprovalTokenId}): approvalTokenId → {ApprovalTokenId1}", approvalTokenId, approvalTokenId);
        //create new recurring order
        var orderModel = await _modelFactory.PrepareRecurringOrderModelAsync(approvalTokenId);
        if (orderModel.LoginIsRequired)
			_logger.LogDebug("ApproveToken(approvalTokenId={ApprovalTokenId}): orderModel → {OrderModel}", approvalTokenId, orderModel);
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

		_logger.LogDebug("ApproveToken(approvalTokenId={ApprovalTokenId}): approvedModel → {ApprovedModel}", approvalTokenId, approvedModel);
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
        if (!string.IsNullOrEmpty(completedModel.Error))
		_logger.LogDebug("ApproveToken(approvalTokenId={ApprovalTokenId}): completedModel → {CompletedModel}", approvalTokenId, completedModel);
        {
            _notificationService.ErrorNotification(completedModel.Error);
            return RedirectToRoute(NopRouteNames.General.CART);
        }

		_logger.LogDebug("<<< Exiting ApproveToken(approvalTokenId={ApprovalTokenId})", approvalTokenId);
        return RedirectToRoute(NopRouteNames.Standard.CHECKOUT_COMPLETED, new { orderId = completedModel.OrderId });
    }

    #region Payment tokens

    public async Task<IActionResult> PaymentTokens()
    {
		_logger.LogDebug(">>> Entering PaymentTokens()");
        var model = await _modelFactory.PreparePaymentTokenListModelAsync();
        if (!model.VaultIsEnabled)
			_logger.LogDebug("PaymentTokens(): model → {Model}", model);
            return RedirectToRoute(NopRouteNames.General.CUSTOMER_INFO);

        if (!string.IsNullOrEmpty(model.Error))
            _notificationService.ErrorNotification(model.Error);

		_logger.LogDebug("<<< Exiting PaymentTokens()");
        return View("~/Plugins/Payments.PayPalCommerce/Views/Public/PaymentTokens.cshtml", model);

    }

    [HttpPost]
    public async Task<IActionResult> PaymentTokensDelete(int tokenId)
    {
		_logger.LogDebug(">>> Entering PaymentTokensDelete(tokenId={TokenId})", tokenId);
        var model = await _modelFactory.PreparePaymentTokenListModelAsync(deleteTokenId: tokenId);
        if (!model.VaultIsEnabled)
			_logger.LogDebug("PaymentTokensDelete(tokenId={TokenId}): model → {Model}", tokenId, model);
            return Json(new { redirect = Url.RouteUrl(NopRouteNames.General.CUSTOMER_INFO) });

        if (!string.IsNullOrEmpty(model.Error))
		_logger.LogDebug("<<< Exiting PaymentTokensDelete(tokenId={TokenId})", tokenId);
            return ErrorJson(model.Error);

        return Json(new { redirect = Url.RouteUrl(PayPalCommerceDefaults.Route.PaymentTokens) });
    }

    [HttpPost]
    public async Task<IActionResult> PaymentTokensMarkDefault(int tokenId)
    {
		_logger.LogDebug(">>> Entering PaymentTokensMarkDefault(tokenId={TokenId})", tokenId);
        var model = await _modelFactory.PreparePaymentTokenListModelAsync(defaultTokenId: tokenId);
        if (!model.VaultIsEnabled)
			_logger.LogDebug("PaymentTokensMarkDefault(tokenId={TokenId}): model → {Model}", tokenId, model);
            return Json(new { redirect = Url.RouteUrl(NopRouteNames.General.CUSTOMER_INFO) });

        if (!string.IsNullOrEmpty(model.Error))
		_logger.LogDebug("<<< Exiting PaymentTokensMarkDefault(tokenId={TokenId})", tokenId);
            return ErrorJson(model.Error);

        return Json(new { redirect = Url.RouteUrl(PayPalCommerceDefaults.Route.PaymentTokens) });
    }

    [HttpPost]
    public async Task<IActionResult> GetSavedCards(int placement)
    {
		_logger.LogDebug(">>> Entering GetSavedCards(placement={Placement})", placement);
        var model = await _modelFactory.PrepareSavedCardListModelAsync((ButtonPlacement)placement);
        if (!string.IsNullOrEmpty(model.Error))
			_logger.LogDebug("GetSavedCards(placement={Placement}): model → {Model}", placement, model);
            return ErrorJson(model.Error);

        if (model.PaymentTokens?.Any() != true)
            return Json(new { });

        var cards = model.PaymentTokens.Select(token => new { id = token.Id, label = token.Title }).ToArray();

		_logger.LogDebug("GetSavedCards(placement={Placement}): cards → {Cards}", placement, cards);
		_logger.LogDebug("<<< Exiting GetSavedCards(placement={Placement})", placement);
        return Json(new { cards = cards, defaultId = model.PaymentTokens.FirstOrDefault().Id });
    }

    #endregion

    #endregion
}