using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Configuration;
using Microsoft.Extensions.Logging;

namespace Nop.Data.Mapping.Builders.Configuration;

/// <summary>
/// Represents a setting entity builder
/// </summary>
public partial class SettingBuilder : NopEntityBuilder<Setting>
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
            .WithColumn(nameof(Setting.Name)).AsString(200).NotNullable()
            .WithColumn(nameof(Setting.Value)).AsString(6000).NotNullable();
			_logger.LogWarning("<<< Exiting MapEntity(table={Table})", table);
    }

    #endregion
}