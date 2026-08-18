using FluentMigrator;
using Microsoft.AspNetCore.Http.Features;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Web.Framework.WebOptimizer;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.Migrations.UpgradeTo470;

[NopMigration("2023-11-17 00:00:00", "Pseudo-migration to update appSettings.json file", MigrationProcessType.Update)]
public class AppSettingsMigration : MigrationBase
{
private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;

    public override void Up()
    {
		_logger.LogDebug(">>> Entering Up()");
        var fileProvider = EngineContext.Current.Resolve<INopFileProvider>();

		_logger.LogDebug("Up(): fileProvider → {FileProvider}", fileProvider);
        var dataConfig = DataSettingsManager.LoadSettings();
		_logger.LogDebug("Up(): dataConfig → {DataConfig}", dataConfig);
		_logger.LogDebug("Up(): dataConfig.WithNoLock → {Arg0}", dataConfig.WithNoLock);
        dataConfig.WithNoLock = false;

        var rootDir = fileProvider.MapPath("~/");
		_logger.LogDebug("Up(): rootDir → {RootDir}", rootDir);
        var woConfig = new WebOptimizerConfig
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
		_logger.LogDebug("<<< Exiting Up()");
    }

    public override void Down() { }
}