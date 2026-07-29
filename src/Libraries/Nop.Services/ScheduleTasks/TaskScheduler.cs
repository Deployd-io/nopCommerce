using Microsoft.Extensions.DependencyInjection;
using Nop.Core;
using Nop.Core.Configuration;
using Nop.Core.Domain.ScheduleTasks;
using Nop.Core.Http;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Services.Logging;

namespace Nop.Services.ScheduleTasks;

/// <summary>
/// Represents task manager
/// </summary>
public partial class TaskScheduler : ITaskScheduler
{
    #region Fields

    protected static readonly List<TaskThread> _taskThreads = new();
    protected readonly AppSettings _appSettings;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private Task _taskThread;

    #endregion

    #region Ctor

    public TaskScheduler(AppSettings appSettings,
        IHttpClientFactory httpClientFactory,
        IServiceScopeFactory serviceScopeFactory)
    {
        _appSettings = appSettings;
        _serviceScopeFactory = serviceScopeFactory;
        TaskThread.HttpClientFactory = httpClientFactory;
        TaskThread.ServiceScopeFactory = serviceScopeFactory;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Initializes task scheduler
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InitializeAsync()
    {
		logger.LogDebug(">>> Entering InitializeAsync()");
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        if (_taskThreads.Any())
            return;

        using var scope = _serviceScopeFactory.CreateScope();
        var scheduleTaskService = scope.ServiceProvider.GetService<IScheduleTaskService>() ?? throw new NullReferenceException($"Can't get {nameof(IScheduleTaskService)} implementation from the scope");
		logger.LogDebug("InitializeAsync(): scope → {Scope}", scope);
		logger.LogDebug("InitializeAsync(): scheduleTaskService → {ScheduleTaskService}", scheduleTaskService);

        //initialize and start schedule tasks
        var scheduleTasks = (await scheduleTaskService.GetAllTasksAsync())
            .OrderBy(x => x.Seconds)
            .ToList();

        var storeContext = scope.ServiceProvider.GetService<IStoreContext>() ?? throw new NullReferenceException($"Can't get {nameof(IStoreContext)} implementation from the scope");

		logger.LogDebug("InitializeAsync(): storeContext → {StoreContext}", storeContext);
        var store = await storeContext.GetCurrentStoreAsync();

		logger.LogDebug("InitializeAsync(): store → {Store}", store);
        var scheduleTaskUrl = $"{store.Url.TrimEnd('/')}/{NopTaskDefaults.ScheduleTaskPath}";
        var timeout = _appSettings.Get<CommonConfig>().ScheduleTaskRunTimeout;
		logger.LogDebug("InitializeAsync(): scheduleTaskUrl → {ScheduleTaskUrl}", scheduleTaskUrl);
		logger.LogDebug("InitializeAsync(): timeout → {Timeout}", timeout);

        foreach (var scheduleTask in scheduleTasks)
        {
            var taskThread = new TaskThread(scheduleTask, scheduleTaskUrl, timeout)
            {
                Seconds = scheduleTask.Seconds
            };

            //sometimes a task period could be set to several hours (or even days)
            //in this case a probability that it'll be run is quite small (an application could be restarted)
            //calculate time before start an interrupted task
            if (scheduleTask.LastStartUtc.HasValue)
            {
                //seconds left since the last start
                var secondsLeft = (DateTime.UtcNow - scheduleTask.LastStartUtc).Value.TotalSeconds;

				logger.LogDebug("InitializeAsync(): secondsLeft → {SecondsLeft}", secondsLeft);
                if (secondsLeft >= scheduleTask.Seconds)
                    //run now (immediately)
                    taskThread.InitSeconds = 0;
					logger.LogDebug("InitializeAsync(): taskThread.InitSeconds → {Arg0}", taskThread.InitSeconds);
                else
                    //calculate start time
                    //and round it (so "ensureRunOncePerPeriod" parameter was fine)
                    taskThread.InitSeconds = (int)(scheduleTask.Seconds - secondsLeft) + 1;
            }
            else if (scheduleTask.LastEnabledUtc.HasValue)
            {
                //seconds left since the last enable
                var secondsLeft = (DateTime.UtcNow - scheduleTask.LastEnabledUtc).Value.TotalSeconds;

                if (secondsLeft >= scheduleTask.Seconds)
                    //run now (immediately)
                    taskThread.InitSeconds = 0;
                else
                    //calculate start time
                    //and round it (so "ensureRunOncePerPeriod" parameter was fine)
                    taskThread.InitSeconds = (int)(scheduleTask.Seconds - secondsLeft) + 1;
            }
            else
                //first start of a task
                taskThread.InitSeconds = scheduleTask.Seconds;

            _taskThreads.Add(taskThread);
        }
		logger.LogDebug("<<< Exiting InitializeAsync()");
    }

    /// <summary>
    /// Starts the task scheduler
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual Task StartSchedulerAsync()
    {
		logger.LogDebug(">>> Entering StartSchedulerAsync()");
        _taskThread = Task.WhenAll(_taskThreads.Select(taskThread => taskThread.InitTimerAsync()));

		logger.LogDebug("StartSchedulerAsync(): _taskThread → {_taskThread}", _taskThread);
		logger.LogDebug("<<< Exiting StartSchedulerAsync()");
        return Task.CompletedTask;
    }

    /// <summary>
    /// Stops the task scheduler
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task StopSchedulerAsync()
    {
		logger.LogDebug(">>> Entering StopSchedulerAsync()");
        foreach (var taskThread in _taskThreads)
            taskThread.Dispose();

        try
        {
            if (_taskThread is { IsCompleted: false })
                await _taskThread;
        }
        catch
        {
		logger.LogError("Exception in StopSchedulerAsync(): {Arg0}", ex.Message);
           //ignore
        }

        _taskThread?.Dispose();
        _taskThreads.Clear();
		logger.LogDebug("<<< Exiting StopSchedulerAsync()");
    }

    #endregion

    #region Nested class

    /// <summary>
    /// Represents task thread
    /// </summary>
    protected partial class TaskThread : IDisposable
    {
        #region Fields

        protected readonly string _scheduleTaskUrl;
        protected readonly ScheduleTask _scheduleTask;
        protected readonly int? _timeout;
        protected bool _disposed;
        protected readonly CancellationTokenSource _cancellationToken;

        #endregion

        #region Ctor

        public TaskThread(ScheduleTask task, string scheduleTaskUrl, int? timeout)
        {
            _scheduleTaskUrl = scheduleTaskUrl;
            _scheduleTask = task;
            _timeout = timeout;
			logger.LogDebug("TaskThread(task={Task},scheduleTaskUrl={ScheduleTaskUrl},timeout={Timeout}): _timeout → {_timeout}", task, scheduleTaskUrl, timeout, _timeout);
            IsStarted = false;
			logger.LogDebug("TaskThread(task={Task},scheduleTaskUrl={ScheduleTaskUrl},timeout={Timeout}): IsStarted → {IsStarted}", task, scheduleTaskUrl, timeout, IsStarted);
            _cancellationToken = new CancellationTokenSource();
			logger.LogDebug("TaskThread(task={Task},scheduleTaskUrl={ScheduleTaskUrl},timeout={Timeout}): _cancellationToken → {_cancellationToken}", task, scheduleTaskUrl, timeout, _cancellationToken);
            
            Seconds = 10 * 60;
			logger.LogDebug("<<< Exiting TaskThread(task={Task},scheduleTaskUrl={ScheduleTaskUrl},timeout={Timeout})", task, scheduleTaskUrl, timeout);
			logger.LogDebug("TaskThread(task={Task},scheduleTaskUrl={ScheduleTaskUrl},timeout={Timeout}): Seconds → {Seconds}", task, scheduleTaskUrl, timeout, Seconds);
			logger.LogDebug("TaskThread(task={Task},scheduleTaskUrl={ScheduleTaskUrl},timeout={Timeout}): _scheduleTaskUrl → {_scheduleTaskUrl}", task, scheduleTaskUrl, timeout, _scheduleTaskUrl);
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Run task
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task RunAsync()
        {
			logger.LogDebug(">>> Entering RunAsync()");
			var __logStart = System.Diagnostics.Stopwatch.StartNew();
            if (Seconds <= 0)
                return;

            StartedUtc = DateTime.UtcNow;
			logger.LogDebug("RunAsync(): StartedUtc → {StartedUtc}", StartedUtc);
            IsRunning = true;
			logger.LogDebug("RunAsync(): IsRunning → {IsRunning}", IsRunning);
            HttpClient client = null;

			logger.LogDebug("RunAsync(): client → {Client}", client);
            try
            {
                //create and configure client
                client = HttpClientFactory.CreateClient(NopHttpDefaults.DefaultHttpClient);
                if (_timeout.HasValue)
                    client.Timeout = TimeSpan.FromMilliseconds(_timeout.Value);

                //send post data
                var data = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("taskType", _scheduleTask.Type) });
                await client.PostAsync(_scheduleTaskUrl, data);
				logger.LogDebug("RunAsync(): data → {Data}", data);
            }
            catch (Exception ex)
            {
				logger.LogError("Exception in RunAsync(): {Arg0}", ex.Message);
                using var scope = ServiceScopeFactory.CreateScope();

				logger.LogDebug("RunAsync(): scope → {Scope}", scope);
                // Resolve
                var logger = EngineContext.Current.Resolve<ILogger>(scope);
                var localizationService = EngineContext.Current.Resolve<ILocalizationService>(scope);
				logger.LogDebug("RunAsync(): logger → {Logger}", logger);
				logger.LogDebug("RunAsync(): localizationService → {LocalizationService}", localizationService);
                var storeContext = EngineContext.Current.Resolve<IStoreContext>(scope);

				logger.LogDebug("RunAsync(): storeContext → {StoreContext}", storeContext);
                var message = ex.InnerException?.GetType() == typeof(TaskCanceledException) ? await localizationService.GetResourceAsync("ScheduleTasks.TimeoutError") : ex.Message;
                var store = await storeContext.GetCurrentStoreAsync();
				logger.LogDebug("RunAsync(): message → {Message}", message);
				logger.LogDebug("RunAsync(): store → {Store}", store);

                message = string.Format(await localizationService.GetResourceAsync("ScheduleTasks.Error"), _scheduleTask.Name,
                    message, _scheduleTask.Type, store.Name, _scheduleTaskUrl);

                await logger.ErrorAsync(message, ex);
            }
            finally
            {
                client?.Dispose();
            }

            IsRunning = false;
			logger.LogInformation("RunAsync(): external service call took {Arg0} ms", __logStart.ElapsedMilliseconds);
			logger.LogDebug("<<< Exiting RunAsync()");
        }
        
        /// <summary>
        /// Protected implementation of Dispose pattern.
        /// </summary>
        /// <param name="disposing">Specifies whether to disposing resources</param>
        protected virtual void Dispose(bool disposing)
        {
			logger.LogDebug(">>> Entering Dispose(disposing={Disposing})", disposing);
            if (_disposed)
                return;

            IsStarted = false;
			logger.LogDebug("Dispose(disposing={Disposing}): IsStarted → {IsStarted}", disposing, IsStarted);
            _disposed = true;
			logger.LogDebug("Dispose(disposing={Disposing}): _disposed → {_disposed}", disposing, _disposed);
            _cancellationToken.Cancel();
			logger.LogDebug(">>> Entering Dispose()");
			logger.LogDebug("<<< Exiting Dispose(disposing={Disposing})", disposing);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Disposes the instance
        /// </summary>
        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Inits a timer
        /// </summary>
        public virtual async Task InitTimerAsync()
        {
			logger.LogDebug(">>> Entering InitTimerAsync()");
            var interval = TimeSpan.FromMilliseconds(Interval);
            IsStarted = true;
			logger.LogDebug("InitTimerAsync(): interval → {Interval}", interval);
			logger.LogDebug("InitTimerAsync(): IsStarted → {IsStarted}", IsStarted);
            using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(InitInterval > 0 ? InitInterval : 1));
            
			logger.LogDebug("InitTimerAsync(): timer → {Timer}", timer);
            while (await timer.WaitForNextTickAsync(_cancellationToken.Token))
            {
                if (IsDisposed)
                    break;

                try
                {
                    await RunAsync();
                }
                catch
                {
					logger.LogError("Exception in InitTimerAsync(): {Arg0}", ex.Message);
                    // ignore
                }
                finally
                {
                    if (!IsDisposed && RunOnlyOnce) 
                        Dispose();
                }

                if (timer.Period != interval)
                    timer.Period = interval;
					logger.LogDebug("InitTimerAsync(): timer.Period → {Arg0}", timer.Period);
            }
			logger.LogDebug("<<< Exiting InitTimerAsync()");
        }

        #endregion

        #region Properties

        internal static IHttpClientFactory HttpClientFactory { get; set; }

        internal static IServiceScopeFactory ServiceScopeFactory { get; set; }

        /// <summary>
        /// Gets or sets the interval in seconds at which to run the tasks
        /// </summary>
        public int Seconds { get; set; }

        /// <summary>
        /// Get or set the interval before timer first start 
        /// </summary>
        public int InitSeconds { get; set; }

        /// <summary>
        /// Get or sets a datetime when thread has been started
        /// </summary>
        public DateTime StartedUtc { get; protected set; }

        /// <summary>
        /// Get or sets a value indicating whether thread is running
        /// </summary>
        public bool IsRunning { get; protected set; }

        /// <summary>
        /// Gets the interval (in milliseconds) at which to run the task
        /// </summary>
        public int Interval
        {
            get
            {
                //if somebody entered more than "2147483" seconds, then an exception could be thrown (exceeds int.MaxValue)
                var interval = Seconds * 1000;
                if (interval <= 0)
                    interval = int.MaxValue;
                return interval;
            }
        }

        /// <summary>
        /// Gets the due time interval (in milliseconds) at which to begin start the task
        /// </summary>
        public int InitInterval
        {
            get
            {
                //if somebody entered less than "0" seconds, then an exception could be thrown
                var interval = InitSeconds * 1000;
                if (interval <= 0)
                    interval = 0;
                return interval;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the thread would be run only once (on application start)
        /// </summary>
        public bool RunOnlyOnce { get; set; }

        /// <summary>
        /// Gets a value indicating whether the timer is started
        /// </summary>
        public bool IsStarted { get; set; }

        /// <summary>
        /// Gets a value indicating whether the timer is disposed
        /// </summary>
        public bool IsDisposed => _disposed;

        #endregion
    }

    #endregion
}