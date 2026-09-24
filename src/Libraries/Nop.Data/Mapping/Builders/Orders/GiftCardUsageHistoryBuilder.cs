using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Orders;
using Nop.Data.Extensions;
using Microsoft.Extensions.Logging;

namespace Nop.Data.Mapping.Builders.Orders;

/// <summary>
/// Represents a gift card usage history entity builder
/// </summary>
public partial class GiftCardUsageHistoryBuilder : NopEntityBuilder<GiftCardUsageHistory>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Methods

    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
		_logger.LogError(">>> Entering MapEntity(table={Table})", table);
        table
            .WithColumn(nameof(GiftCardUsageHistory.GiftCardId)).AsInt32().ForeignKey<GiftCard>()
            .WithColumn(nameof(GiftCardUsageHistory.UsedWithOrderId)).AsInt32().ForeignKey<Order>();
			_logger.LogWarning("<<< Exiting MapEntity(table={Table})", table);
    }

    #endregion
}