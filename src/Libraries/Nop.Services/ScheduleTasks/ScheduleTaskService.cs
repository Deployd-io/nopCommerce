using Nop.Core.Domain.ScheduleTasks;
using Nop.Data;
using Nop.Services.Common;

namespace Nop.Services.ScheduleTasks;

/// <summary>
/// Task service
/// </summary>
public partial class ScheduleTaskService : IScheduleTaskService
{
    #region Fields

    protected readonly IRepository<ScheduleTask> _taskRepository;

    #endregion

    #region Ctor

    public ScheduleTaskService(IRepository<ScheduleTask> taskRepository)
    {
		logger.LogDebug(">>> Entering ScheduleTaskService(taskRepository={TaskRepository})", taskRepository);
        _taskRepository = taskRepository;
		logger.LogDebug("<<< Exiting ScheduleTaskService(taskRepository={TaskRepository})", taskRepository);
		logger.LogDebug("ScheduleTaskService(taskRepository={TaskRepository}): _taskRepository → {_taskRepository}", taskRepository, _taskRepository);
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
			var __logStart = System.Diagnostics.Stopwatch.StartNew();

        await _taskRepository.DeleteAsync(task, false);
		logger.LogDebug("<<< Exiting DeleteTaskAsync(task={Task})", task);
    }

	logger.LogInformation("DeleteTaskAsync(task)={Task}: delete query executed in {Arg1} ms", task, __logStart.ElapsedMilliseconds);
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
		logger.LogDebug(">>> Entering GetTaskByIdAsync(taskId={TaskId})", taskId);
		logger.LogDebug("<<< Exiting GetTaskByIdAsync(taskId={TaskId})", taskId);
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
		logger.LogDebug(">>> Entering GetTaskByTypeAsync(type={Type})", type);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        if (string.IsNullOrWhiteSpace(type))
            return null;

        var query = _taskRepository.Table;
        query = query.Where(st => st.Type == type);
		logger.LogDebug("GetTaskByTypeAsync(type={Type}): query → {Query}", type, query);
        query = query.OrderByDescending(t => t.Id);

        var task = await query.FirstOrDefaultAsync();

		logger.LogDebug("GetTaskByTypeAsync(type={Type}): task → {Task}", type, task);
		logger.LogDebug("<<< Exiting GetTaskByTypeAsync(type={Type})", type);
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
		logger.LogDebug(">>> Entering GetAllTasksAsync(showHidden={ShowHidden})", showHidden);
        var tasks = await _taskRepository.GetAllAsync(query =>
        {
			logger.LogDebug("GetAllTasksAsync(showHidden={ShowHidden}): tasks → {Tasks}", showHidden, tasks);
			var __logStart = System.Diagnostics.Stopwatch.StartNew();
            query = query.OrderByDescending(t => t.Seconds);
            return query;
        });

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
				logger.LogDebug("GetAllTasksAsync(showHidden={ShowHidden}): Name → {Name}", showHidden, Name);
				logger.LogDebug("GetAllTasksAsync(showHidden={ShowHidden}): Seconds → {Seconds}", showHidden, Seconds);
				logger.LogDebug("GetAllTasksAsync(showHidden={ShowHidden}): Type → {Type}", showHidden, Type);
				logger.LogInformation("GetAllTasksAsync(showHidden={ShowHidden}): Enabled → {Enabled}", showHidden, Enabled);
				logger.LogDebug("GetAllTasksAsync(showHidden={ShowHidden}): LastEnabledUtc → {LastEnabledUtc}", showHidden, LastEnabledUtc);
				logger.LogDebug("GetAllTasksAsync(showHidden={ShowHidden}): StopOnError → {StopOnError}", showHidden, StopOnError);
            });
        }
        else if (!licenseCheckTask.Enabled)
        {
            licenseCheckTask.Enabled = true;
			logger.LogDebug("GetAllTasksAsync(showHidden={ShowHidden}): licenseCheckTask.Enabled → {Arg1}", showHidden, licenseCheckTask.Enabled);
            await UpdateTaskAsync(licenseCheckTask);
        }

        if (!showHidden)
            tasks = tasks.Where(task => task.Enabled).ToList();

		logger.LogDebug("<<< Exiting GetAllTasksAsync(showHidden={ShowHidden})", showHidden);
        return tasks;
    }

    /// <summary>
    /// Inserts a task
    /// </summary>
    /// <param name="task">Task</param>
    public virtual async Task InsertTaskAsync(ScheduleTask task)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(task);

        if (task.Enabled && !task.LastEnabledUtc.HasValue)
            task.LastEnabledUtc = DateTime.UtcNow;

        await _taskRepository.InsertAsync(task, false);
		logger.LogDebug(">>> Entering UpdateTaskAsync(task={Task})", task);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("UpdateTaskAsync(task)={Task}: update query executed in {Arg1} ms", task, __logStart.ElapsedMilliseconds);
		logger.LogDebug("<<< Exiting UpdateTaskAsync(task={Task})", task);
    }

    /// <summary>
    /// Updates the task
    /// </summary>
    /// <param name="task">Task</param>
    public virtual async Task UpdateTaskAsync(ScheduleTask task)
    {
        if (string.Equals(task.Name, nameof(ResetLicenseCheckTask), StringComparison.InvariantCultureIgnoreCase) && !task.Enabled)
            return;
			var __logStart = System.Diagnostics.Stopwatch.StartNew();

        await _taskRepository.UpdateAsync(task, false);
    }

    #endregion
}
