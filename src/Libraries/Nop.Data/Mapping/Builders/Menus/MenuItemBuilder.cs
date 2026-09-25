using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Menus;
using Nop.Data.Extensions;
using Microsoft.Extensions.Logging;

namespace Nop.Data.Mapping.Builders.Menus;

/// <summary>
/// Represents a menu item entity builder
/// </summary>
public partial class MenuItemBuilder : NopEntityBuilder<MenuItem>
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
            .WithColumn(nameof(MenuItem.Title)).AsString(400).Nullable()
            .WithColumn(nameof(MenuItem.CssClass)).AsString(400).Nullable()
            .WithColumn(nameof(MenuItem.RouteName)).AsString(400).Nullable()
            .WithColumn(nameof(MenuItem.MenuId)).AsInt32().ForeignKey<Menu>();
			_logger.LogWarning("<<< Exiting MapEntity(table={Table})", table);
    }

    #endregion
}
