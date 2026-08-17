using FluentAssertions;
using Nop.Core.Domain.Customers;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Services.Tests.Helpers;

[TestFixture]
public class DateTimeHelperTests : ServiceTest
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    private ICustomerService _customerService;
    private IGenericAttributeService _genericAttributeService;
    private DateTimeSettings _dateTimeSettings;
    private IDateTimeHelper _dateTimeHelper;
    private ISettingService _settingService;
    private Customer _customer;

    /// <summary>
    /// (GMT+02:00) Minsk
    /// </summary>
    private string _gmtPlus2MinskTimeZoneId;

    /// <summary>
    /// (GMT+03:00) Moscow, St. Petersburg, Volgograd
    /// </summary>
    private string _gmtPlus3MoscowTimeZoneId;

    /// <summary>
    /// (GMT+07:00) Krasnoyarsk
    /// </summary>
    private string _gmtPlus7KrasnoyarskTimeZoneId;

    private string _defaultTimeZone;
    private bool _defaultAllowCustomersToSetTimeZone;
    private string _defaultDefaultStoreTimeZoneId;

    [OneTimeSetUp]
    public async Task SetUp()
    {
		_logger.LogDebug(">>> Entering SetUp()");
        _customerService = GetService<ICustomerService>();
        _genericAttributeService = GetService<IGenericAttributeService>();
		_logger.LogDebug("SetUp(): _genericAttributeService → {_genericAttributeService}", _genericAttributeService);
		_logger.LogDebug("SetUp(): _customerService → {_customerService}", _customerService);
        _dateTimeSettings = GetService<DateTimeSettings>();
		_logger.LogDebug("SetUp(): _dateTimeSettings → {_dateTimeSettings}", _dateTimeSettings);
        _dateTimeHelper = GetService<IDateTimeHelper>();
		_logger.LogDebug("SetUp(): _dateTimeHelper → {_dateTimeHelper}", _dateTimeHelper);
        _settingService = GetService<ISettingService>();
		_logger.LogDebug("SetUp(): _settingService → {_settingService}", _settingService);

        _customer = await GetService<ICustomerService>().GetCustomerByEmailAsync(NopTestsDefaults.AdminEmail);
		_logger.LogDebug("SetUp(): _customer → {_customer}", _customer);

        _defaultTimeZone = _customer.TimeZoneId;
		_logger.LogDebug("SetUp(): _defaultTimeZone → {_defaultTimeZone}", _defaultTimeZone);

        _defaultAllowCustomersToSetTimeZone = _dateTimeSettings.AllowCustomersToSetTimeZone;
        _defaultDefaultStoreTimeZoneId = _dateTimeSettings.DefaultStoreTimeZoneId;

        _gmtPlus2MinskTimeZoneId = "E. Europe Standard Time";  //(GMT+02:00) Minsk
		_logger.LogDebug("SetUp(): _gmtPlus2MinskTimeZoneId → {_gmtPlus2MinskTimeZoneId}", _gmtPlus2MinskTimeZoneId);
		_logger.LogDebug("SetUp(): _defaultAllowCustomersToSetTimeZone → {_defaultAllowCustomersToSetTimeZone}", _defaultAllowCustomersToSetTimeZone);
		_logger.LogDebug("SetUp(): _defaultDefaultStoreTimeZoneId → {_defaultDefaultStoreTimeZoneId}", _defaultDefaultStoreTimeZoneId);
        _gmtPlus3MoscowTimeZoneId = "Russian Standard Time"; //(GMT+03:00) Moscow, St. Petersburg, Volgograd
		_logger.LogDebug("SetUp(): _gmtPlus3MoscowTimeZoneId → {_gmtPlus3MoscowTimeZoneId}", _gmtPlus3MoscowTimeZoneId);
        _gmtPlus7KrasnoyarskTimeZoneId = "North Asia Standard Time"; //(GMT+07:00) Krasnoyarsk;
		_logger.LogDebug("SetUp(): _gmtPlus7KrasnoyarskTimeZoneId → {_gmtPlus7KrasnoyarskTimeZoneId}", _gmtPlus7KrasnoyarskTimeZoneId);

        if (Environment.OSVersion.Platform != PlatformID.Unix)
            return;

        _gmtPlus2MinskTimeZoneId = "Europe/Minsk";  //(GMT+02:00) Minsk;
        _gmtPlus3MoscowTimeZoneId = "Europe/Moscow"; //(GMT+03:00) Moscow, St. Petersburg, Volgograd
        _gmtPlus7KrasnoyarskTimeZoneId = "Asia/Krasnoyarsk"; //(GMT+07:00) Krasnoyarsk;
		_logger.LogDebug("<<< Exiting SetUp()");
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
		_logger.LogDebug(">>> Entering TearDown()");
        _customer.TimeZoneId = _defaultTimeZone;
        _dateTimeSettings.AllowCustomersToSetTimeZone = _defaultAllowCustomersToSetTimeZone;
		_logger.LogDebug("TearDown(): _dateTimeSettings.AllowCustomersToSetTimeZone → {Arg0}", _dateTimeSettings.AllowCustomersToSetTimeZone);
		_logger.LogDebug("TearDown(): _dateTimeSettings.DefaultStoreTimeZoneId → {Arg0}", _dateTimeSettings.DefaultStoreTimeZoneId);
		_logger.LogDebug("TearDown(): _customer.TimeZoneId → {Arg0}", _customer.TimeZoneId);
        _dateTimeSettings.DefaultStoreTimeZoneId = _defaultDefaultStoreTimeZoneId;

        await _settingService.SaveSettingAsync(_dateTimeSettings);
		_logger.LogDebug("<<< Exiting TearDown()");
    }

    [Test]
    public void CanGetAllSystemTimeZones()
    {
		_logger.LogDebug(">>> Entering CanGetAllSystemTimeZones()");
        var systemTimeZones = _dateTimeHelper.GetSystemTimeZones();
        systemTimeZones.Should().NotBeNull();
		_logger.LogDebug("CanGetAllSystemTimeZones(): systemTimeZones → {SystemTimeZones}", systemTimeZones);
        systemTimeZones.Any().Should().BeTrue();
		_logger.LogDebug("<<< Exiting CanGetAllSystemTimeZones()");
    }

    [Test]
    public async Task CanGetCustomerTimeZoneWithCustomTimeZonesEnabled()
    {
		_logger.LogDebug(">>> Entering CanGetCustomerTimeZoneWithCustomTimeZonesEnabled()");
        _dateTimeSettings.AllowCustomersToSetTimeZone = true;
        _dateTimeSettings.DefaultStoreTimeZoneId = _gmtPlus2MinskTimeZoneId; //(GMT+02:00) Minsk;
		_logger.LogDebug("CanGetCustomerTimeZoneWithCustomTimeZonesEnabled(): _dateTimeSettings.AllowCustomersToSetTimeZone → {Arg0}", _dateTimeSettings.AllowCustomersToSetTimeZone);
        await _settingService.SaveSettingAsync(_dateTimeSettings);

        _customer.TimeZoneId = _gmtPlus3MoscowTimeZoneId;
		_logger.LogDebug("CanGetCustomerTimeZoneWithCustomTimeZonesEnabled(): _customer.TimeZoneId → {Arg0}", _customer.TimeZoneId);
        await _customerService.UpdateCustomerAsync(_customer);

        var timeZone = await GetService<IDateTimeHelper>().GetCustomerTimeZoneAsync(_customer);
		_logger.LogDebug("CanGetCustomerTimeZoneWithCustomTimeZonesEnabled(): timeZone → {TimeZone}", timeZone);
        timeZone.Should().NotBeNull();
        timeZone.Id.Should().Be(_gmtPlus3MoscowTimeZoneId);
		_logger.LogDebug("<<< Exiting CanGetCustomerTimeZoneWithCustomTimeZonesEnabled()");
    }

    [Test]
    public async Task CanGetCustomerTimezoneWithCustomTimeZonesDisabled()
    {
		_logger.LogDebug(">>> Entering CanGetCustomerTimezoneWithCustomTimeZonesDisabled()");
        _dateTimeSettings.AllowCustomersToSetTimeZone = false;
        _dateTimeSettings.DefaultStoreTimeZoneId = _gmtPlus2MinskTimeZoneId; //(GMT+02:00) Minsk;
		_logger.LogDebug("CanGetCustomerTimezoneWithCustomTimeZonesDisabled(): _dateTimeSettings.AllowCustomersToSetTimeZone → {Arg0}", _dateTimeSettings.AllowCustomersToSetTimeZone);
        await _settingService.SaveSettingAsync(_dateTimeSettings);

        _customer.TimeZoneId = _gmtPlus3MoscowTimeZoneId;
		_logger.LogDebug("CanGetCustomerTimezoneWithCustomTimeZonesDisabled(): _customer.TimeZoneId → {Arg0}", _customer.TimeZoneId);
        await _customerService.UpdateCustomerAsync(_customer);

        var timeZone = await GetService<IDateTimeHelper>().GetCustomerTimeZoneAsync(_customer);
		_logger.LogDebug("CanGetCustomerTimezoneWithCustomTimeZonesDisabled(): timeZone → {TimeZone}", timeZone);
        timeZone.Should().NotBeNull();
        timeZone.Id.Should().Be(_gmtPlus2MinskTimeZoneId);  //(GMT+02:00) Minsk
		_logger.LogDebug("<<< Exiting CanGetCustomerTimezoneWithCustomTimeZonesDisabled()");
    }

    [Test]
    public void CanConvertDatetimeToUserTime()
    {
		_logger.LogDebug(">>> Entering CanConvertDatetimeToUserTime()");
        var sourceDateTime = TimeZoneInfo.FindSystemTimeZoneById(_gmtPlus2MinskTimeZoneId); //(GMT+02:00) Minsk;
        sourceDateTime.Should().NotBeNull();
		_logger.LogDebug("CanConvertDatetimeToUserTime(): sourceDateTime → {SourceDateTime}", sourceDateTime);

        var destinationDateTime = TimeZoneInfo.FindSystemTimeZoneById(_gmtPlus7KrasnoyarskTimeZoneId); //(GMT+07:00) Krasnoyarsk;
		_logger.LogDebug("CanConvertDatetimeToUserTime(): destinationDateTime → {DestinationDateTime}", destinationDateTime);
        destinationDateTime.Should().NotBeNull();

        //summer time
        _dateTimeHelper.ConvertToUserTime(new DateTime(2010, 06, 01, 0, 0, 0), sourceDateTime, destinationDateTime)
            .Should().Be(new DateTime(2010, 06, 01, 5, 0, 0));

        //winter time
        _dateTimeHelper.ConvertToUserTime(new DateTime(2010, 01, 01, 0, 0, 0), sourceDateTime, destinationDateTime)
            .Should().Be(new DateTime(2010, 01, 01, 5, 0, 0));
			_logger.LogDebug("<<< Exiting CanConvertDatetimeToUserTime()");
    }

    [Test]
    public void CanConvertDatetimeToUtcDateTime()
    {
		_logger.LogDebug(">>> Entering CanConvertDatetimeToUtcDateTime()");
        var sourceDateTime = TimeZoneInfo.FindSystemTimeZoneById(_gmtPlus2MinskTimeZoneId); //(GMT+02:00) Minsk;
        sourceDateTime.Should().NotBeNull();
		_logger.LogDebug("CanConvertDatetimeToUtcDateTime(): sourceDateTime → {SourceDateTime}", sourceDateTime);

        //summer time
        var dateTime1 = new DateTime(2010, 06, 01, 0, 0, 0);
		_logger.LogDebug("CanConvertDatetimeToUtcDateTime(): dateTime1 → {DateTime1}", dateTime1);
        var convertedDateTime1 = _dateTimeHelper.ConvertToUtcTime(dateTime1, sourceDateTime);
		_logger.LogDebug("CanConvertDatetimeToUtcDateTime(): convertedDateTime1 → {ConvertedDateTime1}", convertedDateTime1);
        convertedDateTime1.Should().Be(new DateTime(2010, 05, 31, 21, 0, 0));

        //winter time
        var dateTime2 = new DateTime(2010, 01, 01, 0, 0, 0);
		_logger.LogDebug("CanConvertDatetimeToUtcDateTime(): dateTime2 → {DateTime2}", dateTime2);
        var convertedDateTime2 = _dateTimeHelper.ConvertToUtcTime(dateTime2, sourceDateTime);
		_logger.LogDebug("CanConvertDatetimeToUtcDateTime(): convertedDateTime2 → {ConvertedDateTime2}", convertedDateTime2);
        convertedDateTime2.Should().Be(new DateTime(2009, 12, 31, 22, 0, 0));
		_logger.LogDebug("<<< Exiting CanConvertDatetimeToUtcDateTime()");
    }
}