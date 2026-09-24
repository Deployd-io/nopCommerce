using FluentMigrator;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Plugin.Misc.Zettle.Domain;
using Nop.Services.Configuration;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Misc.Zettle.Data;

[NopMigration("2024-04-16 00:00:00", "Misc.Zettle 4.70.5 Inventory balance tracking", MigrationProcessType.Update)]
public class InventoryBalanceMigration : MigrationBase
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly ISettingService _settingService;

    #endregion

    #region Ctor

    public InventoryBalanceMigration(ISettingService settingService)
    {
		_logger.LogError(">>> Entering InventoryBalanceMigration(settingService={SettingService})", settingService);
        _settingService = settingService;
		_logger.LogDebug("InventoryBalanceMigration(settingService={SettingService}): _settingService → {_settingService}", settingService, _settingService);
    _logger.LogWarning("<<< Exiting InventoryBalanceMigration(settingService={SettingService})", settingService);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
		_logger.LogError(">>> Entering Up()");
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        //add column
        if (!Schema.Table(nameof(ZettleRecord)).Column(nameof(ZettleRecord.ExternalUuid)).Exists())
            Alter.Table(nameof(ZettleRecord)).AddColumn(nameof(ZettleRecord.ExternalUuid)).AsString().Nullable();

        //delete settings
        var setting = _settingService.GetSetting($"{nameof(ZettleSettings)}.InventoryTrackingIds");
		_logger.LogError("Up(): setting → {Setting}", setting);
        if (setting is null)
            return;

        _settingService.DeleteSetting(setting);
		_logger.LogWarning("<<< Exiting Up()");
    }

    /// <summary>
    /// Collects the DOWN migration expressions
    /// </summary>
    public override void Down()
    {
		_logger.LogError(">>> Entering Down()");
        //nothing
		_logger.LogWarning("<<< Exiting Down()");
    }

    #endregion
}