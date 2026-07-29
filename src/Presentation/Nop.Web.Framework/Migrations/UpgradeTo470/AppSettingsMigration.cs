using FluentMigrator;
using Microsoft.AspNetCore.Http.Features;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Web.Framework.WebOptimizer;

namespace Nop.Web.Framework.Migrations.UpgradeTo470;

[NopMigration("2023-11-17 00:00:00", "Pseudo-migration to update appSettings.json file", MigrationProcessType.Update)]
public class AppSettingsMigration : MigrationBase
{

    public override void Up()
    {
		logger.LogDebug(">>> Entering Up()");
        var fileProvider = EngineContext.Current.Resolve<INopFileProvider>();

		logger.LogDebug("Up(): fileProvider → {FileProvider}", fileProvider);
        var dataConfig = DataSettingsManager.LoadSettings();
        dataConfig.WithNoLock = false;
		logger.LogDebug("Up(): dataConfig → {DataConfig}", dataConfig);
		logger.LogDebug("Up(): dataConfig.WithNoLock → {Arg0}", dataConfig.WithNoLock);

        var rootDir = fileProvider.MapPath("~/");
        var woConfig = new WebOptimizerConfig
		logger.LogDebug("Up(): rootDir → {RootDir}", rootDir);
        {
            CdnUrl = string.Empty,
            HttpsCompression = HttpsCompressionMode.Compress,
            EnableTagHelperBundling = false,
            EnableCaching = true,
            EnableDiskCache = true,
            AllowEmptyBundle = true,
            CacheDirectory = fileProvider.Combine(rootDir, @"wwwroot\bundles")
        };


        AppSettingsHelper.SaveAppSettings(new List<IConfig> { dataConfig, woConfig }, fileProvider);
		logger.LogDebug("<<< Exiting Up()");
    }

    public override void Down() { }
}