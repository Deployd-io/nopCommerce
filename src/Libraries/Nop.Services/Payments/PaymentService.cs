using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Payments;

/// <summary>
/// Payment service
/// </summary>
public partial class PaymentService : IPaymentService
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly ICustomerService _customerService;
    protected readonly IPaymentPluginManager _paymentPluginManager;
    protected readonly IPriceCalculationService _priceCalculationService;
    protected readonly PaymentSettings _paymentSettings;
    protected readonly ShoppingCartSettings _shoppingCartSettings;

    #endregion

    #region Ctor

    public PaymentService(ICustomerService customerService,
        IPaymentPluginManager paymentPluginManager,
        IPriceCalculationService priceCalculationService,
        PaymentSettings paymentSettings,
        ShoppingCartSettings shoppingCartSettings)
    {
		_logger.LogDebug(">>> Entering PaymentService(customerService={CustomerService},paymentPluginManager={PaymentPluginManager},priceCalculationService={PriceCalculationService},paymentSettings={PaymentSettings},shoppingCartSettings=<redacted>)", customerService, paymentPluginManager, priceCalculationService, paymentSettings);
        _customerService = customerService;
		_logger.LogDebug("PaymentService(customerService={CustomerService},paymentPluginManager={PaymentPluginManager},priceCalculationService={PriceCalculationService},paymentSettings={PaymentSettings},shoppingCartSettings=<redacted>): _customerService → {_customerService}", customerService, paymentPluginManager, priceCalculationService, paymentSettings, _customerService);
        _paymentPluginManager = paymentPluginManager;
		_logger.LogDebug("PaymentService(customerService={CustomerService},paymentPluginManager={PaymentPluginManager},priceCalculationService={PriceCalculationService},paymentSettings={PaymentSettings},shoppingCartSettings=<redacted>): _paymentPluginManager → {_paymentPluginManager}", customerService, paymentPluginManager, priceCalculationService, paymentSettings, _paymentPluginManager);
        _priceCalculationService = priceCalculationService;
		_logger.LogDebug("PaymentService(customerService={CustomerService},paymentPluginManager={PaymentPluginManager},priceCalculationService={PriceCalculationService},paymentSettings={PaymentSettings},shoppingCartSettings=<redacted>): _priceCalculationService → {_priceCalculationService}", customerService, paymentPluginManager, priceCalculationService, paymentSettings, _priceCalculationService);
        _paymentSettings = paymentSettings;
		_logger.LogDebug("PaymentService(customerService={CustomerService},paymentPluginManager={PaymentPluginManager},priceCalculationService={PriceCalculationService},paymentSettings={PaymentSettings},shoppingCartSettings=<redacted>): _paymentSettings → {_paymentSettings}", customerService, paymentPluginManager, priceCalculationService, paymentSettings, _paymentSettings);
        _shoppingCartSettings = shoppingCartSettings;
		_logger.LogDebug("PaymentService(customerService={CustomerService},paymentPluginManager={PaymentPluginManager},priceCalculationService={PriceCalculationService},paymentSettings={PaymentSettings},shoppingCartSettings=<redacted>): _shoppingCartSettings → {_shoppingCartSettings}", customerService, paymentPluginManager, priceCalculationService, paymentSettings, _shoppingCartSettings);
		_logger.LogDebug("<<< Exiting PaymentService(customerService={CustomerService},paymentPluginManager={PaymentPluginManager},priceCalculationService={PriceCalculationService},paymentSettings={PaymentSettings},shoppingCartSettings=<redacted>)", customerService, paymentPluginManager, priceCalculationService, paymentSettings);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Process a payment
    /// </summary>
    /// <param name="processPaymentRequest">Payment info required for an order processing</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the process payment result
    /// </returns>
    public virtual async Task<ProcessPaymentResult> ProcessPaymentAsync(ProcessPaymentRequest processPaymentRequest)
    {
		_logger.LogDebug(">>> Entering ProcessPaymentAsync(processPaymentRequest={ProcessPaymentRequest})", processPaymentRequest);
        if (processPaymentRequest.OrderTotal == decimal.Zero)
        {
            var result = new ProcessPaymentResult
            {
                NewPaymentStatus = PaymentStatus.Paid
            };
            return result;
        }

        //We should strip out any white space or dash in the CC number entered.
        if (!string.IsNullOrWhiteSpace(processPaymentRequest.CreditCardNumber))
        {
            processPaymentRequest.CreditCardNumber = processPaymentRequest.CreditCardNumber.Replace(" ", string.Empty);
            processPaymentRequest.CreditCardNumber = processPaymentRequest.CreditCardNumber.Replace("-", string.Empty);
        }

        var customer = await _customerService.GetCustomerByIdAsync(processPaymentRequest.CustomerId);
		_logger.LogDebug("ProcessPaymentAsync(processPaymentRequest={ProcessPaymentRequest}): customer → {Customer}", processPaymentRequest, customer);
        var paymentMethod = await _paymentPluginManager
                                .LoadPluginBySystemNameAsync(processPaymentRequest.PaymentMethodSystemName, customer, processPaymentRequest.StoreId)
                            ?? throw new NopException("Payment method couldn't be loaded");

        return await paymentMethod.ProcessPaymentAsync(processPaymentRequest);
    }

    /// <summary>
    /// Post process payment (used by payment gateways that require redirecting to a third-party URL)
    /// </summary>
    /// <param name="postProcessPaymentRequest">Payment info required for an order processing</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task PostProcessPaymentAsync(PostProcessPaymentRequest postProcessPaymentRequest)
    {
		_logger.LogDebug(">>> Entering PostProcessPaymentAsync(postProcessPaymentRequest={PostProcessPaymentRequest})", postProcessPaymentRequest);
        //already paid or order.OrderTotal == decimal.Zero
        if (postProcessPaymentRequest.Order.PaymentStatus == PaymentStatus.Paid)
            return;

        var customer = await _customerService.GetCustomerByIdAsync(postProcessPaymentRequest.Order.CustomerId);
		_logger.LogDebug("PostProcessPaymentAsync(postProcessPaymentRequest={PostProcessPaymentRequest}): customer → {Customer}", postProcessPaymentRequest, customer);
        var paymentMethod = await _paymentPluginManager
                                .LoadPluginBySystemNameAsync(postProcessPaymentRequest.Order.PaymentMethodSystemName, customer, postProcessPaymentRequest.Order.StoreId)
                            ?? throw new NopException("Payment method couldn't be loaded");

        await paymentMethod.PostProcessPaymentAsync(postProcessPaymentRequest);
		_logger.LogDebug("<<< Exiting PostProcessPaymentAsync(postProcessPaymentRequest={PostProcessPaymentRequest})", postProcessPaymentRequest);
    }

    /// <summary>
    /// Gets a value indicating whether customers can complete a payment after order is placed but not completed (for redirection payment methods)
    /// </summary>
    /// <param name="order">Order</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    public virtual async Task<bool> CanRePostProcessPaymentAsync(Order order)
    {
		_logger.LogDebug(">>> Entering CanRePostProcessPaymentAsync(order={Order})", order);
        ArgumentNullException.ThrowIfNull(order);

        if (!_paymentSettings.AllowRePostingPayments)
            return false;

        var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);
		_logger.LogDebug("CanRePostProcessPaymentAsync(order={Order}): customer → {Customer}", order, customer);
        var paymentMethod = await _paymentPluginManager.LoadPluginBySystemNameAsync(order.PaymentMethodSystemName, customer, order.StoreId);
		_logger.LogDebug("CanRePostProcessPaymentAsync(order={Order}): paymentMethod → {PaymentMethod}", order, paymentMethod);
        if (paymentMethod == null)
            return false; //Payment method couldn't be loaded (for example, was uninstalled)

        if (paymentMethod.PaymentMethodType != PaymentMethodType.Redirection)
            return false;   //this option is available only for redirection payment methods

        if (order.Deleted)
            return false;  //do not allow for deleted orders

        if (order.OrderStatus == OrderStatus.Cancelled)
            return false;  //do not allow for cancelled orders

        if (order.PaymentStatus != PaymentStatus.Pending)
            return false;  //payment status should be Pending

        return await paymentMethod.CanRePostProcessPaymentAsync(order);
    }

    /// <summary>
    /// Gets an additional handling fee of a payment method
    /// </summary>
    /// <param name="cart">Shopping cart</param>
    /// <param name="paymentMethodSystemName">Payment method system name</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the additional handling fee
    /// </returns>
    public virtual async Task<decimal> GetAdditionalHandlingFeeAsync(IList<ShoppingCartItem> cart, string paymentMethodSystemName)
    {
		_logger.LogDebug(">>> Entering GetAdditionalHandlingFeeAsync(cart={Cart},paymentMethodSystemName={PaymentMethodSystemName})", cart, paymentMethodSystemName);
        if (string.IsNullOrEmpty(paymentMethodSystemName))
            return decimal.Zero;

        var customer = await _customerService.GetCustomerByIdAsync(cart.FirstOrDefault()?.CustomerId ?? 0);
		_logger.LogDebug("GetAdditionalHandlingFeeAsync(cart={Cart},paymentMethodSystemName={PaymentMethodSystemName}): customer → {Customer}", cart, paymentMethodSystemName, customer);
        var paymentMethod = await _paymentPluginManager.LoadPluginBySystemNameAsync(paymentMethodSystemName, customer, cart.FirstOrDefault()?.StoreId ?? 0);
		_logger.LogDebug("GetAdditionalHandlingFeeAsync(cart={Cart},paymentMethodSystemName={PaymentMethodSystemName}): paymentMethod → {PaymentMethod}", cart, paymentMethodSystemName, paymentMethod);
        if (paymentMethod == null)
            return decimal.Zero;

        var result = await paymentMethod.GetAdditionalHandlingFeeAsync(cart);
		_logger.LogDebug("GetAdditionalHandlingFeeAsync(cart={Cart},paymentMethodSystemName={PaymentMethodSystemName}): result → {Result}", cart, paymentMethodSystemName, result);
        if (result < decimal.Zero)
            result = decimal.Zero;

        if (!_shoppingCartSettings.RoundPricesDuringCalculation)
            return result;

        result = await _priceCalculationService.RoundPriceAsync(result);

		_logger.LogDebug("<<< Exiting GetAdditionalHandlingFeeAsync(cart={Cart},paymentMethodSystemName={PaymentMethodSystemName})", cart, paymentMethodSystemName);
        return result;
    }

    /// <summary>
    /// Gets a value indicating whether capture is supported by payment method
    /// </summary>
    /// <param name="paymentMethodSystemName">Payment method system name</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains a value indicating whether capture is supported
    /// </returns>
    public virtual async Task<bool> SupportCaptureAsync(string paymentMethodSystemName)
    {
		_logger.LogDebug(">>> Entering SupportCaptureAsync(paymentMethodSystemName={PaymentMethodSystemName})", paymentMethodSystemName);
        var paymentMethod = await _paymentPluginManager.LoadPluginBySystemNameAsync(paymentMethodSystemName);
        if (paymentMethod == null)
            return false;
		_logger.LogDebug(">>> Entering CaptureAsync(capturePaymentRequest={CapturePaymentRequest})", capturePaymentRequest);
		_logger.LogDebug("<<< Exiting CaptureAsync(capturePaymentRequest={CapturePaymentRequest})", capturePaymentRequest);
		_logger.LogDebug("<<< Exiting SupportCaptureAsync(paymentMethodSystemName={PaymentMethodSystemName})", paymentMethodSystemName);
        return paymentMethod.SupportCapture;
    }

    /// <summary>
    /// Captures payment
    /// </summary>
    /// <param name="capturePaymentRequest">Capture payment request</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the capture payment result
    /// </returns>
    public virtual async Task<CapturePaymentResult> CaptureAsync(CapturePaymentRequest capturePaymentRequest)
    {
        var paymentMethod = await _paymentPluginManager.LoadPluginBySystemNameAsync(capturePaymentRequest.Order.PaymentMethodSystemName)
                            ?? throw new NopException("Payment method couldn't be loaded");

        return await paymentMethod.CaptureAsync(capturePaymentRequest);
    }

    /// <summary>
    /// Gets a value indicating whether partial refund is supported by payment method
    /// </summary>
    /// <param name="paymentMethodSystemName">Payment method system name</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains a value indicating whether partial refund is supported
    /// </returns>
    public virtual async Task<bool> SupportPartiallyRefundAsync(string paymentMethodSystemName)
    {
		_logger.LogDebug(">>> Entering SupportPartiallyRefundAsync(paymentMethodSystemName={PaymentMethodSystemName})", paymentMethodSystemName);
        var paymentMethod = await _paymentPluginManager.LoadPluginBySystemNameAsync(paymentMethodSystemName);
        if (paymentMethod == null)
            return false;
		_logger.LogDebug(">>> Entering RefundAsync(refundPaymentRequest={RefundPaymentRequest})", refundPaymentRequest);
		_logger.LogDebug("<<< Exiting RefundAsync(refundPaymentRequest={RefundPaymentRequest})", refundPaymentRequest);
		_logger.LogDebug("<<< Exiting SupportPartiallyRefundAsync(paymentMethodSystemName={PaymentMethodSystemName})", paymentMethodSystemName);
        return paymentMethod.SupportPartiallyRefund;
    }

    /// <summary>
    /// Gets a value indicating whether refund is supported by payment method
    /// </summary>
    /// <param name="paymentMethodSystemName">Payment method system name</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains a value indicating whether refund is supported
    /// </returns>
    public virtual async Task<bool> SupportRefundAsync(string paymentMethodSystemName)
    {
		_logger.LogDebug(">>> Entering SupportRefundAsync(paymentMethodSystemName={PaymentMethodSystemName})", paymentMethodSystemName);
        var paymentMethod = await _paymentPluginManager.LoadPluginBySystemNameAsync(paymentMethodSystemName);
        if (paymentMethod == null)
            return false;
        return paymentMethod.SupportRefund;
    }

    /// <summary>
    /// Refunds a payment
    /// </summary>
    /// <param name="refundPaymentRequest">Request</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    public virtual async Task<RefundPaymentResult> RefundAsync(RefundPaymentRequest refundPaymentRequest)
    {
        var paymentMethod = await _paymentPluginManager.LoadPluginBySystemNameAsync(refundPaymentRequest.Order.PaymentMethodSystemName)
                            ?? throw new NopException("Payment method couldn't be loaded");

        return await paymentMethod.RefundAsync(refundPaymentRequest);
    }

    /// <summary>
    /// Gets a value indicating whether void is supported by payment method
    /// </summary>
    /// <param name="paymentMethodSystemName">Payment method system name</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains a value indicating whether void is supported
    /// </returns>
    public virtual async Task<bool> SupportVoidAsync(string paymentMethodSystemName)
    {
		_logger.LogDebug(">>> Entering SupportVoidAsync(paymentMethodSystemName={PaymentMethodSystemName})", paymentMethodSystemName);
        var paymentMethod = await _paymentPluginManager.LoadPluginBySystemNameAsync(paymentMethodSystemName);
        if (paymentMethod == null)
            return false;
		_logger.LogDebug(">>> Entering VoidAsync(voidPaymentRequest={VoidPaymentRequest})", voidPaymentRequest);
		_logger.LogDebug("<<< Exiting VoidAsync(voidPaymentRequest={VoidPaymentRequest})", voidPaymentRequest);
		_logger.LogDebug("<<< Exiting SupportVoidAsync(paymentMethodSystemName={PaymentMethodSystemName})", paymentMethodSystemName);
        return paymentMethod.SupportVoid;
    }

    /// <summary>
    /// Voids a payment
    /// </summary>
    /// <param name="voidPaymentRequest">Request</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    public virtual async Task<VoidPaymentResult> VoidAsync(VoidPaymentRequest voidPaymentRequest)
    {
        var paymentMethod = await _paymentPluginManager.LoadPluginBySystemNameAsync(voidPaymentRequest.Order.PaymentMethodSystemName)
                            ?? throw new NopException("Payment method couldn't be loaded");

        return await paymentMethod.VoidAsync(voidPaymentRequest);
    }

    /// <summary>
    /// Gets a recurring payment type of payment method
    /// </summary>
    /// <param name="paymentMethodSystemName">Payment method system name</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains a recurring payment type of payment method
    /// </returns>
    public virtual async Task<RecurringPaymentType> GetRecurringPaymentTypeAsync(string paymentMethodSystemName)
    {
		_logger.LogDebug(">>> Entering GetRecurringPaymentTypeAsync(paymentMethodSystemName={PaymentMethodSystemName})", paymentMethodSystemName);
        var paymentMethod = await _paymentPluginManager.LoadPluginBySystemNameAsync(paymentMethodSystemName);
        if (paymentMethod == null)
            return RecurringPaymentType.NotSupported;

        return paymentMethod.RecurringPaymentType;
    }

    /// <summary>
    /// Process recurring payment
    /// </summary>
    /// <param name="processPaymentRequest">Payment info required for an order processing</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the process payment result
    /// </returns>
    public virtual async Task<ProcessPaymentResult> ProcessRecurringPaymentAsync(ProcessPaymentRequest processPaymentRequest)
    {
		_logger.LogDebug(">>> Entering ProcessRecurringPaymentAsync(processPaymentRequest={ProcessPaymentRequest})", processPaymentRequest);
        if (processPaymentRequest.OrderTotal == decimal.Zero)
        {
            var result = new ProcessPaymentResult
            {
                NewPaymentStatus = PaymentStatus.Paid
            };
            return result;
        }

        var customer = await _customerService.GetCustomerByIdAsync(processPaymentRequest.CustomerId);
		_logger.LogDebug("ProcessRecurringPaymentAsync(processPaymentRequest={ProcessPaymentRequest}): customer → {Customer}", processPaymentRequest, customer);
        var paymentMethod = await _paymentPluginManager
                                .LoadPluginBySystemNameAsync(processPaymentRequest.PaymentMethodSystemName, customer, processPaymentRequest.StoreId)
                            ?? throw new NopException("Payment method couldn't be loaded");

		_logger.LogDebug("<<< Exiting ProcessRecurringPaymentAsync(processPaymentRequest={ProcessPaymentRequest})", processPaymentRequest);
        return await paymentMethod.ProcessRecurringPaymentAsync(processPaymentRequest);
    }

    /// <summary>
    /// Cancels a recurring payment
    /// </summary>
    /// <param name="cancelPaymentRequest">Request</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    public virtual async Task<CancelRecurringPaymentResult> CancelRecurringPaymentAsync(CancelRecurringPaymentRequest cancelPaymentRequest)
    {
		_logger.LogDebug(">>> Entering CancelRecurringPaymentAsync(cancelPaymentRequest={CancelPaymentRequest})", cancelPaymentRequest);
        if (cancelPaymentRequest.Order.OrderTotal == decimal.Zero)
            return new CancelRecurringPaymentResult();

        var paymentMethod = await _paymentPluginManager.LoadPluginBySystemNameAsync(cancelPaymentRequest.Order.PaymentMethodSystemName)
                            ?? throw new NopException("Payment method couldn't be loaded");

		_logger.LogDebug("<<< Exiting CancelRecurringPaymentAsync(cancelPaymentRequest={CancelPaymentRequest})", cancelPaymentRequest);
        return await paymentMethod.CancelRecurringPaymentAsync(cancelPaymentRequest);
    }

    /// <summary>
    /// Gets masked credit card number
    /// </summary>
    /// <param name="creditCardNumber">Credit card number</param>
    /// <returns>Masked credit card number</returns>
    public virtual string GetMaskedCreditCardNumber(string creditCardNumber)
    {
		_logger.LogDebug(">>> Entering GetMaskedCreditCardNumber(creditCardNumber=<redacted>)");
        if (string.IsNullOrEmpty(creditCardNumber))
            return string.Empty;

        if (creditCardNumber.Length <= 4)
            return creditCardNumber;

        var last4 = creditCardNumber[(creditCardNumber.Length - 4)..creditCardNumber.Length];
		_logger.LogDebug("GetMaskedCreditCardNumber(creditCardNumber=<redacted>): last4 → {Last4}", last4);
        var maskedChars = string.Empty;
        for (var i = 0; i < creditCardNumber.Length - 4; i++)
        {
            maskedChars += "*";
        }

		_logger.LogDebug("<<< Exiting GetMaskedCreditCardNumber(creditCardNumber=<redacted>)");
        return maskedChars + last4;
    }

    #endregion
}