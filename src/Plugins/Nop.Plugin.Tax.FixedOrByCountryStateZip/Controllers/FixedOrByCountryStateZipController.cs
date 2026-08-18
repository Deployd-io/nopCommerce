using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Plugin.Tax.FixedOrByCountryStateZip.Domain;
using Nop.Plugin.Tax.FixedOrByCountryStateZip.Models;
using Nop.Plugin.Tax.FixedOrByCountryStateZip.Services;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Models.Extensions;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Tax.FixedOrByCountryStateZip.Controllers;

[AuthorizeAdmin]
[Area(AreaNames.ADMIN)]
[AutoValidateAntiforgeryToken]
public class FixedOrByCountryStateZipController : BasePluginController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly FixedOrByCountryStateZipTaxSettings _countryStateZipSettings;
    protected readonly ICountryService _countryService;
    protected readonly ICountryStateZipService _taxRateService;
    protected readonly ILocalizationService _localizationService;
    protected readonly IPermissionService _permissionService;
    protected readonly ISettingService _settingService;
    protected readonly IStateProvinceService _stateProvinceService;
    protected readonly IStoreService _storeService;
    protected readonly ITaxCategoryService _taxCategoryService;

    #endregion

    #region Ctor

    public FixedOrByCountryStateZipController(FixedOrByCountryStateZipTaxSettings countryStateZipSettings,
        ICountryService countryService,
        ICountryStateZipService taxRateService,
        ILocalizationService localizationService,
        IPermissionService permissionService,
        ISettingService settingService,
        IStateProvinceService stateProvinceService,
        IStoreService storeService,
        ITaxCategoryService taxCategoryService)

    {
        _countryStateZipSettings = countryStateZipSettings;
		_logger.LogDebug("FixedOrByCountryStateZipController(countryStateZipSettings={CountryStateZipSettings},countryService={CountryService},taxRateService={TaxRateService},localizationService={LocalizationService},permissionService={PermissionService},settingService={SettingService},stateProvinceService={StateProvinceService},storeService={StoreService},taxCategoryService={TaxCategoryService}): _countryStateZipSettings → {_countryStateZipSettings}", countryStateZipSettings, countryService, taxRateService, localizationService, permissionService, settingService, stateProvinceService, storeService, taxCategoryService, _countryStateZipSettings);
        _countryService = countryService;
		_logger.LogDebug("FixedOrByCountryStateZipController(countryStateZipSettings={CountryStateZipSettings},countryService={CountryService},taxRateService={TaxRateService},localizationService={LocalizationService},permissionService={PermissionService},settingService={SettingService},stateProvinceService={StateProvinceService},storeService={StoreService},taxCategoryService={TaxCategoryService}): _countryService → {_countryService}", countryStateZipSettings, countryService, taxRateService, localizationService, permissionService, settingService, stateProvinceService, storeService, taxCategoryService, _countryService);
        _taxRateService = taxRateService;
		_logger.LogDebug("FixedOrByCountryStateZipController(countryStateZipSettings={CountryStateZipSettings},countryService={CountryService},taxRateService={TaxRateService},localizationService={LocalizationService},permissionService={PermissionService},settingService={SettingService},stateProvinceService={StateProvinceService},storeService={StoreService},taxCategoryService={TaxCategoryService}): _taxRateService → {_taxRateService}", countryStateZipSettings, countryService, taxRateService, localizationService, permissionService, settingService, stateProvinceService, storeService, taxCategoryService, _taxRateService);
        _permissionService = permissionService;
		_logger.LogDebug("FixedOrByCountryStateZipController(countryStateZipSettings={CountryStateZipSettings},countryService={CountryService},taxRateService={TaxRateService},localizationService={LocalizationService},permissionService={PermissionService},settingService={SettingService},stateProvinceService={StateProvinceService},storeService={StoreService},taxCategoryService={TaxCategoryService}): _permissionService → {_permissionService}", countryStateZipSettings, countryService, taxRateService, localizationService, permissionService, settingService, stateProvinceService, storeService, taxCategoryService, _permissionService);
        _localizationService = localizationService;
		_logger.LogDebug("FixedOrByCountryStateZipController(countryStateZipSettings={CountryStateZipSettings},countryService={CountryService},taxRateService={TaxRateService},localizationService={LocalizationService},permissionService={PermissionService},settingService={SettingService},stateProvinceService={StateProvinceService},storeService={StoreService},taxCategoryService={TaxCategoryService}): _localizationService → {_localizationService}", countryStateZipSettings, countryService, taxRateService, localizationService, permissionService, settingService, stateProvinceService, storeService, taxCategoryService, _localizationService);
        _settingService = settingService;
		_logger.LogDebug("FixedOrByCountryStateZipController(countryStateZipSettings={CountryStateZipSettings},countryService={CountryService},taxRateService={TaxRateService},localizationService={LocalizationService},permissionService={PermissionService},settingService={SettingService},stateProvinceService={StateProvinceService},storeService={StoreService},taxCategoryService={TaxCategoryService}): _settingService → {_settingService}", countryStateZipSettings, countryService, taxRateService, localizationService, permissionService, settingService, stateProvinceService, storeService, taxCategoryService, _settingService);
        _stateProvinceService = stateProvinceService;
		_logger.LogDebug("FixedOrByCountryStateZipController(countryStateZipSettings={CountryStateZipSettings},countryService={CountryService},taxRateService={TaxRateService},localizationService={LocalizationService},permissionService={PermissionService},settingService={SettingService},stateProvinceService={StateProvinceService},storeService={StoreService},taxCategoryService={TaxCategoryService}): _stateProvinceService → {_stateProvinceService}", countryStateZipSettings, countryService, taxRateService, localizationService, permissionService, settingService, stateProvinceService, storeService, taxCategoryService, _stateProvinceService);
        _storeService = storeService;
		_logger.LogDebug("FixedOrByCountryStateZipController(countryStateZipSettings={CountryStateZipSettings},countryService={CountryService},taxRateService={TaxRateService},localizationService={LocalizationService},permissionService={PermissionService},settingService={SettingService},stateProvinceService={StateProvinceService},storeService={StoreService},taxCategoryService={TaxCategoryService}): _storeService → {_storeService}", countryStateZipSettings, countryService, taxRateService, localizationService, permissionService, settingService, stateProvinceService, storeService, taxCategoryService, _storeService);
        _taxCategoryService = taxCategoryService;
		_logger.LogDebug("FixedOrByCountryStateZipController(countryStateZipSettings={CountryStateZipSettings},countryService={CountryService},taxRateService={TaxRateService},localizationService={LocalizationService},permissionService={PermissionService},settingService={SettingService},stateProvinceService={StateProvinceService},storeService={StoreService},taxCategoryService={TaxCategoryService}): _taxCategoryService → {_taxCategoryService}", countryStateZipSettings, countryService, taxRateService, localizationService, permissionService, settingService, stateProvinceService, storeService, taxCategoryService, _taxCategoryService);

	_logger.LogDebug("<<< Exiting FixedOrByCountryStateZipController(countryStateZipSettings={CountryStateZipSettings},countryService={CountryService},taxRateService={TaxRateService},localizationService={LocalizationService},permissionService={PermissionService},settingService={SettingService},stateProvinceService={StateProvinceService},storeService={StoreService},taxCategoryService={TaxCategoryService})", countryStateZipSettings, countryService, taxRateService, localizationService, permissionService, settingService, stateProvinceService, storeService, taxCategoryService);
    }

    #endregion

    #region Methods

    [CheckPermission(StandardPermission.Configuration.MANAGE_TAX_SETTINGS)]
    public async Task<IActionResult> Configure()
    {
		_logger.LogDebug(">>> Entering Configure()");
        var taxCategories = await _taxCategoryService.GetAllTaxCategoriesAsync();

		_logger.LogDebug("Configure(): taxCategories → {TaxCategories}", taxCategories);
        if (!taxCategories.Any())
        {
            var errorModel = new ConfigurationModel
            {
                TaxCategoriesCanNotLoadedError = string.Format(
                    await _localizationService.GetResourceAsync(
                        "Plugins.Tax.FixedOrByCountryStateZip.TaxCategoriesCanNotLoaded"),
                    Url.Action("Categories", "Tax"))
            };

            return View("~/Plugins/Tax.FixedOrByCountryStateZip/Views/Configure.cshtml", errorModel);
        }

        var model = new ConfigurationModel { CountryStateZipEnabled = _countryStateZipSettings.CountryStateZipEnabled };

        //stores
        model.AvailableStores.Add(new SelectListItem { Text = "*", Value = "0" });
        var stores = await _storeService.GetAllStoresAsync();
		_logger.LogDebug("Configure(): stores → {Stores}", stores);
        foreach (var s in stores)
            model.AvailableStores.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });
        //tax categories
        foreach (var tc in taxCategories)
            model.AvailableTaxCategories.Add(new SelectListItem { Text = tc.Name, Value = tc.Id.ToString() });
        //countries
        var countries = await _countryService.GetAllCountriesAsync(showHidden: true);
		_logger.LogDebug("Configure(): countries → {Countries}", countries);
        foreach (var c in countries)
            model.AvailableCountries.Add(new SelectListItem { Text = c.Name, Value = c.Id.ToString() });
        //states
        model.AvailableStates.Add(new SelectListItem { Text = "*", Value = "0" });
        var defaultCountry = countries.FirstOrDefault();
		_logger.LogDebug("Configure(): defaultCountry → {DefaultCountry}", defaultCountry);
        if (defaultCountry != null)
        {
            var states = await _stateProvinceService.GetStateProvincesByCountryIdAsync(defaultCountry.Id);
			_logger.LogDebug("Configure(): states → {States}", states);
            foreach (var s in states)
                model.AvailableStates.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });
        }

		_logger.LogDebug("<<< Exiting Configure()");
        return View("~/Plugins/Tax.FixedOrByCountryStateZip/Views/Configure.cshtml", model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_TAX_SETTINGS)]
    public async Task<IActionResult> SaveMode(bool value)
    {
		_logger.LogDebug(">>> Entering SaveMode(value={Value})", value);
        //save settings
        _countryStateZipSettings.CountryStateZipEnabled = value;
		_logger.LogDebug("SaveMode(value={Value}): _countryStateZipSettings.CountryStateZipEnabled → {Arg1}", value, _countryStateZipSettings.CountryStateZipEnabled);
        await _settingService.SaveSettingAsync(_countryStateZipSettings);

		_logger.LogDebug("<<< Exiting SaveMode(value={Value})", value);
        return Json(new { Result = true });
    }

    #region Fixed tax

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_TAX_SETTINGS)]
    public async Task<IActionResult> FixedRatesList(ConfigurationModel searchModel)
    {
		_logger.LogDebug(">>> Entering FixedRatesList(searchModel={SearchModel})", searchModel);
        var categories = (await _taxCategoryService.GetAllTaxCategoriesAsync()).ToPagedList(searchModel);

		_logger.LogDebug("FixedRatesList(searchModel={SearchModel}): categories → {Categories}", searchModel, categories);
        var gridModel = await new FixedTaxRateListModel().PrepareToGridAsync(searchModel, categories, () =>
        {
            return categories.SelectAwait(async taxCategory => new FixedTaxRateModel
            {
                TaxCategoryId = taxCategory.Id,
                TaxCategoryName = taxCategory.Name,

                Rate = await _settingService
                    .GetSettingByKeyAsync<decimal>(string.Format(FixedOrByCountryStateZipDefaults.FIXED_RATE_SETTINGS_KEY, taxCategory.Id))
            });
        });

		_logger.LogDebug("<<< Exiting FixedRatesList(searchModel={SearchModel})", searchModel);
        return Json(gridModel);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_TAX_SETTINGS)]
    public async Task<IActionResult> FixedRateUpdate(FixedTaxRateModel model)
    {
		_logger.LogDebug(">>> Entering FixedRateUpdate(model={Model})", model);
        await _settingService.SetSettingAsync(string.Format(FixedOrByCountryStateZipDefaults.FIXED_RATE_SETTINGS_KEY, model.TaxCategoryId), model.Rate);

		_logger.LogDebug("<<< Exiting FixedRateUpdate(model={Model})", model);
        return new NullJsonResult();
    }

    #endregion

    #region Tax by country/state/zip

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_TAX_SETTINGS)]
    public async Task<IActionResult> RatesByCountryStateZipList(ConfigurationModel searchModel)
    {
		_logger.LogDebug(">>> Entering RatesByCountryStateZipList(searchModel={SearchModel})", searchModel);
        var records = await _taxRateService.GetAllTaxRatesAsync(searchModel.Page - 1, searchModel.PageSize);

		_logger.LogDebug("RatesByCountryStateZipList(searchModel={SearchModel}): records → {Records}", searchModel, records);
        var gridModel = await new CountryStateZipListModel().PrepareToGridAsync(searchModel, records, () =>
        {
            return records.SelectAwait(async record => new CountryStateZipModel
            {
                Id = record.Id,
                StoreId = record.StoreId,
                StoreName = (await _storeService.GetStoreByIdAsync(record.StoreId))?.Name ?? "*",
                TaxCategoryId = record.TaxCategoryId,
                TaxCategoryName = (await _taxCategoryService.GetTaxCategoryByIdAsync(record.TaxCategoryId))?.Name ?? string.Empty,
                CountryId = record.CountryId,
                CountryName = (await _countryService.GetCountryByIdAsync(record.CountryId))?.Name ?? "Unavailable",
                StateProvinceId = record.StateProvinceId,
                StateProvinceName = (await _stateProvinceService.GetStateProvinceByIdAsync(record.StateProvinceId))?.Name ?? "*",

                Zip = !string.IsNullOrEmpty(record.Zip) ? record.Zip : "*",
                Percentage = record.Percentage
            });
        });

		_logger.LogDebug("<<< Exiting RatesByCountryStateZipList(searchModel={SearchModel})", searchModel);
        return Json(gridModel);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_TAX_SETTINGS)]
    public async Task<IActionResult> AddRateByCountryStateZip(ConfigurationModel model)
    {
		_logger.LogDebug(">>> Entering AddRateByCountryStateZip(model={Model})", model);
        await _taxRateService.InsertTaxRateAsync(new TaxRate
        {
            StoreId = model.AddStoreId,
            TaxCategoryId = model.AddTaxCategoryId,
            CountryId = model.AddCountryId,
            StateProvinceId = model.AddStateProvinceId,
            Zip = model.AddZip,
            Percentage = model.AddPercentage
        });

		_logger.LogDebug("<<< Exiting AddRateByCountryStateZip(model={Model})", model);
        return Json(new { Result = true });
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_TAX_SETTINGS)]
    public async Task<IActionResult> UpdateRateByCountryStateZip(CountryStateZipModel model)
    {
		_logger.LogDebug(">>> Entering UpdateRateByCountryStateZip(model={Model})", model);
        var taxRate = await _taxRateService.GetTaxRateByIdAsync(model.Id);
        taxRate.Zip = model.Zip == "*" ? null : model.Zip;
		_logger.LogDebug("UpdateRateByCountryStateZip(model={Model}): taxRate → {TaxRate}", model, taxRate);
		_logger.LogDebug("UpdateRateByCountryStateZip(model={Model}): taxRate.Percentage → {Arg1}", model, taxRate.Percentage);
        taxRate.Percentage = model.Percentage;
        await _taxRateService.UpdateTaxRateAsync(taxRate);

		_logger.LogDebug("<<< Exiting UpdateRateByCountryStateZip(model={Model})", model);
        return new NullJsonResult();
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_TAX_SETTINGS)]
    public async Task<IActionResult> DeleteRateByCountryStateZip(int id)
    {
		_logger.LogDebug(">>> Entering DeleteRateByCountryStateZip(id={Id})", id);
        var taxRate = await _taxRateService.GetTaxRateByIdAsync(id);
        if (taxRate != null)
			_logger.LogDebug("DeleteRateByCountryStateZip(id={Id}): taxRate → {TaxRate}", id, taxRate);
            await _taxRateService.DeleteTaxRateAsync(taxRate);

		_logger.LogDebug("<<< Exiting DeleteRateByCountryStateZip(id={Id})", id);
        return new NullJsonResult();
    }

    #endregion

    #endregion
}