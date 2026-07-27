using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Shipping.FixedByWeightByTotal.Domain;

namespace Nop.Plugin.Shipping.FixedByWeightByTotal.Data;

public class ShippingByWeightByTotalRecordBuilder : NopEntityBuilder<ShippingByWeightByTotalRecord>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
		logger.LogDebug(">>> Entering MapEntity(table={Table})", table);
        table
            .WithColumn(nameof(ShippingByWeightByTotalRecord.WeightFrom))
            .AsDecimal(18, 4)
            .WithColumn(nameof(ShippingByWeightByTotalRecord.WeightTo))
            .AsDecimal(18, 4)
            .WithColumn(nameof(ShippingByWeightByTotalRecord.OrderSubtotalFrom))
            .AsDecimal(18, 4)
            .WithColumn(nameof(ShippingByWeightByTotalRecord.OrderSubtotalTo))
            .AsDecimal(18, 4)
            .WithColumn(nameof(ShippingByWeightByTotalRecord.AdditionalFixedCost))
            .AsDecimal(18, 4)
            .WithColumn(nameof(ShippingByWeightByTotalRecord.PercentageRateOfSubtotal))
            .AsDecimal(18, 4)
            .WithColumn(nameof(ShippingByWeightByTotalRecord.RatePerWeightUnit))
            .AsDecimal(18, 4)
            .WithColumn(nameof(ShippingByWeightByTotalRecord.LowerWeightLimit))
            .AsDecimal(18, 4)
            .WithColumn(nameof(ShippingByWeightByTotalRecord.Zip))
            .AsString(400)
            .Nullable();
			logger.LogDebug("<<< Exiting MapEntity(table={Table})", table);
    }
}