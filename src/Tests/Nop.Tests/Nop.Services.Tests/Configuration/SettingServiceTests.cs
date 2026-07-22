using FluentAssertions;
using Nop.Core.Configuration;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Configuration;
using Nop.Services.Configuration;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.Configuration;

[TestFixture]
public class SettingServiceTests : ServiceTest<Setting>
{
    private ISettingService _settingService;

    [OneTimeSetUp]
    public void SetUp()
    {
		logger.LogDebug(">>> Entering SetUp()");
        _settingService = GetService<ISettingService>();
		logger.LogDebug("<<< Exiting SetUp()");
		logger.LogDebug("SetUp(): _settingService → {_settingService}", _settingService);
    }

    [TearDown]
    public async Task TearDown()
    {
		logger.LogDebug(">>> Entering TearDown()");
        await _settingService.DeleteSettingAsync<TestSettings>();
		logger.LogDebug("<<< Exiting TearDown()");
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
		logger.LogDebug(">>> Entering CanGetSettings()");
        var setting = await _settingService.LoadSettingAsync<CatalogSettings>();

		logger.LogDebug("CanGetSettings(): setting → {Setting}", setting);
        setting.Should().NotBeNull();
        setting.DefaultViewMode.Should().Be("grid");
        setting.NewProductsPageSizeOptions.Should().Be("6, 3, 9");
		logger.LogDebug("<<< Exiting CanGetSettings()");
    }

    [Test]
    public async Task CanGetAllSettings()
    {
		logger.LogDebug(">>> Entering CanGetAllSettings()");
        var settings = await _settingService.GetAllSettingsAsync();
        settings.Should().NotBeNull();
		logger.LogDebug("CanGetAllSettings(): settings → {Settings}", settings);
        settings.Any().Should().BeTrue();
		logger.LogDebug("<<< Exiting CanGetAllSettings()");
    }

    [Test]
    public async Task CanGetSettingByKey()
    {
		logger.LogDebug(">>> Entering CanGetSettingByKey()");
        var setting = await _settingService.GetSettingByKeyAsync<string>("CatalogSettings.DefaultViewMode");
        setting.Should().Be("grid");
		logger.LogDebug("<<< Exiting CanGetSettingByKey()");
		logger.LogDebug("CanGetSettingByKey(): setting → {Setting}", setting);
    }

    [Test]
    public async Task CanGetTypedSettingValueByKey()
    {
		logger.LogDebug(">>> Entering CanGetTypedSettingValueByKey()");
        var setting = await _settingService.GetSettingByKeyAsync<int>("CatalogSettings.NumberOfProductTags");
        setting.Should().Be(15);
		logger.LogDebug("<<< Exiting CanGetTypedSettingValueByKey()");
		logger.LogDebug("CanGetTypedSettingValueByKey(): setting → {Setting}", setting);
    }

    [Test]
    public async Task DefaultValueReturnedIfSettingDoesNotExist()
    {
		logger.LogDebug(">>> Entering DefaultValueReturnedIfSettingDoesNotExist()");
        var setting = await _settingService.GetSettingByKeyAsync("NonExistentKey", 1.0f);
        setting.Should().Be(1.0f);
		logger.LogDebug("<<< Exiting DefaultValueReturnedIfSettingDoesNotExist()");
		logger.LogDebug("DefaultValueReturnedIfSettingDoesNotExist(): setting → {Setting}", setting);
    }

    [Test]
    public async Task CanDeleteSetting()
    {
		logger.LogDebug(">>> Entering CanDeleteSetting()");
        var setting = new TestSettings
        {
			logger.LogDebug("CanDeleteSetting(): setting → {Setting}", setting);
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
				logger.LogDebug("CanDeleteSetting(): settings → {Settings}", settings);
        setting = await _settingService.LoadSettingAsync<TestSettings>();
        setting.Should().NotBeNull();
        setting.Username.Should().BeNullOrEmpty();
        setting.Password.Should().BeNullOrEmpty();
        var ipExists = await _settingService.SettingExistsAsync(setting, settings => setting.Ip);
        ipExists.Should().BeTrue();
		logger.LogDebug("CanDeleteSetting(): ipExists → {IpExists}", ipExists);
        await _settingService.DeleteSettingAsync<TestSettings>();
        var portNumber =
            await _settingService.GetSettingAsync(_settingService.GetSettingKey(setting,
                settings => settings.PortNumber));

        portNumber.Should().BeNull();

        ipExists = await _settingService.SettingExistsAsync(setting, settings => setting.Ip);
        ipExists.Should().BeFalse();
		logger.LogDebug("<<< Exiting CanDeleteSetting()");
    }

    [Test]
    public async Task CanSaveSetting()
    {
		logger.LogDebug(">>> Entering CanSaveSetting()");
        var setting = new TestSettings
        {
			logger.LogDebug("CanSaveSetting(): setting → {Setting}", setting);
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
		logger.LogDebug("CanSaveSetting(): setting.PortNumber → {Arg0}", setting.PortNumber);

        await _settingService.SaveSettingAsync(setting, settings => settings.PortNumber);

        setting = await _settingService.LoadSettingAsync<TestSettings>();
        setting.Should().NotBeNull();

        setting.PortNumber.Should().Be(24);

        setting.Ip = "192.168.1.1";
		logger.LogDebug("CanSaveSetting(): setting.Ip → {Arg0}", setting.Ip);

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
		logger.LogDebug("<<< Exiting CanSaveSetting()");
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
