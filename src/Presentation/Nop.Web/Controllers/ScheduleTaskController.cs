using Microsoft.AspNetCore.Mvc;
using Nop.Services.ScheduleTasks;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Controllers;

//do not inherit it from BasePublicController. otherwise a lot of extra action filters will be called
//they can create guest account(s), etc
[AutoValidateAntiforgeryToken]
public partial class ScheduleTaskController : Controller
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    protected readonly IScheduleTaskService _scheduleTaskService;
    protected readonly IScheduleTaskRunner _taskRunner;

    public ScheduleTaskController(IScheduleTaskService scheduleTaskService,
        IScheduleTaskRunner taskRunner)
    {
		_logger.LogDebug(">>> Entering ScheduleTaskController(scheduleTaskService={ScheduleTaskService},taskRunner={TaskRunner})", scheduleTaskService, taskRunner);
        _scheduleTaskService = scheduleTaskService;
        _taskRunner = taskRunner;
		_logger.LogDebug("<<< Exiting ScheduleTaskController(scheduleTaskService={ScheduleTaskService},taskRunner={TaskRunner})", scheduleTaskService, taskRunner);
		_logger.LogDebug("ScheduleTaskController(scheduleTaskService={ScheduleTaskService},taskRunner={TaskRunner}): _taskRunner → {_taskRunner}", scheduleTaskService, taskRunner, _taskRunner);
		_logger.LogDebug("ScheduleTaskController(scheduleTaskService={ScheduleTaskService},taskRunner={TaskRunner}): _scheduleTaskService → {_scheduleTaskService}", scheduleTaskService, taskRunner, _scheduleTaskService);
    }

    [HttpPost]
    [IgnoreAntiforgeryToken]
    public virtual async Task<IActionResult> RunTask(string taskType)
    {
		_logger.LogDebug(">>> Entering RunTask(taskType={TaskType})", taskType);
        var scheduleTask = await _scheduleTaskService.GetTaskByTypeAsync(taskType);
        if (scheduleTask == null)
			_logger.LogDebug("RunTask(taskType={TaskType}): scheduleTask → {ScheduleTask}", taskType, scheduleTask);
            //schedule task cannot be loaded
            return NoContent();

        await _taskRunner.ExecuteAsync(scheduleTask);

		_logger.LogDebug("<<< Exiting RunTask(taskType={TaskType})", taskType);
        return NoContent();
    }
}