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
		_logger.LogDebug("ScheduleTaskRunner(localizationService={LocalizationService},locker={Locker},logger={Logger},scheduleTaskService={ScheduleTaskService},storeContext={StoreContext}): _localizationService → {_localizationService}", localizationService, locker, logger, scheduleTaskService, storeContext, _localizationService);
        _locker = locker;
		_logger.LogDebug("ScheduleTaskRunner(localizationService={LocalizationService},locker={Locker},logger={Logger},scheduleTaskService={ScheduleTaskService},storeContext={StoreContext}): _locker → {_locker}", localizationService, locker, logger, scheduleTaskService, storeContext, _locker);
        _logger = logger;
		_logger.LogDebug(">>> Entering ScheduleTaskRunner(localizationService={LocalizationService},locker={Locker},logger={Logger},scheduleTaskService={ScheduleTaskService},storeContext={StoreContext})", localizationService, locker, logger, scheduleTaskService, storeContext);
        _scheduleTaskService = scheduleTaskService;
        _storeContext = storeContext;
		_logger.LogDebug("ScheduleTaskRunner(localizationService={LocalizationService},locker={Locker},logger={Logger},scheduleTaskService={ScheduleTaskService},storeContext={StoreContext}): _scheduleTaskService → {_scheduleTaskService}", localizationService, locker, logger, scheduleTaskService, storeContext, _scheduleTaskService);
		_logger.LogDebug("ScheduleTaskRunner(localizationService={LocalizationService},locker={Locker},logger={Logger},scheduleTaskService={ScheduleTaskService},storeContext={StoreContext}): _storeContext → {_storeContext}", localizationService, locker, logger, scheduleTaskService, storeContext, _storeContext);
		_logger.LogDebug("<<< Exiting ScheduleTaskRunner(localizationService={LocalizationService},locker={Locker},logger={Logger},scheduleTaskService={ScheduleTaskService},storeContext={StoreContext})", localizationService, locker, logger, scheduleTaskService, storeContext);
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Initialize and execute task
    /// </summary>
    protected virtual async Task PerformTaskAsync(ScheduleTask scheduleTask)
    {
		_logger.LogDebug(">>> Entering PerformTaskAsync(scheduleTask={ScheduleTask})", scheduleTask);
        var type = (Type.GetType(scheduleTask.Type) ??
                    //ensure that it works fine when only the type name is specified (do not require fully qualified names)
                    AppDomain.CurrentDomain.GetAssemblies()
                        .Select(a => a.GetType(scheduleTask.Type))
                        .FirstOrDefault(t => t != null)) ?? throw new Exception($"Schedule task ({scheduleTask.Type}) cannot by instantiated");

        object instance = null;
		_logger.LogDebug("PerformTaskAsync(scheduleTask={ScheduleTask}): instance → {Instance}", scheduleTask, instance);

        try
        {
            instance = EngineContext.Current.Resolve(type);
        }
        catch
        {
			_logger.LogError("Exception in PerformTaskAsync(scheduleTask={ScheduleTask}): {Arg1}", scheduleTask, ex.Message);
            // ignored
        }

        instance ??= EngineContext.Current.ResolveUnregistered(type);

        if (instance is not IScheduleTask task)
            return;

        scheduleTask.LastStartUtc = DateTime.UtcNow;
		_logger.LogDebug("PerformTaskAsync(scheduleTask={ScheduleTask}): scheduleTask.LastStartUtc → {Arg1}", scheduleTask, scheduleTask.LastStartUtc);
        //update appropriate datetime properties
        await _scheduleTaskService.UpdateTaskAsync(scheduleTask);
        await task.ExecuteAsync();
        scheduleTask.LastEndUtc = scheduleTask.LastSuccessUtc = DateTime.UtcNow;
		_logger.LogDebug("PerformTaskAsync(scheduleTask={ScheduleTask}): scheduleTask.LastEndUtc → {Arg1}", scheduleTask, scheduleTask.LastEndUtc);
        //update appropriate datetime properties
        await _scheduleTaskService.UpdateTaskAsync(scheduleTask);
		_logger.LogDebug("<<< Exiting PerformTaskAsync(scheduleTask={ScheduleTask})", scheduleTask);
    }

    /// <summary>
    /// Is task already running?
    /// </summary>
    /// <param name="scheduleTask">Schedule task</param>
    /// <returns>Result</returns>
    protected virtual bool IsTaskAlreadyRunning(ScheduleTask scheduleTask)
    {
		_logger.LogDebug(">>> Entering IsTaskAlreadyRunning(scheduleTask={ScheduleTask})", scheduleTask);
        //task run for the first time
        if (!scheduleTask.LastStartUtc.HasValue && !scheduleTask.LastEndUtc.HasValue)
            return false;

        var lastStartUtc = scheduleTask.LastStartUtc ?? DateTime.UtcNow;

        //task already finished
        if (scheduleTask.LastEndUtc.HasValue && lastStartUtc < scheduleTask.LastEndUtc)
            return false;

        //task wasn't finished last time
        if (lastStartUtc.AddSeconds(scheduleTask.Seconds) <= DateTime.UtcNow)
		_logger.LogDebug("<<< Exiting IsTaskAlreadyRunning(scheduleTask={ScheduleTask})", scheduleTask);
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
		_logger.LogDebug(">>> Entering ExecuteAsync(scheduleTask={ScheduleTask},forceRun={ForceRun},throwException={ThrowException},ensureRunOncePerPeriod=<redacted>)", scheduleTask, forceRun, throwException);
        var enabled = forceRun || (scheduleTask?.Enabled ?? false);

		_logger.LogDebug("ExecuteAsync(scheduleTask={ScheduleTask},forceRun={ForceRun},throwException={ThrowException},ensureRunOncePerPeriod=<redacted>): enabled → {Enabled}", scheduleTask, forceRun, throwException, enabled);
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
			_logger.LogDebug("ExecuteAsync(scheduleTask={ScheduleTask},forceRun={ForceRun},throwException={ThrowException},ensureRunOncePerPeriod=<redacted>): expirationInSeconds → {ExpirationInSeconds}", scheduleTask, forceRun, throwException, expirationInSeconds);
            var expiration = TimeSpan.FromSeconds(expirationInSeconds);
			_logger.LogDebug("ExecuteAsync(scheduleTask={ScheduleTask},forceRun={ForceRun},throwException={ThrowException},ensureRunOncePerPeriod=<redacted>): expiration → {Expiration}", scheduleTask, forceRun, throwException, expiration);

            //execute task with lock
            await _locker.PerformActionWithLockAsync(scheduleTask.Type, expiration, () => PerformTaskAsync(scheduleTask));
        }
        catch (Exception exc)
        {
			_logger.LogError("Exception in ExecuteAsync(scheduleTask={ScheduleTask},forceRun={ForceRun},throwException={ThrowException},ensureRunOncePerPeriod=<redacted>): {Arg4}", scheduleTask, forceRun, throwException, exc.getMessage());
            var store = await _storeContext.GetCurrentStoreAsync();
			_logger.LogDebug("ExecuteAsync(scheduleTask={ScheduleTask},forceRun={ForceRun},throwException={ThrowException},ensureRunOncePerPeriod=<redacted>): store → {Store}", scheduleTask, forceRun, throwException, store);

            var scheduleTaskUrl = $"{store.Url}{NopTaskDefaults.ScheduleTaskPath}";
			_logger.LogDebug("ExecuteAsync(scheduleTask={ScheduleTask},forceRun={ForceRun},throwException={ThrowException},ensureRunOncePerPeriod=<redacted>): scheduleTaskUrl → {ScheduleTaskUrl}", scheduleTask, forceRun, throwException, scheduleTaskUrl);

            scheduleTask.Enabled = scheduleTask.Enabled && !scheduleTask.StopOnError;
			_logger.LogDebug("ExecuteAsync(scheduleTask={ScheduleTask},forceRun={ForceRun},throwException={ThrowException},ensureRunOncePerPeriod=<redacted>): scheduleTask.Enabled → {Arg4}", scheduleTask, forceRun, throwException, scheduleTask.Enabled);
			_logger.LogDebug("ExecuteAsync(scheduleTask={ScheduleTask},forceRun={ForceRun},throwException={ThrowException},ensureRunOncePerPeriod=<redacted>): scheduleTask.LastEndUtc → {Arg4}", scheduleTask, forceRun, throwException, scheduleTask.LastEndUtc);
            scheduleTask.LastEndUtc = DateTime.UtcNow;
            await _scheduleTaskService.UpdateTaskAsync(scheduleTask);

            var message = string.Format(await _localizationService.GetResourceAsync("ScheduleTasks.Error"), scheduleTask.Name,
                exc.Message, scheduleTask.Type, store.Name, scheduleTaskUrl);

            //log error
            await _logger.ErrorAsync(message, exc);
            if (throwException)
		_logger.LogDebug("<<< Exiting ExecuteAsync(scheduleTask={ScheduleTask},forceRun={ForceRun},throwException={ThrowException},ensureRunOncePerPeriod=<redacted>)", scheduleTask, forceRun, throwException);
                throw;
        }
    }

    #endregion
}