using Microsoft.AspNetCore.Mvc;
using Nop.Services.ScheduleTasks;

namespace Nop.Web.Controllers;

//do not inherit it from BasePublicController. otherwise a lot of extra action filters will be called
//they can create guest account(s), etc
[AutoValidateAntiforgeryToken]
public partial class ScheduleTaskController : Controller
{
    protected readonly IScheduleTaskService _scheduleTaskService;
    protected readonly IScheduleTaskRunner _taskRunner;

    public ScheduleTaskController(IScheduleTaskService scheduleTaskService,
        IScheduleTaskRunner taskRunner)
    {
        _scheduleTaskService = scheduleTaskService;
        _taskRunner = taskRunner;
    }

    [HttpPost]
    [IgnoreAntiforgeryToken]
    public virtual async Task<IActionResult> RunTask(string taskType)
    {
		logger.LogDebug(">>> Entering RunTask(taskType={TaskType})", taskType);
        var scheduleTask = await _scheduleTaskService.GetTaskByTypeAsync(taskType);
        if (scheduleTask == null)
			logger.LogDebug("RunTask(taskType={TaskType}): scheduleTask → {ScheduleTask}", taskType, scheduleTask);
            //schedule task cannot be loaded
            return NoContent();

        await _taskRunner.ExecuteAsync(scheduleTask);

		logger.LogDebug("<<< Exiting RunTask(taskType={TaskType})", taskType);
        return NoContent();
    }
}
