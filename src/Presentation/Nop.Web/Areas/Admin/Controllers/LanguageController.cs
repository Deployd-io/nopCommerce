using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Infrastructure;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Localization;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class LanguageController : BaseAdminController
{
    #region Const

    protected const string FLAGS_PATH = "flags";

    #endregion

    #region Fields

    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly ILanguageModelFactory _languageModelFactory;
    protected readonly ILanguageService _languageService;
    protected readonly ILocalizationService _localizationService;
    protected readonly INopFileProvider _fileProvider;
    protected readonly INotificationService _notificationService;
    protected readonly IStoreMappingService _storeMappingService;
    protected readonly MediaSettings _mediaSettings;

    #endregion

    #region Ctor

    public LanguageController(ICustomerActivityService customerActivityService,
        ILanguageModelFactory languageModelFactory,
        ILanguageService languageService,
        ILocalizationService localizationService,
        INopFileProvider fileProvider,
        INotificationService notificationService,
        IStoreMappingService storeMappingService,
        MediaSettings mediaSettings)
    {
		_logger.LogDebug(">>> Entering LanguageController(customerActivityService={CustomerActivityService},languageModelFactory={LanguageModelFactory},languageService={LanguageService},localizationService={LocalizationService},fileProvider={FileProvider},notificationService={NotificationService},storeMappingService=<redacted>,mediaSettings={MediaSettings})", customerActivityService, languageModelFactory, languageService, localizationService, fileProvider, notificationService, mediaSettings);
        _customerActivityService = customerActivityService;
		_logger.LogDebug("LanguageController(customerActivityService={CustomerActivityService},languageModelFactory={LanguageModelFactory},languageService={LanguageService},localizationService={LocalizationService},fileProvider={FileProvider},notificationService={NotificationService},storeMappingService=<redacted>,mediaSettings={MediaSettings}): _customerActivityService → {_customerActivityService}", customerActivityService, languageModelFactory, languageService, localizationService, fileProvider, notificationService, mediaSettings, _customerActivityService);
        _languageModelFactory = languageModelFactory;
		_logger.LogDebug("LanguageController(customerActivityService={CustomerActivityService},languageModelFactory={LanguageModelFactory},languageService={LanguageService},localizationService={LocalizationService},fileProvider={FileProvider},notificationService={NotificationService},storeMappingService=<redacted>,mediaSettings={MediaSettings}): _languageModelFactory → {_languageModelFactory}", customerActivityService, languageModelFactory, languageService, localizationService, fileProvider, notificationService, mediaSettings, _languageModelFactory);
        _languageService = languageService;
		_logger.LogDebug("LanguageController(customerActivityService={CustomerActivityService},languageModelFactory={LanguageModelFactory},languageService={LanguageService},localizationService={LocalizationService},fileProvider={FileProvider},notificationService={NotificationService},storeMappingService=<redacted>,mediaSettings={MediaSettings}): _languageService → {_languageService}", customerActivityService, languageModelFactory, languageService, localizationService, fileProvider, notificationService, mediaSettings, _languageService);
        _localizationService = localizationService;
		_logger.LogDebug("LanguageController(customerActivityService={CustomerActivityService},languageModelFactory={LanguageModelFactory},languageService={LanguageService},localizationService={LocalizationService},fileProvider={FileProvider},notificationService={NotificationService},storeMappingService=<redacted>,mediaSettings={MediaSettings}): _localizationService → {_localizationService}", customerActivityService, languageModelFactory, languageService, localizationService, fileProvider, notificationService, mediaSettings, _localizationService);
        _fileProvider = fileProvider;
		_logger.LogDebug("LanguageController(customerActivityService={CustomerActivityService},languageModelFactory={LanguageModelFactory},languageService={LanguageService},localizationService={LocalizationService},fileProvider={FileProvider},notificationService={NotificationService},storeMappingService=<redacted>,mediaSettings={MediaSettings}): _fileProvider → {_fileProvider}", customerActivityService, languageModelFactory, languageService, localizationService, fileProvider, notificationService, mediaSettings, _fileProvider);
        _notificationService = notificationService;
		_logger.LogDebug("LanguageController(customerActivityService={CustomerActivityService},languageModelFactory={LanguageModelFactory},languageService={LanguageService},localizationService={LocalizationService},fileProvider={FileProvider},notificationService={NotificationService},storeMappingService=<redacted>,mediaSettings={MediaSettings}): _notificationService → {_notificationService}", customerActivityService, languageModelFactory, languageService, localizationService, fileProvider, notificationService, mediaSettings, _notificationService);
        _storeMappingService = storeMappingService;
		_logger.LogDebug("LanguageController(customerActivityService={CustomerActivityService},languageModelFactory={LanguageModelFactory},languageService={LanguageService},localizationService={LocalizationService},fileProvider={FileProvider},notificationService={NotificationService},storeMappingService=<redacted>,mediaSettings={MediaSettings}): _storeMappingService → {_storeMappingService}", customerActivityService, languageModelFactory, languageService, localizationService, fileProvider, notificationService, mediaSettings, _storeMappingService);
        _mediaSettings = mediaSettings;
		_logger.LogDebug("<<< Exiting LanguageController(customerActivityService={CustomerActivityService},languageModelFactory={LanguageModelFactory},languageService={LanguageService},localizationService={LocalizationService},fileProvider={FileProvider},notificationService={NotificationService},storeMappingService=<redacted>,mediaSettings={MediaSettings})", customerActivityService, languageModelFactory, languageService, localizationService, fileProvider, notificationService, mediaSettings);
		_logger.LogDebug("LanguageController(customerActivityService={CustomerActivityService},languageModelFactory={LanguageModelFactory},languageService={LanguageService},localizationService={LocalizationService},fileProvider={FileProvider},notificationService={NotificationService},storeMappingService=<redacted>,mediaSettings={MediaSettings}): _mediaSettings → {_mediaSettings}", customerActivityService, languageModelFactory, languageService, localizationService, fileProvider, notificationService, mediaSettings, _mediaSettings);
    }

    #endregion

    #region Languages

    public virtual IActionResult Index()
    {
		_logger.LogDebug(">>> Entering Index()");
		_logger.LogDebug("<<< Exiting Index()");
        return RedirectToAction("List");
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_LANGUAGES)]
    public virtual async Task<IActionResult> List()
    {
		_logger.LogDebug(">>> Entering List()");
        //prepare model
        var model = await _languageModelFactory.PrepareLanguageSearchModelAsync(new LanguageSearchModel());
		_logger.LogDebug("List(): model → {Model}", model);

		_logger.LogDebug("<<< Exiting List()");
		_logger.LogDebug("<<< Exiting List(searchModel={SearchModel})", searchModel);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_LANGUAGES)]
    public virtual async Task<IActionResult> List(LanguageSearchModel searchModel)
    {
        //prepare model
        var model = await _languageModelFactory.PrepareLanguageListModelAsync(searchModel);

        return Json(model);
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_LANGUAGES)]
    public virtual async Task<IActionResult> Create()
    {
		_logger.LogDebug(">>> Entering Create()");
        //prepare model
        var model = await _languageModelFactory.PrepareLanguageModelAsync(new LanguageModel(), null);
		_logger.LogDebug("Create(): model → {Model}", model);

		_logger.LogDebug("<<< Exiting Create()");
		_logger.LogDebug("<<< Exiting Create(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_LANGUAGES)]
    public virtual async Task<IActionResult> Create(LanguageModel model, bool continueEditing)
    {
        if (ModelState.IsValid)
        {
            var language = model.ToEntity<Language>();
            await _languageService.InsertLanguageAsync(language);

            //activity log
            await _customerActivityService.InsertActivityAsync("AddNewLanguage",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewLanguage"), language.Id), language);

            //Stores
            await _storeMappingService.SaveStoreMappingsAsync(language, model.SelectedStoreIds);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Languages.Added"));
            _notificationService.WarningNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Languages.NeedRestart"));

            if (!continueEditing)
                return RedirectToAction("List");

            return RedirectToAction("Edit", new { id = language.Id });
        }

        //prepare model
        model = await _languageModelFactory.PrepareLanguageModelAsync(model, null, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_LANGUAGES)]
    public virtual async Task<IActionResult> Edit(int id)
    {
		_logger.LogDebug(">>> Entering Edit(id={Id})", id);
        //try to get a language with the specified id
        var language = await _languageService.GetLanguageByIdAsync(id);
		_logger.LogDebug("Edit(id={Id}): language → {Language}", id, language);
        if (language == null)
            return RedirectToAction("List");

        //prepare model
        var model = await _languageModelFactory.PrepareLanguageModelAsync(null, language);
		_logger.LogDebug("Edit(id={Id}): model → {Model}", id, model);

		_logger.LogDebug("<<< Exiting Edit(id={Id})", id);
		_logger.LogDebug("<<< Exiting Edit(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_LANGUAGES)]
    public virtual async Task<IActionResult> Edit(LanguageModel model, bool continueEditing)
    {
        //try to get a language with the specified id
        var language = await _languageService.GetLanguageByIdAsync(model.Id);
        if (language == null)
            return RedirectToAction("List");

        if (ModelState.IsValid)
        {
            //ensure we have at least one published language
            var allLanguages = await _languageService.GetAllLanguagesAsync();
            if (allLanguages.Count == 1 && allLanguages[0].Id == language.Id && !model.Published)
            {
                _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Languages.PublishedLanguageRequired"));
                return RedirectToAction("Edit", new { id = language.Id });
            }

            //update
            language = model.ToEntity(language);
            await _languageService.UpdateLanguageAsync(language);

            //activity log
            await _customerActivityService.InsertActivityAsync("EditLanguage",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditLanguage"), language.Id), language);

            //Stores
            await _storeMappingService.SaveStoreMappingsAsync(language, model.SelectedStoreIds);

            //notification
            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Languages.Updated"));
            _notificationService.WarningNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Languages.NeedRestart"));

            if (!continueEditing)
                return RedirectToAction("List");

            return RedirectToAction("Edit", new { id = language.Id });
        }

        //prepare model
        model = await _languageModelFactory.PrepareLanguageModelAsync(model, language, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_LANGUAGES)]
    public virtual async Task<IActionResult> Delete(int id)
    {
		_logger.LogDebug(">>> Entering Delete(id={Id})", id);
        //try to get a language with the specified id
        var language = await _languageService.GetLanguageByIdAsync(id);
		_logger.LogDebug("Delete(id={Id}): language → {Language}", id, language);
        if (language == null)
            return RedirectToAction("List");

        //ensure we have at least one published language
        var allLanguages = await _languageService.GetAllLanguagesAsync();
		_logger.LogDebug("Delete(id={Id}): allLanguages → {AllLanguages}", id, allLanguages);
        if (allLanguages.Count == 1 && allLanguages[0].Id == language.Id)
        {
            _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Languages.PublishedLanguageRequired"));
            return RedirectToAction("Edit", new { id = language.Id });
        }

        //delete
        await _languageService.DeleteLanguageAsync(language);

        //activity log
        await _customerActivityService.InsertActivityAsync("DeleteLanguage",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteLanguage"), language.Id), language);

        //notification
        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Languages.Deleted"));
        _notificationService.WarningNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Languages.NeedRestart"));

		_logger.LogDebug("<<< Exiting Delete(id={Id})", id);
        return RedirectToAction("List");
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_LANGUAGES)]
    public virtual JsonResult GetAvailableFlagFileNames()
    {
		_logger.LogDebug(">>> Entering GetAvailableFlagFileNames()");
        var flagNames = _fileProvider
            .EnumerateFiles(_fileProvider.Combine(_fileProvider.GetLocalImagesPath(_mediaSettings), FLAGS_PATH), "*.png")
            .Select(_fileProvider.GetFileName)
            .ToList();

        var availableFlagFileNames = flagNames.Select(flagName => new SelectListItem
        {
            Text = flagName,
            Value = flagName
        }).ToList();

		_logger.LogDebug("<<< Exiting GetAvailableFlagFileNames()");
        return Json(availableFlagFileNames);
    }

    //action displaying notification (warning) to a store owner that changed culture
    public virtual async Task<IActionResult> LanguageCultureWarning(string currentCulture, string changedCulture)
    {
		_logger.LogDebug(">>> Entering LanguageCultureWarning(currentCulture={CurrentCulture},changedCulture={ChangedCulture})", currentCulture, changedCulture);
        if (currentCulture != changedCulture)
        {
            return Json(new
            {
                Result = string.Format(await _localizationService.GetResourceAsync("Admin.Configuration.Languages.CLDR.Warning"),
                    Url.Action("GeneralCommon", "Setting"))
            });
        }

		_logger.LogDebug("<<< Exiting LanguageCultureWarning(currentCulture={CurrentCulture},changedCulture={ChangedCulture})", currentCulture, changedCulture);
        return Json(new { Result = string.Empty });
    }

    #endregion

    #region Resources

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_LANGUAGES)]
    public virtual async Task<IActionResult> Resources(LocaleResourceSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering Resources(searchModel={SearchModel})", searchModel);
        //try to get a language with the specified id
        var language = await _languageService.GetLanguageByIdAsync(searchModel.LanguageId);
		_logger.LogDebug("Resources(searchModel={SearchModel}): language → {Language}", searchModel, language);
        if (language == null)
            return RedirectToAction("List");

        //prepare model
        var model = await _languageModelFactory.PrepareLocaleResourceListModelAsync(searchModel, language);
		_logger.LogDebug("Resources(searchModel={SearchModel}): model → {Model}", searchModel, model);

		_logger.LogDebug("<<< Exiting Resources(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    //ValidateAttribute is used to force model validation
    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_LANGUAGES)]
    public virtual async Task<IActionResult> ResourceUpdate([Validate] LocaleResourceModel model)
    {
		_logger.LogDebug(">>> Entering ResourceUpdate(model={Model})", model);
        if (!ModelState.IsValid)
        {
            return ErrorJson(ModelState.SerializeErrors());
        }

        var resource = await _localizationService.GetLocaleStringResourceByIdAsync(model.Id);
		_logger.LogDebug("ResourceUpdate(model={Model}): resource → {Resource}", model, resource);
        // if the resourceName changed, ensure it isn't being used by another resource
        if (!resource.ResourceName.Equals(model.ResourceName, StringComparison.InvariantCultureIgnoreCase))
        {
            var res = await _localizationService.GetLocaleStringResourceByNameAsync(model.ResourceName, model.LanguageId, false);
			_logger.LogDebug("ResourceUpdate(model={Model}): res → {Res}", model, res);
            if (res != null && res.Id != resource.Id)
            {
                return ErrorJson(string.Format(await _localizationService.GetResourceAsync("Admin.Configuration.Languages.Resources.NameAlreadyExists"), res.ResourceName));
            }
        }

        //fill entity from model
        resource = model.ToEntity(resource);

        await _localizationService.UpdateLocaleStringResourceAsync(resource);

		_logger.LogDebug("<<< Exiting ResourceUpdate(model={Model})", model);
        return new NullJsonResult();
    }

    //ValidateAttribute is used to force model validation
    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_LANGUAGES)]
    public virtual async Task<IActionResult> ResourceAdd(int languageId, [Validate] LocaleResourceModel model)
    {
		_logger.LogDebug(">>> Entering ResourceAdd(languageId={LanguageId},model={Model})", languageId, model);
        if (!ModelState.IsValid)
        {
            return ErrorJson(ModelState.SerializeErrors());
        }

        var res = await _localizationService.GetLocaleStringResourceByNameAsync(model.ResourceName, model.LanguageId, false);
		_logger.LogDebug("ResourceAdd(languageId={LanguageId},model={Model}): res → {Res}", languageId, model, res);
        if (res == null)
        {
            //fill entity from model
            var resource = model.ToEntity<LocaleStringResource>();
			_logger.LogDebug("ResourceAdd(languageId={LanguageId},model={Model}): resource → {Resource}", languageId, model, resource);

            resource.LanguageId = languageId;
			_logger.LogDebug("ResourceAdd(languageId={LanguageId},model={Model}): resource.LanguageId → {Arg2}", languageId, model, resource.LanguageId);

            await _localizationService.InsertLocaleStringResourceAsync(resource);
        }
        else
        {
		_logger.LogDebug("<<< Exiting ResourceAdd(languageId={LanguageId},model={Model})", languageId, model);
            return ErrorJson(string.Format(await _localizationService.GetResourceAsync("Admin.Configuration.Languages.Resources.NameAlreadyExists"), model.ResourceName));
        }

        return Json(new { Result = true });
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_LANGUAGES)]
    public virtual async Task<IActionResult> ResourceDelete(int id)
    {
		_logger.LogDebug(">>> Entering ResourceDelete(id={Id})", id);
        //try to get a locale resource with the specified id
        var resource = await _localizationService.GetLocaleStringResourceByIdAsync(id)
            ?? throw new ArgumentException("No resource found with the specified id", nameof(id));

        await _localizationService.DeleteLocaleStringResourceAsync(resource);

		_logger.LogDebug("<<< Exiting ResourceDelete(id={Id})", id);
        return new NullJsonResult();
    }

    #endregion

    #region Export / Import

    [CheckPermission(StandardPermission.Configuration.MANAGE_LANGUAGES)]
    public virtual async Task<IActionResult> ExportXml(int id)
    {
		_logger.LogDebug(">>> Entering ExportXml(id={Id})", id);
        //try to get a language with the specified id
        var language = await _languageService.GetLanguageByIdAsync(id);
		_logger.LogDebug("ExportXml(id={Id}): language → {Language}", id, language);
        if (language == null)
            return RedirectToAction("List");

        try
        {
            var xml = await _localizationService.ExportResourcesToXmlAsync(language);
			_logger.LogDebug("ExportXml(id={Id}): xml → {Xml}", id, xml);
            return File(Encoding.UTF8.GetBytes(xml), "application/xml", "language_pack.xml");
        }
        catch (Exception exc)
        {
			_logger.LogError("Exception in ExportXml(id={Id}): {Arg1}", id, exc.getMessage());
            await _notificationService.ErrorNotificationAsync(exc);
			_logger.LogDebug("<<< Exiting ExportXml(id={Id})", id);
            return RedirectToAction("List");
        }
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_LANGUAGES)]
    public virtual async Task<IActionResult> ImportXml(int id, IFormFile importxmlfile)
    {
		_logger.LogDebug(">>> Entering ImportXml(id={Id},importxmlfile={Importxmlfile})", id, importxmlfile);
        //try to get a language with the specified id
        var language = await _languageService.GetLanguageByIdAsync(id);
		_logger.LogDebug("ImportXml(id={Id},importxmlfile={Importxmlfile}): language → {Language}", id, importxmlfile, language);
        if (language == null)
            return RedirectToAction("List");

        try
        {
            if (importxmlfile != null && importxmlfile.Length > 0)
            {
                using var sr = new StreamReader(importxmlfile.OpenReadStream(), Encoding.UTF8);
				using Microsoft.Extensions.Logging;
                await _localizationService.ImportResourcesFromXmlAsync(language, sr);
				_logger.LogDebug("ImportXml(id={Id},importxmlfile={Importxmlfile}): sr → {Sr}", id, importxmlfile, sr);
            }
            else
            {
				private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
                _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Common.UploadFile"));
                return RedirectToAction("Edit", new { id = language.Id });
            }

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Languages.Imported"));
            return RedirectToAction("Edit", new { id = language.Id });
        }
        catch (Exception exc)
        {
			_logger.LogError("Exception in ImportXml(id={Id},importxmlfile={Importxmlfile}): {Arg2}", id, importxmlfile, exc.getMessage());
            await _notificationService.ErrorNotificationAsync(exc);
			_logger.LogDebug("<<< Exiting ImportXml(id={Id},importxmlfile={Importxmlfile})", id, importxmlfile);
            return RedirectToAction("Edit", new { id = language.Id });
        }
    }

    #endregion
}