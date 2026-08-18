using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Logging;
using Nop.Data;

namespace Nop.Services.Logging;

/// <summary>
/// Default logger
/// </summary>
public partial class DefaultLogger : ILogger
{
    #region Fields

    protected readonly CommonSettings _commonSettings;
    protected readonly CustomerSettings _customerSettings;

    protected readonly IRepository<Log> _logRepository;
    protected readonly IWebHelper _webHelper;

    #endregion

    #region Ctor

    public DefaultLogger(CommonSettings commonSettings,
        CustomerSettings customerSettings,
        IRepository<Log> logRepository,
        IWebHelper webHelper)
    {
		DefaultLogger.LogDebug(">>> Entering DefaultLogger(commonSettings={CommonSettings},customerSettings={CustomerSettings},logRepository={LogRepository},webHelper={WebHelper})", commonSettings, customerSettings, logRepository, webHelper);
        _commonSettings = commonSettings;
        _customerSettings = customerSettings;
		DefaultLogger.LogDebug("DefaultLogger(commonSettings={CommonSettings},customerSettings={CustomerSettings},logRepository={LogRepository},webHelper={WebHelper}): _customerSettings → {_customerSettings}", commonSettings, customerSettings, logRepository, webHelper, _customerSettings);
		DefaultLogger.LogDebug("DefaultLogger(commonSettings={CommonSettings},customerSettings={CustomerSettings},logRepository={LogRepository},webHelper={WebHelper}): _commonSettings → {_commonSettings}", commonSettings, customerSettings, logRepository, webHelper, _commonSettings);
        _logRepository = logRepository;
		DefaultLogger.LogDebug("DefaultLogger(commonSettings={CommonSettings},customerSettings={CustomerSettings},logRepository={LogRepository},webHelper={WebHelper}): _logRepository → {_logRepository}", commonSettings, customerSettings, logRepository, webHelper, _logRepository);
        _webHelper = webHelper;
		DefaultLogger.LogDebug("<<< Exiting DefaultLogger(commonSettings={CommonSettings},customerSettings={CustomerSettings},logRepository={LogRepository},webHelper={WebHelper})", commonSettings, customerSettings, logRepository, webHelper);
		DefaultLogger.LogDebug("DefaultLogger(commonSettings={CommonSettings},customerSettings={CustomerSettings},logRepository={LogRepository},webHelper={WebHelper}): _webHelper → {_webHelper}", commonSettings, customerSettings, logRepository, webHelper, _webHelper);
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Gets a value indicating whether this message should not be logged
    /// </summary>
    /// <param name="message">Message</param>
    /// <returns>Result</returns>
    protected virtual bool IgnoreLog(string message)
    {
		DefaultLogger.LogDebug(">>> Entering IgnoreLog(message={Message})", message);
        if (!_commonSettings.IgnoreLogWordlist.Any())
            return false;

        if (string.IsNullOrWhiteSpace(message))
		DefaultLogger.LogDebug("<<< Exiting IgnoreLog(message={Message})", message);
            return false;

        return _commonSettings
            .IgnoreLogWordlist
            .Any(x => message.Contains(x, StringComparison.InvariantCultureIgnoreCase));
    }

    /// <summary>
    /// Prepare log item
    /// </summary>
    /// <param name="logLevel">Log level</param>
    /// <param name="shortMessage">The short message</param>
    /// <param name="fullMessage">The full message</param>
    /// <param name="customer">The customer to associate log record with</param>
    /// <returns>Log item</returns>
    protected virtual Log PrepareLog(LogLevel logLevel, string shortMessage, string fullMessage = "", Customer customer = null)
    {
		DefaultLogger.LogDebug(">>> Entering PrepareLog(logLevel={LogLevel},shortMessage={ShortMessage},fullMessage={FullMessage},customer={Customer})", logLevel, shortMessage, fullMessage, customer);
		DefaultLogger.LogDebug("<<< Exiting PrepareLog(logLevel={LogLevel},shortMessage={ShortMessage},fullMessage={FullMessage},customer={Customer})", logLevel, shortMessage, fullMessage, customer);
        return new Log
        {
            LogLevel = logLevel,
            ShortMessage = shortMessage,
            FullMessage = fullMessage,
            IpAddress = _customerSettings.StoreIpAddresses ? _webHelper.GetCurrentIpAddress() : string.Empty,
            CustomerId = customer?.Id,
            PageUrl = _webHelper.GetThisPageUrl(true),
            ReferrerUrl = _webHelper.GetUrlReferrer(),
            CreatedOnUtc = DateTime.UtcNow
        };
    }

    #endregion

    #region Methods

    /// <summary>
    /// Determines whether a log level is enabled
    /// </summary>
    /// <param name="level">Log level</param>
    /// <returns>Result</returns>
    public virtual bool IsEnabled(LogLevel level)
    {
		DefaultLogger.LogDebug(">>> Entering IsEnabled(level={Level})", level);
		DefaultLogger.LogDebug("IsEnabled(level={Level}): LogLevel.Debug → {Arg1}", level, LogLevel.Debug);
		DefaultLogger.LogDebug("<<< Exiting IsEnabled(level={Level})", level);
        return level switch
        {
            LogLevel.Debug => false,
            _ => true,
        };
    }

    /// <summary>
    /// Deletes a log item
    /// </summary>
    /// <param name="log">Log item</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteLogAsync(Log log)
    {
		DefaultLogger.LogDebug(">>> Entering DeleteLogAsync(log={Log})", log);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(log);

        await _logRepository.DeleteAsync(log, false);
		DefaultLogger.LogInformation("DeleteLogAsync(log)={Log}: external service call took {Arg1} ms", log, __logStart.ElapsedMilliseconds);
		DefaultLogger.LogDebug("<<< Exiting DeleteLogAsync(log={Log})", log);
    }

    /// <summary>
    /// Deletes a log items
    /// </summary>
    /// <param name="logs">Log items</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteLogsAsync(IList<Log> logs)
    {
		DefaultLogger.LogDebug(">>> Entering DeleteLogsAsync(logs={Logs})", logs);
		DefaultLogger.LogInformation("DeleteLogsAsync(logs)={Logs}: external service call took {Arg1} ms", logs, __logStart.ElapsedMilliseconds);
        await _logRepository.DeleteAsync(logs, false);
		DefaultLogger.LogDebug("<<< Exiting DeleteLogsAsync(logs={Logs})", logs);
    }

    /// <summary>
    /// Clears a log
    /// </summary>
    /// <param name="olderThan">The date that sets the restriction on deleting records. Leave null to remove all records</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task ClearLogAsync(DateTime? olderThan = null)
    {
		DefaultLogger.LogDebug(">>> Entering ClearLogAsync(olderThan={OlderThan})", olderThan);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        if (olderThan == null)
            await _logRepository.TruncateAsync();
        else
            await _logRepository.DeleteAsync(p => p.CreatedOnUtc < olderThan.Value);
			DefaultLogger.LogInformation("ClearLogAsync(olderThan)={OlderThan}: external service call took {Arg1} ms", olderThan, __logStart.ElapsedMilliseconds);
			DefaultLogger.LogInformation("ClearLogAsync(olderThan)={OlderThan}: database query executed in {Arg1} ms", olderThan, __logStart.ElapsedMilliseconds);
			DefaultLogger.LogDebug("<<< Exiting ClearLogAsync(olderThan={OlderThan})", olderThan);
    }

    /// <summary>
    /// Gets all log items
    /// </summary>
    /// <param name="fromUtc">Log item creation from; null to load all records</param>
    /// <param name="toUtc">Log item creation to; null to load all records</param>
    /// <param name="message">Message</param>
    /// <param name="logLevel">Log level; null to load all records</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the log item items
    /// </returns>
    public virtual async Task<IPagedList<Log>> GetAllLogsAsync(DateTime? fromUtc = null, DateTime? toUtc = null,
        string message = "", LogLevel? logLevel = null,
        int pageIndex = 0, int pageSize = int.MaxValue)
    {
		DefaultLogger.LogDebug(">>> Entering GetAllLogsAsync(fromUtc={FromUtc},toUtc={ToUtc},message={Message},logLevel={LogLevel},pageIndex={PageIndex},pageSize={PageSize})", fromUtc, toUtc, message, logLevel, pageIndex, pageSize);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var logs = await _logRepository.GetAllPagedAsync(query =>
        {
            if (fromUtc.HasValue)
                query = query.Where(l => fromUtc.Value <= l.CreatedOnUtc);
            if (toUtc.HasValue)
				DefaultLogger.LogInformation("GetAllLogsAsync(fromUtc,toUtc,message,logLevel,pageIndex,pageSize)={FromUtc},{ToUtc},{Message},{LogLevel},{PageIndex},{PageSize}: find query executed in {Arg6} ms", fromUtc, toUtc, message, logLevel, pageIndex, pageSize, __logStart.ElapsedMilliseconds);
                query = query.Where(l => toUtc.Value >= l.CreatedOnUtc);
            if (logLevel.HasValue)
            {
                var logLevelId = (int)logLevel.Value;
				DefaultLogger.LogDebug("GetAllLogsAsync(fromUtc={FromUtc},toUtc={ToUtc},message={Message},logLevel={LogLevel},pageIndex={PageIndex},pageSize={PageSize}): logLevelId → {LogLevelId}", fromUtc, toUtc, message, logLevel, pageIndex, pageSize, logLevelId);
                query = query.Where(l => logLevelId == l.LogLevelId);
            }

            if (!string.IsNullOrEmpty(message))
                query = query.Where(l => l.ShortMessage.Contains(message) || l.FullMessage.Contains(message));
            query = query.OrderByDescending(l => l.CreatedOnUtc);

            return query;
        }, pageIndex, pageSize);

		DefaultLogger.LogDebug("<<< Exiting GetAllLogsAsync(fromUtc={FromUtc},toUtc={ToUtc},message={Message},logLevel={LogLevel},pageIndex={PageIndex},pageSize={PageSize})", fromUtc, toUtc, message, logLevel, pageIndex, pageSize);
        return logs;
    }

    /// <summary>
    /// Gets a log item
    /// </summary>
    /// <param name="logId">Log item identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the log item
    /// </returns>
    public virtual async Task<Log> GetLogByIdAsync(int logId)
    {
		DefaultLogger.LogDebug(">>> Entering GetLogByIdAsync(logId={LogId})", logId);
		DefaultLogger.LogDebug("<<< Exiting GetLogByIdAsync(logId={LogId})", logId);
        return await _logRepository.GetByIdAsync(logId);
    }

    /// <summary>
    /// Get log items by identifiers
    /// </summary>
    /// <param name="logIds">Log item identifiers</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the log items
    /// </returns>
    public virtual async Task<IList<Log>> GetLogByIdsAsync(int[] logIds)
    {
		DefaultLogger.LogDebug(">>> Entering GetLogByIdsAsync(logIds={LogIds})", logIds);
		DefaultLogger.LogDebug("<<< Exiting GetLogByIdsAsync(logIds={LogIds})", logIds);
        return await _logRepository.GetByIdsAsync(logIds);
    }

    /// <summary>
    /// Inserts a log item
    /// </summary>
    /// <param name="logLevel">Log level</param>
    /// <param name="shortMessage">The short message</param>
    /// <param name="fullMessage">The full message</param>
    /// <param name="customer">The customer to associate log record with</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// </returns>
    public virtual async Task InsertLogAsync(LogLevel logLevel, string shortMessage, string fullMessage = "", Customer customer = null)
    {
		DefaultLogger.LogDebug(">>> Entering InsertLogAsync(logLevel={LogLevel},shortMessage={ShortMessage},fullMessage={FullMessage},customer={Customer})", logLevel, shortMessage, fullMessage, customer);
        //check ignore word/phrase list?
        if (IgnoreLog(shortMessage) || IgnoreLog(fullMessage))
            return;
			var __logStart = System.Diagnostics.Stopwatch.StartNew();

        await _logRepository.InsertAsync(PrepareLog(logLevel, shortMessage, fullMessage, customer), false);
		DefaultLogger.LogInformation("InsertLogAsync(logLevel,shortMessage,fullMessage,customer)={LogLevel},{ShortMessage},{FullMessage},{Customer}: save query executed in {Arg4} ms", logLevel, shortMessage, fullMessage, customer, __logStart.ElapsedMilliseconds);
		DefaultLogger.LogDebug("<<< Exiting InsertLogAsync(logLevel={LogLevel},shortMessage={ShortMessage},fullMessage={FullMessage},customer={Customer})", logLevel, shortMessage, fullMessage, customer);
    }

    /// <summary>
    /// Inserts a log item
    /// </summary>
    /// <param name="logLevel">Log level</param>
    /// <param name="shortMessage">The short message</param>
    /// <param name="fullMessage">The full message</param>
    /// <param name="customer">The customer to associate log record with</param>
    public virtual void InsertLog(LogLevel logLevel, string shortMessage, string fullMessage = "", Customer customer = null)
    {
		DefaultLogger.LogDebug(">>> Entering InsertLog(logLevel={LogLevel},shortMessage={ShortMessage},fullMessage={FullMessage},customer={Customer})", logLevel, shortMessage, fullMessage, customer);
        //check ignore word/phrase list?
        if (IgnoreLog(shortMessage) || IgnoreLog(fullMessage))
            return;
			var __logStart = System.Diagnostics.Stopwatch.StartNew();
        
        _logRepository.Insert(PrepareLog(logLevel, shortMessage, fullMessage, customer), false);
		DefaultLogger.LogInformation("InsertLog(logLevel,shortMessage,fullMessage,customer)={LogLevel},{ShortMessage},{FullMessage},{Customer}: save query executed in {Arg4} ms", logLevel, shortMessage, fullMessage, customer, __logStart.ElapsedMilliseconds);
    }

    /// <summary>
    /// Information
    /// </summary>
    /// <param name="message">Message</param>
    /// <param name="exception">Exception</param>
    /// <param name="customer">Customer</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InformationAsync(string message, Exception exception = null, Customer customer = null)
    {
		DefaultLogger.LogDebug(">>> Entering InformationAsync(message={Message},exception={Exception},customer={Customer})", message, exception, customer);
        //don't log thread abort exception
        if (exception is ThreadAbortException)
            return;

        if (IsEnabled(LogLevel.Information))
            await InsertLogAsync(LogLevel.Information, message, exception?.ToString() ?? string.Empty, customer);
			DefaultLogger.LogDebug("<<< Exiting InformationAsync(message={Message},exception={Exception},customer={Customer})", message, exception, customer);
    }

    /// <summary>
    /// Information
    /// </summary>
    /// <param name="message">Message</param>
    /// <param name="exception">Exception</param>
    /// <param name="customer">Customer</param>
    public virtual void Information(string message, Exception exception = null, Customer customer = null)
    {
		DefaultLogger.LogDebug(">>> Entering Information(message={Message},exception={Exception},customer={Customer})", message, exception, customer);
        //don't log thread abort exception
        if (exception is ThreadAbortException)
            return;

        if (IsEnabled(LogLevel.Information))
            InsertLog(LogLevel.Information, message, exception?.ToString() ?? string.Empty, customer);
    }

    /// <summary>
    /// Warning
    /// </summary>
    /// <param name="message">Message</param>
    /// <param name="exception">Exception</param>
    /// <param name="customer">Customer</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task WarningAsync(string message, Exception exception = null, Customer customer = null)
    {
		DefaultLogger.LogDebug(">>> Entering WarningAsync(message={Message},exception={Exception},customer={Customer})", message, exception, customer);
        //don't log thread abort exception
        if (exception is ThreadAbortException)
            return;

        if (IsEnabled(LogLevel.Warning))
            await InsertLogAsync(LogLevel.Warning, message, exception?.ToString() ?? string.Empty, customer);
			DefaultLogger.LogDebug("<<< Exiting WarningAsync(message={Message},exception={Exception},customer={Customer})", message, exception, customer);
    }

    /// <summary>
    /// Warning
    /// </summary>
    /// <param name="message">Message</param>
    /// <param name="exception">Exception</param>
    /// <param name="customer">Customer</param>
    public virtual void Warning(string message, Exception exception = null, Customer customer = null)
    {
		DefaultLogger.LogDebug(">>> Entering Warning(message={Message},exception={Exception},customer={Customer})", message, exception, customer);
        //don't log thread abort exception
        if (exception is ThreadAbortException)
            return;

        if (IsEnabled(LogLevel.Warning))
            InsertLog(LogLevel.Warning, message, exception?.ToString() ?? string.Empty, customer);
    }

    /// <summary>
    /// Error
    /// </summary>
    /// <param name="message">Message</param>
    /// <param name="exception">Exception</param>
    /// <param name="customer">Customer</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task ErrorAsync(string message, Exception exception = null, Customer customer = null)
    {
		DefaultLogger.LogDebug(">>> Entering ErrorAsync(message={Message},exception={Exception},customer={Customer})", message, exception, customer);
        //don't log thread abort exception
        if (exception is ThreadAbortException)
            return;

        if (IsEnabled(LogLevel.Error))
            await InsertLogAsync(LogLevel.Error, message, exception?.ToString() ?? string.Empty, customer);
			DefaultLogger.LogDebug("<<< Exiting ErrorAsync(message={Message},exception={Exception},customer={Customer})", message, exception, customer);
    }

    /// <summary>
    /// Error
    /// </summary>
    /// <param name="message">Message</param>
    /// <param name="exception">Exception</param>
    /// <param name="customer">Customer</param>
    public virtual void Error(string message, Exception exception = null, Customer customer = null)
    {
		DefaultLogger.LogDebug(">>> Entering Error(message={Message},exception={Exception},customer={Customer})", message, exception, customer);
        //don't log thread abort exception
        if (exception is ThreadAbortException)
            return;

        if (IsEnabled(LogLevel.Error))
            InsertLog(LogLevel.Error, message, exception?.ToString() ?? string.Empty, customer);
    }

    #endregion
}