using FluentMigrator;
using Nop.Core.Infrastructure;
using Nop.Data.Migrations;
using Nop.Services.Configuration;

namespace Nop.Plugin.Shipping.FixedByWeightByTotal.Migrations;

[NopMigration("2023-08-17 15:00:00", "Shipping.FixedByWeightByTotal add LoadAllRecord setting", MigrationProcessType.Update)]
public class AddLoadAllRecordSetting : Migration
{
    public override void Up()
    {
		logger.LogDebug(">>> Entering Up()");
        //do not use DI, because it produces exception on the installation process
        var settingService = EngineContext.Current.Resolve<ISettingService>();

		logger.LogDebug("Up(): settingService → {SettingService}", settingService);
        var pluginSettings = settingService.LoadSetting<FixedByWeightByTotalSettings>();

		logger.LogDebug("Up(): pluginSettings → {PluginSettings}", pluginSettings);
        if (!settingService.SettingExists(pluginSettings, settings => settings.LoadAllRecord))
        {
            pluginSettings.LoadAllRecord = true;
			logger.LogDebug("Up(): pluginSettings.LoadAllRecord → {Arg0}", pluginSettings.LoadAllRecord);
            settingService.SaveSetting(pluginSettings, settings => settings.LoadAllRecord);
        }
		logger.LogDebug("<<< Exiting Up()");
    }

    public override void Down()
    {
		logger.LogDebug(">>> Entering Down()");
        //add the downgrade logic if necessary 
		logger.LogDebug("<<< Exiting Down()");
    }
}
