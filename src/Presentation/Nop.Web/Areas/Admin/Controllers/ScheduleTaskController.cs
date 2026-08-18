using Microsoft.AspNetCore.Mvc;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.ScheduleTasks;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Tasks;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class ScheduleTaskController : BaseAdminController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly ILocalizationService _localizationService;
    protected readonly INotificationService _notificationService;
    protected readonly IPermissionService _permissionService;
    protected readonly IScheduleTaskModelFactory _scheduleTaskModelFactory;
    protected readonly IScheduleTaskService _scheduleTaskService;
    protected readonly IScheduleTaskRunner _taskRunner;

    #endregion

    #region Ctor

    public ScheduleTaskController(ICustomerActivityService customerActivityService,
        ILocalizationService localizationService,
        INotificationService notificationService,
        IPermissionService permissionService,
        IScheduleTaskModelFactory scheduleTaskModelFactory,
        IScheduleTaskService scheduleTaskService,
        IScheduleTaskRunner taskRunner)
    {
		_logger.LogDebug(">>> Entering ScheduleTaskController(customerActivityService={CustomerActivityService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},scheduleTaskModelFactory={ScheduleTaskModelFactory},scheduleTaskService={ScheduleTaskService},taskRunner={TaskRunner})", customerActivityService, localizationService, notificationService, permissionService, scheduleTaskModelFactory, scheduleTaskService, taskRunner);
        _customerActivityService = customerActivityService;
		_logger.LogDebug("ScheduleTaskController(customerActivityService={CustomerActivityService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},scheduleTaskModelFactory={ScheduleTaskModelFactory},scheduleTaskService={ScheduleTaskService},taskRunner={TaskRunner}): _customerActivityService → {_customerActivityService}", customerActivityService, localizationService, notificationService, permissionService, scheduleTaskModelFactory, scheduleTaskService, taskRunner, _customerActivityService);
        _localizationService = localizationService;
		_logger.LogDebug("ScheduleTaskController(customerActivityService={CustomerActivityService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},scheduleTaskModelFactory={ScheduleTaskModelFactory},scheduleTaskService={ScheduleTaskService},taskRunner={TaskRunner}): _localizationService → {_localizationService}", customerActivityService, localizationService, notificationService, permissionService, scheduleTaskModelFactory, scheduleTaskService, taskRunner, _localizationService);
        _notificationService = notificationService;
		_logger.LogDebug("ScheduleTaskController(customerActivityService={CustomerActivityService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},scheduleTaskModelFactory={ScheduleTaskModelFactory},scheduleTaskService={ScheduleTaskService},taskRunner={TaskRunner}): _notificationService → {_notificationService}", customerActivityService, localizationService, notificationService, permissionService, scheduleTaskModelFactory, scheduleTaskService, taskRunner, _notificationService);
        _permissionService = permissionService;
		_logger.LogDebug("ScheduleTaskController(customerActivityService={CustomerActivityService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},scheduleTaskModelFactory={ScheduleTaskModelFactory},scheduleTaskService={ScheduleTaskService},taskRunner={TaskRunner}): _permissionService → {_permissionService}", customerActivityService, localizationService, notificationService, permissionService, scheduleTaskModelFactory, scheduleTaskService, taskRunner, _permissionService);
        _scheduleTaskModelFactory = scheduleTaskModelFactory;
		_logger.LogDebug("ScheduleTaskController(customerActivityService={CustomerActivityService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},scheduleTaskModelFactory={ScheduleTaskModelFactory},scheduleTaskService={ScheduleTaskService},taskRunner={TaskRunner}): _scheduleTaskModelFactory → {_scheduleTaskModelFactory}", customerActivityService, localizationService, notificationService, permissionService, scheduleTaskModelFactory, scheduleTaskService, taskRunner, _scheduleTaskModelFactory);
        _scheduleTaskService = scheduleTaskService;
		_logger.LogDebug("ScheduleTaskController(customerActivityService={CustomerActivityService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},scheduleTaskModelFactory={ScheduleTaskModelFactory},scheduleTaskService={ScheduleTaskService},taskRunner={TaskRunner}): _scheduleTaskService → {_scheduleTaskService}", customerActivityService, localizationService, notificationService, permissionService, scheduleTaskModelFactory, scheduleTaskService, taskRunner, _scheduleTaskService);
        _taskRunner = taskRunner;
		_logger.LogDebug("<<< Exiting ScheduleTaskController(customerActivityService={CustomerActivityService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},scheduleTaskModelFactory={ScheduleTaskModelFactory},scheduleTaskService={ScheduleTaskService},taskRunner={TaskRunner})", customerActivityService, localizationService, notificationService, permissionService, scheduleTaskModelFactory, scheduleTaskService, taskRunner);
		_logger.LogDebug("ScheduleTaskController(customerActivityService={CustomerActivityService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},scheduleTaskModelFactory={ScheduleTaskModelFactory},scheduleTaskService={ScheduleTaskService},taskRunner={TaskRunner}): _taskRunner → {_taskRunner}", customerActivityService, localizationService, notificationService, permissionService, scheduleTaskModelFactory, scheduleTaskService, taskRunner, _taskRunner);
    }

    #endregion

    #region Methods

    public virtual IActionResult Index()
    {
		_logger.LogDebug(">>> Entering Index()");
		_logger.LogDebug("<<< Exiting Index()");
        return RedirectToAction("List");
    }

    [CheckPermission(StandardPermission.System.MANAGE_SCHEDULE_TASKS)]
    public virtual async Task<IActionResult> List()
    {
		_logger.LogDebug(">>> Entering List()");
        //prepare model
        var model = await _scheduleTaskModelFactory.PrepareScheduleTaskSearchModelAsync(new ScheduleTaskSearchModel());
		_logger.LogDebug("List(): model → {Model}", model);

		_logger.LogDebug("<<< Exiting List()");
		_logger.LogDebug("<<< Exiting List(searchModel={SearchModel})", searchModel);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.System.MANAGE_SCHEDULE_TASKS)]
    public virtual async Task<IActionResult> List(ScheduleTaskSearchModel searchModel)
    {
        //prepare model
        var model = await _scheduleTaskModelFactory.PrepareScheduleTaskListModelAsync(searchModel);

        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.System.MANAGE_SCHEDULE_TASKS)]
    public virtual async Task<IActionResult> TaskUpdate(ScheduleTaskModel model)
    {
		_logger.LogDebug(">>> Entering TaskUpdate(model={Model})", model);
        //try to get a schedule task with the specified id
        var scheduleTask = await _scheduleTaskService.GetTaskByIdAsync(model.Id)
            ?? throw new ArgumentException("Schedule task cannot be loaded");
			_logger.LogDebug("TaskUpdate(model={Model}): scheduleTask → {ScheduleTask}", model, scheduleTask);

        //To prevent inject the XSS payload in Schedule tasks ('Name' field), we must disable editing this field, 
        //but since it is required, we need to get its value before updating the entity.
        if (!string.IsNullOrEmpty(scheduleTask.Name))
        {
            model.Name = scheduleTask.Name;
			_logger.LogDebug("TaskUpdate(model={Model}): model.Name → {Arg1}", model, model.Name);
            ModelState.Remove(nameof(model.Name));
        }

        if (!ModelState.IsValid)
            return ErrorJson(ModelState.SerializeErrors());

        if (!scheduleTask.Enabled && model.Enabled)
            scheduleTask.LastEnabledUtc = DateTime.UtcNow;
			_logger.LogDebug("TaskUpdate(model={Model}): scheduleTask.LastEnabledUtc → {Arg1}", model, scheduleTask.LastEnabledUtc);

        scheduleTask = model.ToEntity(scheduleTask);

        await _scheduleTaskService.UpdateTaskAsync(scheduleTask);

        //activity log
        await _customerActivityService.InsertActivityAsync("EditTask",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditTask"), scheduleTask.Id), scheduleTask);

		_logger.LogDebug("<<< Exiting TaskUpdate(model={Model})", model);
        return new NullJsonResult();
    }

    [CheckPermission(StandardPermission.System.MANAGE_SCHEDULE_TASKS)]
    public virtual async Task<IActionResult> RunNow(int id)
    {
		_logger.LogDebug(">>> Entering RunNow(id={Id})", id);
        try
        {
            //try to get a schedule task with the specified id
            var scheduleTask = await _scheduleTaskService.GetTaskByIdAsync(id)
                ?? throw new ArgumentException("Schedule task cannot be loaded", nameof(id));

            await _taskRunner.ExecuteAsync(scheduleTask, true, true, false);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.System.ScheduleTasks.RunNow.Done"));
        }
        catch (Exception exc)
        {
			_logger.LogError("Exception in RunNow(id={Id}): {Arg1}", id, exc.getMessage());
            await _notificationService.ErrorNotificationAsync(exc);
        }

		_logger.LogDebug("<<< Exiting RunNow(id={Id})", id);
        return RedirectToAction("List");
    }

    #endregion
}