using Microsoft.AspNetCore.Mvc;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Logging;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using ILogger = Nop.Services.Logging.ILogger;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class LogController : BaseAdminController
{
    #region Fields

    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly ILocalizationService _localizationService;
    protected readonly ILogger _logger;
    protected readonly ILogModelFactory _logModelFactory;
    protected readonly INotificationService _notificationService;
    protected readonly IPermissionService _permissionService;

    #endregion

    #region Ctor

    public LogController(ICustomerActivityService customerActivityService,
        ILocalizationService localizationService,
        ILogger logger,
        ILogModelFactory logModelFactory,
        INotificationService notificationService,
        IPermissionService permissionService)
    {
        _customerActivityService = customerActivityService;
		_logger.LogDebug("LogController(customerActivityService={CustomerActivityService},localizationService={LocalizationService},logger={Logger},logModelFactory={LogModelFactory},notificationService={NotificationService},permissionService={PermissionService}): _customerActivityService → {_customerActivityService}", customerActivityService, localizationService, logger, logModelFactory, notificationService, permissionService, _customerActivityService);
        _localizationService = localizationService;
		_logger.LogDebug("LogController(customerActivityService={CustomerActivityService},localizationService={LocalizationService},logger={Logger},logModelFactory={LogModelFactory},notificationService={NotificationService},permissionService={PermissionService}): _localizationService → {_localizationService}", customerActivityService, localizationService, logger, logModelFactory, notificationService, permissionService, _localizationService);
        _logger = logger;
		_logger.LogDebug(">>> Entering LogController(customerActivityService={CustomerActivityService},localizationService={LocalizationService},logger={Logger},logModelFactory={LogModelFactory},notificationService={NotificationService},permissionService={PermissionService})", customerActivityService, localizationService, logger, logModelFactory, notificationService, permissionService);
        _logModelFactory = logModelFactory;
        _notificationService = notificationService;
		_logger.LogDebug("LogController(customerActivityService={CustomerActivityService},localizationService={LocalizationService},logger={Logger},logModelFactory={LogModelFactory},notificationService={NotificationService},permissionService={PermissionService}): _logModelFactory → {_logModelFactory}", customerActivityService, localizationService, logger, logModelFactory, notificationService, permissionService, _logModelFactory);
		_logger.LogDebug("LogController(customerActivityService={CustomerActivityService},localizationService={LocalizationService},logger={Logger},logModelFactory={LogModelFactory},notificationService={NotificationService},permissionService={PermissionService}): _notificationService → {_notificationService}", customerActivityService, localizationService, logger, logModelFactory, notificationService, permissionService, _notificationService);
        _permissionService = permissionService;
		_logger.LogDebug("<<< Exiting LogController(customerActivityService={CustomerActivityService},localizationService={LocalizationService},logger={Logger},logModelFactory={LogModelFactory},notificationService={NotificationService},permissionService={PermissionService})", customerActivityService, localizationService, logger, logModelFactory, notificationService, permissionService);
		_logger.LogDebug("LogController(customerActivityService={CustomerActivityService},localizationService={LocalizationService},logger={Logger},logModelFactory={LogModelFactory},notificationService={NotificationService},permissionService={PermissionService}): _permissionService → {_permissionService}", customerActivityService, localizationService, logger, logModelFactory, notificationService, permissionService, _permissionService);
    }

    #endregion

    #region Methods

    public virtual IActionResult Index()
    {
		_logger.LogDebug(">>> Entering Index()");
		_logger.LogDebug("<<< Exiting Index()");
        return RedirectToAction("List");
    }

    [CheckPermission(StandardPermission.System.MANAGE_SYSTEM_LOG)]
    public virtual async Task<IActionResult> List()
    {
		_logger.LogDebug(">>> Entering List()");
        //prepare model
        var model = await _logModelFactory.PrepareLogSearchModelAsync(new LogSearchModel());
		_logger.LogDebug("List(): model → {Model}", model);

		_logger.LogDebug("<<< Exiting List()");
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.System.MANAGE_SYSTEM_LOG)]
    public virtual async Task<IActionResult> LogList(LogSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering LogList(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _logModelFactory.PrepareLogListModelAsync(searchModel);
		_logger.LogDebug("LogList(searchModel={SearchModel}): model → {Model}", searchModel, model);

		_logger.LogDebug("<<< Exiting LogList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [HttpPost, ActionName("List")]
    [FormValueRequired("clearall")]
    [CheckPermission(StandardPermission.System.MANAGE_SYSTEM_LOG)]
    public virtual async Task<IActionResult> ClearAll()
    {
		_logger.LogDebug(">>> Entering ClearAll()");
        await _logger.ClearLogAsync();

        //activity log
        await _customerActivityService.InsertActivityAsync("DeleteSystemLog", await _localizationService.GetResourceAsync("ActivityLog.DeleteSystemLog"));

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.System.Log.Cleared"));

		_logger.LogDebug("<<< Exiting ClearAll()");
        return RedirectToAction("List");
    }

    [CheckPermission(StandardPermission.System.MANAGE_SYSTEM_LOG)]
    public virtual async Task<IActionResult> View(int id)
    {
		_logger.LogDebug(">>> Entering View(id={Id})", id);
        //try to get a log with the specified id
        var log = await _logger.GetLogByIdAsync(id);
		_logger.LogDebug("View(id={Id}): log → {Log}", id, log);
        if (log == null)
            return RedirectToAction("List");

        //prepare model
        var model = await _logModelFactory.PrepareLogModelAsync(null, log);
		_logger.LogDebug("View(id={Id}): model → {Model}", id, model);

		_logger.LogDebug("<<< Exiting View(id={Id})", id);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.System.MANAGE_SYSTEM_LOG)]
    public virtual async Task<IActionResult> Delete(int id)
    {
		_logger.LogDebug(">>> Entering Delete(id={Id})", id);
        //try to get a log with the specified id
        var log = await _logger.GetLogByIdAsync(id);
		_logger.LogDebug("Delete(id={Id}): log → {Log}", id, log);
        if (log == null)
            return RedirectToAction("List");

        await _logger.DeleteLogAsync(log);

        //activity log
        await _customerActivityService.InsertActivityAsync("DeleteSystemLog", await _localizationService.GetResourceAsync("ActivityLog.DeleteSystemLog"), log);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.System.Log.Deleted"));

		_logger.LogDebug("<<< Exiting Delete(id={Id})", id);
        return RedirectToAction("List");
    }

    [HttpPost]
    [CheckPermission(StandardPermission.System.MANAGE_SYSTEM_LOG)]
    public virtual async Task<IActionResult> DeleteSelected(ICollection<int> selectedIds)
    {
		_logger.LogDebug(">>> Entering DeleteSelected(selectedIds={SelectedIds})", selectedIds);
        if (selectedIds == null || !selectedIds.Any())
            return NoContent();

        await _logger.DeleteLogsAsync((await _logger.GetLogByIdsAsync([.. selectedIds])).ToList());

        //activity log
        await _customerActivityService.InsertActivityAsync("DeleteSystemLog", await _localizationService.GetResourceAsync("ActivityLog.DeleteSystemLog"));

		_logger.LogDebug("<<< Exiting DeleteSelected(selectedIds={SelectedIds})", selectedIds);
        return Json(new { Result = true });
    }

    #endregion
}