using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Tax.Avalara.Services;
using Nop.Services.Customers;
using Nop.Services.Tax;
using Nop.Web.Framework.Components;
using Nop.Web.Framework.Infrastructure;
using Nop.Web.Models.ShoppingCart;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Tax.Avalara.Components;

/// <summary>
/// Represents a view component to display applied exemption certificate on the order confirmation page
/// </summary>
public class AppliedCertificateViewComponent : NopViewComponent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly AvalaraTaxManager _avalaraTaxManager;
    protected readonly AvalaraTaxSettings _avalaraTaxSettings;
    protected readonly ICustomerService _customerService;
    protected readonly IStoreContext _storeContext;
    protected readonly ITaxPluginManager _taxPluginManager;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public AppliedCertificateViewComponent(AvalaraTaxManager avalaraTaxManager,
        AvalaraTaxSettings avalaraTaxSettings,
        ICustomerService customerService,
        IStoreContext storeContext,
        ITaxPluginManager taxPluginManager,
        IWorkContext workContext)
    {
		_logger.LogDebug(">>> Entering AppliedCertificateViewComponent(avalaraTaxManager={AvalaraTaxManager},avalaraTaxSettings={AvalaraTaxSettings},customerService={CustomerService},storeContext={StoreContext},taxPluginManager={TaxPluginManager},workContext={WorkContext})", avalaraTaxManager, avalaraTaxSettings, customerService, storeContext, taxPluginManager, workContext);
        _avalaraTaxManager = avalaraTaxManager;
		_logger.LogDebug("AppliedCertificateViewComponent(avalaraTaxManager={AvalaraTaxManager},avalaraTaxSettings={AvalaraTaxSettings},customerService={CustomerService},storeContext={StoreContext},taxPluginManager={TaxPluginManager},workContext={WorkContext}): _avalaraTaxManager → {_avalaraTaxManager}", avalaraTaxManager, avalaraTaxSettings, customerService, storeContext, taxPluginManager, workContext, _avalaraTaxManager);
        _avalaraTaxSettings = avalaraTaxSettings;
		_logger.LogDebug("AppliedCertificateViewComponent(avalaraTaxManager={AvalaraTaxManager},avalaraTaxSettings={AvalaraTaxSettings},customerService={CustomerService},storeContext={StoreContext},taxPluginManager={TaxPluginManager},workContext={WorkContext}): _avalaraTaxSettings → {_avalaraTaxSettings}", avalaraTaxManager, avalaraTaxSettings, customerService, storeContext, taxPluginManager, workContext, _avalaraTaxSettings);
        _customerService = customerService;
		_logger.LogDebug("AppliedCertificateViewComponent(avalaraTaxManager={AvalaraTaxManager},avalaraTaxSettings={AvalaraTaxSettings},customerService={CustomerService},storeContext={StoreContext},taxPluginManager={TaxPluginManager},workContext={WorkContext}): _customerService → {_customerService}", avalaraTaxManager, avalaraTaxSettings, customerService, storeContext, taxPluginManager, workContext, _customerService);
        _storeContext = storeContext;
		_logger.LogDebug("AppliedCertificateViewComponent(avalaraTaxManager={AvalaraTaxManager},avalaraTaxSettings={AvalaraTaxSettings},customerService={CustomerService},storeContext={StoreContext},taxPluginManager={TaxPluginManager},workContext={WorkContext}): _storeContext → {_storeContext}", avalaraTaxManager, avalaraTaxSettings, customerService, storeContext, taxPluginManager, workContext, _storeContext);
        _taxPluginManager = taxPluginManager;
		_logger.LogDebug("AppliedCertificateViewComponent(avalaraTaxManager={AvalaraTaxManager},avalaraTaxSettings={AvalaraTaxSettings},customerService={CustomerService},storeContext={StoreContext},taxPluginManager={TaxPluginManager},workContext={WorkContext}): _taxPluginManager → {_taxPluginManager}", avalaraTaxManager, avalaraTaxSettings, customerService, storeContext, taxPluginManager, workContext, _taxPluginManager);
        _workContext = workContext;
		_logger.LogDebug("AppliedCertificateViewComponent(avalaraTaxManager={AvalaraTaxManager},avalaraTaxSettings={AvalaraTaxSettings},customerService={CustomerService},storeContext={StoreContext},taxPluginManager={TaxPluginManager},workContext={WorkContext}): _workContext → {_workContext}", avalaraTaxManager, avalaraTaxSettings, customerService, storeContext, taxPluginManager, workContext, _workContext);
		_logger.LogDebug("<<< Exiting AppliedCertificateViewComponent(avalaraTaxManager={AvalaraTaxManager},avalaraTaxSettings={AvalaraTaxSettings},customerService={CustomerService},storeContext={StoreContext},taxPluginManager={TaxPluginManager},workContext={WorkContext})", avalaraTaxManager, avalaraTaxSettings, customerService, storeContext, taxPluginManager, workContext);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Invoke the widget view component
    /// </summary>
    /// <param name="widgetZone">Widget zone</param>
    /// <param name="additionalData">Additional parameters</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the view component result
    /// </returns>
    public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
    {
		_logger.LogDebug(">>> Entering InvokeAsync(widgetZone={WidgetZone},additionalData={AdditionalData})", widgetZone, additionalData);
        //ensure that Avalara tax provider is active
        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogDebug("InvokeAsync(widgetZone={WidgetZone},additionalData={AdditionalData}): customer → {Customer}", widgetZone, additionalData, customer);
        if (!await _taxPluginManager.IsPluginActiveAsync(AvalaraTaxDefaults.SystemName, customer))
            return Content(string.Empty);

        if (!_avalaraTaxSettings.EnableCertificates)
            return Content(string.Empty);

        //ACL
        if (_avalaraTaxSettings.CustomerRoleIds.Any())
        {
            var customerRoleIds = await _customerService.GetCustomerRoleIdsAsync(customer);
			_logger.LogDebug("InvokeAsync(widgetZone={WidgetZone},additionalData={AdditionalData}): customerRoleIds → {CustomerRoleIds}", widgetZone, additionalData, customerRoleIds);
            if (!customerRoleIds.Intersect(_avalaraTaxSettings.CustomerRoleIds).Any())
                return Content(string.Empty);
        }

        //ensure that it's a proper widget zone
        if (!widgetZone.Equals(PublicWidgetZones.OrderSummaryContentBefore))
            return Content(string.Empty);

        //ensure that model is passed
        if (additionalData is not ShoppingCartModel cartModel || cartModel.OrderReviewData?.Display != true)
            return Content(string.Empty);

        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogDebug("InvokeAsync(widgetZone={WidgetZone},additionalData={AdditionalData}): store → {Store}", widgetZone, additionalData, store);
        var validCertificate = await _avalaraTaxManager.GetValidCertificatesAsync(customer, store.Id);
		_logger.LogDebug("InvokeAsync(widgetZone={WidgetZone},additionalData={AdditionalData}): validCertificate → {ValidCertificate}", widgetZone, additionalData, validCertificate);
        var certificateValue = !string.IsNullOrEmpty(validCertificate?.exemptionNumber)
            ? validCertificate.exemptionNumber
            : validCertificate?.id?.ToString();

		_logger.LogDebug("<<< Exiting InvokeAsync(widgetZone={WidgetZone},additionalData={AdditionalData})", widgetZone, additionalData);
        return View("~/Plugins/Tax.Avalara/Views/Checkout/AppliedCertificate.cshtml", certificateValue);
    }

    #endregion
}