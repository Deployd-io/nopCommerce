using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Directory;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class CurrencyController : BaseAdminController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly CurrencySettings _currencySettings;
    protected readonly ICurrencyModelFactory _currencyModelFactory;
    protected readonly ICurrencyService _currencyService;
    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly ILocalizationService _localizationService;
    protected readonly ILocalizedEntityService _localizedEntityService;
    protected readonly INotificationService _notificationService;
    protected readonly ISettingService _settingService;
    protected readonly IStoreMappingService _storeMappingService;

    #endregion

    #region Ctor

    public CurrencyController(CurrencySettings currencySettings,
        ICurrencyModelFactory currencyModelFactory,
        ICurrencyService currencyService,
        ICustomerActivityService customerActivityService,
        ILocalizationService localizationService,
        ILocalizedEntityService localizedEntityService,
        INotificationService notificationService,
        ISettingService settingService,
        IStoreMappingService storeMappingService)
    {
		_logger.LogError(">>> Entering CurrencyController(currencySettings={CurrencySettings},currencyModelFactory={CurrencyModelFactory},currencyService={CurrencyService},customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},settingService={SettingService},storeMappingService=<redacted>)", currencySettings, currencyModelFactory, currencyService, customerActivityService, localizationService, localizedEntityService, notificationService, settingService);
        _currencySettings = currencySettings;
        _currencyModelFactory = currencyModelFactory;
		_logger.LogDebug("CurrencyController(currencySettings={CurrencySettings},currencyModelFactory={CurrencyModelFactory},currencyService={CurrencyService},customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},settingService={SettingService},storeMappingService=<redacted>): _currencyModelFactory → {_currencyModelFactory}", currencySettings, currencyModelFactory, currencyService, customerActivityService, localizationService, localizedEntityService, notificationService, settingService, _currencyModelFactory);
        _currencyService = currencyService;
		_logger.LogDebug("CurrencyController(currencySettings={CurrencySettings},currencyModelFactory={CurrencyModelFactory},currencyService={CurrencyService},customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},settingService={SettingService},storeMappingService=<redacted>): _currencyService → {_currencyService}", currencySettings, currencyModelFactory, currencyService, customerActivityService, localizationService, localizedEntityService, notificationService, settingService, _currencyService);
        _customerActivityService = customerActivityService;
		_logger.LogDebug("CurrencyController(currencySettings={CurrencySettings},currencyModelFactory={CurrencyModelFactory},currencyService={CurrencyService},customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},settingService={SettingService},storeMappingService=<redacted>): _customerActivityService → {_customerActivityService}", currencySettings, currencyModelFactory, currencyService, customerActivityService, localizationService, localizedEntityService, notificationService, settingService, _customerActivityService);
        _localizationService = localizationService;
		_logger.LogDebug("CurrencyController(currencySettings={CurrencySettings},currencyModelFactory={CurrencyModelFactory},currencyService={CurrencyService},customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},settingService={SettingService},storeMappingService=<redacted>): _localizationService → {_localizationService}", currencySettings, currencyModelFactory, currencyService, customerActivityService, localizationService, localizedEntityService, notificationService, settingService, _localizationService);
        _localizedEntityService = localizedEntityService;
		_logger.LogDebug("CurrencyController(currencySettings={CurrencySettings},currencyModelFactory={CurrencyModelFactory},currencyService={CurrencyService},customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},settingService={SettingService},storeMappingService=<redacted>): _localizedEntityService → {_localizedEntityService}", currencySettings, currencyModelFactory, currencyService, customerActivityService, localizationService, localizedEntityService, notificationService, settingService, _localizedEntityService);
        _notificationService = notificationService;
		_logger.LogDebug("CurrencyController(currencySettings={CurrencySettings},currencyModelFactory={CurrencyModelFactory},currencyService={CurrencyService},customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},settingService={SettingService},storeMappingService=<redacted>): _notificationService → {_notificationService}", currencySettings, currencyModelFactory, currencyService, customerActivityService, localizationService, localizedEntityService, notificationService, settingService, _notificationService);
        _settingService = settingService;
		_logger.LogDebug("CurrencyController(currencySettings={CurrencySettings},currencyModelFactory={CurrencyModelFactory},currencyService={CurrencyService},customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},settingService={SettingService},storeMappingService=<redacted>): _settingService → {_settingService}", currencySettings, currencyModelFactory, currencyService, customerActivityService, localizationService, localizedEntityService, notificationService, settingService, _settingService);
        _storeMappingService = storeMappingService;
		_logger.LogDebug("CurrencyController(currencySettings={CurrencySettings},currencyModelFactory={CurrencyModelFactory},currencyService={CurrencyService},customerActivityService={CustomerActivityService},localizationService={LocalizationService},localizedEntityService={LocalizedEntityService},notificationService={NotificationService},settingService={SettingService},storeMappingService=<redacted>): _storeMappingService → {_storeMappingService}", currencySettings, currencyModelFactory, currencyService, customerActivityService, localizationService, localizedEntityService, notificationService, settingService, _storeMappingService);
    }

    #endregion

    #region Utilities

    protected virtual async Task UpdateLocalesAsync(Currency currency, CurrencyModel model)
    {
		_logger.LogError(">>> Entering UpdateLocalesAsync(currency={Currency},model={Model})", currency, model);
        foreach (var localized in model.Locales)
        {
            await _localizedEntityService.SaveLocalizedValueAsync(currency, x => x.Name, localized.Name, localized.LanguageId);
        }
		_logger.LogWarning("<<< Exiting UpdateLocalesAsync(currency={Currency},model={Model})", currency, model);
    }

    #endregion

    #region Methods

    public virtual IActionResult Index()
    {
		_logger.LogError(">>> Entering Index()");
        _logger.LogWarning("<<< Exiting Index()");
        return RedirectToAction("List");
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_CURRENCIES)]
    public virtual async Task<IActionResult> List(bool liveRates = false)
    {
		_logger.LogError(">>> Entering List(liveRates={LiveRates})", liveRates);
        var model = new CurrencySearchModel();

		_logger.LogError("List(liveRates={LiveRates}): model → {Model}", liveRates, model);
        try
        {
            //prepare model
            model = await _currencyModelFactory.PrepareCurrencySearchModelAsync(model, liveRates);
        }
        catch (Exception e)
        {
			_logger.LogError("Exception in List(liveRates={LiveRates}): {Arg1}", liveRates, e.Message);
            await _notificationService.ErrorNotificationAsync(e);
        }

		_logger.LogWarning("<<< Exiting List(model={Model})", model);
        return View(model);
    }

    [HttpPost]
    [FormValueRequired("save")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_CURRENCIES)]
    public virtual async Task<IActionResult> List(CurrencySearchModel model)
    {
        _currencySettings.ActiveExchangeRateProviderSystemName = model.ExchangeRateProviderModel.ExchangeRateProvider;
        _currencySettings.AutoUpdateEnabled = model.ExchangeRateProviderModel.AutoUpdateEnabled;
        await _settingService.SaveSettingAsync(_currencySettings);

        return RedirectToAction("List", "Currency");
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_CURRENCIES)]
    public virtual async Task<IActionResult> ListGrid(CurrencySearchModel searchModel)
    {
		_logger.LogError(">>> Entering ListGrid(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _currencyModelFactory.PrepareCurrencyListModelAsync(searchModel);
		_logger.LogError("ListGrid(searchModel={SearchModel}): model → {Model}", searchModel, model);

		_logger.LogWarning("<<< Exiting ListGrid(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_CURRENCIES)]
    public virtual async Task<IActionResult> ApplyRates(IEnumerable<CurrencyExchangeRateModel> rateModels)
    {
		_logger.LogError(">>> Entering ApplyRates(rateModels={RateModels})", rateModels);
        foreach (var rate in rateModels)
        {
            var currency = await _currencyService.GetCurrencyByCodeAsync(rate.CurrencyCode);
			_logger.LogError("ApplyRates(rateModels={RateModels}): currency → {Currency}", rateModels, currency);
            if (currency == null)
                continue;

            currency.Rate = rate.Rate;
			_logger.LogDebug("ApplyRates(rateModels={RateModels}): currency.Rate → {Arg1}", rateModels, currency.Rate);
			_logger.LogDebug("ApplyRates(rateModels={RateModels}): currency.UpdatedOnUtc → {Arg1}", rateModels, currency.UpdatedOnUtc);
            currency.UpdatedOnUtc = DateTime.UtcNow;
            await _currencyService.UpdateCurrencyAsync(currency);
        }

		_logger.LogWarning("<<< Exiting ApplyRates(rateModels={RateModels})", rateModels);
        return Json(new { result = true });
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_CURRENCIES)]
    public virtual async Task<IActionResult> MarkAsPrimaryExchangeRateCurrency(int id)
    {
		_logger.LogError(">>> Entering MarkAsPrimaryExchangeRateCurrency(id={Id})", id);
        _currencySettings.PrimaryExchangeRateCurrencyId = id;
        await _settingService.SaveSettingAsync(_currencySettings);
		_logger.LogDebug("MarkAsPrimaryExchangeRateCurrency(id={Id}): _currencySettings.PrimaryExchangeRateCurrencyId → {Arg1}", id, _currencySettings.PrimaryExchangeRateCurrencyId);

		_logger.LogWarning("<<< Exiting MarkAsPrimaryExchangeRateCurrency(id={Id})", id);
        return Json(new { result = true });
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_CURRENCIES)]
    public virtual async Task<IActionResult> MarkAsPrimaryStoreCurrency(int id)
    {
		_logger.LogError(">>> Entering MarkAsPrimaryStoreCurrency(id={Id})", id);
        _currencySettings.PrimaryStoreCurrencyId = id;
        await _settingService.SaveSettingAsync(_currencySettings);
		_logger.LogDebug("MarkAsPrimaryStoreCurrency(id={Id}): _currencySettings.PrimaryStoreCurrencyId → {Arg1}", id, _currencySettings.PrimaryStoreCurrencyId);

		_logger.LogWarning("<<< Exiting MarkAsPrimaryStoreCurrency(id={Id})", id);
        return Json(new { result = true });
    }

    #endregion

    #region Create / Edit / Delete

    [CheckPermission(StandardPermission.Configuration.MANAGE_CURRENCIES)]
    public virtual async Task<IActionResult> Create()
    {
		_logger.LogError(">>> Entering Create()");
        //prepare model
        var model = await _currencyModelFactory.PrepareCurrencyModelAsync(new CurrencyModel(), null);
		_logger.LogError("Create(): model → {Model}", model);

		_logger.LogWarning("<<< Exiting Create()");
		_logger.LogWarning("<<< Exiting Create(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_CURRENCIES)]
    public virtual async Task<IActionResult> Create(CurrencyModel model, bool continueEditing)
    {
        if (ModelState.IsValid)
        {
            var currency = model.ToEntity<Currency>();
            currency.CreatedOnUtc = DateTime.UtcNow;
            currency.UpdatedOnUtc = DateTime.UtcNow;
            await _currencyService.InsertCurrencyAsync(currency);

            //activity log
            await _customerActivityService.InsertActivityAsync("AddNewCurrency",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewCurrency"), currency.Id), currency);

            //locales
            await UpdateLocalesAsync(currency, model);

            //stores
            await _storeMappingService.SaveStoreMappingsAsync(currency, model.SelectedStoreIds);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Currencies.Added"));

            if (!continueEditing)
                return RedirectToAction("List");

            return RedirectToAction("Edit", new { id = currency.Id });
        }

        //prepare model
        model = await _currencyModelFactory.PrepareCurrencyModelAsync(model, null, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_CURRENCIES)]
    public virtual async Task<IActionResult> Edit(int id)
    {
		_logger.LogError(">>> Entering Edit(id={Id})", id);
        //try to get a currency with the specified id
        var currency = await _currencyService.GetCurrencyByIdAsync(id);
		_logger.LogError("Edit(id={Id}): currency → {Currency}", id, currency);
        if (currency == null)
            return RedirectToAction("List");

        //prepare model
        var model = await _currencyModelFactory.PrepareCurrencyModelAsync(null, currency);
		_logger.LogError("Edit(id={Id}): model → {Model}", id, model);

		_logger.LogWarning("<<< Exiting Edit(id={Id})", id);
		_logger.LogWarning("<<< Exiting Edit(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_CURRENCIES)]
    public virtual async Task<IActionResult> Edit(CurrencyModel model, bool continueEditing)
    {
        //try to get a currency with the specified id
        var currency = await _currencyService.GetCurrencyByIdAsync(model.Id);
        if (currency == null)
            return RedirectToAction("List");

        if (ModelState.IsValid)
        {
            //ensure we have at least one published currency
            var allCurrencies = await _currencyService.GetAllCurrenciesAsync();
            if (allCurrencies.Count == 1 && allCurrencies[0].Id == currency.Id && !model.Published)
            {
                _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Currencies.PublishedCurrencyRequired"));
                return RedirectToAction("Edit", new { id = currency.Id });
            }

            currency = model.ToEntity(currency);
            currency.UpdatedOnUtc = DateTime.UtcNow;
            await _currencyService.UpdateCurrencyAsync(currency);

            //activity log
            await _customerActivityService.InsertActivityAsync("EditCurrency",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditCurrency"), currency.Id), currency);

            //locales
            await UpdateLocalesAsync(currency, model);

            //stores
            await _storeMappingService.SaveStoreMappingsAsync(currency, model.SelectedStoreIds);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Currencies.Updated"));

            if (!continueEditing)
                return RedirectToAction("List");

            return RedirectToAction("Edit", new { id = currency.Id });
        }

        //prepare model
        model = await _currencyModelFactory.PrepareCurrencyModelAsync(model, currency, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_CURRENCIES)]
    public virtual async Task<IActionResult> Delete(int id)
    {
		_logger.LogError(">>> Entering Delete(id={Id})", id);
        //try to get a currency with the specified id
        var currency = await _currencyService.GetCurrencyByIdAsync(id);
		_logger.LogError("Delete(id={Id}): currency → {Currency}", id, currency);
        if (currency == null)
            return RedirectToAction("List");

        try
        {
            if (currency.Id == _currencySettings.PrimaryStoreCurrencyId)
				_logger.LogError("Throwing NopException from Delete(id={Id})", id);
                throw new NopException(await _localizationService.GetResourceAsync("Admin.Configuration.Currencies.CantDeletePrimary"));

            if (currency.Id == _currencySettings.PrimaryExchangeRateCurrencyId)
                throw new NopException(await _localizationService.GetResourceAsync("Admin.Configuration.Currencies.CantDeleteExchange"));

            //ensure we have at least one published currency
            var allCurrencies = await _currencyService.GetAllCurrenciesAsync();
			_logger.LogError("Delete(id={Id}): allCurrencies → {AllCurrencies}", id, allCurrencies);
            if (allCurrencies.Count == 1 && allCurrencies[0].Id == currency.Id)
            {
                _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Currencies.PublishedCurrencyRequired"));
                return RedirectToAction("Edit", new { id = currency.Id });
            }

            await _currencyService.DeleteCurrencyAsync(currency);

            //activity log
            await _customerActivityService.InsertActivityAsync("DeleteCurrency",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteCurrency"), currency.Id), currency);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Currencies.Deleted"));

            return RedirectToAction("List");
        }
        catch (Exception exc)
        {
			_logger.LogError("Exception in Delete(id={Id}): {Arg1}", id, exc.Message);
            await _notificationService.ErrorNotificationAsync(exc);
			_logger.LogWarning("<<< Exiting Delete(id={Id})", id);
            return RedirectToAction("Edit", new { id = currency.Id });
        }
    }

    #endregion
}