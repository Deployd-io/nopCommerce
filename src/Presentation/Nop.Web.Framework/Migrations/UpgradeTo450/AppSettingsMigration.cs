using FluentMigrator;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Data.Migrations;
using Nop.Web.Framework.WebOptimizer;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.Migrations.UpgradeTo450;

[NopMigration("2021-10-07 00:00:00", "Pseudo-migration to update appSettings.json file", MigrationProcessType.Update)]
public class AppSettingsMigration : MigrationBase
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public override void Up()
    {
		_logger.LogError(">>> Entering Up()");
        var fileProvider = EngineContext.Current.Resolve<INopFileProvider>();

		_logger.LogError("Up(): fileProvider → {FileProvider}", fileProvider);
        var rootDir = fileProvider.MapPath("~/");
		_logger.LogError("Up(): rootDir → {RootDir}", rootDir);

        var config = new WebOptimizerConfig
        {
            EnableTagHelperBundling = false,
            EnableCaching = true,
            EnableDiskCache = true,
            AllowEmptyBundle = true,
            CacheDirectory = fileProvider.Combine(rootDir, @"wwwroot\bundles")
        };

        AppSettingsHelper.SaveAppSettings(new List<IConfig> { config }, fileProvider);
		_logger.LogWarning("<<< Exiting Up()");
    }

    public override void Down()
    {
		_logger.LogError(">>> Entering Down()");
        //add the downgrade logic if necessary 
		_logger.LogWarning("<<< Exiting Down()");
    }
}