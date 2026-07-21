using FluentMigrator;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Data.Migrations;
using Nop.Web.Framework.WebOptimizer;

namespace Nop.Web.Framework.Migrations.UpgradeTo450;

[NopMigration("2021-10-07 00:00:00", "Pseudo-migration to update appSettings.json file", MigrationProcessType.Update)]
public class AppSettingsMigration : MigrationBase
{
    public override void Up()
    {
		logger.LogDebug(">>> Entering Up()");
        var fileProvider = EngineContext.Current.Resolve<INopFileProvider>();

		logger.LogDebug("Up(): fileProvider → {FileProvider}", fileProvider);
        var rootDir = fileProvider.MapPath("~/");

		logger.LogDebug("Up(): rootDir → {RootDir}", rootDir);
        var config = new WebOptimizerConfig
        {
            EnableTagHelperBundling = false,
            EnableCaching = true,
            EnableDiskCache = true,
            AllowEmptyBundle = true,
            CacheDirectory = fileProvider.Combine(rootDir, @"wwwroot\bundles")
        };

        AppSettingsHelper.SaveAppSettings(new List<IConfig> { config }, fileProvider);
		logger.LogDebug("<<< Exiting Up()");
    }

    public override void Down()
    {
		logger.LogDebug(">>> Entering Down()");
        //add the downgrade logic if necessary 
		logger.LogDebug("<<< Exiting Down()");
    }
}
