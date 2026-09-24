using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Security;
using Microsoft.Extensions.Logging;

namespace Nop.Data.Mapping.Builders.Security;

/// <summary>
/// Represents a permission record entity builder
/// </summary>
public partial class PermissionRecordBuilder : NopEntityBuilder<PermissionRecord>
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
            .WithColumn(nameof(PermissionRecord.Name)).AsString(int.MaxValue).NotNullable()
            .WithColumn(nameof(PermissionRecord.SystemName)).AsString(255).NotNullable()
            .WithColumn(nameof(PermissionRecord.Category)).AsString(255).NotNullable();
			_logger.LogWarning("<<< Exiting MapEntity(table={Table})", table);
    }

    #endregion
}