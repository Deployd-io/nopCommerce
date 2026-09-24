using System.Collections.ObjectModel;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Helpers;

/// <summary>
/// Represents a datetime helper
/// </summary>
public partial class DateTimeHelper : IDateTimeHelper
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly DateTimeSettings _dateTimeSettings;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public DateTimeHelper(DateTimeSettings dateTimeSettings,
        IWorkContext workContext)
    {
		_logger.LogError(">>> Entering DateTimeHelper(dateTimeSettings={DateTimeSettings},workContext={WorkContext})", dateTimeSettings, workContext);
        _dateTimeSettings = dateTimeSettings;
        _workContext = workContext;
		_logger.LogDebug("DateTimeHelper(dateTimeSettings={DateTimeSettings},workContext={WorkContext}): _dateTimeSettings → {_dateTimeSettings}", dateTimeSettings, workContext, _dateTimeSettings);
    _logger.LogWarning("<<< Exiting DateTimeHelper(dateTimeSettings={DateTimeSettings},workContext={WorkContext})", dateTimeSettings, workContext);
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Retrieves a System.TimeZoneInfo object from the registry based on its identifier.
    /// </summary>
    /// <param name="id">The time zone identifier, which corresponds to the System.TimeZoneInfo.Id property.</param>
    /// <returns>A System.TimeZoneInfo object whose identifier is the value of the id parameter.</returns>
    protected virtual TimeZoneInfo FindTimeZoneById(string id)
    {
		_logger.LogError(">>> Entering FindTimeZoneById(id={Id})", id);
        _logger.LogWarning("<<< Exiting FindTimeZoneById(id={Id})", id);
        return TimeZoneInfo.FindSystemTimeZoneById(id);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Returns a sorted collection of all the time zones
    /// </summary>
    /// <returns>A read-only collection of System.TimeZoneInfo objects.</returns>
    public virtual ReadOnlyCollection<TimeZoneInfo> GetSystemTimeZones()
    {
		_logger.LogError(">>> Entering GetSystemTimeZones()");
        _logger.LogWarning("<<< Exiting GetSystemTimeZones()");
        return TimeZoneInfo.GetSystemTimeZones();
    }

    /// <summary>
    /// Converts the date and time to current user date and time
    /// </summary>
    /// <param name="dt">The date and time (represents local system time or UTC time) to convert.</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains a DateTime value that represents time that corresponds to the dateTime parameter in customer time zone.
    /// </returns>
    public virtual async Task<DateTime> ConvertToUserTimeAsync(DateTime dt)
    {
		_logger.LogError(">>> Entering ConvertToUserTimeAsync(dt={Dt})", dt);
        _logger.LogWarning("<<< Exiting ConvertToUserTimeAsync(dt={Dt})", dt);
		_logger.LogWarning("<<< Exiting ConvertToUserTimeAsync(dt={Dt},sourceDateTimeKind={SourceDateTimeKind})", dt, sourceDateTimeKind);
        return await ConvertToUserTimeAsync(dt, dt.Kind);
    }

    /// <summary>
    /// Converts the date and time to current user date and time
    /// </summary>
    /// <param name="dt">The date and time (represents local system time or UTC time) to convert.</param>
    /// <param name="sourceDateTimeKind">The source datetimekind</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains a DateTime value that represents time that corresponds to the dateTime parameter in customer time zone.
    /// </returns>
    public virtual async Task<DateTime> ConvertToUserTimeAsync(DateTime dt, DateTimeKind sourceDateTimeKind)
    {
        dt = DateTime.SpecifyKind(dt, sourceDateTimeKind);
        if (sourceDateTimeKind == DateTimeKind.Local && TimeZoneInfo.Local.IsInvalidTime(dt))
            return dt;

        var currentUserTimeZoneInfo = await GetCurrentTimeZoneAsync();
        return TimeZoneInfo.ConvertTime(dt, currentUserTimeZoneInfo);
    }

    /// <summary>
    /// Converts the date and time to current user date and time
    /// </summary>
    /// <param name="dt">The date and time to convert.</param>
    /// <param name="sourceTimeZone">The time zone of dateTime.</param>
    /// <param name="destinationTimeZone">The time zone to convert dateTime to.</param>
    /// <returns>A DateTime value that represents time that corresponds to the dateTime parameter in customer time zone.</returns>
    public virtual DateTime ConvertToUserTime(DateTime dt, TimeZoneInfo sourceTimeZone, TimeZoneInfo destinationTimeZone)
    {
		_logger.LogError(">>> Entering ConvertToUserTime(dt={Dt},sourceTimeZone={SourceTimeZone},destinationTimeZone={DestinationTimeZone})", dt, sourceTimeZone, destinationTimeZone);
        if (sourceTimeZone.IsInvalidTime(dt))
            return dt;

        return TimeZoneInfo.ConvertTime(dt, sourceTimeZone, destinationTimeZone);
    }

    /// <summary>
    /// Converts the date and time to Coordinated Universal Time (UTC)
    /// </summary>
    /// <param name="dt">The date and time (represents local system time or UTC time) to convert.</param>
    /// <returns>A DateTime value that represents the Coordinated Universal Time (UTC) that corresponds to the dateTime parameter. The DateTime value's Kind property is always set to DateTimeKind.Utc.</returns>
    public virtual DateTime ConvertToUtcTime(DateTime dt)
    {
		_logger.LogError(">>> Entering ConvertToUtcTime(dt={Dt})", dt);
		_logger.LogWarning("<<< Exiting ConvertToUtcTime(dt={Dt})", dt);
		_logger.LogWarning("<<< Exiting ConvertToUtcTime(dt={Dt},sourceDateTimeKind={SourceDateTimeKind})", dt, sourceDateTimeKind);
		_logger.LogWarning("<<< Exiting ConvertToUtcTime(dt={Dt},sourceTimeZone={SourceTimeZone})", dt, sourceTimeZone);
        return ConvertToUtcTime(dt, dt.Kind);
    }

    /// <summary>
    /// Converts the date and time to Coordinated Universal Time (UTC)
    /// </summary>
    /// <param name="dt">The date and time (represents local system time or UTC time) to convert.</param>
    /// <param name="sourceDateTimeKind">The source datetimekind</param>
    /// <returns>A DateTime value that represents the Coordinated Universal Time (UTC) that corresponds to the dateTime parameter. The DateTime value's Kind property is always set to DateTimeKind.Utc.</returns>
    public virtual DateTime ConvertToUtcTime(DateTime dt, DateTimeKind sourceDateTimeKind)
    {
        dt = DateTime.SpecifyKind(dt, sourceDateTimeKind);
        if (sourceDateTimeKind == DateTimeKind.Local && TimeZoneInfo.Local.IsInvalidTime(dt))
            return dt;

        return TimeZoneInfo.ConvertTimeToUtc(dt);
    }

    /// <summary>
    /// Converts the date and time to Coordinated Universal Time (UTC)
    /// </summary>
    /// <param name="dt">The date and time to convert.</param>
    /// <param name="sourceTimeZone">The time zone of dateTime.</param>
    /// <returns>A DateTime value that represents the Coordinated Universal Time (UTC) that corresponds to the dateTime parameter. The DateTime value's Kind property is always set to DateTimeKind.Utc.</returns>
    public virtual DateTime ConvertToUtcTime(DateTime dt, TimeZoneInfo sourceTimeZone)
    {
        if (sourceTimeZone.IsInvalidTime(dt))
        {
            //could not convert
            return dt;
        }

        return TimeZoneInfo.ConvertTimeToUtc(dt, sourceTimeZone);
    }

    /// <summary>
    /// Gets a customer time zone
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer time zone; if customer is null, then default store time zone
    /// </returns>
    public virtual Task<TimeZoneInfo> GetCustomerTimeZoneAsync(Customer customer)
    {
		_logger.LogError(">>> Entering GetCustomerTimeZoneAsync(customer={Customer})", customer);
        if (!_dateTimeSettings.AllowCustomersToSetTimeZone)
            return Task.FromResult(DefaultStoreTimeZone);

        TimeZoneInfo timeZoneInfo = null;
		_logger.LogError("GetCustomerTimeZoneAsync(customer={Customer}): timeZoneInfo → {TimeZoneInfo}", customer, timeZoneInfo);

        var timeZoneId = string.Empty;
		_logger.LogError("GetCustomerTimeZoneAsync(customer={Customer}): timeZoneId → {TimeZoneId}", customer, timeZoneId);
        if (customer != null)
            timeZoneId = customer.TimeZoneId;

        try
        {
            if (!string.IsNullOrEmpty(timeZoneId))
                timeZoneInfo = FindTimeZoneById(timeZoneId);
        }
        catch
        {
			_logger.LogError("Exception in GetCustomerTimeZoneAsync(customer={Customer}): {Arg1}", customer, ex.Message);
            //ignore
        }

		_logger.LogWarning("<<< Exiting GetCustomerTimeZoneAsync(customer={Customer})", customer);
        return Task.FromResult(timeZoneInfo ?? DefaultStoreTimeZone);
    }

    /// <summary>
    /// Gets the current user time zone
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the current user time zone
    /// </returns>
    public virtual async Task<TimeZoneInfo> GetCurrentTimeZoneAsync()
    {
		_logger.LogError(">>> Entering GetCurrentTimeZoneAsync()");
        _logger.LogWarning("<<< Exiting GetCurrentTimeZoneAsync()");
        return await GetCustomerTimeZoneAsync(await _workContext.GetCurrentCustomerAsync());
    }

    /// <summary>
    /// Gets or sets a default store time zone
    /// </summary>
    public virtual TimeZoneInfo DefaultStoreTimeZone
    {
        get
        {
            TimeZoneInfo timeZoneInfo = null;
            try
            {
                if (!string.IsNullOrEmpty(_dateTimeSettings.DefaultStoreTimeZoneId))
                    timeZoneInfo = FindTimeZoneById(_dateTimeSettings.DefaultStoreTimeZoneId);
            }
            catch
            {
                //ignore
            }

            return timeZoneInfo ?? TimeZoneInfo.Local;
        }
    }

    #endregion
}