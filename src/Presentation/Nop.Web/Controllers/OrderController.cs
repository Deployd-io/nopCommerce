using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Http;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Shipping;
using Nop.Web.Factories;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Controllers;

[AutoValidateAntiforgeryToken]
public partial class OrderController : BasePublicController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly ICustomerService _customerService;
    protected readonly ILocalizationService _localizationService;
    protected readonly INotificationService _notificationService;
    protected readonly IOrderModelFactory _orderModelFactory;
    protected readonly IOrderProcessingService _orderProcessingService;
    protected readonly IOrderService _orderService;
    protected readonly IPaymentService _paymentService;
    protected readonly IPdfService _pdfService;
    protected readonly IShipmentService _shipmentService;
    protected readonly IWebHelper _webHelper;
    protected readonly IWorkContext _workContext;
    protected readonly OrderSettings _orderSettings;
    protected readonly RewardPointsSettings _rewardPointsSettings;

    #endregion

    #region Ctor

    public OrderController(ICustomerService customerService,
        ILocalizationService localizationService,
        INotificationService notificationService,
        IOrderModelFactory orderModelFactory,
        IOrderProcessingService orderProcessingService,
        IOrderService orderService,
        IPaymentService paymentService,
        IPdfService pdfService,
        IShipmentService shipmentService,
        IWebHelper webHelper,
        IWorkContext workContext,
        OrderSettings orderSettings,
        RewardPointsSettings rewardPointsSettings)
    {
		_logger.LogDebug(">>> Entering OrderController(customerService={CustomerService},localizationService={LocalizationService},notificationService={NotificationService},orderModelFactory={OrderModelFactory},orderProcessingService={OrderProcessingService},orderService={OrderService},paymentService={PaymentService},pdfService={PdfService},shipmentService={ShipmentService},webHelper={WebHelper},workContext={WorkContext},orderSettings={OrderSettings},rewardPointsSettings={RewardPointsSettings})", customerService, localizationService, notificationService, orderModelFactory, orderProcessingService, orderService, paymentService, pdfService, shipmentService, webHelper, workContext, orderSettings, rewardPointsSettings);
        _customerService = customerService;
		_logger.LogDebug("OrderController(customerService={CustomerService},localizationService={LocalizationService},notificationService={NotificationService},orderModelFactory={OrderModelFactory},orderProcessingService={OrderProcessingService},orderService={OrderService},paymentService={PaymentService},pdfService={PdfService},shipmentService={ShipmentService},webHelper={WebHelper},workContext={WorkContext},orderSettings={OrderSettings},rewardPointsSettings={RewardPointsSettings}): _customerService → {_customerService}", customerService, localizationService, notificationService, orderModelFactory, orderProcessingService, orderService, paymentService, pdfService, shipmentService, webHelper, workContext, orderSettings, rewardPointsSettings, _customerService);
        _localizationService = localizationService;
		_logger.LogDebug("OrderController(customerService={CustomerService},localizationService={LocalizationService},notificationService={NotificationService},orderModelFactory={OrderModelFactory},orderProcessingService={OrderProcessingService},orderService={OrderService},paymentService={PaymentService},pdfService={PdfService},shipmentService={ShipmentService},webHelper={WebHelper},workContext={WorkContext},orderSettings={OrderSettings},rewardPointsSettings={RewardPointsSettings}): _localizationService → {_localizationService}", customerService, localizationService, notificationService, orderModelFactory, orderProcessingService, orderService, paymentService, pdfService, shipmentService, webHelper, workContext, orderSettings, rewardPointsSettings, _localizationService);
        _notificationService = notificationService;
		_logger.LogDebug("OrderController(customerService={CustomerService},localizationService={LocalizationService},notificationService={NotificationService},orderModelFactory={OrderModelFactory},orderProcessingService={OrderProcessingService},orderService={OrderService},paymentService={PaymentService},pdfService={PdfService},shipmentService={ShipmentService},webHelper={WebHelper},workContext={WorkContext},orderSettings={OrderSettings},rewardPointsSettings={RewardPointsSettings}): _notificationService → {_notificationService}", customerService, localizationService, notificationService, orderModelFactory, orderProcessingService, orderService, paymentService, pdfService, shipmentService, webHelper, workContext, orderSettings, rewardPointsSettings, _notificationService);
        _orderModelFactory = orderModelFactory;
		_logger.LogDebug("OrderController(customerService={CustomerService},localizationService={LocalizationService},notificationService={NotificationService},orderModelFactory={OrderModelFactory},orderProcessingService={OrderProcessingService},orderService={OrderService},paymentService={PaymentService},pdfService={PdfService},shipmentService={ShipmentService},webHelper={WebHelper},workContext={WorkContext},orderSettings={OrderSettings},rewardPointsSettings={RewardPointsSettings}): _orderModelFactory → {_orderModelFactory}", customerService, localizationService, notificationService, orderModelFactory, orderProcessingService, orderService, paymentService, pdfService, shipmentService, webHelper, workContext, orderSettings, rewardPointsSettings, _orderModelFactory);
        _orderProcessingService = orderProcessingService;
		_logger.LogDebug("OrderController(customerService={CustomerService},localizationService={LocalizationService},notificationService={NotificationService},orderModelFactory={OrderModelFactory},orderProcessingService={OrderProcessingService},orderService={OrderService},paymentService={PaymentService},pdfService={PdfService},shipmentService={ShipmentService},webHelper={WebHelper},workContext={WorkContext},orderSettings={OrderSettings},rewardPointsSettings={RewardPointsSettings}): _orderProcessingService → {_orderProcessingService}", customerService, localizationService, notificationService, orderModelFactory, orderProcessingService, orderService, paymentService, pdfService, shipmentService, webHelper, workContext, orderSettings, rewardPointsSettings, _orderProcessingService);
        _orderService = orderService;
		_logger.LogDebug("OrderController(customerService={CustomerService},localizationService={LocalizationService},notificationService={NotificationService},orderModelFactory={OrderModelFactory},orderProcessingService={OrderProcessingService},orderService={OrderService},paymentService={PaymentService},pdfService={PdfService},shipmentService={ShipmentService},webHelper={WebHelper},workContext={WorkContext},orderSettings={OrderSettings},rewardPointsSettings={RewardPointsSettings}): _orderService → {_orderService}", customerService, localizationService, notificationService, orderModelFactory, orderProcessingService, orderService, paymentService, pdfService, shipmentService, webHelper, workContext, orderSettings, rewardPointsSettings, _orderService);
        _paymentService = paymentService;
		_logger.LogDebug("OrderController(customerService={CustomerService},localizationService={LocalizationService},notificationService={NotificationService},orderModelFactory={OrderModelFactory},orderProcessingService={OrderProcessingService},orderService={OrderService},paymentService={PaymentService},pdfService={PdfService},shipmentService={ShipmentService},webHelper={WebHelper},workContext={WorkContext},orderSettings={OrderSettings},rewardPointsSettings={RewardPointsSettings}): _paymentService → {_paymentService}", customerService, localizationService, notificationService, orderModelFactory, orderProcessingService, orderService, paymentService, pdfService, shipmentService, webHelper, workContext, orderSettings, rewardPointsSettings, _paymentService);
        _pdfService = pdfService;
		_logger.LogDebug("OrderController(customerService={CustomerService},localizationService={LocalizationService},notificationService={NotificationService},orderModelFactory={OrderModelFactory},orderProcessingService={OrderProcessingService},orderService={OrderService},paymentService={PaymentService},pdfService={PdfService},shipmentService={ShipmentService},webHelper={WebHelper},workContext={WorkContext},orderSettings={OrderSettings},rewardPointsSettings={RewardPointsSettings}): _pdfService → {_pdfService}", customerService, localizationService, notificationService, orderModelFactory, orderProcessingService, orderService, paymentService, pdfService, shipmentService, webHelper, workContext, orderSettings, rewardPointsSettings, _pdfService);
        _shipmentService = shipmentService;
		_logger.LogDebug("OrderController(customerService={CustomerService},localizationService={LocalizationService},notificationService={NotificationService},orderModelFactory={OrderModelFactory},orderProcessingService={OrderProcessingService},orderService={OrderService},paymentService={PaymentService},pdfService={PdfService},shipmentService={ShipmentService},webHelper={WebHelper},workContext={WorkContext},orderSettings={OrderSettings},rewardPointsSettings={RewardPointsSettings}): _shipmentService → {_shipmentService}", customerService, localizationService, notificationService, orderModelFactory, orderProcessingService, orderService, paymentService, pdfService, shipmentService, webHelper, workContext, orderSettings, rewardPointsSettings, _shipmentService);
        _webHelper = webHelper;
		_logger.LogDebug("OrderController(customerService={CustomerService},localizationService={LocalizationService},notificationService={NotificationService},orderModelFactory={OrderModelFactory},orderProcessingService={OrderProcessingService},orderService={OrderService},paymentService={PaymentService},pdfService={PdfService},shipmentService={ShipmentService},webHelper={WebHelper},workContext={WorkContext},orderSettings={OrderSettings},rewardPointsSettings={RewardPointsSettings}): _webHelper → {_webHelper}", customerService, localizationService, notificationService, orderModelFactory, orderProcessingService, orderService, paymentService, pdfService, shipmentService, webHelper, workContext, orderSettings, rewardPointsSettings, _webHelper);
        _workContext = workContext;
		_logger.LogDebug("OrderController(customerService={CustomerService},localizationService={LocalizationService},notificationService={NotificationService},orderModelFactory={OrderModelFactory},orderProcessingService={OrderProcessingService},orderService={OrderService},paymentService={PaymentService},pdfService={PdfService},shipmentService={ShipmentService},webHelper={WebHelper},workContext={WorkContext},orderSettings={OrderSettings},rewardPointsSettings={RewardPointsSettings}): _workContext → {_workContext}", customerService, localizationService, notificationService, orderModelFactory, orderProcessingService, orderService, paymentService, pdfService, shipmentService, webHelper, workContext, orderSettings, rewardPointsSettings, _workContext);
        _orderSettings = orderSettings;
		_logger.LogDebug("OrderController(customerService={CustomerService},localizationService={LocalizationService},notificationService={NotificationService},orderModelFactory={OrderModelFactory},orderProcessingService={OrderProcessingService},orderService={OrderService},paymentService={PaymentService},pdfService={PdfService},shipmentService={ShipmentService},webHelper={WebHelper},workContext={WorkContext},orderSettings={OrderSettings},rewardPointsSettings={RewardPointsSettings}): _orderSettings → {_orderSettings}", customerService, localizationService, notificationService, orderModelFactory, orderProcessingService, orderService, paymentService, pdfService, shipmentService, webHelper, workContext, orderSettings, rewardPointsSettings, _orderSettings);
        _rewardPointsSettings = rewardPointsSettings;
		_logger.LogDebug("<<< Exiting OrderController(customerService={CustomerService},localizationService={LocalizationService},notificationService={NotificationService},orderModelFactory={OrderModelFactory},orderProcessingService={OrderProcessingService},orderService={OrderService},paymentService={PaymentService},pdfService={PdfService},shipmentService={ShipmentService},webHelper={WebHelper},workContext={WorkContext},orderSettings={OrderSettings},rewardPointsSettings={RewardPointsSettings})", customerService, localizationService, notificationService, orderModelFactory, orderProcessingService, orderService, paymentService, pdfService, shipmentService, webHelper, workContext, orderSettings, rewardPointsSettings);
		_logger.LogDebug("OrderController(customerService={CustomerService},localizationService={LocalizationService},notificationService={NotificationService},orderModelFactory={OrderModelFactory},orderProcessingService={OrderProcessingService},orderService={OrderService},paymentService={PaymentService},pdfService={PdfService},shipmentService={ShipmentService},webHelper={WebHelper},workContext={WorkContext},orderSettings={OrderSettings},rewardPointsSettings={RewardPointsSettings}): _rewardPointsSettings → {_rewardPointsSettings}", customerService, localizationService, notificationService, orderModelFactory, orderProcessingService, orderService, paymentService, pdfService, shipmentService, webHelper, workContext, orderSettings, rewardPointsSettings, _rewardPointsSettings);
    }

    #endregion

    #region Methods

    //My account / Orders
    public virtual async Task<IActionResult> CustomerOrders(int? pageNumber, OrderHistoryPeriods limit)
    {
		_logger.LogDebug(">>> Entering CustomerOrders(pageNumber={PageNumber},limit={Limit})", pageNumber, limit);
        if (!await _customerService.IsRegisteredAsync(await _workContext.GetCurrentCustomerAsync()))
            return Challenge();

        var model = await _orderModelFactory.PrepareCustomerOrderListModelAsync(pageNumber, limit);
		_logger.LogDebug("CustomerOrders(pageNumber={PageNumber},limit={Limit}): model → {Model}", pageNumber, limit, model);
		_logger.LogDebug("<<< Exiting CustomerOrders(pageNumber={PageNumber},limit={Limit})", pageNumber, limit);
        return View(model);
    }

    //My account / Recurring payments
    public virtual async Task<IActionResult> CustomerRecurringPayments()
    {
		_logger.LogDebug(">>> Entering CustomerRecurringPayments()");
        if (!await _customerService.IsRegisteredAsync(await _workContext.GetCurrentCustomerAsync()))
            return Challenge();

        var model = await _orderModelFactory.PrepareCustomerRecurringPaymentListModelAsync();
		_logger.LogDebug("CustomerRecurringPayments(): model → {Model}", model);
		_logger.LogDebug("<<< Exiting CustomerRecurringPayments()");
        return View(model);
    }

    //My account / Orders / Cancel recurring order
    [HttpPost, ActionName("CustomerRecurringPayments")]
    [FormValueRequired(FormValueRequirement.StartsWith, "cancelRecurringPayment")]
    public virtual async Task<IActionResult> CancelRecurringPayment(IFormCollection form)
    {
		_logger.LogDebug(">>> Entering CancelRecurringPayment(form={Form})", form);
        var customer = await _workContext.GetCurrentCustomerAsync();
        if (!await _customerService.IsRegisteredAsync(customer))
			_logger.LogDebug("CancelRecurringPayment(form={Form}): customer → {Customer}", form, customer);
            return Challenge();

        //get recurring payment identifier
        var recurringPaymentId = 0;
		_logger.LogDebug("CancelRecurringPayment(form={Form}): recurringPaymentId → {RecurringPaymentId}", form, recurringPaymentId);
        foreach (var formValue in form.Keys)
            if (formValue.StartsWith("cancelRecurringPayment", StringComparison.InvariantCultureIgnoreCase))
                recurringPaymentId = Convert.ToInt32(formValue["cancelRecurringPayment".Length..]);

        var recurringPayment = await _orderService.GetRecurringPaymentByIdAsync(recurringPaymentId);
		_logger.LogDebug("CancelRecurringPayment(form={Form}): recurringPayment → {RecurringPayment}", form, recurringPayment);
        if (recurringPayment == null)
        {
            return RedirectToRoute(NopRouteNames.Standard.CUSTOMER_RECURRING_PAYMENTS);
        }

        if (await _orderProcessingService.CanCancelRecurringPaymentAsync(customer, recurringPayment))
        {
            var errors = await _orderProcessingService.CancelRecurringPaymentAsync(recurringPayment);
			_logger.LogDebug("CancelRecurringPayment(form={Form}): errors → {Errors}", form, errors);

            var model = await _orderModelFactory.PrepareCustomerRecurringPaymentListModelAsync();
			_logger.LogDebug("CancelRecurringPayment(form={Form}): model → {Model}", form, model);
            model.RecurringPaymentErrors = errors.ToList();

		_logger.LogDebug("<<< Exiting CancelRecurringPayment(form={Form})", form);
            return View(model);
        }

        return RedirectToRoute(NopRouteNames.Standard.CUSTOMER_RECURRING_PAYMENTS);
    }

    //My account / Orders / Retry last recurring order
    [HttpPost, ActionName("CustomerRecurringPayments")]
    [FormValueRequired(FormValueRequirement.StartsWith, "retryLastPayment")]
    public virtual async Task<IActionResult> RetryLastRecurringPayment(IFormCollection form)
    {
		_logger.LogDebug(">>> Entering RetryLastRecurringPayment(form={Form})", form);
        var customer = await _workContext.GetCurrentCustomerAsync();
        if (!await _customerService.IsRegisteredAsync(customer))
			_logger.LogDebug("RetryLastRecurringPayment(form={Form}): customer → {Customer}", form, customer);
            return Challenge();

        //get recurring payment identifier
        var recurringPaymentId = 0;
        if (!form.Keys.Any(formValue => formValue.StartsWith("retryLastPayment", StringComparison.InvariantCultureIgnoreCase) &&
                                        int.TryParse(formValue[(formValue.IndexOf('_') + 1)..], out recurringPaymentId)))
        {
            return RedirectToRoute(NopRouteNames.Standard.CUSTOMER_RECURRING_PAYMENTS);
        }

        var recurringPayment = await _orderService.GetRecurringPaymentByIdAsync(recurringPaymentId);
		_logger.LogDebug("RetryLastRecurringPayment(form={Form}): recurringPayment → {RecurringPayment}", form, recurringPayment);
        if (recurringPayment == null)
            return RedirectToRoute(NopRouteNames.Standard.CUSTOMER_RECURRING_PAYMENTS);

        if (!await _orderProcessingService.CanRetryLastRecurringPaymentAsync(customer, recurringPayment))
            return RedirectToRoute(NopRouteNames.Standard.CUSTOMER_RECURRING_PAYMENTS);

        var errors = await _orderProcessingService.ProcessNextRecurringPaymentAsync(recurringPayment);
		_logger.LogDebug("RetryLastRecurringPayment(form={Form}): errors → {Errors}", form, errors);
        var model = await _orderModelFactory.PrepareCustomerRecurringPaymentListModelAsync();
		_logger.LogDebug("RetryLastRecurringPayment(form={Form}): model → {Model}", form, model);
        model.RecurringPaymentErrors = errors.ToList();

		_logger.LogDebug("<<< Exiting RetryLastRecurringPayment(form={Form})", form);
        return View(model);
    }

    //My account / Reward points
    public virtual async Task<IActionResult> CustomerRewardPoints(int? pageNumber)
    {
		_logger.LogDebug(">>> Entering CustomerRewardPoints(pageNumber={PageNumber})", pageNumber);
        if (!await _customerService.IsRegisteredAsync(await _workContext.GetCurrentCustomerAsync()))
            return Challenge();

        if (!_rewardPointsSettings.Enabled)
            return RedirectToRoute(NopRouteNames.General.CUSTOMER_INFO);

        var model = await _orderModelFactory.PrepareCustomerRewardPointsAsync(pageNumber);
		_logger.LogDebug("CustomerRewardPoints(pageNumber={PageNumber}): model → {Model}", pageNumber, model);
		_logger.LogDebug("<<< Exiting CustomerRewardPoints(pageNumber={PageNumber})", pageNumber);
        return View(model);
    }

    //My account / Order details page
    public virtual async Task<IActionResult> Details(int orderId)
    {
		_logger.LogDebug(">>> Entering Details(orderId={OrderId})", orderId);
        var order = await _orderService.GetOrderByIdAsync(orderId);
        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogDebug("Details(orderId={OrderId}): customer → {Customer}", orderId, customer);
		_logger.LogDebug("Details(orderId={OrderId}): order → {Order}", orderId, order);

        if (order == null || order.Deleted || customer.Id != order.CustomerId)
            return Challenge();

        var model = await _orderModelFactory.PrepareOrderDetailsModelAsync(order);
		_logger.LogDebug("Details(orderId={OrderId}): model → {Model}", orderId, model);
		_logger.LogDebug("<<< Exiting Details(orderId={OrderId})", orderId);
        return View(model);
    }

    //My account / Order details page / Print
    public virtual async Task<IActionResult> PrintOrderDetails(int orderId)
    {
		_logger.LogDebug(">>> Entering PrintOrderDetails(orderId={OrderId})", orderId);
        var order = await _orderService.GetOrderByIdAsync(orderId);
        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogDebug("PrintOrderDetails(orderId={OrderId}): customer → {Customer}", orderId, customer);
        if (order == null || order.Deleted || customer.Id != order.CustomerId)
			_logger.LogDebug("PrintOrderDetails(orderId={OrderId}): order → {Order}", orderId, order);
            return Challenge();

        var model = await _orderModelFactory.PrepareOrderDetailsModelAsync(order);
		_logger.LogDebug("PrintOrderDetails(orderId={OrderId}): model → {Model}", orderId, model);
		_logger.LogDebug("PrintOrderDetails(orderId={OrderId}): model.PrintMode → {Arg1}", orderId, model.PrintMode);
        model.PrintMode = true;

		_logger.LogDebug("<<< Exiting PrintOrderDetails(orderId={OrderId})", orderId);
        return View("Details", model);
    }

    //My account / Order details page / PDF invoice
    [CheckLanguageSeoCode(ignore: true)]
    public virtual async Task<IActionResult> GetPdfInvoice(int orderId)
    {
		_logger.LogDebug(">>> Entering GetPdfInvoice(orderId={OrderId})", orderId);
        var order = await _orderService.GetOrderByIdAsync(orderId);
        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogDebug("GetPdfInvoice(orderId={OrderId}): customer → {Customer}", orderId, customer);
        if (order == null || order.Deleted || customer.Id != order.CustomerId)
			_logger.LogDebug("GetPdfInvoice(orderId={OrderId}): order → {Order}", orderId, order);
            return Challenge();

        byte[] bytes;
        await using (var stream = new MemoryStream())
        {
            await _pdfService.PrintOrderToPdfAsync(stream, order, await _workContext.GetWorkingLanguageAsync());
            bytes = stream.ToArray();
        }
		_logger.LogDebug("<<< Exiting GetPdfInvoice(orderId={OrderId})", orderId);
        return File(bytes, MimeTypes.ApplicationPdf, string.Format(await _localizationService.GetResourceAsync("PDFInvoice.FileName"), order.CustomOrderNumber) + ".pdf");
    }

    public async Task<IActionResult> CancelOrder(int orderId)
    {
		_logger.LogDebug(">>> Entering CancelOrder(orderId={OrderId})", orderId);
        if(!_orderSettings.AllowCustomersCancelOrders)
            return RedirectToRoute(NopRouteNames.Standard.ORDER_DETAILS, new { orderId });

        var order = await _orderService.GetOrderByIdAsync(orderId);
		_logger.LogDebug("CancelOrder(orderId={OrderId}): order → {Order}", orderId, order);
        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogDebug("CancelOrder(orderId={OrderId}): customer → {Customer}", orderId, customer);
        if (order == null || customer.Id != order.CustomerId)
            return Challenge();
        
        try
        {
            if (_orderProcessingService.CanCancelOrder(order))
                await _orderProcessingService.CancelOrderAsync(order, false);
        }
        catch
        {
			_logger.LogError("Exception in CancelOrder(orderId={OrderId}): {Arg1}", orderId, ex.Message);
            _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Order.Cancel.Failed"));
            return RedirectToRoute(NopRouteNames.Standard.ORDER_DETAILS, new { orderId });
        }

        await _orderService.UpdateOrderAsync(order);
       _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Order.Cancelled"));

		_logger.LogDebug("<<< Exiting CancelOrder(orderId={OrderId})", orderId);
        return RedirectToRoute(NopRouteNames.Standard.ORDER_DETAILS, new { orderId });
    }

    //My account / Order details page / re-order
    public virtual async Task<IActionResult> ReOrder(int orderId)
    {
		_logger.LogDebug(">>> Entering ReOrder(orderId={OrderId})", orderId);
        var order = await _orderService.GetOrderByIdAsync(orderId);
        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogDebug("ReOrder(orderId={OrderId}): customer → {Customer}", orderId, customer);
        if (order == null || order.Deleted || customer.Id != order.CustomerId)
			_logger.LogDebug("ReOrder(orderId={OrderId}): order → {Order}", orderId, order);
            return Challenge();

        var warnings = await _orderProcessingService.ReOrderAsync(order);
		_logger.LogDebug("ReOrder(orderId={OrderId}): warnings → {Warnings}", orderId, warnings);

        if (warnings.Any())
            _notificationService.WarningNotification(await _localizationService.GetResourceAsync("ShoppingCart.ReorderWarning"));

		_logger.LogDebug("<<< Exiting ReOrder(orderId={OrderId})", orderId);
        return RedirectToRoute(NopRouteNames.General.CART);
    }

    //My account / Order details page / Complete payment
    [HttpPost, ActionName("Details")]

    [FormValueRequired("repost-payment")]
    public virtual async Task<IActionResult> RePostPayment(int orderId)
    {
		_logger.LogDebug(">>> Entering RePostPayment(orderId={OrderId})", orderId);
        var order = await _orderService.GetOrderByIdAsync(orderId);
        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogDebug("RePostPayment(orderId={OrderId}): customer → {Customer}", orderId, customer);
        if (order == null || order.Deleted || customer.Id != order.CustomerId)
			_logger.LogDebug("RePostPayment(orderId={OrderId}): order → {Order}", orderId, order);
            return Challenge();

        if (!await _paymentService.CanRePostProcessPaymentAsync(order))
            return RedirectToRoute(NopRouteNames.Standard.ORDER_DETAILS, new { orderId = orderId });

        var postProcessPaymentRequest = new PostProcessPaymentRequest
        {
            Order = order
        };
        await _paymentService.PostProcessPaymentAsync(postProcessPaymentRequest);

        if (_webHelper.IsRequestBeingRedirected || _webHelper.IsPostBeingDone)
        {
            //redirection or POST has been done in PostProcessPayment
		_logger.LogDebug("<<< Exiting RePostPayment(orderId={OrderId})", orderId);
            return new EmptyResult();
        }

        //if no redirection has been done (to a third-party payment page)
        //theoretically it's not possible
        return RedirectToRoute(NopRouteNames.Standard.ORDER_DETAILS, new { orderId = orderId });
    }

    //My account / Order details page / Shipment details page
    public virtual async Task<IActionResult> ShipmentDetails(int shipmentId)
    {
		_logger.LogDebug(">>> Entering ShipmentDetails(shipmentId={ShipmentId})", shipmentId);
        var shipment = await _shipmentService.GetShipmentByIdAsync(shipmentId);
        if (shipment == null)
			_logger.LogDebug("ShipmentDetails(shipmentId={ShipmentId}): shipment → {Shipment}", shipmentId, shipment);
            return Challenge();

        var order = await _orderService.GetOrderByIdAsync(shipment.OrderId);
		_logger.LogDebug("ShipmentDetails(shipmentId={ShipmentId}): order → {Order}", shipmentId, order);
        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogDebug("ShipmentDetails(shipmentId={ShipmentId}): customer → {Customer}", shipmentId, customer);

        if (order == null || order.Deleted || customer.Id != order.CustomerId)
            return Challenge();

        var model = await _orderModelFactory.PrepareShipmentDetailsModelAsync(shipment);
		_logger.LogDebug("ShipmentDetails(shipmentId={ShipmentId}): model → {Model}", shipmentId, model);
		_logger.LogDebug("<<< Exiting ShipmentDetails(shipmentId={ShipmentId})", shipmentId);
        return View(model);
    }

    #endregion
}