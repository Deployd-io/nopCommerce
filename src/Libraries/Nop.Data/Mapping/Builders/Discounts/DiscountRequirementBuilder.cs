using System.Data;
using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Discounts;
using Nop.Data.Extensions;
using Microsoft.Extensions.Logging;

namespace Nop.Data.Mapping.Builders.Discounts;

/// <summary>
/// Represents a discount requirement entity builder
/// </summary>
public partial class DiscountRequirementBuilder : NopEntityBuilder<DiscountRequirement>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Methods

    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
		_logger.LogDebug(">>> Entering MapEntity(table={Table})", table);
        table
            .WithColumn(nameof(DiscountRequirement.DiscountId)).AsInt32().ForeignKey<Discount>()
            .WithColumn(nameof(DiscountRequirement.ParentId)).AsInt32().Nullable().ForeignKey<DiscountRequirement>(onDelete: Rule.None);
			_logger.LogDebug("<<< Exiting MapEntity(table={Table})", table);
    }

    #endregion
}