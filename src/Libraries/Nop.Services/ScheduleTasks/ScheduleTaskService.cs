using Nop.Core.Domain.ScheduleTasks;
using Nop.Data;
using Nop.Services.Common;
using Microsoft.Extensions.Logging;

namespace Nop.Services.ScheduleTasks;

/// <summary>
/// Task service
/// </summary>
public partial class ScheduleTaskService : IScheduleTaskService
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IRepository<ScheduleTask> _taskRepository;

    #endregion

    #region Ctor

    public ScheduleTaskService(IRepository<ScheduleTask> taskRepository)
    {
		_logger.LogDebug(">>> Entering ScheduleTaskService(taskRepository={TaskRepository})", taskRepository);
        _taskRepository = taskRepository;
		_logger.LogDebug("<<< Exiting ScheduleTaskService(taskRepository={TaskRepository})", taskRepository);
		_logger.LogDebug("ScheduleTaskService(taskRepository={TaskRepository}): _taskRepository → {_taskRepository}", taskRepository, _taskRepository);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Deletes a task
    /// </summary>
    /// <param name="task">Task</param>
    public virtual async Task DeleteTaskAsync(ScheduleTask task)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        if (string.Equals(task.Name, nameof(ResetLicenseCheckTask), StringComparison.InvariantCultureIgnoreCase))
            return;

        await _taskRepository.DeleteAsync(task, false);
		_logger.LogDebug("<<< Exiting DeleteTaskAsync(task={Task})", task);
    }

    /// <summary>
    /// Gets a task
    /// </summary>
    /// <param name="taskId">Task identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the schedule task
    /// </returns>
    public virtual async Task<ScheduleTask> GetTaskByIdAsync(int taskId)
    {
		_logger.LogDebug(">>> Entering GetTaskByIdAsync(taskId={TaskId})", taskId);
		_logger.LogDebug("<<< Exiting GetTaskByIdAsync(taskId={TaskId})", taskId);
        return await _taskRepository.GetByIdAsync(taskId, _ => default);
    }

    /// <summary>
    /// Gets a task by its type
    /// </summary>
    /// <param name="type">Task type</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the schedule task
    /// </returns>
    public virtual async Task<ScheduleTask> GetTaskByTypeAsync(string type)
    {
		_logger.LogDebug(">>> Entering GetTaskByTypeAsync(type={Type})", type);
        if (string.IsNullOrWhiteSpace(type))
            return null;

        var query = _taskRepository.Table;
		_logger.LogDebug("GetTaskByTypeAsync(type={Type}): query → {Query}", type, query);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        query = query.Where(st => st.Type == type);
        query = query.OrderByDescending(t => t.Id);

        var task = await query.FirstOrDefaultAsync();

		_logger.LogDebug("GetTaskByTypeAsync(type={Type}): task → {Task}", type, task);
		_logger.LogInformation("GetTaskByTypeAsync(type)={Type}: database query executed in {Arg1} ms", type, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting GetTaskByTypeAsync(type={Type})", type);
        return task;
    }

    /// <summary>
    /// Gets all tasks
    /// </summary>
    /// <param name="showHidden">A value indicating whether to show hidden records</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of schedule task
    /// </returns>
    public virtual async Task<IList<ScheduleTask>> GetAllTasksAsync(bool showHidden = false)
    {
		_logger.LogDebug(">>> Entering GetAllTasksAsync(showHidden={ShowHidden})", showHidden);
        var tasks = await _taskRepository.GetAllAsync(query =>
        {
			_logger.LogDebug("GetAllTasksAsync(showHidden={ShowHidden}): tasks → {Tasks}", showHidden, tasks);
			var __logStart = System.Diagnostics.Stopwatch.StartNew();
            query = query.OrderByDescending(t => t.Seconds);
			_logger.LogDebug("GetAllTasksAsync(showHidden={ShowHidden}): query → {Query}", showHidden);
			_logger.LogInformation("GetAllTasksAsync(showHidden)={ShowHidden}: find query executed in {Arg1} ms", showHidden, __logStart.ElapsedMilliseconds);
            return query;
        });
		_logger.LogDebug("GetAllTasksAsync(showHidden={ShowHidden}): query → {Query}", showHidden, query);
		_logger.LogInformation("GetAllTasksAsync(showHidden)={ShowHidden}: find query executed in {Arg1} ms", showHidden, __logStart.ElapsedMilliseconds);

        var licenseCheckTask = tasks
            .FirstOrDefault(task => string.Equals(task.Name, nameof(ResetLicenseCheckTask), StringComparison.InvariantCultureIgnoreCase));
        if (licenseCheckTask is null)
        {
			var __logStart = System.Diagnostics.Stopwatch.StartNew();
            await InsertTaskAsync(new()
            {
                Name = nameof(ResetLicenseCheckTask),
                Seconds = 2073600,
                Type = "Nop.Services.Common.ResetLicenseCheckTask, Nop.Services",
                Enabled = true,
                LastEnabledUtc = DateTime.UtcNow,
                StopOnError = false
				_logger.LogDebug("GetAllTasksAsync(showHidden={ShowHidden}): Name → {Name}", showHidden, Name);
				_logger.LogDebug("GetAllTasksAsync(showHidden={ShowHidden}): Seconds → {Seconds}", showHidden, Seconds);
				_logger.LogDebug("GetAllTasksAsync(showHidden={ShowHidden}): Type → {Type}", showHidden, Type);
				_logger.LogInformation("GetAllTasksAsync(showHidden={ShowHidden}): Enabled → {Enabled}", showHidden, Enabled);
				_logger.LogDebug("GetAllTasksAsync(showHidden={ShowHidden}): LastEnabledUtc → {LastEnabledUtc}", showHidden, LastEnabledUtc);
				_logger.LogDebug("GetAllTasksAsync(showHidden={ShowHidden}): StopOnError → {StopOnError}", showHidden, StopOnError);
            });
        }
        else if (!licenseCheckTask.Enabled)
        {
            licenseCheckTask.Enabled = true;
			_logger.LogDebug("GetAllTasksAsync(showHidden={ShowHidden}): licenseCheckTask.Enabled → {Arg1}", showHidden, licenseCheckTask.Enabled);
            await UpdateTaskAsync(licenseCheckTask);
        }

        if (!showHidden)
            tasks = tasks.Where(task => task.Enabled).ToList();

		_logger.LogDebug("<<< Exiting GetAllTasksAsync(showHidden={ShowHidden})", showHidden);
        return tasks;
    }

    /// <summary>
    /// Inserts a task
    /// </summary>
    /// <param name="task">Task</param>
    public virtual async Task InsertTaskAsync(ScheduleTask task)
    {
        ArgumentNullException.ThrowIfNull(task);

        if (task.Enabled && !task.LastEnabledUtc.HasValue)
            task.LastEnabledUtc = DateTime.UtcNow;

        await _taskRepository.InsertAsync(task, false);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		_logger.LogDebug("<<< Exiting UpdateTaskAsync(task={Task})", task);
    }

    /// <summary>
    /// Updates the task
    /// </summary>
    /// <param name="task">Task</param>
    public virtual async Task UpdateTaskAsync(ScheduleTask task)
    {
        if (string.Equals(task.Name, nameof(ResetLicenseCheckTask), StringComparison.InvariantCultureIgnoreCase) && !task.Enabled)
            return;

        await _taskRepository.UpdateAsync(task, false);
    }

    #endregion
}