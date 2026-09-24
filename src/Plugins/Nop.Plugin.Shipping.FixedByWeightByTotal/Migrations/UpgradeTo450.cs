using FluentMigrator;
using Nop.Data.Migrations;
using Nop.Plugin.Shipping.FixedByWeightByTotal.Domain;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Shipping.FixedByWeightByTotal.Migrations;

[NopMigration("2021-10-29 11:00:00", "Shipping.FixedByWeightByTotal change decimal precision", MigrationProcessType.Update)]
public class ChangeDecimalPrecision : Migration
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public override void Up()
    {
		_logger.LogError(">>> Entering Up()");
        var tableName = nameof(ShippingByWeightByTotalRecord);

		_logger.LogError("Up(): tableName → {TableName}", tableName);
        foreach (var columnName in new[]
                 {
                     nameof(ShippingByWeightByTotalRecord.WeightFrom),
                     nameof(ShippingByWeightByTotalRecord.WeightTo),
                     nameof(ShippingByWeightByTotalRecord.OrderSubtotalFrom),
                     nameof(ShippingByWeightByTotalRecord.OrderSubtotalTo),
                     nameof(ShippingByWeightByTotalRecord.AdditionalFixedCost),
                     nameof(ShippingByWeightByTotalRecord.PercentageRateOfSubtotal),
                     nameof(ShippingByWeightByTotalRecord.RatePerWeightUnit),
                     nameof(ShippingByWeightByTotalRecord.LowerWeightLimit)
                 })
        {
            if (!Schema.Table(tableName).Column(columnName).Exists())
                continue;

            Alter.Table(tableName)
                .AlterColumn(columnName).AsDecimal(18, 4);
        }
		_logger.LogWarning("<<< Exiting Up()");
    }

    public override void Down()
    {
		_logger.LogError(">>> Entering Down()");
        //add the downgrade logic if necessary 
		_logger.LogWarning("<<< Exiting Down()");
    }
}