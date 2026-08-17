using FluentAssertions;
using Nop.Core.Configuration;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Configuration;
using Nop.Services.Configuration;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Services.Tests.Configuration;

[TestFixture]
public class SettingServiceTests : ServiceTest<Setting>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    private ISettingService _settingService;

    [OneTimeSetUp]
    public void SetUp()
    {
		_logger.LogDebug(">>> Entering SetUp()");
        _settingService = GetService<ISettingService>();
		_logger.LogDebug("<<< Exiting SetUp()");
		_logger.LogDebug("SetUp(): _settingService → {_settingService}", _settingService);
    }

    [TearDown]
    public async Task TearDown()
    {
		_logger.LogDebug(">>> Entering TearDown()");
        await _settingService.DeleteSettingAsync<TestSettings>();
		_logger.LogDebug("<<< Exiting TearDown()");
    }

    protected override CrudData<Setting> CrudData
    {
        get
        {
            var newSettings = new Setting { Name = "SettingServiceTests.TestCrud", Value = "TestCrud", StoreId = 1 };

            var updateSettings = new Setting
            {
                Name = "SettingServiceTests.TestCrud",
                Value = "TestCrud updated",
                StoreId = 1
            };

            return new CrudData<Setting>
            {
                BaseEntity = newSettings,
                UpdatedEntity = updateSettings,
                Insert = async (setting) =>
                {
                    await _settingService.SetSettingAsync(setting.Name, setting.Value, setting.StoreId);
                    var s = await _settingService.GetSettingAsync("SettingServiceTests.TestCrud", 1);
                    setting.Id = s.Id;

                },
                Update = async setting => await _settingService.SetSettingAsync(setting.Name, setting.Value, setting.StoreId),
                GetById = _settingService.GetSettingByIdAsync,
                Delete = _settingService.DeleteSettingAsync,
                IsEqual = (first, second) => first.StoreId == second.StoreId && first.Name.Equals(second.Name, StringComparison.InvariantCultureIgnoreCase) && first.Value == second.Value
            };
        }
    }
    
    [Test]
    public async Task CanGetSettings()
    {
		_logger.LogDebug(">>> Entering CanGetSettings()");
        var setting = await _settingService.LoadSettingAsync<CatalogSettings>();

		_logger.LogDebug("CanGetSettings(): setting → {Setting}", setting);
        setting.Should().NotBeNull();
        setting.DefaultViewMode.Should().Be("grid");
        setting.NewProductsPageSizeOptions.Should().Be("6, 3, 9");
		_logger.LogDebug("<<< Exiting CanGetSettings()");
    }

    [Test]
    public async Task CanGetAllSettings()
    {
		_logger.LogDebug(">>> Entering CanGetAllSettings()");
        var settings = await _settingService.GetAllSettingsAsync();
        settings.Should().NotBeNull();
		_logger.LogDebug("CanGetAllSettings(): settings → {Settings}", settings);
        settings.Any().Should().BeTrue();
		_logger.LogDebug("<<< Exiting CanGetAllSettings()");
    }

    [Test]
    public async Task CanGetSettingByKey()
    {
		_logger.LogDebug(">>> Entering CanGetSettingByKey()");
        var setting = await _settingService.GetSettingByKeyAsync<string>("CatalogSettings.DefaultViewMode");
        setting.Should().Be("grid");
		_logger.LogDebug("<<< Exiting CanGetSettingByKey()");
		_logger.LogDebug("CanGetSettingByKey(): setting → {Setting}", setting);
    }

    [Test]
    public async Task CanGetTypedSettingValueByKey()
    {
		_logger.LogDebug(">>> Entering CanGetTypedSettingValueByKey()");
        var setting = await _settingService.GetSettingByKeyAsync<int>("CatalogSettings.NumberOfProductTags");
        setting.Should().Be(15);
		_logger.LogDebug("<<< Exiting CanGetTypedSettingValueByKey()");
		_logger.LogDebug("CanGetTypedSettingValueByKey(): setting → {Setting}", setting);
    }

    [Test]
    public async Task DefaultValueReturnedIfSettingDoesNotExist()
    {
		_logger.LogDebug(">>> Entering DefaultValueReturnedIfSettingDoesNotExist()");
        var setting = await _settingService.GetSettingByKeyAsync("NonExistentKey", 1.0f);
        setting.Should().Be(1.0f);
		_logger.LogDebug("<<< Exiting DefaultValueReturnedIfSettingDoesNotExist()");
		_logger.LogDebug("DefaultValueReturnedIfSettingDoesNotExist(): setting → {Setting}", setting);
    }

    [Test]
    public async Task CanDeleteSetting()
    {
		_logger.LogDebug(">>> Entering CanDeleteSetting()");
        var setting = new TestSettings
        {
			_logger.LogDebug("CanDeleteSetting(): setting → {Setting}", setting);
            ServerName = "Ruby",
            Ip = "192.168.0.1",
            PortNumber = 21,
            Username = "admin",
            Password = "password"
        };

        await _settingService.SaveSettingAsync(setting);
        setting = await _settingService.LoadSettingAsync<TestSettings>();
        setting.Should().NotBeNull();
        setting.Username.Should().Be("admin");
        setting.Password.Should().Be("password");
        await _settingService.DeleteSettingAsync(setting, settings => settings.Username);
        await _settingService.DeleteSettingAsync(
            await _settingService.GetSettingAsync(_settingService.GetSettingKey(setting,
                settings => settings.Password)));
				_logger.LogDebug("CanDeleteSetting(): settings → {Settings}", settings);
        setting = await _settingService.LoadSettingAsync<TestSettings>();
        setting.Should().NotBeNull();
        setting.Username.Should().BeNullOrEmpty();
        setting.Password.Should().BeNullOrEmpty();
        var ipExists = await _settingService.SettingExistsAsync(setting, settings => setting.Ip);
		_logger.LogDebug("CanDeleteSetting(): ipExists → {IpExists}", ipExists);
        ipExists.Should().BeTrue();
        await _settingService.DeleteSettingAsync<TestSettings>();
        var portNumber =
            await _settingService.GetSettingAsync(_settingService.GetSettingKey(setting,
                settings => settings.PortNumber));

        portNumber.Should().BeNull();

        ipExists = await _settingService.SettingExistsAsync(setting, settings => setting.Ip);
        ipExists.Should().BeFalse();
		_logger.LogDebug("<<< Exiting CanDeleteSetting()");
    }

    [Test]
    public async Task CanSaveSetting()
    {
		_logger.LogDebug(">>> Entering CanSaveSetting()");
        var setting = new TestSettings
        {
			_logger.LogDebug("CanSaveSetting(): setting → {Setting}", setting);
            ServerName = "Ruby",
            Ip = "192.168.0.1",
            PortNumber = 21,
            Username = "admin",
            Password = "password"
        };

        await _settingService.SaveSettingAsync(setting);
        setting = await _settingService.LoadSettingAsync<TestSettings>();
        setting.Should().NotBeNull();

        setting.ServerName.Should().Be("Ruby");
        setting.Ip.Should().Be("192.168.0.1");
        setting.PortNumber.Should().Be(21);
        setting.Username.Should().Be("admin");
        setting.Password.Should().Be("password");

        setting.PortNumber = 24;
		_logger.LogDebug("CanSaveSetting(): setting.PortNumber → {Arg0}", setting.PortNumber);

        await _settingService.SaveSettingAsync(setting, settings => settings.PortNumber);

        setting = await _settingService.LoadSettingAsync<TestSettings>();
        setting.Should().NotBeNull();

        setting.PortNumber.Should().Be(24);

        setting.Ip = "192.168.1.1";
		_logger.LogDebug("CanSaveSetting(): setting.Ip → {Arg0}", setting.Ip);

        await _settingService.SaveSettingOverridablePerStoreAsync(setting, settings => settings.Ip, false);

        var ip = (await _settingService.GetSettingAsync(
            _settingService.GetSettingKey(setting, settings => settings.Ip)))?.Value;

        ip.Should().NotBeNull().And.Be("192.168.1.1");

        setting.Ip = "192.168.0.2";

        await _settingService.SaveSettingOverridablePerStoreAsync(setting, settings => settings.Ip, false, 1);

        ip = (await _settingService.GetSettingAsync(
            _settingService.GetSettingKey(setting, settings => settings.Ip), 1))?.Value;

        ip.Should().BeNullOrEmpty();

        await _settingService.SaveSettingOverridablePerStoreAsync(setting, settings => settings.Ip, true, 1);

        ip = (await _settingService.GetSettingAsync(
            _settingService.GetSettingKey(setting, settings => settings.Ip), 1))?.Value;

        ip.Should().NotBeNull().And.Be("192.168.0.2");
		_logger.LogDebug("<<< Exiting CanSaveSetting()");
    }

    #region Nested class

    public class TestSettings : ISettings
    {
        public string ServerName { get; set; }
        public string Ip { get; set; }
        public int PortNumber { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    #endregion
}