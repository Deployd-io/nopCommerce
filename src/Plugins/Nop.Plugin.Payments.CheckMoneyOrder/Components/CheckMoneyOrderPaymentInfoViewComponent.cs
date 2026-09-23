using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Payments.CheckMoneyOrder.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Components;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Payments.CheckMoneyOrder.Components;

public class CheckMoneyOrderViewComponent : NopViewComponent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    protected readonly CheckMoneyOrderPaymentSettings _checkMoneyOrderPaymentSettings;
    protected readonly ILocalizationService _localizationService;
    protected readonly IStoreContext _storeContext;
    protected readonly IWorkContext _workContext;

    public CheckMoneyOrderViewComponent(CheckMoneyOrderPaymentSettings checkMoneyOrderPaymentSettings,
        ILocalizationService localizationService,
        IStoreContext storeContext,
        IWorkContext workContext)
    {
		_logger.LogError(">>> Entering CheckMoneyOrderViewComponent(checkMoneyOrderPaymentSettings={CheckMoneyOrderPaymentSettings},localizationService={LocalizationService},storeContext={StoreContext},workContext={WorkContext})", checkMoneyOrderPaymentSettings, localizationService, storeContext, workContext);
        _checkMoneyOrderPaymentSettings = checkMoneyOrderPaymentSettings;
        _localizationService = localizationService;
		_logger.LogDebug("CheckMoneyOrderViewComponent(checkMoneyOrderPaymentSettings={CheckMoneyOrderPaymentSettings},localizationService={LocalizationService},storeContext={StoreContext},workContext={WorkContext}): _localizationService → {_localizationService}", checkMoneyOrderPaymentSettings, localizationService, storeContext, workContext, _localizationService);
		_logger.LogDebug("CheckMoneyOrderViewComponent(checkMoneyOrderPaymentSettings={CheckMoneyOrderPaymentSettings},localizationService={LocalizationService},storeContext={StoreContext},workContext={WorkContext}): _checkMoneyOrderPaymentSettings → {_checkMoneyOrderPaymentSettings}", checkMoneyOrderPaymentSettings, localizationService, storeContext, workContext, _checkMoneyOrderPaymentSettings);
    _logger.LogWarning("<<< Exiting CheckMoneyOrderViewComponent(checkMoneyOrderPaymentSettings={CheckMoneyOrderPaymentSettings},localizationService={LocalizationService},storeContext={StoreContext},workContext={WorkContext})", checkMoneyOrderPaymentSettings, localizationService, storeContext, workContext);
        _storeContext = storeContext;
		_logger.LogDebug("CheckMoneyOrderViewComponent(checkMoneyOrderPaymentSettings={CheckMoneyOrderPaymentSettings},localizationService={LocalizationService},storeContext={StoreContext},workContext={WorkContext}): _storeContext → {_storeContext}", checkMoneyOrderPaymentSettings, localizationService, storeContext, workContext, _storeContext);
        _workContext = workContext;
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task<IViewComponentResult> InvokeAsync()
    {
		_logger.LogError(">>> Entering InvokeAsync()");
        var store = await _storeContext.GetCurrentStoreAsync();

		_logger.LogError("InvokeAsync(): store → {Store}", store);
        var model = new PaymentInfoModel
        {
            DescriptionText = await _localizationService.GetLocalizedSettingAsync(_checkMoneyOrderPaymentSettings,
                x => x.DescriptionText, (await _workContext.GetWorkingLanguageAsync()).Id, store.Id)
        };

		_logger.LogWarning("<<< Exiting InvokeAsync()");
        return View("~/Plugins/Payments.CheckMoneyOrder/Views/PaymentInfo.cshtml", model);
    }
}