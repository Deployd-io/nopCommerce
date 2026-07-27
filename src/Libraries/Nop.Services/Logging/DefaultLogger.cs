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
        _commonSettings = commonSettings;
        _customerSettings = customerSettings;
        _logRepository = logRepository;
        _webHelper = webHelper;
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
		logger.LogDebug(">>> Entering IgnoreLog(message={Message})", message);
        if (!_commonSettings.IgnoreLogWordlist.Any())
            return false;

        if (string.IsNullOrWhiteSpace(message))
		logger.LogDebug("<<< Exiting IgnoreLog(message={Message})", message);
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
		logger.LogDebug(">>> Entering PrepareLog(logLevel={LogLevel},shortMessage={ShortMessage},fullMessage={FullMessage},customer={Customer})", logLevel, shortMessage, fullMessage, customer);
		logger.LogDebug("<<< Exiting PrepareLog(logLevel={LogLevel},shortMessage={ShortMessage},fullMessage={FullMessage},customer={Customer})", logLevel, shortMessage, fullMessage, customer);
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
		logger.LogDebug(">>> Entering IsEnabled(level={Level})", level);
		logger.LogDebug("IsEnabled(level={Level}): LogLevel.Debug → {Arg1}", level, LogLevel.Debug);
		logger.LogDebug("<<< Exiting IsEnabled(level={Level})", level);
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
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogDebug(">>> Entering DeleteLogAsync(log={Log})", log);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(log);

        await _logRepository.DeleteAsync(log, false);
		logger.LogDebug("<<< Exiting DeleteLogAsync(log={Log})", log);
		logger.LogInformation("DeleteLogAsync(log)={Log}: external service call took {Arg1} ms", log, __logStart.ElapsedMilliseconds);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("DeleteLogAsync(log)={Log}: delete query executed in {Arg1} ms", log, __logStart.ElapsedMilliseconds);
    }

    /// <summary>
    /// Deletes a log items
    /// </summary>
    /// <param name="logs">Log items</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteLogsAsync(IList<Log> logs)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _logRepository.DeleteAsync(logs, false);
    }

	var __logStart = System.Diagnostics.Stopwatch.StartNew();
    /// <summary>
    /// Clears a log
    /// </summary>
    /// <param name="olderThan">The date that sets the restriction on deleting records. Leave null to remove all records</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task ClearLogAsync(DateTime? olderThan = null)
    {
		logger.LogDebug(">>> Entering ClearLogAsync(olderThan={OlderThan})", olderThan);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        if (olderThan == null)
            await _logRepository.TruncateAsync();
			var __logStart = System.Diagnostics.Stopwatch.StartNew();
        else
            await _logRepository.DeleteAsync(p => p.CreatedOnUtc < olderThan.Value);
			logger.LogDebug("<<< Exiting ClearLogAsync(olderThan={OlderThan})", olderThan);
			logger.LogInformation("ClearLogAsync(olderThan)={OlderThan}: external service call took {Arg1} ms", olderThan, __logStart.ElapsedMilliseconds);
			logger.LogInformation("ClearLogAsync(olderThan)={OlderThan}: database query executed in {Arg1} ms", olderThan, __logStart.ElapsedMilliseconds);
			logger.LogInformation("ClearLogAsync(olderThan)={OlderThan}: delete query executed in {Arg1} ms", olderThan, __logStart.ElapsedMilliseconds);
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
        var logs = await _logRepository.GetAllPagedAsync(query =>
        {
            if (fromUtc.HasValue)
                query = query.Where(l => fromUtc.Value <= l.CreatedOnUtc);
            if (toUtc.HasValue)
                query = query.Where(l => toUtc.Value >= l.CreatedOnUtc);
            if (logLevel.HasValue)
            {
                var logLevelId = (int)logLevel.Value;
                query = query.Where(l => logLevelId == l.LogLevelId);
            }

            if (!string.IsNullOrEmpty(message))
                query = query.Where(l => l.ShortMessage.Contains(message) || l.FullMessage.Contains(message));
            query = query.OrderByDescending(l => l.CreatedOnUtc);

            return query;
        }, pageIndex, pageSize);

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
		logger.LogDebug(">>> Entering GetLogByIdAsync(logId={LogId})", logId);
		logger.LogDebug("<<< Exiting GetLogByIdAsync(logId={LogId})", logId);
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
		logger.LogDebug(">>> Entering GetLogByIdsAsync(logIds={LogIds})", logIds);
		logger.LogDebug("<<< Exiting GetLogByIdsAsync(logIds={LogIds})", logIds);
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
		logger.LogDebug(">>> Entering InsertLogAsync(logLevel={LogLevel},shortMessage={ShortMessage},fullMessage={FullMessage},customer={Customer})", logLevel, shortMessage, fullMessage, customer);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        //check ignore word/phrase list?
        if (IgnoreLog(shortMessage) || IgnoreLog(fullMessage))
            return;

        await _logRepository.InsertAsync(PrepareLog(logLevel, shortMessage, fullMessage, customer), false);
		logger.LogInformation("InsertLogAsync(logLevel,shortMessage,fullMessage,customer)={LogLevel},{ShortMessage},{FullMessage},{Customer}: save query executed in {Arg4} ms", logLevel, shortMessage, fullMessage, customer, __logStart.ElapsedMilliseconds);
		logger.LogDebug("<<< Exiting InsertLogAsync(logLevel={LogLevel},shortMessage={ShortMessage},fullMessage={FullMessage},customer={Customer})", logLevel, shortMessage, fullMessage, customer);
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
		logger.LogDebug(">>> Entering InsertLog(logLevel={LogLevel},shortMessage={ShortMessage},fullMessage={FullMessage},customer={Customer})", logLevel, shortMessage, fullMessage, customer);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        //check ignore word/phrase list?
        if (IgnoreLog(shortMessage) || IgnoreLog(fullMessage))
            return;
        
        _logRepository.Insert(PrepareLog(logLevel, shortMessage, fullMessage, customer), false);
		logger.LogInformation("InsertLog(logLevel,shortMessage,fullMessage,customer)={LogLevel},{ShortMessage},{FullMessage},{Customer}: save query executed in {Arg4} ms", logLevel, shortMessage, fullMessage, customer, __logStart.ElapsedMilliseconds);
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
		logger.LogDebug(">>> Entering InformationAsync(message={Message},exception={Exception},customer={Customer})", message, exception, customer);
        //don't log thread abort exception
        if (exception is ThreadAbortException)
            return;

        if (IsEnabled(LogLevel.Information))
            await InsertLogAsync(LogLevel.Information, message, exception?.ToString() ?? string.Empty, customer);
			logger.LogDebug("<<< Exiting InformationAsync(message={Message},exception={Exception},customer={Customer})", message, exception, customer);
    }

    /// <summary>
    /// Information
    /// </summary>
    /// <param name="message">Message</param>
    /// <param name="exception">Exception</param>
    /// <param name="customer">Customer</param>
    public virtual void Information(string message, Exception exception = null, Customer customer = null)
    {
		logger.LogDebug(">>> Entering Information(message={Message},exception={Exception},customer={Customer})", message, exception, customer);
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
		logger.LogDebug(">>> Entering WarningAsync(message={Message},exception={Exception},customer={Customer})", message, exception, customer);
        //don't log thread abort exception
        if (exception is ThreadAbortException)
            return;

        if (IsEnabled(LogLevel.Warning))
            await InsertLogAsync(LogLevel.Warning, message, exception?.ToString() ?? string.Empty, customer);
			logger.LogDebug("<<< Exiting WarningAsync(message={Message},exception={Exception},customer={Customer})", message, exception, customer);
    }

    /// <summary>
    /// Warning
    /// </summary>
    /// <param name="message">Message</param>
    /// <param name="exception">Exception</param>
    /// <param name="customer">Customer</param>
    public virtual void Warning(string message, Exception exception = null, Customer customer = null)
    {
		logger.LogDebug(">>> Entering Warning(message={Message},exception={Exception},customer={Customer})", message, exception, customer);
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
		logger.LogDebug(">>> Entering ErrorAsync(message={Message},exception={Exception},customer={Customer})", message, exception, customer);
        //don't log thread abort exception
        if (exception is ThreadAbortException)
            return;

        if (IsEnabled(LogLevel.Error))
            await InsertLogAsync(LogLevel.Error, message, exception?.ToString() ?? string.Empty, customer);
			logger.LogDebug("<<< Exiting ErrorAsync(message={Message},exception={Exception},customer={Customer})", message, exception, customer);
    }

    /// <summary>
    /// Error
    /// </summary>
    /// <param name="message">Message</param>
    /// <param name="exception">Exception</param>
    /// <param name="customer">Customer</param>
    public virtual void Error(string message, Exception exception = null, Customer customer = null)
    {
		logger.LogDebug(">>> Entering Error(message={Message},exception={Exception},customer={Customer})", message, exception, customer);
        //don't log thread abort exception
        if (exception is ThreadAbortException)
            return;

        if (IsEnabled(LogLevel.Error))
            InsertLog(LogLevel.Error, message, exception?.ToString() ?? string.Empty, customer);
    }

    #endregion
}