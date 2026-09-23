using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Logging;
using Nop.Services.Customers;
using Nop.Services.Logging;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Widgets.GoogleAnalytics.Components;

public class WidgetsGoogleAnalyticsViewComponent : NopViewComponent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
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
		_logger.LogError(">>> Entering WidgetsGoogleAnalyticsViewComponent(googleAnalyticsSettings={GoogleAnalyticsSettings},customerService={CustomerService},logger={Logger},workContext={WorkContext})", googleAnalyticsSettings, customerService, logger, workContext);
        _workContext = workContext;
		_logger.LogDebug("WidgetsGoogleAnalyticsViewComponent(googleAnalyticsSettings={GoogleAnalyticsSettings},customerService={CustomerService},logger={Logger},workContext={WorkContext}): _workContext → {_workContext}", googleAnalyticsSettings, customerService, logger, workContext, _workContext);
    }

    #endregion

    #region Utilities

    /// <returns>A task that represents the asynchronous operation</returns>
    protected async Task<string> GetScriptAsync()
    {
		_logger.LogError(">>> Entering GetScriptAsync()");
        try
        {
            var analyticsTrackingScript = _googleAnalyticsSettings.TrackingScript + "\n";
			_logger.LogError("GetScriptAsync(): analyticsTrackingScript → {AnalyticsTrackingScript}", analyticsTrackingScript);
            analyticsTrackingScript = analyticsTrackingScript.Replace("{GOOGLEID}", _googleAnalyticsSettings.GoogleId);
            //remove {ECOMMERCE} (used in previous versions of the plugin)
            analyticsTrackingScript = analyticsTrackingScript.Replace("{ECOMMERCE}", "");
            //remove {CustomerID} (used in previous versions of the plugin)
            analyticsTrackingScript = analyticsTrackingScript.Replace("{CustomerID}", "");

            //whether to include customer identifier
            var customerIdCode = string.Empty;
			_logger.LogError("GetScriptAsync(): customerIdCode → {CustomerIdCode}", customerIdCode);
            var customer = await _workContext.GetCurrentCustomerAsync();
			_logger.LogError("GetScriptAsync(): customer → {Customer}", customer);
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

		_logger.LogWarning("<<< Exiting GetScriptAsync()");
        return "";
    }

    #endregion

    #region Methods

    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
    {
		_logger.LogError(">>> Entering InvokeAsync(widgetZone={WidgetZone},additionalData={AdditionalData})", widgetZone, additionalData);
        var script = await GetScriptAsync();
		_logger.LogError("InvokeAsync(widgetZone={WidgetZone},additionalData={AdditionalData}): script → {Script}", widgetZone, additionalData, script);
		_logger.LogWarning("<<< Exiting InvokeAsync(widgetZone={WidgetZone},additionalData={AdditionalData})", widgetZone, additionalData);
        return View("~/Plugins/Widgets.GoogleAnalytics/Views/PublicInfo.cshtml", script);
    }

    #endregion
}