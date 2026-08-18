using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain;
using Nop.Core.Domain.Customers;
using Nop.Core.Http;
using Nop.Services.Common;
using Nop.Web.Framework.Components;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Components;

public partial class EuCookieLawViewComponent : NopViewComponent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly IStoreContext _storeContext;
    protected readonly IWorkContext _workContext;
    protected readonly StoreInformationSettings _storeInformationSettings;

    public EuCookieLawViewComponent(IGenericAttributeService genericAttributeService,
        IStoreContext storeContext,
        IWorkContext workContext,
        StoreInformationSettings storeInformationSettings)
    {
		_logger.LogDebug(">>> Entering EuCookieLawViewComponent(genericAttributeService={GenericAttributeService},storeContext={StoreContext},workContext={WorkContext},storeInformationSettings={StoreInformationSettings})", genericAttributeService, storeContext, workContext, storeInformationSettings);
        _genericAttributeService = genericAttributeService;
        _storeContext = storeContext;
		_logger.LogDebug("EuCookieLawViewComponent(genericAttributeService={GenericAttributeService},storeContext={StoreContext},workContext={WorkContext},storeInformationSettings={StoreInformationSettings}): _storeContext → {_storeContext}", genericAttributeService, storeContext, workContext, storeInformationSettings, _storeContext);
		_logger.LogDebug("EuCookieLawViewComponent(genericAttributeService={GenericAttributeService},storeContext={StoreContext},workContext={WorkContext},storeInformationSettings={StoreInformationSettings}): _genericAttributeService → {_genericAttributeService}", genericAttributeService, storeContext, workContext, storeInformationSettings, _genericAttributeService);
        _workContext = workContext;
		_logger.LogDebug("EuCookieLawViewComponent(genericAttributeService={GenericAttributeService},storeContext={StoreContext},workContext={WorkContext},storeInformationSettings={StoreInformationSettings}): _workContext → {_workContext}", genericAttributeService, storeContext, workContext, storeInformationSettings, _workContext);
        _storeInformationSettings = storeInformationSettings;
		_logger.LogDebug("EuCookieLawViewComponent(genericAttributeService={GenericAttributeService},storeContext={StoreContext},workContext={WorkContext},storeInformationSettings={StoreInformationSettings}): _storeInformationSettings → {_storeInformationSettings}", genericAttributeService, storeContext, workContext, storeInformationSettings, _storeInformationSettings);
		_logger.LogDebug("<<< Exiting EuCookieLawViewComponent(genericAttributeService={GenericAttributeService},storeContext={StoreContext},workContext={WorkContext},storeInformationSettings={StoreInformationSettings})", genericAttributeService, storeContext, workContext, storeInformationSettings);
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
		_logger.LogDebug(">>> Entering InvokeAsync()");
        if (!_storeInformationSettings.DisplayEuCookieLawWarning)
            //disabled
            return Content("");

        //ignore search engines because some pages could be indexed with the EU cookie as description
        var customer = await _workContext.GetCurrentCustomerAsync();
        if (customer.IsSearchEngineAccount())
            return Content("");

        var store = await _storeContext.GetCurrentStoreAsync();

        if (await _genericAttributeService.GetAttributeAsync<bool>(customer, NopCustomerDefaults.EuCookieLawAcceptedAttribute, store.Id))
            //already accepted
            return Content("");

        //ignore notification?
        //right now it's used during logout so popup window is not displayed twice
        if (TempData[$"{NopCookieDefaults.Prefix}{NopCookieDefaults.IgnoreEuCookieLawWarning}"] != null && Convert.ToBoolean(TempData[$"{NopCookieDefaults.Prefix}{NopCookieDefaults.IgnoreEuCookieLawWarning}"]))
		_logger.LogDebug("<<< Exiting InvokeAsync()");
            return Content("");

        return View();
    }
}