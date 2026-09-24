using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Common;
using Nop.Data.Extensions;
using Microsoft.Extensions.Logging;

namespace Nop.Data.Mapping.Builders.Common;

/// <summary>
/// Represents an address attribute value entity builder
/// </summary>
public partial class AddressAttributeValueBuilder : NopEntityBuilder<AddressAttributeValue>
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
            .WithColumn(nameof(AddressAttributeValue.Name)).AsString(400).NotNullable()
            .WithColumn(NameCompatibilityManager.GetColumnName(typeof(AddressAttributeValue), nameof(AddressAttributeValue.AttributeId))).AsInt32().ForeignKey<AddressAttribute>();
			_logger.LogWarning("<<< Exiting MapEntity(table={Table})", table);
    }

    #endregion
}