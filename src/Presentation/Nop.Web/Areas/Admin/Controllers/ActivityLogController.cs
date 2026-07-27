using Microsoft.AspNetCore.Mvc;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Logging;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class ActivityLogController : BaseAdminController
{
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
        _activityLogModelFactory = activityLogModelFactory;
        _customerActivityService = customerActivityService;
        _localizationService = localizationService;
        _notificationService = notificationService;
        _permissionService = permissionService;
    }

    #endregion

    #region Methods

    [CheckPermission(StandardPermission.Customers.ACTIVITY_LOG_VIEW)]
    public virtual async Task<IActionResult> ActivityTypes()
    {
		logger.LogDebug(">>> Entering ActivityTypes()");
        //prepare model
        var model = await _activityLogModelFactory.PrepareActivityLogTypeSearchModelAsync(new ActivityLogTypeSearchModel());

		logger.LogDebug("ActivityTypes(): model → {Model}", model);
		logger.LogDebug("<<< Exiting ActivityTypes()");
        return View(model);
    }

    [HttpPost, ActionName("SaveTypes")]
    [CheckPermission(StandardPermission.Customers.ACTIVITY_LOG_MANAGE_TYPES)]
    public virtual async Task<IActionResult> SaveTypes(IFormCollection form)
    {
		logger.LogDebug(">>> Entering SaveTypes(form={Form})", form);
        //activity log
        await _customerActivityService.InsertActivityAsync("EditActivityLogTypes", await _localizationService.GetResourceAsync("ActivityLog.EditActivityLogTypes"));

        //get identifiers of selected activity types
        var selectedActivityTypesIds = form["checkbox_activity_types"]
            .SelectMany(value => value.Split(_separator, StringSplitOptions.RemoveEmptyEntries))
            .Select(idString => int.TryParse(idString, out var id) ? id : 0)
            .Distinct().ToList();

        //update activity types
        var activityTypes = await _customerActivityService.GetAllActivityTypesAsync();
        foreach (var activityType in activityTypes)
        {
		logger.LogDebug("SaveTypes(form={Form}): activityTypes → {ActivityTypes}", form, activityTypes);
            activityType.Enabled = selectedActivityTypesIds.Contains(activityType.Id);
            await _customerActivityService.UpdateActivityTypeAsync(activityType);
        }

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Customers.ActivityLogType.Updated"));

		logger.LogDebug("<<< Exiting SaveTypes(form={Form})", form);
        return RedirectToAction("ActivityTypes");
    }

    [CheckPermission(StandardPermission.Customers.ACTIVITY_LOG_VIEW)]
    public virtual async Task<IActionResult> ActivityLogs()
    {
		logger.LogDebug(">>> Entering ActivityLogs()");
        //prepare model
        var model = await _activityLogModelFactory.PrepareActivityLogSearchModelAsync(new ActivityLogSearchModel());

		logger.LogDebug("ActivityLogs(): model → {Model}", model);
		logger.LogDebug("<<< Exiting ActivityLogs()");
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Customers.ACTIVITY_LOG_VIEW)]
    public virtual async Task<IActionResult> ListLogs(ActivityLogSearchModel searchModel)
    {
		logger.LogDebug(">>> Entering ListLogs(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _activityLogModelFactory.PrepareActivityLogListModelAsync(searchModel);

		logger.LogDebug("ListLogs(searchModel={SearchModel}): model → {Model}", searchModel, model);
		logger.LogDebug("<<< Exiting ListLogs(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Customers.ACTIVITY_LOG_DELETE)]
    public virtual async Task<IActionResult> ActivityLogDelete(int id)
    {
		logger.LogDebug(">>> Entering ActivityLogDelete(id={Id})", id);
        //try to get a log item with the specified id
        var logItem = await _customerActivityService.GetActivityByIdAsync(id)
            ?? throw new ArgumentException("No activity log found with the specified id", nameof(id));

        await _customerActivityService.DeleteActivityAsync(logItem);

        //activity log
        await _customerActivityService.InsertActivityAsync("DeleteActivityLog",
            await _localizationService.GetResourceAsync("ActivityLog.DeleteActivityLog"), logItem);

		logger.LogDebug("<<< Exiting ActivityLogDelete(id={Id})", id);
        return new NullJsonResult();
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Customers.ACTIVITY_LOG_DELETE)]
    public virtual async Task<IActionResult> ClearAll()
    {
		logger.LogDebug(">>> Entering ClearAll()");
        await _customerActivityService.ClearAllActivitiesAsync();

        //activity log
        await _customerActivityService.InsertActivityAsync("DeleteActivityLog", await _localizationService.GetResourceAsync("ActivityLog.DeleteActivityLog"));

		logger.LogDebug("<<< Exiting ClearAll()");
        return RedirectToAction("ActivityLogs");
    }

    #endregion
}