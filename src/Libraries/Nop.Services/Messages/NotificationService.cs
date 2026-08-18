using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Services.Logging;

namespace Nop.Services.Messages;

/// <summary>
/// Notification service
/// </summary>
public partial class NotificationService : INotificationService
{
    #region Fields

    protected readonly IHttpContextAccessor _httpContextAccessor;
    protected readonly ILogger _logger;
    protected readonly ITempDataDictionaryFactory _tempDataDictionaryFactory;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public NotificationService(IHttpContextAccessor httpContextAccessor,
        ILogger logger,
        ITempDataDictionaryFactory tempDataDictionaryFactory,
        IWorkContext workContext)
    {
        _httpContextAccessor = httpContextAccessor;
		_logger.LogDebug("NotificationService(httpContextAccessor={HttpContextAccessor},logger={Logger},tempDataDictionaryFactory={TempDataDictionaryFactory},workContext={WorkContext}): _httpContextAccessor → {_httpContextAccessor}", httpContextAccessor, logger, tempDataDictionaryFactory, workContext, _httpContextAccessor);
        _logger = logger;
		_logger.LogDebug(">>> Entering NotificationService(httpContextAccessor={HttpContextAccessor},logger={Logger},tempDataDictionaryFactory={TempDataDictionaryFactory},workContext={WorkContext})", httpContextAccessor, logger, tempDataDictionaryFactory, workContext);
        _tempDataDictionaryFactory = tempDataDictionaryFactory;
        _workContext = workContext;
		_logger.LogDebug("NotificationService(httpContextAccessor={HttpContextAccessor},logger={Logger},tempDataDictionaryFactory={TempDataDictionaryFactory},workContext={WorkContext}): _workContext → {_workContext}", httpContextAccessor, logger, tempDataDictionaryFactory, workContext, _workContext);
		_logger.LogDebug("<<< Exiting NotificationService(httpContextAccessor={HttpContextAccessor},logger={Logger},tempDataDictionaryFactory={TempDataDictionaryFactory},workContext={WorkContext})", httpContextAccessor, logger, tempDataDictionaryFactory, workContext);
		_logger.LogDebug("NotificationService(httpContextAccessor={HttpContextAccessor},logger={Logger},tempDataDictionaryFactory={TempDataDictionaryFactory},workContext={WorkContext}): _tempDataDictionaryFactory → {_tempDataDictionaryFactory}", httpContextAccessor, logger, tempDataDictionaryFactory, workContext, _tempDataDictionaryFactory);
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Save message into TempData
    /// </summary>
    /// <param name="type">Notification type</param>
    /// <param name="message">Message</param>
    /// <param name="encode">A value indicating whether the message should not be encoded</param>
    /// <param name="timeout">The time (in milliseconds), on which notification will close automatically; leave 0 if notification shouldn't close automatically</param>
    protected virtual void PrepareTempData(NotifyType type, string message, bool encode = true, int timeout = 0)
    {
		_logger.LogDebug(">>> Entering PrepareTempData(type={Type},message={Message},encode={Encode},timeout={Timeout})", type, message, encode, timeout);
        var context = _httpContextAccessor.HttpContext;
        var tempData = _tempDataDictionaryFactory.GetTempData(context);
		_logger.LogDebug("PrepareTempData(type={Type},message={Message},encode={Encode},timeout={Timeout}): tempData → {TempData}", type, message, encode, timeout, tempData);

        //Messages have stored in a serialized list
        var messages = tempData.TryGetValue(NopMessageDefaults.NotificationListKey, out var value) 
            ? JsonConvert.DeserializeObject<IList<NotifyData>>(value.ToString())
            : new List<NotifyData>();

        messages.Add(new NotifyData
        {
            Message = message,
            Type = type,
            Encode = encode,
            Timeout = timeout
        });

        tempData[NopMessageDefaults.NotificationListKey] = JsonConvert.SerializeObject(messages);
		_logger.LogDebug("<<< Exiting PrepareTempData(type={Type},message={Message},encode={Encode},timeout={Timeout})", type, message, encode, timeout);
    }

    /// <summary>
    /// Log exception
    /// </summary>
    /// <param name="exception">Exception</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task LogExceptionAsync(Exception exception)
    {
		_logger.LogDebug(">>> Entering LogExceptionAsync(exception={Exception})", exception);
        if (exception == null)
            return;
        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogDebug("LogExceptionAsync(exception={Exception}): customer → {Customer}", exception, customer);
        await _logger.ErrorAsync(exception.Message, exception, customer);
		_logger.LogDebug("<<< Exiting LogExceptionAsync(exception={Exception})", exception);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Display notification
    /// </summary>
    /// <param name="type">Notification type</param>
    /// <param name="message">Message</param>
    /// <param name="encode">A value indicating whether the message should not be encoded</param>
    /// <param name="timeout">The time (in milliseconds) on which notification will close automatically; leave 0 if notification shouldn't close automatically</param>
    public virtual void Notification(NotifyType type, string message, bool encode = true, int timeout = 0)
    {
		_logger.LogDebug(">>> Entering Notification(type={Type},message={Message},encode={Encode},timeout={Timeout})", type, message, encode, timeout);
        PrepareTempData(type, message, encode, timeout);
		_logger.LogDebug("<<< Exiting Notification(type={Type},message={Message},encode={Encode},timeout={Timeout})", type, message, encode, timeout);
    }

    /// <summary>
    /// Display success notification
    /// </summary>
    /// <param name="message">Message</param>
    /// <param name="encode">A value indicating whether the message should not be encoded</param>
    /// <param name="timeout">The time (in milliseconds) on which notification will close automatically; leave 0 if notification shouldn't close automatically</param>
    public virtual void SuccessNotification(string message, bool encode = true, int timeout = 0)
    {
		_logger.LogDebug(">>> Entering SuccessNotification(message={Message},encode={Encode},timeout={Timeout})", message, encode, timeout);
        PrepareTempData(NotifyType.Success, message, encode, timeout);
		_logger.LogDebug("<<< Exiting SuccessNotification(message={Message},encode={Encode},timeout={Timeout})", message, encode, timeout);
    }

    /// <summary>
    /// Display warning notification
    /// </summary>
    /// <param name="message">Message</param>
    /// <param name="encode">A value indicating whether the message should not be encoded</param>
    /// <param name="timeout">The time (in milliseconds) on which notification will close automatically; leave 0 if notification shouldn't close automatically</param>
    public virtual void WarningNotification(string message, bool encode = true, int timeout = 0)
    {
		_logger.LogDebug(">>> Entering WarningNotification(message={Message},encode={Encode},timeout={Timeout})", message, encode, timeout);
        PrepareTempData(NotifyType.Warning, message, encode, timeout);
		_logger.LogDebug("<<< Exiting WarningNotification(message={Message},encode={Encode},timeout={Timeout})", message, encode, timeout);
    }

    /// <summary>
    /// Display error notification
    /// </summary>
    /// <param name="message">Message</param>
    /// <param name="encode">A value indicating whether the message should not be encoded</param>
    /// <param name="timeout">The time (in milliseconds) on which notification will close automatically; leave 0 if notification shouldn't close automatically</param>
    public virtual void ErrorNotification(string message, bool encode = true, int timeout = 0)
    {
		_logger.LogDebug(">>> Entering ErrorNotification(message={Message},encode={Encode},timeout={Timeout})", message, encode, timeout);
        PrepareTempData(NotifyType.Error, message, encode, timeout);
		_logger.LogDebug("<<< Exiting ErrorNotification(message={Message},encode={Encode},timeout={Timeout})", message, encode, timeout);
    }

    /// <summary>
    /// Display error notification
    /// </summary>
    /// <param name="exception">Exception</param>
    /// <param name="logException">A value indicating whether exception should be logged</param>
    /// <param name="timeout">The time (in milliseconds) on which notification will close automatically; leave 0 if notification shouldn't close automatically</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task ErrorNotificationAsync(Exception exception, bool logException = true, int timeout = 0)
    {
		_logger.LogDebug(">>> Entering ErrorNotificationAsync(exception={Exception},logException={LogException},timeout={Timeout})", exception, logException, timeout);
        if (exception == null)
            return;

        if (logException)
            await LogExceptionAsync(exception);

        ErrorNotification(exception.Message, timeout: timeout);
		_logger.LogDebug("<<< Exiting ErrorNotificationAsync(exception={Exception},logException={LogException},timeout={Timeout})", exception, logException, timeout);
    }

    #endregion
}