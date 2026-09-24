using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Data.Extensions;
using Microsoft.Extensions.Logging;

namespace Nop.Data.Mapping.Builders.Customers;

/// <summary>
/// Represents a customer attribute value entity builder
/// </summary>
public partial class CustomerAttributeValueBuilder : NopEntityBuilder<CustomerAttributeValue>
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
            .WithColumn(nameof(CustomerAttributeValue.Name)).AsString(400).NotNullable()
            .WithColumn(NameCompatibilityManager.GetColumnName(typeof(CustomerAttributeValue), nameof(CustomerAttributeValue.AttributeId))).AsInt32().ForeignKey<CustomerAttribute>();
			_logger.LogWarning("<<< Exiting MapEntity(table={Table})", table);
    }

    #endregion
}