using FluentMigrator;
using Nop.Core.Infrastructure;
using Nop.Data.Migrations;
using Nop.Services.Configuration;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Shipping.FixedByWeightByTotal.Migrations;

[NopMigration("2023-08-17 15:00:00", "Shipping.FixedByWeightByTotal add LoadAllRecord setting", MigrationProcessType.Update)]
public class AddLoadAllRecordSetting : Migration
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public override void Up()
    {
		_logger.LogDebug(">>> Entering Up()");
        //do not use DI, because it produces exception on the installation process
        var settingService = EngineContext.Current.Resolve<ISettingService>();

		_logger.LogDebug("Up(): settingService → {SettingService}", settingService);
        var pluginSettings = settingService.LoadSetting<FixedByWeightByTotalSettings>();

		_logger.LogDebug("Up(): pluginSettings → {PluginSettings}", pluginSettings);
        if (!settingService.SettingExists(pluginSettings, settings => settings.LoadAllRecord))
        {
            pluginSettings.LoadAllRecord = true;
			_logger.LogDebug("Up(): pluginSettings.LoadAllRecord → {Arg0}", pluginSettings.LoadAllRecord);
            settingService.SaveSetting(pluginSettings, settings => settings.LoadAllRecord);
        }
		_logger.LogDebug("<<< Exiting Up()");
    }

    public override void Down()
    {
		_logger.LogDebug(">>> Entering Down()");
        //add the downgrade logic if necessary 
		_logger.LogDebug("<<< Exiting Down()");
    }
}