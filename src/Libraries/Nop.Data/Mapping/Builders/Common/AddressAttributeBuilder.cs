using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Common;
using Microsoft.Extensions.Logging;

namespace Nop.Data.Mapping.Builders.Common;

/// <summary>
/// Represents a address attribute entity builder
/// </summary>
public partial class AddressAttributeBuilder : NopEntityBuilder<AddressAttribute>
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
        table.WithColumn(nameof(AddressAttribute.Name)).AsString(400).NotNullable();
		_logger.LogWarning("<<< Exiting MapEntity(table={Table})", table);
    }

    #endregion
}