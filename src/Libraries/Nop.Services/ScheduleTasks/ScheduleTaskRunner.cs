using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.ScheduleTasks;
using Nop.Core.Infrastructure;
using Nop.Services.Localization;
using Nop.Services.Logging;

namespace Nop.Services.ScheduleTasks;

/// <summary>
/// Schedule task runner
/// </summary>
public partial class ScheduleTaskRunner : IScheduleTaskRunner
{
    #region Fields

    protected readonly ILocalizationService _localizationService;
    protected readonly ILocker _locker;
    protected readonly ILogger _logger;
    protected readonly IScheduleTaskService _scheduleTaskService;
    protected readonly IStoreContext _storeContext;

    #endregion

    #region Ctor

    public ScheduleTaskRunner(ILocalizationService localizationService,
        ILocker locker,
        ILogger logger,
        IScheduleTaskService scheduleTaskService,
        IStoreContext storeContext)
    {
        _localizationService = localizationService;
        _locker = locker;
        _logger = logger;
        _scheduleTaskService = scheduleTaskService;
        _storeContext = storeContext;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Initialize and execute task
    /// </summary>
    protected virtual async Task PerformTaskAsync(ScheduleTask scheduleTask)
    {
		logger.LogDebug(">>> Entering PerformTaskAsync(scheduleTask={ScheduleTask})", scheduleTask);
        var type = (Type.GetType(scheduleTask.Type) ??
                    //ensure that it works fine when only the type name is specified (do not require fully qualified names)
                    AppDomain.CurrentDomain.GetAssemblies()
                        .Select(a => a.GetType(scheduleTask.Type))
                        .FirstOrDefault(t => t != null)) ?? throw new Exception($"Schedule task ({scheduleTask.Type}) cannot by instantiated");

        object instance = null;

		logger.LogDebug("PerformTaskAsync(scheduleTask={ScheduleTask}): instance → {Instance}", scheduleTask, instance);
        try
        {
            instance = EngineContext.Current.Resolve(type);
        }
        catch
        {
			logger.LogError("Exception in PerformTaskAsync(scheduleTask={ScheduleTask}): {Arg1}", scheduleTask, ex.Message);
            // ignored
        }

        instance ??= EngineContext.Current.ResolveUnregistered(type);

        if (instance is not IScheduleTask task)
            return;

        scheduleTask.LastStartUtc = DateTime.UtcNow;
		logger.LogDebug("PerformTaskAsync(scheduleTask={ScheduleTask}): scheduleTask.LastStartUtc → {Arg1}", scheduleTask, scheduleTask.LastStartUtc);
        //update appropriate datetime properties
        await _scheduleTaskService.UpdateTaskAsync(scheduleTask);
        await task.ExecuteAsync();
        scheduleTask.LastEndUtc = scheduleTask.LastSuccessUtc = DateTime.UtcNow;
		logger.LogDebug("PerformTaskAsync(scheduleTask={ScheduleTask}): scheduleTask.LastEndUtc → {Arg1}", scheduleTask, scheduleTask.LastEndUtc);
        //update appropriate datetime properties
        await _scheduleTaskService.UpdateTaskAsync(scheduleTask);
		logger.LogDebug("<<< Exiting PerformTaskAsync(scheduleTask={ScheduleTask})", scheduleTask);
    }

    /// <summary>
    /// Is task already running?
    /// </summary>
    /// <param name="scheduleTask">Schedule task</param>
    /// <returns>Result</returns>
    protected virtual bool IsTaskAlreadyRunning(ScheduleTask scheduleTask)
    {
		logger.LogDebug(">>> Entering IsTaskAlreadyRunning(scheduleTask={ScheduleTask})", scheduleTask);
        //task run for the first time
        if (!scheduleTask.LastStartUtc.HasValue && !scheduleTask.LastEndUtc.HasValue)
            return false;

        var lastStartUtc = scheduleTask.LastStartUtc ?? DateTime.UtcNow;

        //task already finished
        if (scheduleTask.LastEndUtc.HasValue && lastStartUtc < scheduleTask.LastEndUtc)
            return false;

        //task wasn't finished last time
        if (lastStartUtc.AddSeconds(scheduleTask.Seconds) <= DateTime.UtcNow)
		logger.LogDebug("<<< Exiting IsTaskAlreadyRunning(scheduleTask={ScheduleTask})", scheduleTask);
            return false;

        return true;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Executes the task
    /// </summary>
    /// <param name="scheduleTask">Schedule task</param>
    /// <param name="forceRun">Force run</param>
    /// <param name="throwException">A value indicating whether exception should be thrown if some error happens</param>
    /// <param name="ensureRunOncePerPeriod">A value indicating whether we should ensure this task is run once per run period</param>
    public virtual async Task ExecuteAsync(ScheduleTask scheduleTask, bool forceRun = false, bool throwException = false, bool ensureRunOncePerPeriod = true)
    {
		logger.LogDebug(">>> Entering ExecuteAsync(scheduleTask={ScheduleTask},forceRun={ForceRun},throwException={ThrowException},ensureRunOncePerPeriod={EnsureRunOncePerPeriod})", scheduleTask, forceRun, throwException, ensureRunOncePerPeriod);
        var enabled = forceRun || (scheduleTask?.Enabled ?? false);

		logger.LogDebug("ExecuteAsync(scheduleTask={ScheduleTask},forceRun={ForceRun},throwException={ThrowException},ensureRunOncePerPeriod={EnsureRunOncePerPeriod}): enabled → {Enabled}", scheduleTask, forceRun, throwException, ensureRunOncePerPeriod, enabled);
        if (scheduleTask == null || !enabled)
            return;

        if (ensureRunOncePerPeriod)
        {
            //task already running
            if (IsTaskAlreadyRunning(scheduleTask))
                return;

            //validation (so nobody else can invoke this method when he wants)
            if (scheduleTask.LastStartUtc.HasValue && Math.Round((DateTime.UtcNow - scheduleTask.LastStartUtc).Value.TotalSeconds, 0) < scheduleTask.Seconds)
                //too early
                return;
        }

        try
        {
            //get expiration time
            var expirationInSeconds = Math.Min(scheduleTask.Seconds, 300) - 1;
            var expiration = TimeSpan.FromSeconds(expirationInSeconds);
			logger.LogDebug("ExecuteAsync(scheduleTask={ScheduleTask},forceRun={ForceRun},throwException={ThrowException},ensureRunOncePerPeriod={EnsureRunOncePerPeriod}): expirationInSeconds → {ExpirationInSeconds}", scheduleTask, forceRun, throwException, ensureRunOncePerPeriod, expirationInSeconds);
			logger.LogDebug("ExecuteAsync(scheduleTask={ScheduleTask},forceRun={ForceRun},throwException={ThrowException},ensureRunOncePerPeriod={EnsureRunOncePerPeriod}): expiration → {Expiration}", scheduleTask, forceRun, throwException, ensureRunOncePerPeriod, expiration);

            //execute task with lock
            await _locker.PerformActionWithLockAsync(scheduleTask.Type, expiration, () => PerformTaskAsync(scheduleTask));
        }
        catch (Exception exc)
        {
			logger.LogError("Exception in ExecuteAsync(scheduleTask={ScheduleTask},forceRun={ForceRun},throwException={ThrowException},ensureRunOncePerPeriod={EnsureRunOncePerPeriod}): {Arg4}", scheduleTask, forceRun, throwException, ensureRunOncePerPeriod, ex.Message);
            var store = await _storeContext.GetCurrentStoreAsync();

			logger.LogDebug("ExecuteAsync(scheduleTask={ScheduleTask},forceRun={ForceRun},throwException={ThrowException},ensureRunOncePerPeriod={EnsureRunOncePerPeriod}): store → {Store}", scheduleTask, forceRun, throwException, ensureRunOncePerPeriod, store);
            var scheduleTaskUrl = $"{store.Url}{NopTaskDefaults.ScheduleTaskPath}";

			logger.LogDebug("ExecuteAsync(scheduleTask={ScheduleTask},forceRun={ForceRun},throwException={ThrowException},ensureRunOncePerPeriod={EnsureRunOncePerPeriod}): scheduleTaskUrl → {ScheduleTaskUrl}", scheduleTask, forceRun, throwException, ensureRunOncePerPeriod, scheduleTaskUrl);
			logger.LogDebug("ExecuteAsync(scheduleTask={ScheduleTask},forceRun={ForceRun},throwException={ThrowException},ensureRunOncePerPeriod={EnsureRunOncePerPeriod}): scheduleTask.Enabled → {Arg4}", scheduleTask, forceRun, throwException, ensureRunOncePerPeriod, scheduleTask.Enabled);
			logger.LogDebug("ExecuteAsync(scheduleTask={ScheduleTask},forceRun={ForceRun},throwException={ThrowException},ensureRunOncePerPeriod={EnsureRunOncePerPeriod}): scheduleTask.LastEndUtc → {Arg4}", scheduleTask, forceRun, throwException, ensureRunOncePerPeriod, scheduleTask.LastEndUtc);
            scheduleTask.Enabled = scheduleTask.Enabled && !scheduleTask.StopOnError;
            scheduleTask.LastEndUtc = DateTime.UtcNow;
            await _scheduleTaskService.UpdateTaskAsync(scheduleTask);

            var message = string.Format(await _localizationService.GetResourceAsync("ScheduleTasks.Error"), scheduleTask.Name,
                exc.Message, scheduleTask.Type, store.Name, scheduleTaskUrl);

            //log error
            await _logger.ErrorAsync(message, exc);
            if (throwException)
                throw;
        }
		logger.LogDebug("<<< Exiting ExecuteAsync(scheduleTask={ScheduleTask},forceRun={ForceRun},throwException={ThrowException},ensureRunOncePerPeriod={EnsureRunOncePerPeriod})", scheduleTask, forceRun, throwException, ensureRunOncePerPeriod);
    }

    #endregion
}