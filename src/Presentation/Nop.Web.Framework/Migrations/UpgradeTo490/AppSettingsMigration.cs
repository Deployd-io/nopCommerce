using FluentMigrator;
using Microsoft.AspNetCore.Http.Features;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Web.Framework.WebOptimizer;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.Migrations.UpgradeTo490;

[NopMigration("2025-09-15 00:00:00", "Pseudo-migration to update appSettings.json file", MigrationProcessType.Update)]
public class AppSettingsMigration : MigrationBase
{
private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;

    public override void Up()
    {
		_logger.LogDebug(">>> Entering Up()");
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        var woConfig = Singleton<AppSettings>.Instance.Get<WebOptimizerConfig>();
		_logger.LogDebug("Up(): woConfig → {WoConfig}", woConfig);
        woConfig.MemoryCacheTimeToLive = TimeSpan.FromMinutes(60);

        AppSettingsHelper.SaveAppSettings(new List<IConfig> { woConfig }, EngineContext.Current.Resolve<INopFileProvider>());
		_logger.LogDebug("<<< Exiting Up()");
    }

    public override void Down() { }
}