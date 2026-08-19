using System.Text;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Messages;
using Nop.Services.ExportImport;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Messages;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class NewsLetterSubscriptionController : BaseAdminController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly IExportManager _exportManager;
    protected readonly IImportManager _importManager;
    protected readonly ILocalizationService _localizationService;
    protected readonly INewsLetterSubscriptionModelFactory _newsletterSubscriptionModelFactory;
    protected readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
    protected readonly INewsLetterSubscriptionTypeService _newsLetterSubscriptionTypeService;
    protected readonly INotificationService _notificationService;
    protected readonly IPermissionService _permissionService;

    #endregion

    #region Ctor

    public NewsLetterSubscriptionController(IDateTimeHelper dateTimeHelper,
        IExportManager exportManager,
        IImportManager importManager,
        ILocalizationService localizationService,
        INewsLetterSubscriptionModelFactory newsletterSubscriptionModelFactory,
        INewsLetterSubscriptionService newsLetterSubscriptionService,
        INewsLetterSubscriptionTypeService newsLetterSubscriptionTypeService,
        INotificationService notificationService,
        IPermissionService permissionService)
    {
		_logger.LogDebug(">>> Entering NewsLetterSubscriptionController(dateTimeHelper={DateTimeHelper},exportManager={ExportManager},importManager={ImportManager},localizationService={LocalizationService},newsletterSubscriptionModelFactory={NewsletterSubscriptionModelFactory},newsLetterSubscriptionService={NewsLetterSubscriptionService},newsLetterSubscriptionTypeService={NewsLetterSubscriptionTypeService},notificationService={NotificationService},permissionService={PermissionService})", dateTimeHelper, exportManager, importManager, localizationService, newsletterSubscriptionModelFactory, newsLetterSubscriptionService, newsLetterSubscriptionTypeService, notificationService, permissionService);
        _dateTimeHelper = dateTimeHelper;
		_logger.LogDebug("NewsLetterSubscriptionController(dateTimeHelper={DateTimeHelper},exportManager={ExportManager},importManager={ImportManager},localizationService={LocalizationService},newsletterSubscriptionModelFactory={NewsletterSubscriptionModelFactory},newsLetterSubscriptionService={NewsLetterSubscriptionService},newsLetterSubscriptionTypeService={NewsLetterSubscriptionTypeService},notificationService={NotificationService},permissionService={PermissionService}): _dateTimeHelper → {_dateTimeHelper}", dateTimeHelper, exportManager, importManager, localizationService, newsletterSubscriptionModelFactory, newsLetterSubscriptionService, newsLetterSubscriptionTypeService, notificationService, permissionService, _dateTimeHelper);
        _exportManager = exportManager;
		_logger.LogDebug("NewsLetterSubscriptionController(dateTimeHelper={DateTimeHelper},exportManager={ExportManager},importManager={ImportManager},localizationService={LocalizationService},newsletterSubscriptionModelFactory={NewsletterSubscriptionModelFactory},newsLetterSubscriptionService={NewsLetterSubscriptionService},newsLetterSubscriptionTypeService={NewsLetterSubscriptionTypeService},notificationService={NotificationService},permissionService={PermissionService}): _exportManager → {_exportManager}", dateTimeHelper, exportManager, importManager, localizationService, newsletterSubscriptionModelFactory, newsLetterSubscriptionService, newsLetterSubscriptionTypeService, notificationService, permissionService, _exportManager);
        _importManager = importManager;
		_logger.LogDebug("NewsLetterSubscriptionController(dateTimeHelper={DateTimeHelper},exportManager={ExportManager},importManager={ImportManager},localizationService={LocalizationService},newsletterSubscriptionModelFactory={NewsletterSubscriptionModelFactory},newsLetterSubscriptionService={NewsLetterSubscriptionService},newsLetterSubscriptionTypeService={NewsLetterSubscriptionTypeService},notificationService={NotificationService},permissionService={PermissionService}): _importManager → {_importManager}", dateTimeHelper, exportManager, importManager, localizationService, newsletterSubscriptionModelFactory, newsLetterSubscriptionService, newsLetterSubscriptionTypeService, notificationService, permissionService, _importManager);
        _localizationService = localizationService;
		_logger.LogDebug("NewsLetterSubscriptionController(dateTimeHelper={DateTimeHelper},exportManager={ExportManager},importManager={ImportManager},localizationService={LocalizationService},newsletterSubscriptionModelFactory={NewsletterSubscriptionModelFactory},newsLetterSubscriptionService={NewsLetterSubscriptionService},newsLetterSubscriptionTypeService={NewsLetterSubscriptionTypeService},notificationService={NotificationService},permissionService={PermissionService}): _localizationService → {_localizationService}", dateTimeHelper, exportManager, importManager, localizationService, newsletterSubscriptionModelFactory, newsLetterSubscriptionService, newsLetterSubscriptionTypeService, notificationService, permissionService, _localizationService);
        _newsletterSubscriptionModelFactory = newsletterSubscriptionModelFactory;
		_logger.LogDebug("NewsLetterSubscriptionController(dateTimeHelper={DateTimeHelper},exportManager={ExportManager},importManager={ImportManager},localizationService={LocalizationService},newsletterSubscriptionModelFactory={NewsletterSubscriptionModelFactory},newsLetterSubscriptionService={NewsLetterSubscriptionService},newsLetterSubscriptionTypeService={NewsLetterSubscriptionTypeService},notificationService={NotificationService},permissionService={PermissionService}): _newsletterSubscriptionModelFactory → {_newsletterSubscriptionModelFactory}", dateTimeHelper, exportManager, importManager, localizationService, newsletterSubscriptionModelFactory, newsLetterSubscriptionService, newsLetterSubscriptionTypeService, notificationService, permissionService, _newsletterSubscriptionModelFactory);
        _newsLetterSubscriptionService = newsLetterSubscriptionService;
		_logger.LogDebug("NewsLetterSubscriptionController(dateTimeHelper={DateTimeHelper},exportManager={ExportManager},importManager={ImportManager},localizationService={LocalizationService},newsletterSubscriptionModelFactory={NewsletterSubscriptionModelFactory},newsLetterSubscriptionService={NewsLetterSubscriptionService},newsLetterSubscriptionTypeService={NewsLetterSubscriptionTypeService},notificationService={NotificationService},permissionService={PermissionService}): _newsLetterSubscriptionService → {_newsLetterSubscriptionService}", dateTimeHelper, exportManager, importManager, localizationService, newsletterSubscriptionModelFactory, newsLetterSubscriptionService, newsLetterSubscriptionTypeService, notificationService, permissionService, _newsLetterSubscriptionService);
        _newsLetterSubscriptionTypeService = newsLetterSubscriptionTypeService;
		_logger.LogDebug("NewsLetterSubscriptionController(dateTimeHelper={DateTimeHelper},exportManager={ExportManager},importManager={ImportManager},localizationService={LocalizationService},newsletterSubscriptionModelFactory={NewsletterSubscriptionModelFactory},newsLetterSubscriptionService={NewsLetterSubscriptionService},newsLetterSubscriptionTypeService={NewsLetterSubscriptionTypeService},notificationService={NotificationService},permissionService={PermissionService}): _newsLetterSubscriptionTypeService → {_newsLetterSubscriptionTypeService}", dateTimeHelper, exportManager, importManager, localizationService, newsletterSubscriptionModelFactory, newsLetterSubscriptionService, newsLetterSubscriptionTypeService, notificationService, permissionService, _newsLetterSubscriptionTypeService);
        _notificationService = notificationService;
		_logger.LogDebug("NewsLetterSubscriptionController(dateTimeHelper={DateTimeHelper},exportManager={ExportManager},importManager={ImportManager},localizationService={LocalizationService},newsletterSubscriptionModelFactory={NewsletterSubscriptionModelFactory},newsLetterSubscriptionService={NewsLetterSubscriptionService},newsLetterSubscriptionTypeService={NewsLetterSubscriptionTypeService},notificationService={NotificationService},permissionService={PermissionService}): _notificationService → {_notificationService}", dateTimeHelper, exportManager, importManager, localizationService, newsletterSubscriptionModelFactory, newsLetterSubscriptionService, newsLetterSubscriptionTypeService, notificationService, permissionService, _notificationService);
        _permissionService = permissionService;
		_logger.LogDebug("<<< Exiting NewsLetterSubscriptionController(dateTimeHelper={DateTimeHelper},exportManager={ExportManager},importManager={ImportManager},localizationService={LocalizationService},newsletterSubscriptionModelFactory={NewsletterSubscriptionModelFactory},newsLetterSubscriptionService={NewsLetterSubscriptionService},newsLetterSubscriptionTypeService={NewsLetterSubscriptionTypeService},notificationService={NotificationService},permissionService={PermissionService})", dateTimeHelper, exportManager, importManager, localizationService, newsletterSubscriptionModelFactory, newsLetterSubscriptionService, newsLetterSubscriptionTypeService, notificationService, permissionService);
		_logger.LogDebug("NewsLetterSubscriptionController(dateTimeHelper={DateTimeHelper},exportManager={ExportManager},importManager={ImportManager},localizationService={LocalizationService},newsletterSubscriptionModelFactory={NewsletterSubscriptionModelFactory},newsLetterSubscriptionService={NewsLetterSubscriptionService},newsLetterSubscriptionTypeService={NewsLetterSubscriptionTypeService},notificationService={NotificationService},permissionService={PermissionService}): _permissionService → {_permissionService}", dateTimeHelper, exportManager, importManager, localizationService, newsletterSubscriptionModelFactory, newsLetterSubscriptionService, newsLetterSubscriptionTypeService, notificationService, permissionService, _permissionService);
    }

    #endregion

    #region Methods

    public virtual IActionResult Index()
    {
		_logger.LogDebug(">>> Entering Index()");
		_logger.LogDebug("<<< Exiting Index()");
        return RedirectToAction("List");
    }

    [CheckPermission(StandardPermission.Promotions.SUBSCRIBERS_VIEW)]
    public virtual async Task<IActionResult> List()
    {
		_logger.LogDebug(">>> Entering List()");
        //prepare model
        var model = await _newsletterSubscriptionModelFactory.PrepareNewsLetterSubscriptionSearchModelAsync(new NewsLetterSubscriptionSearchModel());
		_logger.LogDebug("List(): model → {Model}", model);

		_logger.LogDebug("<<< Exiting List()");
		_logger.LogDebug("<<< Exiting List(searchModel={SearchModel})", searchModel);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Promotions.SUBSCRIBERS_VIEW)]
    public virtual async Task<IActionResult> List(NewsLetterSubscriptionSearchModel searchModel)
    {
        //prepare model
        var model = await _newsletterSubscriptionModelFactory.PrepareNewsLetterSubscriptionListModelAsync(searchModel);

        return Json(model);
    }

    [CheckPermission(StandardPermission.Promotions.SUBSCRIBERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Create()
    {
		_logger.LogDebug(">>> Entering Create()");
        //prepare model
        var model = await _newsletterSubscriptionModelFactory.PrepareNewsLetterSubscriptionModelAsync(new NewsLetterSubscriptionModel(), null);
		_logger.LogDebug("Create(): model → {Model}", model);

		_logger.LogDebug("<<< Exiting Create()");
		_logger.LogDebug("<<< Exiting Create(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Promotions.SUBSCRIBERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Create(NewsLetterSubscriptionModel model, bool continueEditing)
    {
        if (ModelState.IsValid)
        {
            var subscription = model.ToEntity<NewsLetterSubscription>();

            subscription.TypeId = model.SelectedNewsLetterSubscriptionTypeId;
            subscription.StoreId = model.SelectedNewsLetterSubscriptionStoreId;
            subscription.LanguageId = model.SelectedNewsLetterSubscriptionLanguageId;

            var subscriptionsByEmail = await _newsLetterSubscriptionService
                .GetNewsLetterSubscriptionsByEmailAsync(subscription.Email, storeId: subscription.StoreId);
            var availableSubscriptionTypes = await _newsLetterSubscriptionTypeService.GetAllNewsLetterSubscriptionTypesAsync(subscription.StoreId);

            var existingSubscription = subscriptionsByEmail.FirstOrDefault(s => s.TypeId == subscription.TypeId);
            if (existingSubscription == null && availableSubscriptionTypes.Any(x => x.Id == subscription.TypeId))
            {
                subscription.NewsLetterSubscriptionGuid = subscriptionsByEmail.FirstOrDefault()?.NewsLetterSubscriptionGuid ?? Guid.NewGuid();
                subscription.CreatedOnUtc = DateTime.UtcNow;
                await _newsLetterSubscriptionService.InsertNewsLetterSubscriptionAsync(subscription);
                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Promotions.NewsLetterSubscription.Added"));

                return continueEditing ? RedirectToAction("Edit", new { id = subscription.Id }) : RedirectToAction("List");
            }
            else
            {
                _notificationService.WarningNotification(await _localizationService.GetResourceAsync("Admin.Promotions.NewsLetterSubscription.Warning"));
            }
        }

        //prepare model
        model = await _newsletterSubscriptionModelFactory.PrepareNewsLetterSubscriptionModelAsync(model, null, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [CheckPermission(StandardPermission.Promotions.SUBSCRIBERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Edit(int id)
    {
		_logger.LogDebug(">>> Entering Edit(id={Id})", id);
        var subscription = await _newsLetterSubscriptionService.GetNewsLetterSubscriptionByIdAsync(id);
        if (subscription == null)
            return RedirectToAction("List");

        //prepare model
        var model = await _newsletterSubscriptionModelFactory.PrepareNewsLetterSubscriptionModelAsync(null, subscription);
		_logger.LogDebug("Edit(id={Id}): model → {Model}", id, model);

		_logger.LogDebug("<<< Exiting Edit(id={Id})", id);
		_logger.LogDebug("<<< Exiting Edit(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Promotions.SUBSCRIBERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Edit(NewsLetterSubscriptionModel model, bool continueEditing)
    {
        var subscription = await _newsLetterSubscriptionService.GetNewsLetterSubscriptionByIdAsync(model.Id);
        if (subscription == null)
            return RedirectToAction("List");

        if (ModelState.IsValid)
        {
            //fill entity from model
            subscription = model.ToEntity(subscription);

            subscription.TypeId = model.SelectedNewsLetterSubscriptionTypeId;
            subscription.LanguageId = model.SelectedNewsLetterSubscriptionLanguageId;
            subscription.StoreId = model.SelectedNewsLetterSubscriptionStoreId;

            var currentSubscription = (await _newsLetterSubscriptionService
                .GetNewsLetterSubscriptionsByEmailAsync(subscription.Email, storeId: subscription.StoreId, subscriptionTypeId: subscription.TypeId))
                .FirstOrDefault();

            var availableSubscriptionTypes = await _newsLetterSubscriptionTypeService.GetAllNewsLetterSubscriptionTypesAsync(subscription.StoreId);
            var isAvailableSubscriptionType = availableSubscriptionTypes.Any(x => x.Id == subscription.TypeId);

            if ((currentSubscription == null || (currentSubscription.Id == subscription.Id)) && isAvailableSubscriptionType)
            {
                await _newsLetterSubscriptionService.UpdateNewsLetterSubscriptionAsync(subscription);
                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Promotions.NewsLetterSubscription.Updated"));

                return continueEditing ? RedirectToAction("Edit", new { id = subscription.Id }) : RedirectToAction("List");
            }
            else
            {
                _notificationService.WarningNotification(await _localizationService.GetResourceAsync("Admin.Promotions.NewsLetterSubscription.Warning"));
            }
        }

        //prepare model
        model = await _newsletterSubscriptionModelFactory.PrepareNewsLetterSubscriptionModelAsync(model, subscription, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Promotions.SUBSCRIBERS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Delete(int id)
    {
		_logger.LogDebug(">>> Entering Delete(id={Id})", id);
        var subscription = await _newsLetterSubscriptionService.GetNewsLetterSubscriptionByIdAsync(id);
        if (subscription == null)
            return RedirectToAction("List");

        try
        {
            await _newsLetterSubscriptionService.DeleteNewsLetterSubscriptionAsync(subscription);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Promotions.NewsLetterSubscription.Deleted"));

            return RedirectToAction("List");
        }
        catch (Exception exc)
        {
			_logger.LogError("Exception in Delete(id={Id}): {Arg1}", id, exc.getMessage());
            await _notificationService.ErrorNotificationAsync(exc);
			_logger.LogDebug("<<< Exiting Delete(id={Id})", id);
            return RedirectToAction("Edit", new { id = subscription.Id });
        }
    }

    #region Export/Import

    [HttpPost, ActionName("ExportCSV")]
    [FormValueRequired("exportcsv")]
    [CheckPermission(StandardPermission.Promotions.SUBSCRIBERS_IMPORT_EXPORT)]
    public virtual async Task<IActionResult> ExportCsv(NewsLetterSubscriptionSearchModel model)
    {
		_logger.LogDebug(">>> Entering ExportCsv(model={Model})", model);
        bool? isActive = null;
        if (model.ActiveId == 1)
            isActive = true;
        else if (model.ActiveId == 2)
            isActive = false;

        var startDateValue = model.StartDate == null ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.StartDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());
        var endDateValue = model.EndDate == null ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.EndDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);

        var subscriptions = (await _newsLetterSubscriptionService.GetAllNewsLetterSubscriptionsAsync(model.SearchEmail,
            startDateValue, endDateValue, model.StoreId, isActive, model.CustomerRoleId, model.SubscriptionTypeId)).ToList();

        var result = await _exportManager.ExportNewsLetterSubscribersToTxtAsync(subscriptions);
		_logger.LogDebug("ExportCsv(model={Model}): result → {Result}", model, result);

        var fileName = $"newsletter_emails_{DateTime.Now:yyyy-MM-dd-HH-mm-ss}_{CommonHelper.GenerateRandomDigitCode(4)}.csv";
		_logger.LogDebug("ExportCsv(model={Model}): fileName → {FileName}", model, fileName);

		_logger.LogDebug("<<< Exiting ExportCsv(model={Model})", model);
        return File(Encoding.UTF8.GetBytes(result), MimeTypes.TextCsv, fileName);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Promotions.SUBSCRIBERS_IMPORT_EXPORT)]
    public virtual async Task<IActionResult> ImportCsv(IFormFile importcsvfile)
    {
		_logger.LogDebug(">>> Entering ImportCsv(importcsvfile={Importcsvfile})", importcsvfile);
        try
        {
            if (importcsvfile != null && importcsvfile.Length > 0)
            {
                var count = await _importManager.ImportNewsLetterSubscribersFromTxtAsync(importcsvfile.OpenReadStream());
				_logger.LogDebug("ImportCsv(importcsvfile={Importcsvfile}): count → {Count}", importcsvfile, count);

                _notificationService.SuccessNotification(string.Format(await _localizationService.GetResourceAsync("Admin.Promotions.NewsLetterSubscriptions.ImportEmailsSuccess"), count));

                return RedirectToAction("List");
            }

            _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Common.UploadFile"));
            return RedirectToAction("List");
        }
        catch (Exception exc)
        {
			_logger.LogError("Exception in ImportCsv(importcsvfile={Importcsvfile}): {Arg1}", importcsvfile, exc.getMessage());
            await _notificationService.ErrorNotificationAsync(exc);
			_logger.LogDebug("<<< Exiting ImportCsv(importcsvfile={Importcsvfile})", importcsvfile);
            return RedirectToAction("List");
        }
    }

    #endregion

    #endregion
}