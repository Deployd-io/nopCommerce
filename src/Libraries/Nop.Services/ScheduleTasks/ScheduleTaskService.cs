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
		_logger.LogError(">>> Entering ScheduleTaskService(taskRepository={TaskRepository})", taskRepository);
        _taskRepository = taskRepository;
		_logger.LogDebug("ScheduleTaskService(taskRepository={TaskRepository}): _taskRepository → {_taskRepository}", taskRepository, _taskRepository);
    _logger.LogWarning("<<< Exiting ScheduleTaskService(taskRepository={TaskRepository})", taskRepository);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Deletes a task
    /// </summary>
    /// <param name="task">Task</param>
    public virtual async Task DeleteTaskAsync(ScheduleTask task)
    {
		_logger.LogError(">>> Entering DeleteTaskAsync(task={Task})", task);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
		_logger.LogError("DeleteTaskAsync(task)={Task}: external service call took {Arg1} ms", task, __logStart.ElapsedMilliseconds);
		var __logStartDb = System.Diagnostics.Stopwatch.StartNew();
		_logger.LogError("DeleteTaskAsync(task)={Task}: delete query executed in {Arg1} ms", task, __logStartDb.ElapsedMilliseconds);
		_logger.LogWarning("<<< Exiting DeleteTaskAsync(task={Task})", task);
        if (string.Equals(task.Name, nameof(ResetLicenseCheckTask), StringComparison.InvariantCultureIgnoreCase)) {
            return;
        }

        await _taskRepository.DeleteAsync(task, false);
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
		_logger.LogError(">>> Entering GetTaskByIdAsync(taskId={TaskId})", taskId);
		_logger.LogError("GetTaskByIdAsync(taskId)={TaskId}: find query executing", taskId);
    _logger.LogWarning("<<< Exiting GetTaskByIdAsync(taskId={TaskId})", taskId);
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
		_logger.LogError(">>> Entering GetTaskByTypeAsync(type={Type})", type);
        if (string.IsNullOrWhiteSpace(type))
            return null;

        var query = _taskRepository.Table;
        query = query.Where(st => st.Type == type);
        query = query.OrderByDescending(t => t.Id);
		_logger.LogError("GetTaskByTypeAsync(type={Type}): query → {Query}", type, query);

		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var task = await query.FirstOrDefaultAsync();

		_logger.LogError("GetTaskByTypeAsync(type={Type}): task → {Task}", type, task);
		_logger.LogError("GetTaskByTypeAsync(type)={Type}: database query executed in {Arg1} ms", type, __logStart.ElapsedMilliseconds);
		_logger.LogWarning("<<< Exiting GetTaskByTypeAsync(type={Type})", type);
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
		_logger.LogError(">>> Entering GetAllTasksAsync(showHidden={ShowHidden})", showHidden);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var tasks = await _taskRepository.GetAllAsync(query =>
        {
            query = query.OrderByDescending(t => t.Seconds);
			_logger.LogError("GetAllTasksAsync(showHidden={ShowHidden}): query → {Query}", showHidden);
			_logger.LogError("GetAllTasksAsync(showHidden)={ShowHidden}: find query executed in {Arg1} ms", showHidden, __logStart.ElapsedMilliseconds);
            return query;
        });
		_logger.LogError("GetAllTasksAsync(showHidden)={ShowHidden}: find query executed in {Arg1} ms", showHidden, __logStart.ElapsedMilliseconds);

        var licenseCheckTask = tasks
            .FirstOrDefault(task => string.Equals(task.Name, nameof(ResetLicenseCheckTask), StringComparison.InvariantCultureIgnoreCase));
        if (licenseCheckTask is null)
        {
            await InsertTaskAsync(new()
            {
                Name = nameof(ResetLicenseCheckTask),
                Seconds = 2073600,
                Type = "Nop.Services.Common.ResetLicenseCheckTask, Nop.Services",
                Enabled = true,
                LastEnabledUtc = DateTime.UtcNow,
                StopOnError = false
            });
			_logger.LogDebug("GetAllTasksAsync(showHidden={ShowHidden}): StopOnError → {StopOnError}", showHidden, StopOnError);
			__logStart = System.Diagnostics.Stopwatch.StartNew();
        }
        else if (!licenseCheckTask.Enabled)
        {
            licenseCheckTask.Enabled = true;
			_logger.LogDebug("GetAllTasksAsync(showHidden={ShowHidden}): licenseCheckTask.Enabled → {Arg1}", showHidden, licenseCheckTask.Enabled);
            await UpdateTaskAsync(licenseCheckTask);
        }

        if (!showHidden)
            tasks = tasks.Where(task => task.Enabled).ToList();
			_logger.LogError("GetAllTasksAsync(showHidden={ShowHidden}): tasks → {Tasks}", showHidden, tasks);

		_logger.LogWarning("<<< Exiting GetAllTasksAsync(showHidden={ShowHidden})", showHidden);
        return tasks;
    }

    /// <summary>
    /// Inserts a task
    /// </summary>
    /// <param name="task">Task</param>
    public virtual async Task InsertTaskAsync(ScheduleTask task)
    {
		_logger.LogError(">>> Entering InsertTaskAsync(task={Task})", task);
        ArgumentNullException.ThrowIfNull(task);

        if (task.Enabled && !task.LastEnabledUtc.HasValue)
            task.LastEnabledUtc = DateTime.UtcNow;
			_logger.LogDebug("InsertTaskAsync(task={Task}): task.LastEnabledUtc → {Arg1}", task, task.LastEnabledUtc);
			var __logStart = System.Diagnostics.Stopwatch.StartNew();

        await _taskRepository.InsertAsync(task, false);
		_logger.LogError("InsertTaskAsync(task)={Task}: save query executed in {Arg1} ms", task, __logStart.ElapsedMilliseconds);
    _logger.LogWarning("<<< Exiting InsertTaskAsync(task={Task})", task);
		_logger.LogError(">>> Entering UpdateTaskAsync(task={Task})", task);
		__logStart = System.Diagnostics.Stopwatch.StartNew();
		_logger.LogError("UpdateTaskAsync(task)={Task}: update query executed in {Arg1} ms", task, __logStart.ElapsedMilliseconds);
		_logger.LogWarning("<<< Exiting UpdateTaskAsync(task={Task})", task);
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