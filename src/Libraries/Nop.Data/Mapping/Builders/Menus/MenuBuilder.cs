using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Menus;
using Microsoft.Extensions.Logging;

namespace Nop.Data.Mapping.Builders.Menus;

/// <summary>
/// Represents a menu entity builder
/// </summary>
public partial class MenuBuilder : NopEntityBuilder<Menu>
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
            .WithColumn(nameof(Menu.Name)).AsString(400).NotNullable()
            .WithColumn(nameof(Menu.CssClass)).AsString(400).Nullable();
			_logger.LogDebug("<<< Exiting MapEntity(table={Table})", table);
    }

    #endregion
}
