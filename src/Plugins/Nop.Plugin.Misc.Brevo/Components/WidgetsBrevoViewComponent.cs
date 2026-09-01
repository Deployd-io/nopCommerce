using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Customers;
using Nop.Web.Framework.Components;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Misc.Brevo.Components;

/// <summary>
/// Represents view component to embed tracking script on pages
/// </summary>
public class WidgetsBrevoViewComponent : NopViewComponent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly BrevoSettings _brevoSettings;
    protected readonly ICustomerService _customerService;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public WidgetsBrevoViewComponent(BrevoSettings brevoSettings,
        ICustomerService customerService,
        IWorkContext workContext)
    {
		_logger.LogDebug(">>> Entering WidgetsBrevoViewComponent(brevoSettings={BrevoSettings},customerService={CustomerService},workContext={WorkContext})", brevoSettings, customerService, workContext);
        _brevoSettings = brevoSettings;
		_logger.LogDebug("WidgetsBrevoViewComponent(brevoSettings={BrevoSettings},customerService={CustomerService},workContext={WorkContext}): _brevoSettings → {_brevoSettings}", brevoSettings, customerService, workContext, _brevoSettings);
        _customerService = customerService;
		_logger.LogDebug("WidgetsBrevoViewComponent(brevoSettings={BrevoSettings},customerService={CustomerService},workContext={WorkContext}): _customerService → {_customerService}", brevoSettings, customerService, workContext, _customerService);
        _workContext = workContext;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Invoke view component
    /// </summary>
    /// <param name="widgetZone">Widget zone name</param>
    /// <param name="additionalData">Additional data</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the view component result
    /// </returns>
    public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
    {
		_logger.LogDebug(">>> Entering InvokeAsync(widgetZone={WidgetZone},additionalData={AdditionalData})", widgetZone, additionalData);
        var trackingScript = string.Empty;

        //ensure Marketing Automation is enabled
        if (!_brevoSettings.UseMarketingAutomation)
            return Content(trackingScript);

        //get customer email
        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogDebug("InvokeAsync(widgetZone={WidgetZone},additionalData={AdditionalData}): customer → {Customer}", widgetZone, additionalData, customer);
        var customerEmail = !await _customerService.IsGuestAsync(customer)
            ? customer.Email?.Replace("'", "\\'")
            : string.Empty;

        //prepare tracking script
        trackingScript = $"{_brevoSettings.TrackingScript}{Environment.NewLine}"
            .Replace(BrevoDefaults.TrackingScriptId, _brevoSettings.MarketingAutomationKey)
            .Replace(BrevoDefaults.TrackingScriptCustomerEmail, customerEmail);

		_logger.LogDebug("<<< Exiting InvokeAsync(widgetZone={WidgetZone},additionalData={AdditionalData})", widgetZone, additionalData);
        return View("~/Plugins/Misc.Brevo/Views/PublicInfo.cshtml", trackingScript);
    }

    #endregion
}