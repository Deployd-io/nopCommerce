using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Http.Extensions;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class CommonController : BaseAdminController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Const

    protected const string EXPORT_IMPORT_PATH = @"files\exportimport";

    #endregion

    #region Fields

    protected readonly ICommonModelFactory _commonModelFactory;
    protected readonly ICustomerService _customerService;
    protected readonly INopDataProvider _dataProvider;
    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly ILanguageService _languageService;
    protected readonly ILocalizationService _localizationService;
    protected readonly IMaintenanceService _maintenanceService;
    protected readonly INopFileProvider _fileProvider;
    protected readonly INotificationService _notificationService;
    protected readonly IPermissionService _permissionService;
    protected readonly IQueuedEmailService _queuedEmailService;
    protected readonly IShoppingCartService _shoppingCartService;
    protected readonly IStaticCacheManager _staticCacheManager;
    protected readonly IThumbService _thumbService;
    protected readonly IUrlRecordService _urlRecordService;
    protected readonly IWebHelper _webHelper;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public CommonController(ICommonModelFactory commonModelFactory,
        ICustomerService customerService,
        INopDataProvider dataProvider,
        IDateTimeHelper dateTimeHelper,
        ILanguageService languageService,
        ILocalizationService localizationService,
        IMaintenanceService maintenanceService,
        INopFileProvider fileProvider,
        INotificationService notificationService,
        IPermissionService permissionService,
        IQueuedEmailService queuedEmailService,
        IShoppingCartService shoppingCartService,
        IStaticCacheManager staticCacheManager,
        IThumbService thumbService,
        IUrlRecordService urlRecordService,
        IWebHelper webHelper,
        IWorkContext workContext)
    {
		_logger.LogError(">>> Entering CommonController(commonModelFactory={CommonModelFactory},customerService={CustomerService},dataProvider={DataProvider},dateTimeHelper={DateTimeHelper},languageService={LanguageService},localizationService={LocalizationService},maintenanceService={MaintenanceService},fileProvider={FileProvider},notificationService={NotificationService},permissionService={PermissionService},queuedEmailService={QueuedEmailService},shoppingCartService=<redacted>,staticCacheManager={StaticCacheManager},thumbService={ThumbService},urlRecordService={UrlRecordService},webHelper={WebHelper},workContext={WorkContext})", commonModelFactory, customerService, dataProvider, dateTimeHelper, languageService, localizationService, maintenanceService, fileProvider, notificationService, permissionService, queuedEmailService, staticCacheManager, thumbService, urlRecordService, webHelper, workContext);
        _commonModelFactory = commonModelFactory;
        _customerService = customerService;
		_logger.LogDebug("CommonController(commonModelFactory={CommonModelFactory},customerService={CustomerService},dataProvider={DataProvider},dateTimeHelper={DateTimeHelper},languageService={LanguageService},localizationService={LocalizationService},maintenanceService={MaintenanceService},fileProvider={FileProvider},notificationService={NotificationService},permissionService={PermissionService},queuedEmailService={QueuedEmailService},shoppingCartService=<redacted>,staticCacheManager={StaticCacheManager},thumbService={ThumbService},urlRecordService={UrlRecordService},webHelper={WebHelper},workContext={WorkContext}): _customerService → {_customerService}", commonModelFactory, customerService, dataProvider, dateTimeHelper, languageService, localizationService, maintenanceService, fileProvider, notificationService, permissionService, queuedEmailService, staticCacheManager, thumbService, urlRecordService, webHelper, workContext, _customerService);
        _dataProvider = dataProvider;
		_logger.LogError("CommonController(commonModelFactory={CommonModelFactory},customerService={CustomerService},dataProvider={DataProvider},dateTimeHelper={DateTimeHelper},languageService={LanguageService},localizationService={LocalizationService},maintenanceService={MaintenanceService},fileProvider={FileProvider},notificationService={NotificationService},permissionService={PermissionService},queuedEmailService={QueuedEmailService},shoppingCartService=<redacted>,staticCacheManager={StaticCacheManager},thumbService={ThumbService},urlRecordService={UrlRecordService},webHelper={WebHelper},workContext={WorkContext}): _dataProvider → {_dataProvider}", commonModelFactory, customerService, dataProvider, dateTimeHelper, languageService, localizationService, maintenanceService, fileProvider, notificationService, permissionService, queuedEmailService, staticCacheManager, thumbService, urlRecordService, webHelper, workContext, _dataProvider);
		_logger.LogDebug("CommonController(commonModelFactory={CommonModelFactory},customerService={CustomerService},dataProvider={DataProvider},dateTimeHelper={DateTimeHelper},languageService={LanguageService},localizationService={LocalizationService},maintenanceService={MaintenanceService},fileProvider={FileProvider},notificationService={NotificationService},permissionService={PermissionService},queuedEmailService={QueuedEmailService},shoppingCartService=<redacted>,staticCacheManager={StaticCacheManager},thumbService={ThumbService},urlRecordService={UrlRecordService},webHelper={WebHelper},workContext={WorkContext}): _dataProvider → {_dataProvider}", commonModelFactory, customerService, dataProvider, dateTimeHelper, languageService, localizationService, maintenanceService, fileProvider, notificationService, permissionService, queuedEmailService, staticCacheManager, thumbService, urlRecordService, webHelper, workContext, _dataProvider);
        _dateTimeHelper = dateTimeHelper;
        _languageService = languageService;
		_logger.LogDebug("CommonController(commonModelFactory={CommonModelFactory},customerService={CustomerService},dataProvider={DataProvider},dateTimeHelper={DateTimeHelper},languageService={LanguageService},localizationService={LocalizationService},maintenanceService={MaintenanceService},fileProvider={FileProvider},notificationService={NotificationService},permissionService={PermissionService},queuedEmailService={QueuedEmailService},shoppingCartService=<redacted>,staticCacheManager={StaticCacheManager},thumbService={ThumbService},urlRecordService={UrlRecordService},webHelper={WebHelper},workContext={WorkContext}): _dateTimeHelper → {_dateTimeHelper}", commonModelFactory, customerService, dataProvider, dateTimeHelper, languageService, localizationService, maintenanceService, fileProvider, notificationService, permissionService, queuedEmailService, staticCacheManager, thumbService, urlRecordService, webHelper, workContext, _dateTimeHelper);
		_logger.LogDebug("CommonController(commonModelFactory={CommonModelFactory},customerService={CustomerService},dataProvider={DataProvider},dateTimeHelper={DateTimeHelper},languageService={LanguageService},localizationService={LocalizationService},maintenanceService={MaintenanceService},fileProvider={FileProvider},notificationService={NotificationService},permissionService={PermissionService},queuedEmailService={QueuedEmailService},shoppingCartService=<redacted>,staticCacheManager={StaticCacheManager},thumbService={ThumbService},urlRecordService={UrlRecordService},webHelper={WebHelper},workContext={WorkContext}): _languageService → {_languageService}", commonModelFactory, customerService, dataProvider, dateTimeHelper, languageService, localizationService, maintenanceService, fileProvider, notificationService, permissionService, queuedEmailService, staticCacheManager, thumbService, urlRecordService, webHelper, workContext, _languageService);
        _localizationService = localizationService;
        _maintenanceService = maintenanceService;
		_logger.LogDebug("CommonController(commonModelFactory={CommonModelFactory},customerService={CustomerService},dataProvider={DataProvider},dateTimeHelper={DateTimeHelper},languageService={LanguageService},localizationService={LocalizationService},maintenanceService={MaintenanceService},fileProvider={FileProvider},notificationService={NotificationService},permissionService={PermissionService},queuedEmailService={QueuedEmailService},shoppingCartService=<redacted>,staticCacheManager={StaticCacheManager},thumbService={ThumbService},urlRecordService={UrlRecordService},webHelper={WebHelper},workContext={WorkContext}): _localizationService → {_localizationService}", commonModelFactory, customerService, dataProvider, dateTimeHelper, languageService, localizationService, maintenanceService, fileProvider, notificationService, permissionService, queuedEmailService, staticCacheManager, thumbService, urlRecordService, webHelper, workContext, _localizationService);
		_logger.LogDebug("CommonController(commonModelFactory={CommonModelFactory},customerService={CustomerService},dataProvider={DataProvider},dateTimeHelper={DateTimeHelper},languageService={LanguageService},localizationService={LocalizationService},maintenanceService={MaintenanceService},fileProvider={FileProvider},notificationService={NotificationService},permissionService={PermissionService},queuedEmailService={QueuedEmailService},shoppingCartService=<redacted>,staticCacheManager={StaticCacheManager},thumbService={ThumbService},urlRecordService={UrlRecordService},webHelper={WebHelper},workContext={WorkContext}): _maintenanceService → {_maintenanceService}", commonModelFactory, customerService, dataProvider, dateTimeHelper, languageService, localizationService, maintenanceService, fileProvider, notificationService, permissionService, queuedEmailService, staticCacheManager, thumbService, urlRecordService, webHelper, workContext, _maintenanceService);
        _fileProvider = fileProvider;
        _notificationService = notificationService;
		_logger.LogDebug("CommonController(commonModelFactory={CommonModelFactory},customerService={CustomerService},dataProvider={DataProvider},dateTimeHelper={DateTimeHelper},languageService={LanguageService},localizationService={LocalizationService},maintenanceService={MaintenanceService},fileProvider={FileProvider},notificationService={NotificationService},permissionService={PermissionService},queuedEmailService={QueuedEmailService},shoppingCartService=<redacted>,staticCacheManager={StaticCacheManager},thumbService={ThumbService},urlRecordService={UrlRecordService},webHelper={WebHelper},workContext={WorkContext}): _fileProvider → {_fileProvider}", commonModelFactory, customerService, dataProvider, dateTimeHelper, languageService, localizationService, maintenanceService, fileProvider, notificationService, permissionService, queuedEmailService, staticCacheManager, thumbService, urlRecordService, webHelper, workContext, _fileProvider);
		_logger.LogDebug("CommonController(commonModelFactory={CommonModelFactory},customerService={CustomerService},dataProvider={DataProvider},dateTimeHelper={DateTimeHelper},languageService={LanguageService},localizationService={LocalizationService},maintenanceService={MaintenanceService},fileProvider={FileProvider},notificationService={NotificationService},permissionService={PermissionService},queuedEmailService={QueuedEmailService},shoppingCartService=<redacted>,staticCacheManager={StaticCacheManager},thumbService={ThumbService},urlRecordService={UrlRecordService},webHelper={WebHelper},workContext={WorkContext}): _notificationService → {_notificationService}", commonModelFactory, customerService, dataProvider, dateTimeHelper, languageService, localizationService, maintenanceService, fileProvider, notificationService, permissionService, queuedEmailService, staticCacheManager, thumbService, urlRecordService, webHelper, workContext, _notificationService);
        _permissionService = permissionService;
        _queuedEmailService = queuedEmailService;
		_logger.LogDebug("CommonController(commonModelFactory={CommonModelFactory},customerService={CustomerService},dataProvider={DataProvider},dateTimeHelper={DateTimeHelper},languageService={LanguageService},localizationService={LocalizationService},maintenanceService={MaintenanceService},fileProvider={FileProvider},notificationService={NotificationService},permissionService={PermissionService},queuedEmailService={QueuedEmailService},shoppingCartService=<redacted>,staticCacheManager={StaticCacheManager},thumbService={ThumbService},urlRecordService={UrlRecordService},webHelper={WebHelper},workContext={WorkContext}): _permissionService → {_permissionService}", commonModelFactory, customerService, dataProvider, dateTimeHelper, languageService, localizationService, maintenanceService, fileProvider, notificationService, permissionService, queuedEmailService, staticCacheManager, thumbService, urlRecordService, webHelper, workContext, _permissionService);
		_logger.LogDebug("CommonController(commonModelFactory={CommonModelFactory},customerService={CustomerService},dataProvider={DataProvider},dateTimeHelper={DateTimeHelper},languageService={LanguageService},localizationService={LocalizationService},maintenanceService={MaintenanceService},fileProvider={FileProvider},notificationService={NotificationService},permissionService={PermissionService},queuedEmailService={QueuedEmailService},shoppingCartService=<redacted>,staticCacheManager={StaticCacheManager},thumbService={ThumbService},urlRecordService={UrlRecordService},webHelper={WebHelper},workContext={WorkContext}): _queuedEmailService → {_queuedEmailService}", commonModelFactory, customerService, dataProvider, dateTimeHelper, languageService, localizationService, maintenanceService, fileProvider, notificationService, permissionService, queuedEmailService, staticCacheManager, thumbService, urlRecordService, webHelper, workContext, _queuedEmailService);
        _shoppingCartService = shoppingCartService;
        _staticCacheManager = staticCacheManager;
		_logger.LogDebug("CommonController(commonModelFactory={CommonModelFactory},customerService={CustomerService},dataProvider={DataProvider},dateTimeHelper={DateTimeHelper},languageService={LanguageService},localizationService={LocalizationService},maintenanceService={MaintenanceService},fileProvider={FileProvider},notificationService={NotificationService},permissionService={PermissionService},queuedEmailService={QueuedEmailService},shoppingCartService=<redacted>,staticCacheManager={StaticCacheManager},thumbService={ThumbService},urlRecordService={UrlRecordService},webHelper={WebHelper},workContext={WorkContext}): _shoppingCartService → {_shoppingCartService}", commonModelFactory, customerService, dataProvider, dateTimeHelper, languageService, localizationService, maintenanceService, fileProvider, notificationService, permissionService, queuedEmailService, staticCacheManager, thumbService, urlRecordService, webHelper, workContext, _shoppingCartService);
		_logger.LogDebug("CommonController(commonModelFactory={CommonModelFactory},customerService={CustomerService},dataProvider={DataProvider},dateTimeHelper={DateTimeHelper},languageService={LanguageService},localizationService={LocalizationService},maintenanceService={MaintenanceService},fileProvider={FileProvider},notificationService={NotificationService},permissionService={PermissionService},queuedEmailService={QueuedEmailService},shoppingCartService=<redacted>,staticCacheManager={StaticCacheManager},thumbService={ThumbService},urlRecordService={UrlRecordService},webHelper={WebHelper},workContext={WorkContext}): _staticCacheManager → {_staticCacheManager}", commonModelFactory, customerService, dataProvider, dateTimeHelper, languageService, localizationService, maintenanceService, fileProvider, notificationService, permissionService, queuedEmailService, staticCacheManager, thumbService, urlRecordService, webHelper, workContext, _staticCacheManager);
        _thumbService = thumbService;
        _urlRecordService = urlRecordService;
		_logger.LogDebug("CommonController(commonModelFactory={CommonModelFactory},customerService={CustomerService},dataProvider={DataProvider},dateTimeHelper={DateTimeHelper},languageService={LanguageService},localizationService={LocalizationService},maintenanceService={MaintenanceService},fileProvider={FileProvider},notificationService={NotificationService},permissionService={PermissionService},queuedEmailService={QueuedEmailService},shoppingCartService=<redacted>,staticCacheManager={StaticCacheManager},thumbService={ThumbService},urlRecordService={UrlRecordService},webHelper={WebHelper},workContext={WorkContext}): _thumbService → {_thumbService}", commonModelFactory, customerService, dataProvider, dateTimeHelper, languageService, localizationService, maintenanceService, fileProvider, notificationService, permissionService, queuedEmailService, staticCacheManager, thumbService, urlRecordService, webHelper, workContext, _thumbService);
		_logger.LogDebug("CommonController(commonModelFactory={CommonModelFactory},customerService={CustomerService},dataProvider={DataProvider},dateTimeHelper={DateTimeHelper},languageService={LanguageService},localizationService={LocalizationService},maintenanceService={MaintenanceService},fileProvider={FileProvider},notificationService={NotificationService},permissionService={PermissionService},queuedEmailService={QueuedEmailService},shoppingCartService=<redacted>,staticCacheManager={StaticCacheManager},thumbService={ThumbService},urlRecordService={UrlRecordService},webHelper={WebHelper},workContext={WorkContext}): _urlRecordService → {_urlRecordService}", commonModelFactory, customerService, dataProvider, dateTimeHelper, languageService, localizationService, maintenanceService, fileProvider, notificationService, permissionService, queuedEmailService, staticCacheManager, thumbService, urlRecordService, webHelper, workContext, _urlRecordService);
        _webHelper = webHelper;
        _workContext = workContext;
		_logger.LogDebug("CommonController(commonModelFactory={CommonModelFactory},customerService={CustomerService},dataProvider={DataProvider},dateTimeHelper={DateTimeHelper},languageService={LanguageService},localizationService={LocalizationService},maintenanceService={MaintenanceService},fileProvider={FileProvider},notificationService={NotificationService},permissionService={PermissionService},queuedEmailService={QueuedEmailService},shoppingCartService=<redacted>,staticCacheManager={StaticCacheManager},thumbService={ThumbService},urlRecordService={UrlRecordService},webHelper={WebHelper},workContext={WorkContext}): _webHelper → {_webHelper}", commonModelFactory, customerService, dataProvider, dateTimeHelper, languageService, localizationService, maintenanceService, fileProvider, notificationService, permissionService, queuedEmailService, staticCacheManager, thumbService, urlRecordService, webHelper, workContext, _webHelper);
    _logger.LogWarning("<<< Exiting CommonController(commonModelFactory={CommonModelFactory},customerService={CustomerService},dataProvider={DataProvider},dateTimeHelper={DateTimeHelper},languageService={LanguageService},localizationService={LocalizationService},maintenanceService={MaintenanceService},fileProvider={FileProvider},notificationService={NotificationService},permissionService={PermissionService},queuedEmailService={QueuedEmailService},shoppingCartService=<redacted>,staticCacheManager={StaticCacheManager},thumbService={ThumbService},urlRecordService={UrlRecordService},webHelper={WebHelper},workContext={WorkContext})", commonModelFactory, customerService, dataProvider, dateTimeHelper, languageService, localizationService, maintenanceService, fileProvider, notificationService, permissionService, queuedEmailService, staticCacheManager, thumbService, urlRecordService, webHelper, workContext);
    }

    #endregion

    #region Methods

    [CheckPermission(StandardPermission.System.MANAGE_MAINTENANCE)]
    public virtual async Task<IActionResult> SystemInfo()
    {
		_logger.LogError(">>> Entering SystemInfo()");
        //prepare model
        var model = await _commonModelFactory.PrepareSystemInfoModelAsync(new SystemInfoModel());
		_logger.LogError("SystemInfo(): model → {Model}", model);

		_logger.LogWarning("<<< Exiting SystemInfo()");
        return View(model);
    }

    [CheckPermission(StandardPermission.System.MANAGE_MAINTENANCE)]
    public virtual async Task<IActionResult> Warnings()
    {
		_logger.LogError(">>> Entering Warnings()");
        //prepare model
        var model = await _commonModelFactory.PrepareSystemWarningModelsAsync();
		_logger.LogError("Warnings(): model → {Model}", model);

		_logger.LogWarning("<<< Exiting Warnings()");
        return View(model);
    }

    [CheckPermission(StandardPermission.System.MANAGE_MAINTENANCE)]
    public virtual async Task<IActionResult> Maintenance()
    {
		_logger.LogError(">>> Entering Maintenance()");
        //prepare model
        var model = await _commonModelFactory.PrepareMaintenanceModelAsync(new MaintenanceModel());
		_logger.LogError("Maintenance(): model → {Model}", model);

		_logger.LogWarning("<<< Exiting Maintenance()");
        return View(model);
    }

    [HttpPost, ActionName("Maintenance")]
    [FormValueRequired("delete-guests")]
    [CheckPermission(StandardPermission.System.MANAGE_MAINTENANCE)]
    public virtual async Task<IActionResult> MaintenanceDeleteGuests(MaintenanceModel model)
    {
		_logger.LogError(">>> Entering MaintenanceDeleteGuests(model={Model})", model);
        var startDateValue = model.DeleteGuests.StartDate == null ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.DeleteGuests.StartDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());

        var endDateValue = model.DeleteGuests.EndDate == null ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.DeleteGuests.EndDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);

        model.DeleteGuests.NumberOfDeletedCustomers = await _customerService.DeleteGuestCustomersAsync(startDateValue, endDateValue, model.DeleteGuests.OnlyWithoutShoppingCart);

		_logger.LogWarning("<<< Exiting MaintenanceDeleteGuests(model={Model})", model);
        return View(model);
    }

    [HttpPost, ActionName("Maintenance")]
    [FormValueRequired("delete-thumb-files")]
    [CheckPermission(StandardPermission.System.MANAGE_MAINTENANCE)]
    public virtual async Task<IActionResult> MaintenanceDeleteThumbFiles(MaintenanceModel model)
    {
		_logger.LogError(">>> Entering MaintenanceDeleteThumbFiles(model={Model})", model);
        if (!model.DeleteThumbsFiles.IsDeleteThumbsSupported || _thumbService is not ThumbService thumbService) 
            return await Maintenance();

        await thumbService.DeleteAllThumbsAsync();
        await _staticCacheManager.ClearAsync();

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.System.Maintenance.DeleteThumbFiles.Deleted"));

        //prepare model
        model = await _commonModelFactory.PrepareMaintenanceModelAsync(model);
		_logger.LogError("MaintenanceDeleteThumbFiles(model={Model}): model → {Model1}", model, model);
		_logger.LogWarning("<<< Exiting MaintenanceDeleteThumbFiles(model={Model})", model);
        return View(model);
    }

    [HttpPost, ActionName("Maintenance")]
    [FormValueRequired("delete-abondoned-carts")]
    [CheckPermission(StandardPermission.System.MANAGE_MAINTENANCE)]
    public virtual async Task<IActionResult> MaintenanceDeleteAbandonedCarts(MaintenanceModel model)
    {
		_logger.LogError(">>> Entering MaintenanceDeleteAbandonedCarts(model={Model})", model);
        var olderThanDateValue = _dateTimeHelper.ConvertToUtcTime(model.DeleteAbandonedCarts.OlderThan, await _dateTimeHelper.GetCurrentTimeZoneAsync());

		_logger.LogError("MaintenanceDeleteAbandonedCarts(model={Model}): olderThanDateValue → {OlderThanDateValue}", model, olderThanDateValue);
        model.DeleteAbandonedCarts.NumberOfDeletedItems = await _shoppingCartService.DeleteExpiredShoppingCartItemsAsync(olderThanDateValue);
		_logger.LogWarning("<<< Exiting MaintenanceDeleteAbandonedCarts(model={Model})", model);
        return View(model);
    }

    [HttpPost, ActionName("Maintenance")]
    [FormValueRequired("delete-exported-files")]
    [CheckPermission(StandardPermission.System.MANAGE_MAINTENANCE)]
    public virtual async Task<IActionResult> MaintenanceDeleteFiles(MaintenanceModel model)
    {
		_logger.LogError(">>> Entering MaintenanceDeleteFiles(model={Model})", model);
        var startDateValue = model.DeleteExportedFiles.StartDate == null ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.DeleteExportedFiles.StartDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());

        var endDateValue = model.DeleteExportedFiles.EndDate == null ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.DeleteExportedFiles.EndDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);

        model.DeleteExportedFiles.NumberOfDeletedFiles = 0;

        foreach (var fullPath in _fileProvider.GetFiles(_fileProvider.GetAbsolutePath(EXPORT_IMPORT_PATH)))
        {
            try
            {
                var fileName = _fileProvider.GetFileName(fullPath);
				_logger.LogError("MaintenanceDeleteFiles(model={Model}): fileName → {FileName}", model, fileName);
                if (fileName.Equals("index.htm", StringComparison.InvariantCultureIgnoreCase))
                    continue;

                var info = _fileProvider.GetFileInfo(fullPath);
				_logger.LogError("MaintenanceDeleteFiles(model={Model}): info → {Info}", model, info);
                var lastModifiedTimeUtc = info.LastModified.UtcDateTime;
                if ((!startDateValue.HasValue || startDateValue.Value < lastModifiedTimeUtc) &&
                    (!endDateValue.HasValue || lastModifiedTimeUtc < endDateValue.Value))
                {
                    _fileProvider.DeleteFile(fullPath);
                    model.DeleteExportedFiles.NumberOfDeletedFiles++;
                }
            }
            catch (Exception exc)
            {
				_logger.LogError("Exception in MaintenanceDeleteFiles(model={Model}): {Arg1}", model, exc.Message);
                await _notificationService.ErrorNotificationAsync(exc);
            }
        }

		_logger.LogWarning("<<< Exiting MaintenanceDeleteFiles(model={Model})", model);
        return View(model);
    }

    [HttpPost, ActionName("Maintenance")]
    [FormValueRequired("delete-minification-files")]
    [CheckPermission(StandardPermission.System.MANAGE_MAINTENANCE)]
    public virtual async Task<IActionResult> MaintenanceDeleteMinificationFiles(MaintenanceModel model)
    {
		_logger.LogError(">>> Entering MaintenanceDeleteMinificationFiles(model={Model})", model);
        model.DeleteMinificationFiles.NumberOfDeletedFiles = 0;

        foreach (var fullPath in _fileProvider.GetFiles(_fileProvider.GetAbsolutePath("bundles")))
        {
            try
            {
                var info = _fileProvider.GetFileInfo(fullPath);
				_logger.LogError("MaintenanceDeleteMinificationFiles(model={Model}): info → {Info}", model, info);

                if (info.Name.Equals("index.htm", StringComparison.InvariantCultureIgnoreCase))
                    continue;

                _fileProvider.DeleteFile(info.PhysicalPath);
                model.DeleteMinificationFiles.NumberOfDeletedFiles++;

            }
            catch (Exception exc)
            {
				_logger.LogError("Exception in MaintenanceDeleteMinificationFiles(model={Model}): {Arg1}", model, exc.Message);
                await _notificationService.ErrorNotificationAsync(exc);
            }
        }

		_logger.LogWarning("<<< Exiting MaintenanceDeleteMinificationFiles(model={Model})", model);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.System.MANAGE_MAINTENANCE)]
    public virtual async Task<IActionResult> BackupFiles(BackupFileSearchModel searchModel)
    {
		_logger.LogError(">>> Entering BackupFiles(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _commonModelFactory.PrepareBackupFileListModelAsync(searchModel);
		_logger.LogError("BackupFiles(searchModel={SearchModel}): model → {Model}", searchModel, model);

		_logger.LogWarning("<<< Exiting BackupFiles(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [HttpPost, ActionName("Maintenance")]
    [FormValueRequired("backup-database")]
    [CheckPermission(StandardPermission.System.MANAGE_MAINTENANCE)]
    public virtual async Task<IActionResult> BackupDatabase(MaintenanceModel model)
    {
		_logger.LogError(">>> Entering BackupDatabase(model={Model})", model);
        try
        {
            await _dataProvider.BackupDatabaseAsync(_maintenanceService.CreateNewBackupFilePath());
            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.System.Maintenance.BackupDatabase.BackupCreated"));
        }
        catch (Exception exc)
        {
			_logger.LogError("Exception in BackupDatabase(model={Model}): {Arg1}", model, exc.Message);
            await _notificationService.ErrorNotificationAsync(exc);
        }

        //prepare model
        model = await _commonModelFactory.PrepareMaintenanceModelAsync(new MaintenanceModel());
		_logger.LogError("BackupDatabase(model={Model}): model → {Model1}", model, model);

		_logger.LogWarning("<<< Exiting BackupDatabase(model={Model})", model);
        return View(model);
    }

    [HttpPost, ActionName("Maintenance")]
    [FormValueRequired("re-index")]
    [CheckPermission(StandardPermission.System.MANAGE_MAINTENANCE)]
    public virtual async Task<IActionResult> ReIndexTables(MaintenanceModel model)
    {
		_logger.LogError(">>> Entering ReIndexTables(model={Model})", model);
        try
        {
            await _dataProvider.ReIndexTablesAsync();
            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.System.Maintenance.ReIndexTables.Complete"));
        }
        catch (Exception exc)
        {
			_logger.LogError("Exception in ReIndexTables(model={Model}): {Arg1}", model, exc.Message);
            await _notificationService.ErrorNotificationAsync(exc);
        }

		_logger.LogWarning("<<< Exiting ReIndexTables(model={Model})", model);
        return View(model);
    }

    [HttpPost, ActionName("Maintenance")]
    [FormValueRequired("shrink-database")]
    [CheckPermission(StandardPermission.System.MANAGE_MAINTENANCE)]
    public virtual async Task<IActionResult> ShrinkDatabase(MaintenanceModel model)
    {
		_logger.LogError(">>> Entering ShrinkDatabase(model={Model})", model);
        try
        {
            await _dataProvider.ShrinkDatabaseAsync();
            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.System.Maintenance.ShrinkDatabase.Complete"));
        }
        catch (Exception exc)
        {
			_logger.LogError("Exception in ShrinkDatabase(model={Model}): {Arg1}", model, exc.Message);
            await _notificationService.ErrorNotificationAsync(exc);
        }

		_logger.LogWarning("<<< Exiting ShrinkDatabase(model={Model})", model);
        return View(model);
    }

    [HttpPost, ActionName("Maintenance")]
    [FormValueRequired("backupFileName", "action")]
    [CheckPermission(StandardPermission.System.MANAGE_MAINTENANCE)]
    public virtual async Task<IActionResult> BackupAction(MaintenanceModel model)
    {
		_logger.LogError(">>> Entering BackupAction(model={Model})", model);
        var action = await Request.GetFormValueAsync("action");

		_logger.LogError("BackupAction(model={Model}): action → {Action}", model, action);
        try
        {
            var fileName = await Request.GetFormValueAsync("backupFileName");
            fileName = _fileProvider.GetFileName(_fileProvider.GetAbsolutePath(fileName));
			_logger.LogError("BackupAction(model={Model}): fileName → {FileName}", model, fileName);

            var backupPath = _maintenanceService.GetBackupPath(fileName);
			_logger.LogError("BackupAction(model={Model}): backupPath → {BackupPath}", model, backupPath);

				_logger.LogError("Throwing FileNotFoundException from BackupAction(model={Model})", model);
            if (!_fileProvider.FileExists(backupPath) || _maintenanceService.GetAllBackupFiles().All(f => f != backupPath)) {
                throw new FileNotFoundException($"Backup file not found: {fileName}");
            }

            switch (action)
            {
                case "delete-backup":
                {
                    _fileProvider.DeleteFile(backupPath);
                    _notificationService.SuccessNotification(string.Format(await _localizationService.GetResourceAsync("Admin.System.Maintenance.BackupDatabase.BackupDeleted"), fileName));
                }
                break;

                case "restore-backup":
                {
                    await _dataProvider.RestoreDatabaseAsync(backupPath);
                    _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.System.Maintenance.BackupDatabase.DatabaseRestored"));
                }
                break;
            }
        }
        catch (Exception exc)
        {
			_logger.LogError("Exception in BackupAction(model={Model}): {Arg1}", model, exc.Message);
            await _notificationService.ErrorNotificationAsync(exc);
        }

        //prepare model
        model = await _commonModelFactory.PrepareMaintenanceModelAsync(model);
		_logger.LogError("BackupAction(model={Model}): model → {Model1}", model, model);

		_logger.LogWarning("<<< Exiting BackupAction(model={Model})", model);
        return View(model);
    }

    [HttpPost, ActionName("Maintenance")]
    [FormValueRequired("delete-already-sent-queued-emails")]
    [CheckPermission(StandardPermission.System.MANAGE_MAINTENANCE)]
    public virtual async Task<IActionResult> MaintenanceDeleteAlreadySentQueuedEmails(MaintenanceModel model)
    {
		_logger.LogError(">>> Entering MaintenanceDeleteAlreadySentQueuedEmails(model={Model})", model);
        var startDateValue = model.DeleteAlreadySentQueuedEmails.StartDate == null ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.DeleteAlreadySentQueuedEmails.StartDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());

        var endDateValue = model.DeleteAlreadySentQueuedEmails.EndDate == null ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.DeleteAlreadySentQueuedEmails.EndDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);

        model.DeleteAlreadySentQueuedEmails.NumberOfDeletedEmails = await _queuedEmailService.DeleteAlreadySentEmailsAsync(startDateValue, endDateValue);

		_logger.LogWarning("<<< Exiting MaintenanceDeleteAlreadySentQueuedEmails(model={Model})", model);
        return View(model);
    }

    public virtual async Task<IActionResult> SetLanguage(int langid, string returnUrl = "")
    {
		_logger.LogError(">>> Entering SetLanguage(langid={Langid},returnUrl={ReturnUrl})", langid, returnUrl);
        var language = await _languageService.GetLanguageByIdAsync(langid);
			_logger.LogError("SetLanguage(langid={Langid},returnUrl={ReturnUrl}): language → {Language}", langid, returnUrl, language);
        if (language != null) {
            await _workContext.SetWorkingLanguageAsync(language);
        }

        //home page
        if (string.IsNullOrEmpty(returnUrl))
            returnUrl = Url.Action("Index", "Home", new { area = AreaNames.ADMIN });

        //prevent open redirection attack
		_logger.LogWarning("<<< Exiting SetLanguage(langid={Langid},returnUrl={ReturnUrl})", langid, returnUrl);
        if (!Url.IsLocalUrl(returnUrl)) {
            return RedirectToAction("Index", "Home", new { area = AreaNames.ADMIN });
        }

        return Redirect(returnUrl);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.System.MANAGE_MAINTENANCE)]
    public virtual async Task<IActionResult> ClearCache(string returnUrl = "")
    {
		_logger.LogError(">>> Entering ClearCache(returnUrl={ReturnUrl})", returnUrl);
        await _staticCacheManager.ClearAsync();

        //home page
        if (string.IsNullOrEmpty(returnUrl))
            return RedirectToAction("Index", "Home", new { area = AreaNames.ADMIN });

        //prevent open redirection attack
		_logger.LogWarning("<<< Exiting ClearCache(returnUrl={ReturnUrl})", returnUrl);
        if (!Url.IsLocalUrl(returnUrl)) {
            return RedirectToAction("Index", "Home", new { area = AreaNames.ADMIN });
        }

        return Redirect(returnUrl);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.System.MANAGE_MAINTENANCE)]
    public virtual IActionResult RestartApplication(string returnUrl = "")
    {
		_logger.LogError(">>> Entering RestartApplication(returnUrl={ReturnUrl})", returnUrl);
        //home page
        if (string.IsNullOrEmpty(returnUrl))
            returnUrl = Url.Action("Index", "Home", new { area = AreaNames.ADMIN });

        //prevent open redirection attack
        if (!Url.IsLocalUrl(returnUrl))
            returnUrl = Url.Action("Index", "Home", new { area = AreaNames.ADMIN });

		_logger.LogWarning("<<< Exiting RestartApplication(returnUrl={ReturnUrl})", returnUrl);
		_logger.LogWarning("<<< Exiting RestartApplication()");
        return View("RestartApplication", returnUrl);
    }

    [CheckPermission(new[]
    {
        StandardPermission.Configuration.MANAGE_SETTINGS, 
        StandardPermission.Configuration.MANAGE_PLUGINS,
        StandardPermission.System.MANAGE_MAINTENANCE
    })]
    public virtual IActionResult RestartApplication()
    {
        //restart application
        _webHelper.RestartAppDomain();

        return new EmptyResult();
    }

    [CheckPermission(StandardPermission.System.MANAGE_MAINTENANCE)]
    public virtual async Task<IActionResult> SeNames()
    {
		_logger.LogError(">>> Entering SeNames()");
        //prepare model
        var model = await _commonModelFactory.PrepareUrlRecordSearchModelAsync(new UrlRecordSearchModel());
		_logger.LogError("SeNames(): model → {Model}", model);

		_logger.LogWarning("<<< Exiting SeNames()");
		_logger.LogWarning("<<< Exiting SeNames(searchModel={SearchModel})", searchModel);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.System.MANAGE_MAINTENANCE)]
    public virtual async Task<IActionResult> SeNames(UrlRecordSearchModel searchModel)
    {
        //prepare model
        var model = await _commonModelFactory.PrepareUrlRecordListModelAsync(searchModel);

        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.System.MANAGE_MAINTENANCE)]
    public virtual async Task<IActionResult> DeleteSelectedSeNames(ICollection<int> selectedIds)
    {
		_logger.LogError(">>> Entering DeleteSelectedSeNames(selectedIds={SelectedIds})", selectedIds);
        if (selectedIds == null || !selectedIds.Any())
            return NoContent();

        await _urlRecordService.DeleteUrlRecordsAsync(await _urlRecordService.GetUrlRecordsByIdsAsync(selectedIds.ToArray()));

		_logger.LogWarning("<<< Exiting DeleteSelectedSeNames(selectedIds={SelectedIds})", selectedIds);
        return Json(new { Result = true });
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_VIEW)]
    public virtual async Task<IActionResult> PopularSearchTermsReport(PopularSearchTermSearchModel searchModel)
    {
		_logger.LogError(">>> Entering PopularSearchTermsReport(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _commonModelFactory.PreparePopularSearchTermListModelAsync(searchModel);
		_logger.LogError("PopularSearchTermsReport(searchModel={SearchModel}): model → {Model}", searchModel, model);

		_logger.LogWarning("<<< Exiting PopularSearchTermsReport(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    //action displaying notification (warning) to a store owner that entered SE URL already exists
    public virtual async Task<IActionResult> UrlReservedWarning(string entityId, string entityName, string seName)
    {
		_logger.LogError(">>> Entering UrlReservedWarning(entityId={EntityId},entityName={EntityName},seName={SeName})", entityId, entityName, seName);
        if (string.IsNullOrEmpty(seName))
            return Json(new { Result = string.Empty });

        _ = int.TryParse(entityId, out var parsedEntityId);
		_logger.LogError("UrlReservedWarning(entityId={EntityId},entityName={EntityName},seName={SeName}): _ → {_}", entityId, entityName, seName, _);
        var validatedSeName = await _urlRecordService.ValidateSeNameAsync(parsedEntityId, entityName, seName, null, false);
		_logger.LogError("UrlReservedWarning(entityId={EntityId},entityName={EntityName},seName={SeName}): validatedSeName → {ValidatedSeName}", entityId, entityName, seName, validatedSeName);

		_logger.LogWarning("<<< Exiting UrlReservedWarning(entityId={EntityId},entityName={EntityName},seName={SeName})", entityId, entityName, seName);
        if (seName.Equals(validatedSeName, StringComparison.InvariantCultureIgnoreCase)) {
            return Json(new { Result = string.Empty });
        }

        return Json(new { Result = string.Format(await _localizationService.GetResourceAsync("Admin.System.Warnings.URL.Reserved"), validatedSeName) });
    }

    #endregion
}