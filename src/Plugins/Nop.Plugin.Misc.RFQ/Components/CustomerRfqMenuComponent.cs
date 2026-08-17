using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Customers;
using Nop.Web.Framework.Components;
using Nop.Web.Models.Customer;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Misc.RFQ.Components;

/// <summary>
/// Represents the view component to display an item in the customer menu
/// </summary>
public class CustomerRfqMenuComponent : NopViewComponent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    private readonly ICustomerService _customerService;
    private readonly IWorkContext _workContext;
    private readonly RfqSettings _rfqSettings;

    #endregion

    #region Ctor

    public CustomerRfqMenuComponent(ICustomerService customerService, 
        IWorkContext workContext,
        RfqSettings rfqSettings)
    {
		_logger.LogDebug(">>> Entering CustomerRfqMenuComponent(customerService={CustomerService},workContext={WorkContext},rfqSettings={RfqSettings})", customerService, workContext, rfqSettings);
        _customerService = customerService;
        _workContext = workContext;
		_logger.LogDebug("CustomerRfqMenuComponent(customerService={CustomerService},workContext={WorkContext},rfqSettings={RfqSettings}): _workContext → {_workContext}", customerService, workContext, rfqSettings, _workContext);
		_logger.LogDebug("CustomerRfqMenuComponent(customerService={CustomerService},workContext={WorkContext},rfqSettings={RfqSettings}): _customerService → {_customerService}", customerService, workContext, rfqSettings, _customerService);
        _rfqSettings = rfqSettings;
		_logger.LogDebug("<<< Exiting CustomerRfqMenuComponent(customerService={CustomerService},workContext={WorkContext},rfqSettings={RfqSettings})", customerService, workContext, rfqSettings);
		_logger.LogDebug("CustomerRfqMenuComponent(customerService={CustomerService},workContext={WorkContext},rfqSettings={RfqSettings}): _rfqSettings → {_rfqSettings}", customerService, workContext, rfqSettings, _rfqSettings);
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
        if (additionalData is not CustomerNavigationModel model)
            return Content(string.Empty);

        if (await _customerService.IsGuestAsync(await _workContext.GetCurrentCustomerAsync()))
            return Content(string.Empty);

        if (!_rfqSettings.Enabled)
		_logger.LogDebug("<<< Exiting InvokeAsync(widgetZone={WidgetZone},additionalData={AdditionalData})", widgetZone, additionalData);
            return Content(string.Empty);

        return View("~/Plugins/Misc.RFQ/Views/Components/CustomerRfqMenu.cshtml", model);
    }

    #endregion
}