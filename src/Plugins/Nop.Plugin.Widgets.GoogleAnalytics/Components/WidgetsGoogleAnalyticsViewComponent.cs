using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Logging;
using Nop.Services.Customers;
using Nop.Services.Logging;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Widgets.GoogleAnalytics.Components;

public class WidgetsGoogleAnalyticsViewComponent : NopViewComponent
{
    #region Fields

    protected readonly GoogleAnalyticsSettings _googleAnalyticsSettings;
    protected readonly ICustomerService _customerService;
    protected readonly ILogger _logger;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public WidgetsGoogleAnalyticsViewComponent(
        GoogleAnalyticsSettings googleAnalyticsSettings,
        ICustomerService customerService,
        ILogger logger,
        IWorkContext workContext)
    {
        _googleAnalyticsSettings = googleAnalyticsSettings;
		_logger.LogDebug("WidgetsGoogleAnalyticsViewComponent(googleAnalyticsSettings={GoogleAnalyticsSettings},customerService={CustomerService},logger={Logger},workContext={WorkContext}): _googleAnalyticsSettings → {_googleAnalyticsSettings}", googleAnalyticsSettings, customerService, logger, workContext, _googleAnalyticsSettings);
        _customerService = customerService;
		_logger.LogDebug("WidgetsGoogleAnalyticsViewComponent(googleAnalyticsSettings={GoogleAnalyticsSettings},customerService={CustomerService},logger={Logger},workContext={WorkContext}): _customerService → {_customerService}", googleAnalyticsSettings, customerService, logger, workContext, _customerService);
        _logger = logger;
		_logger.LogDebug(">>> Entering WidgetsGoogleAnalyticsViewComponent(googleAnalyticsSettings={GoogleAnalyticsSettings},customerService={CustomerService},logger={Logger},workContext={WorkContext})", googleAnalyticsSettings, customerService, logger, workContext);
		_logger.LogDebug("WidgetsGoogleAnalyticsViewComponent(googleAnalyticsSettings={GoogleAnalyticsSettings},customerService={CustomerService},logger={Logger},workContext={WorkContext}): _logger → {_logger}", googleAnalyticsSettings, customerService, logger, workContext, _logger);
        _workContext = workContext;
		_logger.LogDebug("WidgetsGoogleAnalyticsViewComponent(googleAnalyticsSettings={GoogleAnalyticsSettings},customerService={CustomerService},logger={Logger},workContext={WorkContext}): _workContext → {_workContext}", googleAnalyticsSettings, customerService, logger, workContext, _workContext);
		_logger.LogDebug("<<< Exiting WidgetsGoogleAnalyticsViewComponent(googleAnalyticsSettings={GoogleAnalyticsSettings},customerService={CustomerService},logger={Logger},workContext={WorkContext})", googleAnalyticsSettings, customerService, logger, workContext);
    }

    #endregion

    #region Utilities

    /// <returns>A task that represents the asynchronous operation</returns>
    protected async Task<string> GetScriptAsync()
    {
		_logger.LogDebug(">>> Entering GetScriptAsync()");
        try
        {
            var analyticsTrackingScript = _googleAnalyticsSettings.TrackingScript + "\n";
			_logger.LogDebug("GetScriptAsync(): analyticsTrackingScript → {AnalyticsTrackingScript}", analyticsTrackingScript);
            analyticsTrackingScript = analyticsTrackingScript.Replace("{GOOGLEID}", _googleAnalyticsSettings.GoogleId);
            //remove {ECOMMERCE} (used in previous versions of the plugin)
            analyticsTrackingScript = analyticsTrackingScript.Replace("{ECOMMERCE}", "");
            //remove {CustomerID} (used in previous versions of the plugin)
            analyticsTrackingScript = analyticsTrackingScript.Replace("{CustomerID}", "");

            //whether to include customer identifier
            var customerIdCode = string.Empty;
			_logger.LogDebug("GetScriptAsync(): customerIdCode → {CustomerIdCode}", customerIdCode);
            var customer = await _workContext.GetCurrentCustomerAsync();
			_logger.LogDebug("GetScriptAsync(): customer → {Customer}", customer);
            if (_googleAnalyticsSettings.IncludeCustomerId && !await _customerService.IsGuestAsync(customer))
                customerIdCode = $"gtag('set', {{'user_id': '{customer.Id}'}});{Environment.NewLine}";
            analyticsTrackingScript = analyticsTrackingScript.Replace("{CUSTOMER_TRACKING}", customerIdCode);
            analyticsTrackingScript = analyticsTrackingScript.Replace("{ECOMMERCE_TRACKING}", "");

            return analyticsTrackingScript;
        }
        catch (Exception ex)
        {
            await _logger.InsertLogAsync(LogLevel.Error, "Error creating scripts for Google eCommerce tracking", ex.ToString());
        }

		_logger.LogDebug("<<< Exiting GetScriptAsync()");
        return "";
    }

    #endregion

    #region Methods

    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
    {
		_logger.LogDebug(">>> Entering InvokeAsync(widgetZone={WidgetZone},additionalData={AdditionalData})", widgetZone, additionalData);
        var script = await GetScriptAsync();
		_logger.LogDebug("InvokeAsync(widgetZone={WidgetZone},additionalData={AdditionalData}): script → {Script}", widgetZone, additionalData, script);
		_logger.LogDebug("<<< Exiting InvokeAsync(widgetZone={WidgetZone},additionalData={AdditionalData})", widgetZone, additionalData);
        return View("~/Plugins/Widgets.GoogleAnalytics/Views/PublicInfo.cshtml", script);
    }

    #endregion
}