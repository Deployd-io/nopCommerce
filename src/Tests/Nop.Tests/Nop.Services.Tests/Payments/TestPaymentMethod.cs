using Microsoft.AspNetCore.Http;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Services.Payments;
using Nop.Services.Plugins;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Services.Tests.Payments;

public class TestPaymentMethod : BasePlugin, IPaymentMethod
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Methods

    /// <summary>
    /// Refunds a payment
    /// </summary>
    /// <param name="refundPaymentRequest">Request</param>
    /// <returns>Result</returns>
    public Task<RefundPaymentResult> RefundAsync(RefundPaymentRequest refundPaymentRequest)
    {
		_logger.LogDebug(">>> Entering RefundAsync(refundPaymentRequest={RefundPaymentRequest})", refundPaymentRequest);
        var result = new RefundPaymentResult();
        result.AddError("Refund method not supported");
		_logger.LogDebug("RefundAsync(refundPaymentRequest={RefundPaymentRequest}): result → {Result}", refundPaymentRequest, result);

		_logger.LogDebug("<<< Exiting RefundAsync(refundPaymentRequest={RefundPaymentRequest})", refundPaymentRequest);
        return Task.FromResult(result);
    }

    /// <summary>
    /// Voids a payment
    /// </summary>
    /// <param name="voidPaymentRequest">Request</param>
    /// <returns>Result</returns>
    public Task<VoidPaymentResult> VoidAsync(VoidPaymentRequest voidPaymentRequest)
    {
		_logger.LogDebug(">>> Entering VoidAsync(voidPaymentRequest={VoidPaymentRequest})", voidPaymentRequest);
        var result = new VoidPaymentResult();
        result.AddError("Void method not supported");
		_logger.LogDebug("VoidAsync(voidPaymentRequest={VoidPaymentRequest}): result → {Result}", voidPaymentRequest, result);

		_logger.LogDebug("<<< Exiting VoidAsync(voidPaymentRequest={VoidPaymentRequest})", voidPaymentRequest);
        return Task.FromResult(result);
    }

    /// <summary>
    /// Process recurring payment
    /// </summary>
    /// <param name="processPaymentRequest">Payment info required for an order processing</param>
    /// <returns>Process payment result</returns>
    public Task<ProcessPaymentResult> ProcessRecurringPaymentAsync(ProcessPaymentRequest processPaymentRequest)
    {
		_logger.LogDebug(">>> Entering ProcessRecurringPaymentAsync(processPaymentRequest={ProcessPaymentRequest})", processPaymentRequest);
        var result = new ProcessPaymentResult();
        result.AddError("Recurring method not supported");
		_logger.LogDebug("ProcessRecurringPaymentAsync(processPaymentRequest={ProcessPaymentRequest}): result → {Result}", processPaymentRequest, result);

		_logger.LogDebug("<<< Exiting ProcessRecurringPaymentAsync(processPaymentRequest={ProcessPaymentRequest})", processPaymentRequest);
        return Task.FromResult(result);
    }

    /// <summary>
    /// Cancels a recurring payment
    /// </summary>
    /// <param name="cancelPaymentRequest">Request</param>
    /// <returns>Result</returns>
    public Task<CancelRecurringPaymentResult> CancelRecurringPaymentAsync(CancelRecurringPaymentRequest cancelPaymentRequest)
    {
		_logger.LogDebug(">>> Entering CancelRecurringPaymentAsync(cancelPaymentRequest={CancelPaymentRequest})", cancelPaymentRequest);
        var result = new CancelRecurringPaymentResult();
        result.AddError("Cancelling recurring orders not supported");
		_logger.LogDebug("CancelRecurringPaymentAsync(cancelPaymentRequest={CancelPaymentRequest}): result → {Result}", cancelPaymentRequest, result);

		_logger.LogDebug("<<< Exiting CancelRecurringPaymentAsync(cancelPaymentRequest={CancelPaymentRequest})", cancelPaymentRequest);
        return Task.FromResult(result);
    }

    /// <summary>
    /// Gets a value indicating whether customers can complete a payment after order is placed but not completed (for redirection payment methods)
    /// </summary>
    /// <param name="order">Order</param>
    /// <returns>Result</returns>
    public Task<bool> CanRePostProcessPaymentAsync(Order order)
    {
		_logger.LogDebug(">>> Entering CanRePostProcessPaymentAsync(order={Order})", order);
        ArgumentNullException.ThrowIfNull(order);

        //it's not a redirection payment method. So we always return false
		_logger.LogDebug(">>> Entering PostProcessPaymentAsync(postProcessPaymentRequest={PostProcessPaymentRequest})", postProcessPaymentRequest);
		_logger.LogDebug("<<< Exiting PostProcessPaymentAsync(postProcessPaymentRequest={PostProcessPaymentRequest})", postProcessPaymentRequest);
		_logger.LogDebug("<<< Exiting CanRePostProcessPaymentAsync(order={Order})", order);
        return Task.FromResult(false);
    }

    /// <summary>
    /// Validate payment form
    /// </summary>
    /// <param name="form">The parsed form values</param>
    /// <returns>List of validating errors</returns>
    public Task<IList<string>> ValidatePaymentFormAsync(IFormCollection form)
    {
		_logger.LogDebug(">>> Entering ValidatePaymentFormAsync(form={Form})", form);
		_logger.LogDebug("<<< Exiting ValidatePaymentFormAsync(form={Form})", form);
        return Task.FromResult<IList<string>>(new List<string>());
    }

    /// <summary>
    /// Get payment information
    /// </summary>
    /// <param name="form">The parsed form values</param>
    /// <returns>Payment info holder</returns>
    public Task<ProcessPaymentRequest> GetPaymentInfoAsync(IFormCollection form)
    {
		_logger.LogDebug(">>> Entering GetPaymentInfoAsync(form={Form})", form);
		_logger.LogDebug("<<< Exiting GetPaymentInfoAsync(form={Form})", form);
        return Task.FromResult(new ProcessPaymentRequest());
    }

    /// <summary>
    /// Process a payment
    /// </summary>
    /// <param name="processPaymentRequest">Payment info required for an order processing</param>
    /// <returns>Process payment result</returns>
    public Task<ProcessPaymentResult> ProcessPaymentAsync(ProcessPaymentRequest processPaymentRequest)
    {
		_logger.LogDebug(">>> Entering ProcessPaymentAsync(processPaymentRequest={ProcessPaymentRequest})", processPaymentRequest);
        var result = new ProcessPaymentResult
        {
            NewPaymentStatus = PaymentStatus.Paid
        };

		_logger.LogDebug("<<< Exiting ProcessPaymentAsync(processPaymentRequest={ProcessPaymentRequest})", processPaymentRequest);
        return Task.FromResult(result);
    }

    /// <summary>
    /// Post process payment (used by payment gateways that require redirecting to a third-party URL)
    /// </summary>
    /// <param name="postProcessPaymentRequest">Payment info required for an order processing</param>
    public Task PostProcessPaymentAsync(PostProcessPaymentRequest postProcessPaymentRequest)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Returns a value indicating whether payment method should be hidden during checkout
    /// </summary>
    /// <param name="cart">Shopping cart</param>
    /// <returns>true - hide; false - display.</returns>
    public Task<bool> HidePaymentMethodAsync(IList<ShoppingCartItem> cart)
    {
		_logger.LogDebug(">>> Entering HidePaymentMethodAsync(cart={Cart})", cart);
		_logger.LogDebug("<<< Exiting HidePaymentMethodAsync(cart={Cart})", cart);
        return Task.FromResult(false);
    }

    /// <summary>
    /// Gets additional handling fee
    /// </summary>
    /// <param name="cart">Shopping cart</param>
    /// <returns>Additional handling fee</returns>
    public Task<decimal> GetAdditionalHandlingFeeAsync(IList<ShoppingCartItem> cart)
    {
		_logger.LogDebug(">>> Entering GetAdditionalHandlingFeeAsync(cart={Cart})", cart);
		_logger.LogDebug("<<< Exiting GetAdditionalHandlingFeeAsync(cart={Cart})", cart);
        return Task.FromResult(AdditionalHandlingFee);
    }

    /// <summary>
    /// Captures payment
    /// </summary>
    /// <param name="capturePaymentRequest">Capture payment request</param>
    /// <returns>Capture payment result</returns>
    public Task<CapturePaymentResult> CaptureAsync(CapturePaymentRequest capturePaymentRequest)
    {
		_logger.LogDebug(">>> Entering CaptureAsync(capturePaymentRequest={CapturePaymentRequest})", capturePaymentRequest);
        var result = new CapturePaymentResult();
        result.AddError("Capture method not supported");
		_logger.LogDebug("CaptureAsync(capturePaymentRequest={CapturePaymentRequest}): result → {Result}", capturePaymentRequest, result);

		_logger.LogDebug("<<< Exiting CaptureAsync(capturePaymentRequest={CapturePaymentRequest})", capturePaymentRequest);
        return Task.FromResult(result);
    }

    /// <summary>
    /// Gets a type of a view component for displaying plugin in public store ("payment info" checkout step)
    /// </summary>
    /// <returns>View component type</returns>
    public Type GetPublicViewComponent()
    {
		_logger.LogDebug(">>> Entering GetPublicViewComponent()");
		_logger.LogDebug("<<< Exiting GetPublicViewComponent()");
        return null;
    }

    public Task<string> GetPaymentMethodDescriptionAsync()
    {
		_logger.LogDebug(">>> Entering GetPaymentMethodDescriptionAsync()");
		_logger.LogDebug("<<< Exiting GetPaymentMethodDescriptionAsync()");
        return Task.FromResult(PaymentMethodDescription);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets a value indicating whether capture is supported
    /// </summary>
    public bool SupportCapture => TestSupportCapture;

    /// <summary>
    /// Gets a value indicating whether partial refund is supported
    /// </summary>
    public bool SupportPartiallyRefund => TestSupportPartiallyRefund;

    /// <summary>
    /// Gets a value indicating whether refund is supported
    /// </summary>
    public bool SupportRefund => TestSupportRefund;

    /// <summary>
    /// Gets a value indicating whether void is supported
    /// </summary>
    public bool SupportVoid => TestSupportVoid;

    /// <summary>
    /// Gets a recurring payment type of payment method
    /// </summary>
    /// <returns>A recurring payment type of payment method</returns>
    public RecurringPaymentType RecurringPaymentType => RecurringPaymentType.NotSupported;

    /// <summary>
    /// Gets a payment method type
    /// </summary>
    /// <returns>A payment method type</returns>
    public PaymentMethodType PaymentMethodType => PaymentMethodType.Standard;

    /// <summary>
    /// Gets a value indicating whether we should display a payment information page for this plugin
    /// </summary>
    public bool SkipPaymentInfo => false;

    /// <summary>
    /// Gets a payment method description that will be displayed on checkout pages in the public store
    /// </summary>
    /// <remarks>
    /// return description of this payment method to be display on "payment method" checkout step. good practice is to make it localizable
    /// for example, for a redirection payment method, description may be like this: "You will be redirected to PayPal site to complete the payment"
    /// </remarks>
    public string PaymentMethodDescription => string.Empty;

    #endregion

    #region Test data

    public static bool TestSupportCapture { get; set; } = false;

    public static bool TestSupportRefund { get; set; } = false;

    public static bool TestSupportPartiallyRefund { get; set; } = false;

    public static bool TestSupportVoid { get; set; } = false;

    public static decimal AdditionalHandlingFee { get; set; } = decimal.Zero;

    #endregion


}