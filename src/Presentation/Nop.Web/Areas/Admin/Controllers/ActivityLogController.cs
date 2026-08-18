using Microsoft.AspNetCore.Mvc;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Logging;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class ActivityLogController : BaseAdminController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IActivityLogModelFactory _activityLogModelFactory;
    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly ILocalizationService _localizationService;
    protected readonly IPermissionService _permissionService;
    protected readonly INotificationService _notificationService;
    private static readonly char[] _separator = [','];

    #endregion

    #region Ctor

    public ActivityLogController(IActivityLogModelFactory activityLogModelFactory,
        ICustomerActivityService customerActivityService,
        ILocalizationService localizationService,
        INotificationService notificationService,
        IPermissionService permissionService)
    {
		_logger.LogDebug(">>> Entering ActivityLogController(activityLogModelFactory={ActivityLogModelFactory},customerActivityService={CustomerActivityService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService})", activityLogModelFactory, customerActivityService, localizationService, notificationService, permissionService);
        _activityLogModelFactory = activityLogModelFactory;
		_logger.LogDebug("ActivityLogController(activityLogModelFactory={ActivityLogModelFactory},customerActivityService={CustomerActivityService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService}): _activityLogModelFactory → {_activityLogModelFactory}", activityLogModelFactory, customerActivityService, localizationService, notificationService, permissionService, _activityLogModelFactory);
        _customerActivityService = customerActivityService;
		_logger.LogDebug("ActivityLogController(activityLogModelFactory={ActivityLogModelFactory},customerActivityService={CustomerActivityService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService}): _customerActivityService → {_customerActivityService}", activityLogModelFactory, customerActivityService, localizationService, notificationService, permissionService, _customerActivityService);
        _localizationService = localizationService;
		_logger.LogDebug("ActivityLogController(activityLogModelFactory={ActivityLogModelFactory},customerActivityService={CustomerActivityService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService}): _localizationService → {_localizationService}", activityLogModelFactory, customerActivityService, localizationService, notificationService, permissionService, _localizationService);
        _notificationService = notificationService;
		_logger.LogDebug("ActivityLogController(activityLogModelFactory={ActivityLogModelFactory},customerActivityService={CustomerActivityService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService}): _notificationService → {_notificationService}", activityLogModelFactory, customerActivityService, localizationService, notificationService, permissionService, _notificationService);
        _permissionService = permissionService;
		_logger.LogDebug("ActivityLogController(activityLogModelFactory={ActivityLogModelFactory},customerActivityService={CustomerActivityService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService}): _permissionService → {_permissionService}", activityLogModelFactory, customerActivityService, localizationService, notificationService, permissionService, _permissionService);
		_logger.LogDebug("<<< Exiting ActivityLogController(activityLogModelFactory={ActivityLogModelFactory},customerActivityService={CustomerActivityService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService})", activityLogModelFactory, customerActivityService, localizationService, notificationService, permissionService);
    }

    #endregion

    #region Methods

    [CheckPermission(StandardPermission.Customers.ACTIVITY_LOG_VIEW)]
    public virtual async Task<IActionResult> ActivityTypes()
    {
		_logger.LogDebug(">>> Entering ActivityTypes()");
        //prepare model
        var model = await _activityLogModelFactory.PrepareActivityLogTypeSearchModelAsync(new ActivityLogTypeSearchModel());
		_logger.LogDebug("ActivityTypes(): model → {Model}", model);

		_logger.LogDebug("<<< Exiting ActivityTypes()");
        return View(model);
    }

    [HttpPost, ActionName("SaveTypes")]
    [CheckPermission(StandardPermission.Customers.ACTIVITY_LOG_MANAGE_TYPES)]
    public virtual async Task<IActionResult> SaveTypes(IFormCollection form)
    {
		_logger.LogDebug(">>> Entering SaveTypes(form={Form})", form);
        //activity log
        await _customerActivityService.InsertActivityAsync("EditActivityLogTypes", await _localizationService.GetResourceAsync("ActivityLog.EditActivityLogTypes"));

        //get identifiers of selected activity types
        var selectedActivityTypesIds = form["checkbox_activity_types"]
            .SelectMany(value => value.Split(_separator, StringSplitOptions.RemoveEmptyEntries))
            .Select(idString => int.TryParse(idString, out var id) ? id : 0)
            .Distinct().ToList();

        //update activity types
        var activityTypes = await _customerActivityService.GetAllActivityTypesAsync();
		_logger.LogDebug("SaveTypes(form={Form}): activityTypes → {ActivityTypes}", form, activityTypes);
        foreach (var activityType in activityTypes)
        {
            activityType.Enabled = selectedActivityTypesIds.Contains(activityType.Id);
            await _customerActivityService.UpdateActivityTypeAsync(activityType);
        }

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Customers.ActivityLogType.Updated"));

		_logger.LogDebug("<<< Exiting SaveTypes(form={Form})", form);
        return RedirectToAction("ActivityTypes");
    }

    [CheckPermission(StandardPermission.Customers.ACTIVITY_LOG_VIEW)]
    public virtual async Task<IActionResult> ActivityLogs()
    {
		_logger.LogDebug(">>> Entering ActivityLogs()");
        //prepare model
        var model = await _activityLogModelFactory.PrepareActivityLogSearchModelAsync(new ActivityLogSearchModel());
		_logger.LogDebug("ActivityLogs(): model → {Model}", model);

		_logger.LogDebug("<<< Exiting ActivityLogs()");
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Customers.ACTIVITY_LOG_VIEW)]
    public virtual async Task<IActionResult> ListLogs(ActivityLogSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering ListLogs(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _activityLogModelFactory.PrepareActivityLogListModelAsync(searchModel);
		_logger.LogDebug("ListLogs(searchModel={SearchModel}): model → {Model}", searchModel, model);

		_logger.LogDebug("<<< Exiting ListLogs(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Customers.ACTIVITY_LOG_DELETE)]
    public virtual async Task<IActionResult> ActivityLogDelete(int id)
    {
		_logger.LogDebug(">>> Entering ActivityLogDelete(id={Id})", id);
        //try to get a log item with the specified id
        var logItem = await _customerActivityService.GetActivityByIdAsync(id)
            ?? throw new ArgumentException("No activity log found with the specified id", nameof(id));

        await _customerActivityService.DeleteActivityAsync(logItem);

        //activity log
        await _customerActivityService.InsertActivityAsync("DeleteActivityLog",
            await _localizationService.GetResourceAsync("ActivityLog.DeleteActivityLog"), logItem);

		_logger.LogDebug("<<< Exiting ActivityLogDelete(id={Id})", id);
        return new NullJsonResult();
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Customers.ACTIVITY_LOG_DELETE)]
    public virtual async Task<IActionResult> ClearAll()
    {
		_logger.LogDebug(">>> Entering ClearAll()");
        await _customerActivityService.ClearAllActivitiesAsync();

        //activity log
        await _customerActivityService.InsertActivityAsync("DeleteActivityLog", await _localizationService.GetResourceAsync("ActivityLog.DeleteActivityLog"));

		_logger.LogDebug("<<< Exiting ClearAll()");
        return RedirectToAction("ActivityLogs");
    }

    #endregion
}