using System.Text;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.ExportImport;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Directory;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class CountryController : BaseAdminController
{
    #region Fields

    protected readonly IAddressService _addressService;
    protected readonly ICountryModelFactory _countryModelFactory;
    protected readonly ICountryService _countryService;
    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly IExportManager _exportManager;
    protected readonly IImportManager _importManager;
    protected readonly ILocalizationService _localizationService;
    protected readonly ILocalizedEntityService _localizedEntityService;
    protected readonly INotificationService _notificationService;
    protected readonly IStateProvinceService _stateProvinceService;
    protected readonly IStoreMappingService _storeMappingService;

    #endregion

    #region Ctor

    public CountryController(IAddressService addressService,
        ICountryModelFactory countryModelFactory,
        ICountryService countryService,
        ICustomerActivityService customerActivityService,
        IExportManager exportManager,
        IImportManager importManager,
        ILocalizationService localizationService,
        ILocalizedEntityService localizedEntityService,
        INotificationService notificationService,
        IStateProvinceService stateProvinceService,
        IStoreMappingService storeMappingService)
    {
        _addressService = addressService;
        _countryModelFactory = countryModelFactory;
        _countryService = countryService;
        _customerActivityService = customerActivityService;
        _exportManager = exportManager;
        _importManager = importManager;
        _localizationService = localizationService;
        _localizedEntityService = localizedEntityService;
        _notificationService = notificationService;
        _stateProvinceService = stateProvinceService;
        _storeMappingService = storeMappingService;
    }

    #endregion

    #region Utilities

    protected virtual async Task UpdateLocalesAsync(Country country, CountryModel model)
    {
        foreach (var localized in model.Locales)
        {
            await _localizedEntityService.SaveLocalizedValueAsync(country,
                x => x.Name,
                localized.Name,
                localized.LanguageId);
        }
    }

    protected virtual async Task UpdateLocalesAsync(StateProvince stateProvince, StateProvinceModel model)
    {
        foreach (var localized in model.Locales)
        {
            await _localizedEntityService.SaveLocalizedValueAsync(stateProvince,
                x => x.Name,
                localized.Name,
                localized.LanguageId);
        }
    }

    #endregion

    #region Countries

    public virtual IActionResult Index()
    {
		logger.LogDebug(">>> Entering Index()");
		logger.LogDebug("<<< Exiting Index()");
        return RedirectToAction("List");
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_COUNTRIES)]
    public virtual async Task<IActionResult> List()
    {
		logger.LogDebug(">>> Entering List()");
        //prepare model
        var model = await _countryModelFactory.PrepareCountrySearchModelAsync(new CountrySearchModel());

		logger.LogDebug("List(): model → {Model}", model);
		logger.LogDebug("<<< Exiting List()");
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_COUNTRIES)]
    public virtual async Task<IActionResult> CountryList(CountrySearchModel searchModel)
    {
		logger.LogDebug(">>> Entering CountryList(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _countryModelFactory.PrepareCountryListModelAsync(searchModel);

		logger.LogDebug("CountryList(searchModel={SearchModel}): model → {Model}", searchModel, model);
		logger.LogDebug("<<< Exiting CountryList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_COUNTRIES)]
    public virtual async Task<IActionResult> Create()
    {
		logger.LogDebug(">>> Entering Create()");
        //prepare model
        var model = await _countryModelFactory.PrepareCountryModelAsync(new CountryModel(), null);

		logger.LogDebug("Create(): model → {Model}", model);
		logger.LogDebug("<<< Exiting Create()");
        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_COUNTRIES)]
    public virtual async Task<IActionResult> Create(CountryModel model, bool continueEditing)
    {
        if (ModelState.IsValid)
        {
            var country = model.ToEntity<Country>();
            await _countryService.InsertCountryAsync(country);

            //activity log
            await _customerActivityService.InsertActivityAsync("AddNewCountry",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewCountry"), country.Id), country);

            //locales
            await UpdateLocalesAsync(country, model);

            //Stores
            await _storeMappingService.SaveStoreMappingsAsync(country, model.SelectedStoreIds);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Countries.Added"));

            if (!continueEditing)
                return RedirectToAction("List");

            return RedirectToAction("Edit", new { id = country.Id });
        }

        //prepare model
        model = await _countryModelFactory.PrepareCountryModelAsync(model, null, true);

        //if we got this far, something failed, redisplay form
		logger.LogDebug("<<< Exiting Create(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_COUNTRIES)]
    public virtual async Task<IActionResult> Edit(int id)
    {
		logger.LogDebug(">>> Entering Edit(id={Id})", id);
        //try to get a country with the specified id
        var country = await _countryService.GetCountryByIdAsync(id);
        if (country == null)
			logger.LogDebug("Edit(id={Id}): country → {Country}", id, country);
            return RedirectToAction("List");

        //prepare model
        var model = await _countryModelFactory.PrepareCountryModelAsync(null, country);

		logger.LogDebug("Edit(id={Id}): model → {Model}", id, model);
		logger.LogDebug("<<< Exiting Edit(id={Id})", id);
        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_COUNTRIES)]
    public virtual async Task<IActionResult> Edit(CountryModel model, bool continueEditing)
    {
        //try to get a country with the specified id
        var country = await _countryService.GetCountryByIdAsync(model.Id);
        if (country == null)
            return RedirectToAction("List");

        if (ModelState.IsValid)
        {
            country = model.ToEntity(country);
            await _countryService.UpdateCountryAsync(country);

            //activity log
            await _customerActivityService.InsertActivityAsync("EditCountry",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditCountry"), country.Id), country);

            //locales
            await UpdateLocalesAsync(country, model);

            //stores
            await _storeMappingService.SaveStoreMappingsAsync(country, model.SelectedStoreIds);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Countries.Updated"));

            if (!continueEditing)
                return RedirectToAction("List");

            return RedirectToAction("Edit", new { id = country.Id });
        }

        //prepare model
        model = await _countryModelFactory.PrepareCountryModelAsync(model, country, true);

        //if we got this far, something failed, redisplay form
		logger.LogDebug("<<< Exiting Edit(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_COUNTRIES)]
    public virtual async Task<IActionResult> Delete(int id)
    {
		logger.LogDebug(">>> Entering Delete(id={Id})", id);
        //try to get a country with the specified id
        var country = await _countryService.GetCountryByIdAsync(id);
        if (country == null)
			logger.LogDebug("Delete(id={Id}): country → {Country}", id, country);
            return RedirectToAction("List");

        try
        {
            if (await _addressService.GetAddressTotalByCountryIdAsync(country.Id) > 0)
                throw new NopException("The country can't be deleted. It has associated addresses");

            await _countryService.DeleteCountryAsync(country);

            //activity log
            await _customerActivityService.InsertActivityAsync("DeleteCountry",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteCountry"), country.Id), country);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Countries.Deleted"));

            return RedirectToAction("List");
        }
        catch (Exception exc)
        {
			logger.LogError("Exception in Delete(id={Id}): {Arg1}", id, ex.Message);
            await _notificationService.ErrorNotificationAsync(exc);
			logger.LogDebug("<<< Exiting Delete(id={Id})", id);
            return RedirectToAction("Edit", new { id = country.Id });
        }
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_COUNTRIES)]
    public virtual async Task<IActionResult> PublishSelected(ICollection<int> selectedIds)
    {
		logger.LogDebug(">>> Entering PublishSelected(selectedIds={SelectedIds})", selectedIds);
        if (selectedIds == null || !selectedIds.Any())
            return NoContent();

        var countries = await _countryService.GetCountriesByIdsAsync([.. selectedIds]);
        foreach (var country in countries)
        {
		logger.LogDebug("PublishSelected(selectedIds={SelectedIds}): countries → {Countries}", selectedIds, countries);
            country.Published = true;
            await _countryService.UpdateCountryAsync(country);
        }

		logger.LogDebug("<<< Exiting PublishSelected(selectedIds={SelectedIds})", selectedIds);
        return Json(new { Result = true });
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_COUNTRIES)]
    public virtual async Task<IActionResult> UnpublishSelected(ICollection<int> selectedIds)
    {
		logger.LogDebug(">>> Entering UnpublishSelected(selectedIds={SelectedIds})", selectedIds);
        if (selectedIds == null || !selectedIds.Any())
            return NoContent();

        var countries = await _countryService.GetCountriesByIdsAsync(selectedIds.ToArray());
        foreach (var country in countries)
        {
		logger.LogDebug("UnpublishSelected(selectedIds={SelectedIds}): countries → {Countries}", selectedIds, countries);
            country.Published = false;
            await _countryService.UpdateCountryAsync(country);
        }

		logger.LogDebug("<<< Exiting UnpublishSelected(selectedIds={SelectedIds})", selectedIds);
        return Json(new { Result = true });
    }

    #endregion

    #region States / provinces

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_COUNTRIES)]
    public virtual async Task<IActionResult> States(StateProvinceSearchModel searchModel)
    {
		logger.LogDebug(">>> Entering States(searchModel={SearchModel})", searchModel);
        //try to get a country with the specified id
        var country = await _countryService.GetCountryByIdAsync(searchModel.CountryId)
            ?? throw new ArgumentException("No country found with the specified id");

        //prepare model
        var model = await _countryModelFactory.PrepareStateProvinceListModelAsync(searchModel, country);

		logger.LogDebug("States(searchModel={SearchModel}): model → {Model}", searchModel, model);
		logger.LogDebug("<<< Exiting States(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_COUNTRIES)]
    public virtual async Task<IActionResult> StateCreatePopup(int countryId)
    {
		logger.LogDebug(">>> Entering StateCreatePopup(countryId={CountryId})", countryId);
        //try to get a country with the specified id
        var country = await _countryService.GetCountryByIdAsync(countryId);
        if (country == null)
			logger.LogDebug("StateCreatePopup(countryId={CountryId}): country → {Country}", countryId, country);
            return RedirectToAction("List");

        //prepare model
        var model = await _countryModelFactory.PrepareStateProvinceModelAsync(new StateProvinceModel(), country, null);

		logger.LogDebug("StateCreatePopup(countryId={CountryId}): model → {Model}", countryId, model);
		logger.LogDebug("<<< Exiting StateCreatePopup(countryId={CountryId})", countryId);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_COUNTRIES)]
    public virtual async Task<IActionResult> StateCreatePopup(StateProvinceModel model)
    {
        //try to get a country with the specified id
        var country = await _countryService.GetCountryByIdAsync(model.CountryId);
        if (country == null)
            return RedirectToAction("List");

        if (ModelState.IsValid)
        {
            var sp = model.ToEntity<StateProvince>();

            await _stateProvinceService.InsertStateProvinceAsync(sp);

            //activity log
            await _customerActivityService.InsertActivityAsync("AddNewStateProvince",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewStateProvince"), sp.Id), sp);

            await UpdateLocalesAsync(sp, model);

            ViewBag.RefreshPage = true;

            return View(model);
        }

        //prepare model
        model = await _countryModelFactory.PrepareStateProvinceModelAsync(model, country, null, true);

        //if we got this far, something failed, redisplay form
		logger.LogDebug("<<< Exiting StateCreatePopup(model={Model})", model);
        return View(model);
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_COUNTRIES)]
    public virtual async Task<IActionResult> StateEditPopup(int id)
    {
		logger.LogDebug(">>> Entering StateEditPopup(id={Id})", id);
        //try to get a state with the specified id
        var state = await _stateProvinceService.GetStateProvinceByIdAsync(id);
        if (state == null)
			logger.LogDebug("StateEditPopup(id={Id}): state → {State}", id, state);
            return RedirectToAction("List");

        //try to get a country with the specified id
        var country = await _countryService.GetCountryByIdAsync(state.CountryId);
        if (country == null)
			logger.LogDebug("StateEditPopup(id={Id}): country → {Country}", id, country);
            return RedirectToAction("List");

        //prepare model
        var model = await _countryModelFactory.PrepareStateProvinceModelAsync(null, country, state);

		logger.LogDebug("StateEditPopup(id={Id}): model → {Model}", id, model);
		logger.LogDebug("<<< Exiting StateEditPopup(id={Id})", id);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_COUNTRIES)]
    public virtual async Task<IActionResult> StateEditPopup(StateProvinceModel model)
    {
        //try to get a state with the specified id
        var state = await _stateProvinceService.GetStateProvinceByIdAsync(model.Id);
        if (state == null)
            return RedirectToAction("List");

        //try to get a country with the specified id
        var country = await _countryService.GetCountryByIdAsync(state.CountryId);
        if (country == null)
            return RedirectToAction("List");

        if (ModelState.IsValid)
        {
            state = model.ToEntity(state);
            await _stateProvinceService.UpdateStateProvinceAsync(state);

            //activity log
            await _customerActivityService.InsertActivityAsync("EditStateProvince",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditStateProvince"), state.Id), state);

            await UpdateLocalesAsync(state, model);

            ViewBag.RefreshPage = true;

            return View(model);
        }

        //prepare model
        model = await _countryModelFactory.PrepareStateProvinceModelAsync(model, country, state, true);

        //if we got this far, something failed, redisplay form
		logger.LogDebug("<<< Exiting StateEditPopup(model={Model})", model);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_COUNTRIES)]
    public virtual async Task<IActionResult> StateDelete(int id)
    {
		logger.LogDebug(">>> Entering StateDelete(id={Id})", id);
        //try to get a state with the specified id
        var state = await _stateProvinceService.GetStateProvinceByIdAsync(id)
            ?? throw new ArgumentException("No state found with the specified id");

        if (await _addressService.GetAddressTotalByStateProvinceIdAsync(state.Id) > 0)
        {
            return ErrorJson(await _localizationService.GetResourceAsync("Admin.Configuration.Countries.States.CantDeleteWithAddresses"));
        }

        //int countryId = state.CountryId;
        await _stateProvinceService.DeleteStateProvinceAsync(state);

        //activity log
        await _customerActivityService.InsertActivityAsync("DeleteStateProvince",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteStateProvince"), state.Id), state);

		logger.LogDebug("<<< Exiting StateDelete(id={Id})", id);
        return new NullJsonResult();
    }

    public virtual async Task<IActionResult> GetStatesByCountryId(string countryId, bool? addSelectStateItem, bool? addAsterisk)
    {
		logger.LogDebug(">>> Entering GetStatesByCountryId(countryId={CountryId},addSelectStateItem={AddSelectStateItem},addAsterisk={AddAsterisk})", countryId, addSelectStateItem, addAsterisk);
        //permission validation is not required here

        // This action method gets called via an ajax request
        ArgumentException.ThrowIfNullOrEmpty(countryId);

        var country = await _countryService.GetCountryByIdAsync(Convert.ToInt32(countryId));
        var states = country != null ? (await _stateProvinceService.GetStateProvincesByCountryIdAsync(country.Id, showHidden: true)).ToList() : new List<StateProvince>();
		logger.LogDebug("GetStatesByCountryId(countryId={CountryId},addSelectStateItem={AddSelectStateItem},addAsterisk={AddAsterisk}): country → {Country}", countryId, addSelectStateItem, addAsterisk, country);
		logger.LogDebug("GetStatesByCountryId(countryId={CountryId},addSelectStateItem={AddSelectStateItem},addAsterisk={AddAsterisk}): states → {States}", countryId, addSelectStateItem, addAsterisk, states);
        var result = (from s in states
            select new { id = s.Id, name = s.Name }).ToList();
        if (addAsterisk.HasValue && addAsterisk.Value)
        {
            //asterisk
            result.Insert(0, new { id = 0, name = "*" });
        }
        else
        {
            if (country == null)
            {
                //country is not selected ("choose country" item)
                if (addSelectStateItem.HasValue && addSelectStateItem.Value)
                {
                    result.Insert(0, new { id = 0, name = await _localizationService.GetResourceAsync("Admin.Address.SelectState") });
                }
                else
                {
                    result.Insert(0, new { id = 0, name = await _localizationService.GetResourceAsync("Admin.Address.Other") });
                }
            }
            else
            {
                //some country is selected
                if (!result.Any())
                {
                    //country does not have states
                    result.Insert(0, new { id = 0, name = await _localizationService.GetResourceAsync("Admin.Address.Other") });
                }
                else
                {
                    //country has some states
                    if (addSelectStateItem.HasValue && addSelectStateItem.Value)
                    {
                        result.Insert(0, new { id = 0, name = await _localizationService.GetResourceAsync("Admin.Address.SelectState") });
                    }
                }
            }
        }

		logger.LogDebug("<<< Exiting GetStatesByCountryId(countryId={CountryId},addSelectStateItem={AddSelectStateItem},addAsterisk={AddAsterisk})", countryId, addSelectStateItem, addAsterisk);
        return Json(result);
    }

    #endregion

    #region Export / import

    [CheckPermission(StandardPermission.Configuration.MANAGE_COUNTRIES)]
    public virtual async Task<IActionResult> ExportCsv()
    {
		logger.LogDebug(">>> Entering ExportCsv()");
        var fileName = $"states_{DateTime.Now:yyyy-MM-dd-HH-mm-ss}_{CommonHelper.GenerateRandomDigitCode(4)}.csv";

		logger.LogDebug("ExportCsv(): fileName → {FileName}", fileName);
        var states = await _stateProvinceService.GetStateProvincesAsync(true);
        var result = await _exportManager.ExportStatesToTxtAsync(states);
		logger.LogDebug("ExportCsv(): states → {States}", states);
		logger.LogDebug("ExportCsv(): result → {Result}", result);

		logger.LogDebug("<<< Exiting ExportCsv()");
        return File(Encoding.UTF8.GetBytes(result), MimeTypes.TextCsv, fileName);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_COUNTRIES)]
    public virtual async Task<IActionResult> ImportCsv(IFormFile importcsvfile)
    {
		logger.LogDebug(">>> Entering ImportCsv(importcsvfile={Importcsvfile})", importcsvfile);
        try
        {
            if (importcsvfile != null && importcsvfile.Length > 0)
            {
                var count = await _importManager.ImportStatesFromTxtAsync(importcsvfile.OpenReadStream());

				logger.LogDebug("ImportCsv(importcsvfile={Importcsvfile}): count → {Count}", importcsvfile, count);
                _notificationService.SuccessNotification(string.Format(await _localizationService.GetResourceAsync("Admin.Configuration.Countries.ImportSuccess"), count));

                return RedirectToAction("List");
            }

            _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Common.UploadFile"));

            return RedirectToAction("List");
        }
        catch (Exception exc)
        {
			logger.LogError("Exception in ImportCsv(importcsvfile={Importcsvfile}): {Arg1}", importcsvfile, ex.Message);
            await _notificationService.ErrorNotificationAsync(exc);
			logger.LogDebug("<<< Exiting ImportCsv(importcsvfile={Importcsvfile})", importcsvfile);
            return RedirectToAction("List");
        }
    }

    #endregion
}