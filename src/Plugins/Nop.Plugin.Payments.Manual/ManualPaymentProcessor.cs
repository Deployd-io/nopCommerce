using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Plugin.Payments.Manual.Components;
using Nop.Plugin.Payments.Manual.Models;
using Nop.Plugin.Payments.Manual.Validators;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Plugins;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Payments.Manual;

/// <summary>
/// Manual payment processor
/// </summary>
public class ManualPaymentProcessor : BasePlugin, IPaymentMethod
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly ILocalizationService _localizationService;
    protected readonly IOrderTotalCalculationService _orderTotalCalculationService;
    protected readonly ISettingService _settingService;
    protected readonly IWebHelper _webHelper;
    protected readonly ManualPaymentSettings _manualPaymentSettings;

    #endregion

    #region Ctor

    public ManualPaymentProcessor(ILocalizationService localizationService,
        IOrderTotalCalculationService orderTotalCalculationService,
        ISettingService settingService,
        IWebHelper webHelper,
        ManualPaymentSettings manualPaymentSettings)
    {
		_logger.LogDebug(">>> Entering ManualPaymentProcessor(localizationService={LocalizationService},orderTotalCalculationService={OrderTotalCalculationService},settingService={SettingService},webHelper={WebHelper},manualPaymentSettings={ManualPaymentSettings})", localizationService, orderTotalCalculationService, settingService, webHelper, manualPaymentSettings);
        _localizationService = localizationService;
		_logger.LogDebug("ManualPaymentProcessor(localizationService={LocalizationService},orderTotalCalculationService={OrderTotalCalculationService},settingService={SettingService},webHelper={WebHelper},manualPaymentSettings={ManualPaymentSettings}): _localizationService → {_localizationService}", localizationService, orderTotalCalculationService, settingService, webHelper, manualPaymentSettings, _localizationService);
        _orderTotalCalculationService = orderTotalCalculationService;
		_logger.LogDebug("ManualPaymentProcessor(localizationService={LocalizationService},orderTotalCalculationService={OrderTotalCalculationService},settingService={SettingService},webHelper={WebHelper},manualPaymentSettings={ManualPaymentSettings}): _orderTotalCalculationService → {_orderTotalCalculationService}", localizationService, orderTotalCalculationService, settingService, webHelper, manualPaymentSettings, _orderTotalCalculationService);
        _settingService = settingService;
		_logger.LogDebug("ManualPaymentProcessor(localizationService={LocalizationService},orderTotalCalculationService={OrderTotalCalculationService},settingService={SettingService},webHelper={WebHelper},manualPaymentSettings={ManualPaymentSettings}): _settingService → {_settingService}", localizationService, orderTotalCalculationService, settingService, webHelper, manualPaymentSettings, _settingService);
        _webHelper = webHelper;
		_logger.LogDebug("ManualPaymentProcessor(localizationService={LocalizationService},orderTotalCalculationService={OrderTotalCalculationService},settingService={SettingService},webHelper={WebHelper},manualPaymentSettings={ManualPaymentSettings}): _webHelper → {_webHelper}", localizationService, orderTotalCalculationService, settingService, webHelper, manualPaymentSettings, _webHelper);
        _manualPaymentSettings = manualPaymentSettings;
		_logger.LogDebug("<<< Exiting ManualPaymentProcessor(localizationService={LocalizationService},orderTotalCalculationService={OrderTotalCalculationService},settingService={SettingService},webHelper={WebHelper},manualPaymentSettings={ManualPaymentSettings})", localizationService, orderTotalCalculationService, settingService, webHelper, manualPaymentSettings);
		_logger.LogDebug("ManualPaymentProcessor(localizationService={LocalizationService},orderTotalCalculationService={OrderTotalCalculationService},settingService={SettingService},webHelper={WebHelper},manualPaymentSettings={ManualPaymentSettings}): _manualPaymentSettings → {_manualPaymentSettings}", localizationService, orderTotalCalculationService, settingService, webHelper, manualPaymentSettings, _manualPaymentSettings);
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
    public Task<ProcessPaymentResult> ProcessPaymentAsync(ProcessPaymentRequest processPaymentRequest)
    {
		_logger.LogDebug(">>> Entering ProcessPaymentAsync(processPaymentRequest={ProcessPaymentRequest})", processPaymentRequest);
        var result = new ProcessPaymentResult
        {
            AllowStoringCreditCardNumber = true
        };
        switch (_manualPaymentSettings.TransactMode)
        {
            case TransactMode.Pending:
                result.NewPaymentStatus = PaymentStatus.Pending;
				_logger.LogDebug("ProcessPaymentAsync(processPaymentRequest={ProcessPaymentRequest}): result.NewPaymentStatus → {Arg1}", processPaymentRequest, result.NewPaymentStatus);
                break;
            case TransactMode.Authorize:
                result.NewPaymentStatus = PaymentStatus.Authorized;
                break;
            case TransactMode.AuthorizeAndCapture:
                result.NewPaymentStatus = PaymentStatus.Paid;
                break;
            default:
                result.AddError("Not supported transaction type");
                break;
        }

		_logger.LogDebug("<<< Exiting ProcessPaymentAsync(processPaymentRequest={ProcessPaymentRequest})", processPaymentRequest);
        return Task.FromResult(result);
    }

    /// <summary>
    /// Post process payment (used by payment gateways that require redirecting to a third-party URL)
    /// </summary>
    /// <param name="postProcessPaymentRequest">Payment info required for an order processing</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public Task PostProcessPaymentAsync(PostProcessPaymentRequest postProcessPaymentRequest)
    {
		_logger.LogDebug(">>> Entering PostProcessPaymentAsync(postProcessPaymentRequest={PostProcessPaymentRequest})", postProcessPaymentRequest);
        //nothing
		_logger.LogDebug("<<< Exiting PostProcessPaymentAsync(postProcessPaymentRequest={PostProcessPaymentRequest})", postProcessPaymentRequest);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Returns a value indicating whether payment method should be hidden during checkout
    /// </summary>
    /// <param name="cart">Shopping cart</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains true - hide; false - display.
    /// </returns>
    public Task<bool> HidePaymentMethodAsync(IList<ShoppingCartItem> cart)
    {
		_logger.LogDebug(">>> Entering HidePaymentMethodAsync(cart={Cart})", cart);
        //you can put any logic here
        //for example, hide this payment method if all products in the cart are downloadable
        //or hide this payment method if current customer is from certain country
		_logger.LogDebug("<<< Exiting HidePaymentMethodAsync(cart={Cart})", cart);
        return Task.FromResult(false);
    }

    /// <summary>
    /// Gets additional handling fee
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the additional handling fee
    /// </returns>
    public async Task<decimal> GetAdditionalHandlingFeeAsync(IList<ShoppingCartItem> cart)
    {
		_logger.LogDebug(">>> Entering GetAdditionalHandlingFeeAsync(cart={Cart})", cart);
		_logger.LogDebug("<<< Exiting GetAdditionalHandlingFeeAsync(cart={Cart})", cart);
        return await _orderTotalCalculationService.CalculatePaymentAdditionalFeeAsync(cart,
            _manualPaymentSettings.AdditionalFee, _manualPaymentSettings.AdditionalFeePercentage);
    }

    /// <summary>
    /// Captures payment
    /// </summary>
    /// <param name="capturePaymentRequest">Capture payment request</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the capture payment result
    /// </returns>
    public Task<CapturePaymentResult> CaptureAsync(CapturePaymentRequest capturePaymentRequest)
    {
		_logger.LogDebug(">>> Entering CaptureAsync(capturePaymentRequest={CapturePaymentRequest})", capturePaymentRequest);
		_logger.LogDebug("<<< Exiting CaptureAsync(capturePaymentRequest={CapturePaymentRequest})", capturePaymentRequest);
        return Task.FromResult(new CapturePaymentResult { Errors = new[] { "Capture method not supported" } });
    }

    /// <summary>
    /// Refunds a payment
    /// </summary>
    /// <param name="refundPaymentRequest">Request</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    public Task<RefundPaymentResult> RefundAsync(RefundPaymentRequest refundPaymentRequest)
    {
		_logger.LogDebug(">>> Entering RefundAsync(refundPaymentRequest={RefundPaymentRequest})", refundPaymentRequest);
		_logger.LogDebug("<<< Exiting RefundAsync(refundPaymentRequest={RefundPaymentRequest})", refundPaymentRequest);
        return Task.FromResult(new RefundPaymentResult { Errors = new[] { "Refund method not supported" } });
    }

    /// <summary>
    /// Voids a payment
    /// </summary>
    /// <param name="voidPaymentRequest">Request</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    public Task<VoidPaymentResult> VoidAsync(VoidPaymentRequest voidPaymentRequest)
    {
		_logger.LogDebug(">>> Entering VoidAsync(voidPaymentRequest={VoidPaymentRequest})", voidPaymentRequest);
		_logger.LogDebug("<<< Exiting VoidAsync(voidPaymentRequest={VoidPaymentRequest})", voidPaymentRequest);
        return Task.FromResult(new VoidPaymentResult { Errors = new[] { "Void method not supported" } });
    }

    /// <summary>
    /// Process recurring payment
    /// </summary>
    /// <param name="processPaymentRequest">Payment info required for an order processing</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the process payment result
    /// </returns>
    public Task<ProcessPaymentResult> ProcessRecurringPaymentAsync(ProcessPaymentRequest processPaymentRequest)
    {
		_logger.LogDebug(">>> Entering ProcessRecurringPaymentAsync(processPaymentRequest={ProcessPaymentRequest})", processPaymentRequest);
        var result = new ProcessPaymentResult
        {
            AllowStoringCreditCardNumber = true
        };
        switch (_manualPaymentSettings.TransactMode)
        {
            case TransactMode.Pending:
                result.NewPaymentStatus = PaymentStatus.Pending;
				_logger.LogDebug("ProcessRecurringPaymentAsync(processPaymentRequest={ProcessPaymentRequest}): result.NewPaymentStatus → {Arg1}", processPaymentRequest, result.NewPaymentStatus);
                break;
            case TransactMode.Authorize:
                result.NewPaymentStatus = PaymentStatus.Authorized;
                break;
            case TransactMode.AuthorizeAndCapture:
                result.NewPaymentStatus = PaymentStatus.Paid;
                break;
            default:
                result.AddError("Not supported transaction type");
                break;
        }

		_logger.LogDebug("<<< Exiting ProcessRecurringPaymentAsync(processPaymentRequest={ProcessPaymentRequest})", processPaymentRequest);
        return Task.FromResult(result);
    }

    /// <summary>
    /// Cancels a recurring payment
    /// </summary>
    /// <param name="cancelPaymentRequest">Request</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    public Task<CancelRecurringPaymentResult> CancelRecurringPaymentAsync(CancelRecurringPaymentRequest cancelPaymentRequest)
    {
		_logger.LogDebug(">>> Entering CancelRecurringPaymentAsync(cancelPaymentRequest={CancelPaymentRequest})", cancelPaymentRequest);
        //always success
		_logger.LogDebug("<<< Exiting CancelRecurringPaymentAsync(cancelPaymentRequest={CancelPaymentRequest})", cancelPaymentRequest);
        return Task.FromResult(new CancelRecurringPaymentResult());
    }

    /// <summary>
    /// Gets a value indicating whether customers can complete a payment after order is placed but not completed (for redirection payment methods)
    /// </summary>
    /// <param name="order">Order</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    public Task<bool> CanRePostProcessPaymentAsync(Order order)
    {
		_logger.LogDebug(">>> Entering CanRePostProcessPaymentAsync(order={Order})", order);
        ArgumentNullException.ThrowIfNull(order);

        //it's not a redirection payment method. So we always return false
		_logger.LogDebug("<<< Exiting CanRePostProcessPaymentAsync(order={Order})", order);
        return Task.FromResult(false);
    }

    /// <summary>
    /// Validate payment form
    /// </summary>
    /// <param name="form">The parsed form values</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of validating errors
    /// </returns>
    public Task<IList<string>> ValidatePaymentFormAsync(IFormCollection form)
    {
		_logger.LogDebug(">>> Entering ValidatePaymentFormAsync(form={Form})", form);
        var warnings = new List<string>();

		_logger.LogDebug("ValidatePaymentFormAsync(form={Form}): warnings → {Warnings}", form, warnings);
        //validate
        var validator = new PaymentInfoValidator(_localizationService);
		_logger.LogDebug("ValidatePaymentFormAsync(form={Form}): validator → {Validator}", form, validator);
        var model = new PaymentInfoModel
        {
            CardholderName = form["CardholderName"],
            CardNumber = form["CardNumber"],
            CardCode = form["CardCode"],
            ExpireMonth = form["ExpireMonth"],
            ExpireYear = form["ExpireYear"]
        };
        var validationResult = validator.Validate(model);
		_logger.LogDebug("ValidatePaymentFormAsync(form={Form}): validationResult → {ValidationResult}", form, validationResult);
        if (!validationResult.IsValid)
            warnings.AddRange(validationResult.Errors.Select(error => error.ErrorMessage));

		_logger.LogDebug("<<< Exiting ValidatePaymentFormAsync(form={Form})", form);
        return Task.FromResult<IList<string>>(warnings);
    }

    /// <summary>
    /// Get payment information
    /// </summary>
    /// <param name="form">The parsed form values</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the payment info holder
    /// </returns>
    public Task<ProcessPaymentRequest> GetPaymentInfoAsync(IFormCollection form)
    {
		_logger.LogDebug(">>> Entering GetPaymentInfoAsync(form={Form})", form);
		_logger.LogDebug("<<< Exiting GetPaymentInfoAsync(form={Form})", form);
        return Task.FromResult(new ProcessPaymentRequest
        {
            CreditCardType = form["CreditCardType"],
            CreditCardName = form["CardholderName"],
            CreditCardNumber = form["CardNumber"],
            CreditCardExpireMonth = int.Parse(form["ExpireMonth"]),
            CreditCardExpireYear = int.Parse(form["ExpireYear"]),
            CreditCardCvv2 = form["CardCode"]
        });
    }

    /// <summary>
    /// Gets a configuration page URL
    /// </summary>
    public override string GetConfigurationPageUrl()
    {
		_logger.LogDebug(">>> Entering GetConfigurationPageUrl()");
		_logger.LogDebug("<<< Exiting GetConfigurationPageUrl()");
        return $"{_webHelper.GetStoreLocation()}Admin/PaymentManual/Configure";
    }

    /// <summary>
    /// Gets a type of a view component for displaying plugin in public store ("payment info" checkout step)
    /// </summary>
    /// <returns>View component type</returns>
    public Type GetPublicViewComponent()
    {
		_logger.LogDebug(">>> Entering GetPublicViewComponent()");
		_logger.LogDebug("<<< Exiting GetPublicViewComponent()");
        return typeof(PaymentManualViewComponent);
    }

    /// <summary>
    /// Install the plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task InstallAsync()
    {
		_logger.LogDebug(">>> Entering InstallAsync()");
        //settings
        var settings = new ManualPaymentSettings
        {
            TransactMode = TransactMode.Pending
        };
        await _settingService.SaveSettingAsync(settings);

        //locales
        await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
        {
            ["Plugins.Payments.Manual.Instructions"] = "This payment method stores credit card information in database (it's not sent to any third-party processor). In order to store credit card information, you must be PCI compliant.",
            ["Plugins.Payments.Manual.Fields.AdditionalFee"] = "Additional fee",
            ["Plugins.Payments.Manual.Fields.AdditionalFee.Hint"] = "Enter additional fee to charge your customers.",
            ["Plugins.Payments.Manual.Fields.AdditionalFeePercentage"] = "Additional fee. Use percentage",
            ["Plugins.Payments.Manual.Fields.AdditionalFeePercentage.Hint"] = "Determines whether to apply a percentage additional fee to the order total. If not enabled, a fixed value is used.",
            ["Plugins.Payments.Manual.Fields.TransactMode"] = "After checkout mark payment as",
            ["Plugins.Payments.Manual.Fields.TransactMode.Hint"] = "Specify transaction mode.",
            ["Plugins.Payments.Manual.PaymentMethodDescription"] = "Pay by credit / debit card"
        });

        await base.InstallAsync();
		_logger.LogDebug("<<< Exiting InstallAsync()");
    }

    /// <summary>
    /// Uninstall the plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task UninstallAsync()
    {
		_logger.LogDebug(">>> Entering UninstallAsync()");
        //settings
        await _settingService.DeleteSettingAsync<ManualPaymentSettings>();

        //locales
        await _localizationService.DeleteLocaleResourcesAsync("Plugins.Payments.Manual");

        await base.UninstallAsync();
		_logger.LogDebug("<<< Exiting UninstallAsync()");
    }

    /// <summary>
    /// Gets a payment method description that will be displayed on checkout pages in the public store
    /// </summary>
    /// <remarks>
    /// return description of this payment method to be display on "payment method" checkout step. good practice is to make it localizable
    /// for example, for a redirection payment method, description may be like this: "You will be redirected to PayPal site to complete the payment"
    /// </remarks>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task<string> GetPaymentMethodDescriptionAsync()
    {
		_logger.LogDebug(">>> Entering GetPaymentMethodDescriptionAsync()");
		_logger.LogDebug("<<< Exiting GetPaymentMethodDescriptionAsync()");
        return await _localizationService.GetResourceAsync("Plugins.Payments.Manual.PaymentMethodDescription");
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets a value indicating whether capture is supported
    /// </summary>
    public bool SupportCapture => false;

    /// <summary>
    /// Gets a value indicating whether partial refund is supported
    /// </summary>
    public bool SupportPartiallyRefund => false;

    /// <summary>
    /// Gets a value indicating whether refund is supported
    /// </summary>
    public bool SupportRefund => false;

    /// <summary>
    /// Gets a value indicating whether void is supported
    /// </summary>
    public bool SupportVoid => false;

    /// <summary>
    /// Gets a recurring payment type of payment method
    /// </summary>
    public RecurringPaymentType RecurringPaymentType => RecurringPaymentType.Manual;

    /// <summary>
    /// Gets a payment method type
    /// </summary>
    public PaymentMethodType PaymentMethodType => PaymentMethodType.Standard;

    /// <summary>
    /// Gets a value indicating whether we should display a payment information page for this plugin
    /// </summary>
    public bool SkipPaymentInfo => false;

    #endregion
}