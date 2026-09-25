using FluentMigrator;
using Nop.Data;
using Nop.Data.Mapping;
using Nop.Data.Migrations;
using Nop.Plugin.Tax.Avalara.Domain;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Tax.Avalara.Data;

[NopMigration("2022-07-13 00:00:02", "Tax.Avalara 2.65. Update datetime type precision", MigrationProcessType.Update)]
public class MySqlDateTimeWithPrecisionMigration : Migration
{
private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;

    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
		_logger.LogError(">>> Entering Up()");
        var dataSettings = DataSettingsManager.LoadSettings();

		_logger.LogError("Up(): dataSettings → {DataSettings}", dataSettings);
        //update the types only in MySql 
			_logger.LogWarning("<<< Exiting Up()");
        if (dataSettings.DataProvider != DataProviderType.MySql) {
            return;
        }

        Alter.Table(NameCompatibilityManager.GetTableName(typeof(TaxTransactionLog)))
            .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(TaxTransactionLog), nameof(TaxTransactionLog.CreatedDateUtc)))
            .AsCustom("datetime(6)");
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
}